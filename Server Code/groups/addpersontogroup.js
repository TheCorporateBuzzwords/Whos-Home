var config = require("./../config");
var mysql = require("mysql");
var auth = require('./../middlewares/auth');
var jwt = require('jsonwebtoken');

module.exports = function (app) {
    //Authenticated route to accept an invite to a group
    app.post('/groups/:id(\\d+)', auth.CheckAuthToken, function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        jwt.verify(req.body.invite_token, config.JWTInfo.secret, function (error, invite_token_decoded) {
            if (req.body.decoded.UserID === invite_token_decoded.invitee && req.params.id === invite_token_decoded.group) {
                var insertQuery = "INSERT INTO User_Group (UserID, GroupID) VALUES (" + invite_token_decoded.invitee + ", " + invite_token_decoded.group + ");";
                var checkDupeQuery = "SELECT * FROM User_Group WHERE UserID = " + invite_token_decoded.invitee + " AND GroupID = " + invite_token_decoded.group;
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
                res.status(200);
                res.json({
                    status: "error",
                    message: "you do not have permission to accept this invite"
                });
                res.end();
            }
        });
    });
}