var config = require("./../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');

module.exports = function(app) {
    app.post('/session/', function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        console.log(req.body);
        async.waterfall([
            function checkValidData(callback) {
                if (req.body.Username && req.body.Password) {
                    callback(null);
                }
                else {
                    res.status(400);
                    res.send('Missing parameter in POST request');
                    res.end();
                    return;
                }
            },
            function getSalt(callback) {
                getHashRequest = 'SELECT Salt FROM Users WHERE UserName = "' + req.body.Username + '"';
                con.query(getHashRequest, function(err, results) {
                    if(err) {
                        console.log(err);
                    }
                    else if(results.length) {
                        callback(err, results[0].Salt);
                    }
                    else {
                        res.status(409);
                        res.send("Incorrect username or password.");
                        res.end();
                        return;
                    }
                });
            },
            function getHash(saltData, callback) {
                hashPassword(req.body.Password, saltData, function(err, hash) {
                    callback(err, hash);
                });
            },
            function checkLogin(hash, callback) {
                checkLoginRequest = 'SELECT UserName FROM Users WHERE UserName = "' + req.body.Username + '" AND Pass = "' + hash + '"';
                con.query(checkLoginRequest, function(err, results) {
                    if (!results.length) {
                        res.status(409);
                        res.send("Incorrect username or password.");
                        res.end();
                        return;
                    }
                    else {
                        callback(err);
                    }
                });
            }
        ],
        function complete(err)
        {
            if(err)
                console.log(err);
            else
            {
                res.status(200);
                res.send("Sign in successful.");
                res.end();
            }
            con.end();
            console.log("connection ended");
            /*con.end(function (err) {
                console.log(err);
            });*/
        });
    });
}
function hashPassword(password, salt, callback) {
    crypto.pbkdf2(password, new Buffer(salt, "hex"), config.cryptoConfig.iterations, config.cryptoConfig.hashBytes, config.cryptoConfig.digest, function (err, hash) {
        callback(err, hash.toString('hex'));
    });
}
