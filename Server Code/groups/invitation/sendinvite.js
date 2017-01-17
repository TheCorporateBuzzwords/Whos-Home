var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');
var jwt = require('jsonwebtoken');

module.exports = function (app) {
    //Authenticated route to receive an invite token to add someone to a group.
    app.get('/groups/:groupid(\\d+)/invitation/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        if (req.query.Recipient) {
            console.log("test");
            var recipient = req.query.Recipient;
            if (!/^[a-z][a-z0-9]*$/i.test(recipient)) { //make sure it's a valid username. Should make a module for this.
                res.status(409);
                res.json({
                    status: "error",
                    message: "Invalid recipient."
                });
                res.end();
            } else {
                var getUseridQuery = "SELECT UserID FROM Users WHERE UserName = " + con.escape(recipient);
                //var checkInGroupQuery = "SELECT * FROM User_Groups WHERE UserID = " + req.body.decoded.UserID + " AND GroupID = " + req.params.id;
                con.query(getUseridQuery, function (err, result) {
                    if (err) {
                        console.log(err);
                    }
                    else if (result.length) {
                        var token = jwt.sign({ //create new jwt token for invitation
                            inviter: req.body.decoded.UserID,
                            invitee: result[0].UserID,
                            group: req.params.groupID
                        }, config.JWTInfo.secret);
                        res.status(200);
                        res.json({
                            status: "success",
                            invite_token: token
                        });
                        res.end();
                    }
                    else {
                        res.status(409);
                        res.json({
                            status: "error",
                            message: "user you tried inviting doesn't exist"
                        });
                    }
                });
            }
        }
        else {
            res.status(400);
            res.json({
                    status: "error",
                    message: "missing parameter"
            });
        }
    });
}