var config = require("./config.js");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');

var config = {
    hashBytes: 64,
    saltBytes: 16,
    // more iterations means an attacker has to take longer to brute force an
    // individual password, so larger is better. however, larger also means longer
    // to hash the password. tune so that hashing the password takes about a
    // second
    iterations: 872791,
    digest: 'sha512'
};   

var connectionInfo = {
    host: "96.41.173.205",
    user: "limited",
    password: "Speci@login$$$69$$$",
    database: "WHOSHOME"
}

module.exports = function(app) {
    app.post('/users/', function (req, res) {
        var con = mysql.createConnection(connectionInfo); 
        async.waterfall([
            function checkValidData(callback) {
                if (req.body.Username && req.body.Email && req.body.Password && req.body.Confirm && req.body.Firstname && req.body.Lastname) {
                    if (req.body.Password == req.body.Confirm) {
                        callback(null);
                    }
                    else {
                        //callback(new Error('Passwords do not match'));
                        res.status(409);
                        res.send("Passwords do not match.");
                        return;
                    }
                }
                else {
                    //callback(new Error('Missing parameter in POST request'));
                    res.status(400);
                    res.send("Missing parameter(s) in POST request.");
                    return;
                }
            },
            function checkUsername(callback) {
                console.log(mysql.escape(req.body.Username));
                con.query('SELECT UserName FROM Users WHERE UserName = "' +  req.body.Username + '"', function (err, result, field) {
                    if (!result.length) {
                        callback(err);
                    }
                    else {
                        //callback(new Error('Username already in use'));
                        res.status(409);
                        res.send("Username already in use.");
                        return;
                    }
                });
            },
            function checkEmail(callback) {
                con.query('SELECT UserName FROM Users WHERE Email = "' + req.body.Email + '"', function (err, result, field) {
                    if (!result.length) {
                        callback(err);
                    }
                    else {
                        //callback(new Error('Email already in use'));
                        res.status(409);
                        res.send("Email already in use.");
                        return;
                    }
                });
            },
            function insertIntoDB(callback) {
                hashPassword(req.body.Password, function (err, hash, salt) {
                    //console.log(hash.length);
                    var request = 'INSERT INTO Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Salt, Active, PushNot) values (null, "' +  req.body.Username + '", "' + req.body.Firstname + '", "' + req.body.Lastname + '", "' + req.body.Email + '", "' + hash + '", "' + salt + '", false, false);';
                    con.query(request, function (err) {
                        callback(err);
                    });
                });
            }
        ],
        function complete(err)
        {
            if(err)
            {
                console.log("error: ", err);
            }
            else
            {
                res.status(201);
                res.send("Sign up successful.");
            }
            con.end(function (err) {
                console.log(err);
            });
        });
    });

    function hashPassword(password, callback) {
        crypto.randomBytes(config.saltBytes, function (err, salt) {
            if (err) {
                return callback(err);
            }

            crypto.pbkdf2(password, salt, config.iterations, config.hashBytes, config.digest, function (err, hash) {
                callback(err, hash.toString('hex'), salt.toString('hex'));
            });
        });
    }
}