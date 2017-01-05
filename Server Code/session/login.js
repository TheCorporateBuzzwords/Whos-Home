var config = require("./../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');
var jwt = require('jsonwebtoken');

module.exports = function(app) {
    app.post('/session/', function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        var email, firstName, last, id;
        console.log(req.body);
        async.waterfall([
            function checkValidData(callback) {
                if (req.body.Username && req.body.Password) {
                    callback(null);
                }
                else {
                    res.status(400);
                    res.json({
                            status: "error",
                            message: "Missing parameter in POST request"
                    });
                    res.end();
                }
            },
            function getSalt(callback) {
                getHashRequest = 'SELECT Salt FROM Users WHERE UserName = ' + con.escape(req.body.Username);
                con.query(getHashRequest, function(err, result) {
                    if(!result) {
                        res.status(502);
                        res.json({
                            status: "error",
                            message: "failed to connect to SQL server"
                        });
                        res.end();
                    }
                    else if(err) {
                        console.log(err);
                    }
                    else if(result.length) {
                        callback(err, result[0].Salt);
                    }
                    else {
                        res.status(409);
                        res.send("Incorrect username or password.");
                        res.end();
                    }
                });
            },
            function getHash(saltData, callback) {
                hashPassword(req.body.Password, saltData, function(err, hash) {
                    callback(err, hash);
                });
            },
            function checkLogin(hash, callback) {
                checkLoginRequest = 'SELECT UserID, Email, FirstName, LastName FROM Users WHERE UserName = ' + con.escape(req.body.Username) + ' AND Pass = \'' + hash + '\'';
                con.query(checkLoginRequest, function(err, result) {
                    if(!result) {
                        res.status(502);
                        res.json({
                            status: "error",
                            message: "failed to connect to SQL server"
                        });
                        res.end();
                    }
                    else if (!result.length) {
                        res.status(409);
                        res.send("Incorrect username or password.");
                        res.end();
                    }
                    else {
                        email = result[0].Email;
                        first = result[0].FirstName;
                        last = result[0].LastName;
                        id = result[0].UserID;
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
                var token = jwt.sign({ Username: req.body.Username,
                                       First: first,
                                       Email: email,
                                       Last: last,
                                       UserID: id }, config.JWTInfo.secret);   
                res.status(200);
                res.json({ status: "success",
                           token: token });
                res.end();
            }
            con.end();
        });
    });
}
function hashPassword(password, salt, callback) {
    crypto.pbkdf2(password, new Buffer(salt, "hex"), config.cryptoConfig.iterations, config.cryptoConfig.hashBytes, config.cryptoConfig.digest, function (err, hash) {
        callback(err, hash.toString('hex'));
    });
}
