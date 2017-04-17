var config = require("./../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');
var validator = require('validator');
var jwt = require('jsonwebtoken');
var auth = require('./../middlewares/auth');
var router = require('express').Router();

router.get('/groups', auth.CheckAuthToken, function (req, res) {
    //Check that the needed stuff is in the JSON
    if (req.body.decoded) {
        var getRequest = "Select g.GroupID, g.GroupName" +
                                "From Users as u Inner Join User_Groups as ug" +
                                "On u.UserID = ug.UserID" +
                                "Inner Join Groups as g" +
                                "on ug.GroupID = g.GroupID" +
                                "Where u.UserID = " + req.body.decoded.UserID + ";";

        config.pool.query(getRequest, function (err, result) {
            if (err) {
                //If error, log and handle
                console.log(err);
            }
            else {
                return res.json(result);
            }
        });
    }
    else {
        return res.status(400).json({ status: "error", message: "missing parameter in GET request" });
    }
});
router.get('/invites', auth.CheckAuthToken, function (req, res) {
    //Check that everything needed is passed in
    if (req.body.decoded) {
        var getRequest = "Select (Select UserName from Users as t where t.UserID = u.InviterID) as UserName" +
                            ", (Select FirstName From Users as t Where t.UserID = u.InviterID) as FirstName" +
                            ", (Select LastName From Users as t Where t.UserID = u.InviterID) as LastName" +
                            ", u.GroupID" +
                            ", (Select GroupName From Groups as t Where t.GroupID = u.GroupID) as GroupName" +
                            "From Invites as u" +
                            "Where RecipientID = " + req.body.decoded.UserID + ";";

        config.pool.query(getRequest, function (err, result) {
            if (err) {
                console.log(err);
            }
            else {
                return res.json(result);
            }
        });
    }
    else {
        return res.status(400).json({ status: "error", message: "missing parameter in GET request" });
    }
});

router.put('/location', auth.CheckAuthToken, function (req, res) {
    
    if (!req.body.bssid || req.body.bssid === null) {
        return res.status(409).json({ status: "error", message: "missing bssid in request." });
    }
    // var locationID = "NULL";
    // var getLocationIDRequest = "SELECT LocationID" +
    //                                 "FROM Group_Locations" +
    //                                 "WHERE SSID = " + config.pool.escape(req.body.bssid);
    // config.pool.query(getLocationIDRequest, function (err, locationIDResult) {
    //     if (err) {
    //         console.log(err);
    //     } else {
    //         if (locationIDResult.length > 0) {
    //             locationID = locationIDResult[0].LocationID;
    //         }
    //         var updateRequest = "UPDATE Users" +
    //                                  "SET LocationID = " + locationID +
    //                                  "WHERE UserID = " + req.body.decoded.UserID;
    //         config.pool.query(updateRequest, function (err, updateResult) {
    //             if (err) {
    //                 console.log(err);
    //             } else {
    //                 return res.status(200).json({ status: "success", message: "updated user's location." });
    //             }
    //         });
    //     }
    // });
    var updateRequest = "Call updateUserLocations(" + req.body.decoded.UserID + ", " + config.pool.escape(req.body.bssid) + ");";

    config.pool.query(updateRequest, function (err, result) {
        if(err) {
            console.log(err);
        }
        else {
            return res.status(200).json({ status: "success", message: "updated user's locations." });
        }
    });
});

router.post('/', function (req, res) {
    var token;
    
    async.waterfall([
        //Check to see if passwords match and if all of the needed parameters were passed.
        function checkValidData(callback) {
            if (req.body.Username && req.body.Email && req.body.Password && req.body.Confirm && req.body.Firstname && req.body.Lastname) {
                if (req.body.Password === req.body.Confirm) {
                    callback(null);
                }
                else {
                    res.status(409).json({ status: "error", message: "Passwords do not match." });
                }
            }
            else {
                res.status(400).json({ status: "error", message: "Missing parameter(s) in POST request." });
            }
        },
        //Check to make sure the username isn't in use and that it meets the requirements
        function checkUsername(callback) {
            if (!/^[a-z][a-z0-9]*$/i.test(req.body.Username)) {//(!/^[a-z0-9]+$/i.test(req.body.Username)) {
                res.status(409).json({ status: "error", message: "Username contains invalid characters or begins with a number." });
            }
            else if (req.body.Username.length > 20) {
                res.status(409).json({ status: "error", message: "Username is too long." });
            }
            else {
                //Get the username of a user from the users table using the new username. This is a check for if the username is already taken
                config.pool.query('SELECT UserName FROM Users WHERE UserName = ' + config.pool.escape(req.body.Username), function (err, result, field) {
                    if (!result.length) {
                        callback(err);
                    }
                    else {
                        res.status(409).json({ status: "error", message: "Username already in use." });
                    }
                });
            }
        },
        //Check to see if email is taken by another user and that the email is valid
        function checkEmail(callback) {
            if (!validator.isEmail(req.body.Email)) {
                res.status(409).json({ status: "error", message: "Not a valid email." });
            }
            else {
                //Get the email of a user from the users table using the new email. This is a check for if the email is already taken
                config.pool.query('SELECT UserName FROM Users WHERE Email = ' + config.pool.escape(req.body.Email), function (err, result, field) {
                    if (!result.length) {
                        callback(err);
                    }
                    else {
                        res.status(409).json({ status: "error", message: "Email already in use." });
                    }
                });
            }
        },
        //Insert the user into the database
        function insertIntoDB(callback) {
            hashPassword(req.body.Password, function (err, hash, salt) {
                var request = 'INSERT INTO Users (UserName, FirstName, LastName, Email, Pass, Salt, Active, PushNot, LocationID, LocationActive) values (' + config.pool.escape(req.body.Username) + ', ' + config.pool.escape(req.body.Firstname) + ', ' + config.pool.escape(req.body.Lastname) + ', ' + config.pool.escape(req.body.Email) + ', \'' + hash + '\', \'' + salt + '\', false, false, null, false);';
                config.pool.query(request, function (err, result) {
                    if (!result) {
                        res.status(502).json({ status: "error", message: "failed to connect to SQL server" });
                    }
                    else {
                        callback(err);
                    }
                });
            });
        },
        function getLatestID(callback) {
            //var UserID;
            //config.pool.query("SELECT LAST_INSERT_ID() AS id", function (err, result, field) {
            config.pool.query("SELECT UserID From Users Where UserName = " + config.pool.escape(req.body.Username), function (err, result, field) {
                //UserID = result[0].UserID;
                callback(err, result[0].UserID);
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
                res.status(201).json({ status: "success", token: token });
            }
            //con.end();
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
module.exports = router;