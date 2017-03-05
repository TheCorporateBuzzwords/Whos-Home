var config = require("./../config");
var mysql = require("mysql");
var jwt = require('jsonwebtoken');

module.exports =
    {
        CheckAuthToken: function (req, res, next) {
            var token = req.body.token || req.query.token || req.headers['x-access-token'];
            if (token) {
                jwt.verify(token, config.JWTInfo.secret, function (error, decoded) {
                    if (error) {
                        return res.status(403).json({ status: "Error", message: "Failed to authenticate token." });
                    } else {
                        req.body.decoded = decoded;
                        next();
                    }
                });
            }
            else {
                return res.status(403).json({ status: "Error", message: "no authenticity token provided" });
            }
        },
        CheckInGroup: function (req, res, next) {
            var groupid = req.body.groupid || req.params.groupid || req.query.groupid;
            //var con = mysql.createConnection(config.connectionInfo);
            if (groupid) {
                //MySQL statement for making sure some user is in some group acording to the linking table
                var checkInGroupQuery = "SELECT * FROM User_Groups WHERE UserID = " + req.body.decoded.UserID + " AND GroupID = " + config.pool.escape(groupid);
                config.pool.query(checkInGroupQuery, function (err, result) {
                    if (err) {
                        console.log(err);
                    } else if (result.length) {
                        next();
                    } else {
                        return res.status(200).json({ status: "error", message: "you are not part of this group." });
                    }
                });
            }
            else {
                return res.status(200).json({ status: "error", message: "no group ID supplied" });
            }
        }
    }