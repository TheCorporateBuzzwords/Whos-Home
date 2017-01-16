var config = require("./../config");
var mysql = require("mysql");
var auth = require('./../middlewares/auth');
var jwt = require('jsonwebtoken');

module.exports = function (app) {
    //Authenticated route to accept an invite to a group
    app.post('/groups/invitation', auth.CheckAuthToken, function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        if (req.body.invite_token) {
            jwt.verify(req.body.invite_token, config.JWTInfo.secret, function (error, invite_token_decoded) {
                if (req.body.decoded.UserID === invite_token_decoded.invitee) {
                    var insertQuery = "INSERT INTO User_Groups (UserID, GroupID) VALUES (" + invite_token_decoded.invitee + ", " + invite_token_decoded.group + ");";
                    var checkDupeQuery = "SELECT * FROM User_Groups WHERE UserID = " + invite_token_decoded.invitee + " AND GroupID = " + invite_token_decoded.group;
                    con.query(checkDupeQuery, function (err, result) {
                        if (result.length) {
                            res.status(409);
                            res.json({
                                status: "error",
                                message: "you are already part of this group."
                            });
                            res.end();
                        }
                        else {
                            con.query(insertQuery, function (err, result) {
                                if (err) {
                                    console.log(err);
                                }
                                else {
                                    res.status(200);
                                    res.json({
                                        status: "success",
                                        message: "successfully added to group"
                                    });
                                    res.end();
                                }
                            });
                        }
                    });
                }
                else {
                    res.status(401);
                    res.json({
                        status: "error",
                        message: "you do not have permission to accept this invite"
                    });
                    res.end();
                }
            });
        }
        else{
            res.status(400);
            res.json({
                status: "error",
                message: "missing paramater in POST request"
            });
        }
    });
}