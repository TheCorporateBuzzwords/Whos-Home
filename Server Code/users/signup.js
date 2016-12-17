var config = require("./../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto'); 

module.exports = function(app) {
    app.post('/users/', function (req, res) {
        var con = mysql.createConnection(config.connectionInfo); 
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
                        res.end();
                    }
                }
                else {
                    //callback(new Error('Missing parameter in POST request'));
                    res.status(400);
                    res.send("Missing parameter(s) in POST request.");
                    res.end();
                }
            },
            function checkUsername(callback) {
                if(!/^[a-z0-9]+$/i.test(req.body.Username)) {
                    res.status(409);
                    res.send("Username contains invalid characters.");
                    res.end();
                }
                else if (req.body.Username.length > 20) {
                    res.status(409);
                    res.send("Username is too long.");
                    res.end();
                } 
                else {
                    con.query('SELECT UserName FROM Users WHERE UserName = ' +  con.escape(req.body.Username), function (err, result, field) {
                        if(!result) {
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
            function checkEmail(callback) {
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
                        //callback(new Error('Email already in use'));
                        res.status(409);
                        res.send("Email already in use.");
                        res.end();
                    }
                });
            },
            function insertIntoDB(callback) {
                hashPassword(req.body.Password, function (err, hash, salt) {
                    //console.log(hash.length);
                    var request = 'INSERT INTO Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Salt, Active, PushNot) values (null, ' +  con.escape(req.body.Username) + ', ' + con.escape(req.body.Firstname) + ', ' + con.escape(req.body.Lastname) + ', ' + con.escape(req.body.Email) + ', \'' + hash + '\', \'' + salt + '\', false, false);';
                    con.query(request, function (err, result) {
                        if(!result)
                        {
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
                res.end();
            }
            con.end(function (err) {
                console.log(err);
            });
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