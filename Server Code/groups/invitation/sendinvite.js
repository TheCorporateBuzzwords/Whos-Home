var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');
var jwt = require('jsonwebtoken');

module.exports = function (app) {
    //Authenticated route to receive an invite token to add someone to a group.
    app.get('/groups/:id(\\d+)/invitation/', auth.CheckAuthToken, function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        if (req.query.Recipient) {
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
                var checkInGroupQuery = "SELECT * FROM User_Groups WHERE UserID = " + req.body.decoded.UserID + " AND GroupID = " + req.params.id;
                con.query(checkInGroupQuery, function (err, result1) {
                    if (err) {
                        console.log(err);
                    } else if (result1.length) {
                        con.query(getUseridQuery, function (err, result2) {
                            if (err) {
                                console.log(err);
                            }
                            else if (result2.length) {
                                var token = jwt.sign({ 
                                    inviter: req.body.decoded.UserID,
                                    invitee: result2[0].UserID,
                                    group: req.params.id }, config.JWTInfo.secret);   
                                res.status(200);
                                res.json({ status: "success",
                                           invite_token: token });
                                res.end();
                            }
                            else {
                                //USER DOESN'T EXIST, FIX
                                console.log("user doesn't exist");
                            }
                        });
                    } else {
                        //FIX
                        console.log("error");
                    }
                });
            }
        }
        else {
            console.log("invalid username");
        }
    });
}