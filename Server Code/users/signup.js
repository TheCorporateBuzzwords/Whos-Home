var config = require("./../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');
var validator = require('validator');
var jwt = require('jsonwebtoken');

module.exports = function (app) {
    app.post('/users/', function (req, res) {
        var token;
        var con = mysql.createConnection(config.connectionInfo);
        async.waterfall([
            //Check to see if passwords match and if all of the needed parameters were passed.
            function checkValidData(callback) {
                if (req.body.Username && req.body.Email && req.body.Password && req.body.Confirm && req.body.Firstname && req.body.Lastname) {
                    if (req.body.Password === req.body.Confirm) {
                        callback(null);
                    }
                    else {
                        res.status(409);
                        res.send("Passwords do not match.");
                        res.end();
                    }
                }
                else {
                    res.status(400);
                    res.send("Missing parameter(s) in POST request.");
                    res.end();
                }
            },
            //Check to make sure the username isn't in use and that it meets the requirements
            function checkUsername(callback) {
                if (!/^[a-z][a-z0-9]*$/i.test(req.body.Username)) {//(!/^[a-z0-9]+$/i.test(req.body.Username)) {
                    res.status(409);
                    res.send("Username contains invalid characters or begins with a number.");
                    res.end();
                }
                else if (req.body.Username.length > 20) {
                    res.status(409);
                    res.send("Username is too long.");
                    res.end();
                }
                else {
                    con.query('SELECT UserName FROM Users WHERE UserName = ' + con.escape(req.body.Username), function (err, result, field) {
                        if (!result) {
                            res.status(502);
                            res.json({
                                status: "error",
                                message: "failed to connect to SQL server"
                            });
                            res.end();
                        }
                        else if (!result.length) {
                            callback(err);
                        }
                        else {
                            res.status(409);
                            res.send("Username already in use.");
                            res.end();
                        }
                    });
                }
            },
            //Check to see if email is taken by another user and that the email is valid
            function checkEmail(callback) {
                if (!validator.isEmail(req.body.Email)) {
                    res.status(409);
                    res.send("Not a valid email.");
                    res.end();
                }
                else {
                    con.query('SELECT UserName FROM Users WHERE Email = ' + con.escape(req.body.Email), function (err, result, field) {
                        if (!result) {
                            res.status(502);
                            res.json({
                                status: "error",
                                message: "failed to connect to SQL server"
                            });
                        }
                        else if (!result.length) {
                            callback(err);
                        }
                        else {
                            res.status(409);
                            res.send("Email already in use.");
                            res.end();
                        }
                    });
                }
            },
            //Insert the user into the database
            function insertIntoDB(callback) {
                hashPassword(req.body.Password, function (err, hash, salt) {
                    var request = 'INSERT INTO Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Salt, Active, PushNot) values (null, ' + con.escape(req.body.Username) + ', ' + con.escape(req.body.Firstname) + ', ' + con.escape(req.body.Lastname) + ', ' + con.escape(req.body.Email) + ', \'' + hash + '\', \'' + salt + '\', false, false);';
                    con.query(request, function (err, result) {
                        if(!result) {
                            res.status(502);
                            res.json({
                                status: "error",
                                message: "failed to connect to SQL server"
                            });
                            res.end();
                        }
                        else {
                            callback(err);
                        }
                    });
                });
            },
            function getLatestID(callback) {
                var UserID;
                con.query("SELECT LAST_INSERT_ID() AS id", function (err, result, field) {
                    UserID = result[0].id;
                    callback(err, UserID);
                });
            }
        ],
            function complete(err, UserID) {
                if (err) {
                    console.log("error: ", err);
                }
                else {
                    var token = jwt.sign({
                        Username: req.body.Username,
                        Email: req.body.Email,
                        First: req.body.Firstname,
                        Last: req.body.Lastname,
                        UserID: UserID
                    }, config.JWTInfo.secret);
                    res.status(201);
                    res.json({
                        status: "success",
                        token: token
                    });
                    res.end();
                }
                con.end();
            });
    });

    function hashPassword(password, callback) {
        crypto.randomBytes(config.cryptoConfig.saltBytes, function (err, salt) {
            if (err) {
                return callback(err);
            }

            crypto.pbkdf2(password, salt, config.cryptoConfig.iterations, config.cryptoConfig.hashBytes, config.cryptoConfig.digest, function (err, hash) {
                callback(err, hash.toString('hex'), salt.toString('hex'));
            });
        });
    }
}