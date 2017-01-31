var config = require("./../config");
var mysql = require("mysql");
var jwt = require('jsonwebtoken');

module.exports =
    {
        CheckAuthToken: function (req, res, next) {
            console.log(req.body);
            var token = req.body.token || req.query.token || req.headers['x-access-token'];
            if (token) {
                jwt.verify(token, config.JWTInfo.secret, function (error, decoded) {
                    if (error) {
                        res.status(403);
                        res.json({
                            status: "Error",
                            message: "Failed to authenticate token."
                        });
                        res.end();
                    } else {
                        req.body.decoded = decoded;
                        console.log("decoded", decoded);
                        next();
                    }
                });
            }
            else {
                res.status(403).json({ status: "Error", message: "no authenticity token provided" });
            }
        },
        CheckInGroup: function (req, res, next) {
            var groupid = req.body.groupid || req.params.groupid || req.query.groupid;
            var con = mysql.createConnection(config.connectionInfo);
            if (groupid) {
                //MySQL statement for making sure some user is in some group acording to the linking table
                var checkInGroupQuery = "SELECT * FROM User_Groups WHERE UserID = " + req.body.decoded.UserID + " AND GroupID = " + con.escape(groupid);
                con.query(checkInGroupQuery, function (err, result) {
                    if (err) {
                        console.log(err);
                    } else if (result.length) {
                        next();
                    } else {
                        res.status(200);
                        res.json({
                            status: "error",
                            message: "you are not part of this group."
                        });
                        res.end();
                    }
                });
            }
            else {
                res.status(200);
                res.json({
                    status: "error",
                    message: "no group ID supplied"
                });
                res.end();
            }
        }
    }