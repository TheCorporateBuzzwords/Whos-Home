var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function(app) {
    //Authenticated route to create an invitation to a group
    app.post('/groups/:groupid(\\d+)/invitation/', [auth.CheckAuthToken, auth.CheckInGroup], function(req, res) {
        //var con = mysql.createConnection(config.connectionInfo);
        if (!req.body.recipient) {
            return res.status(400).json({ status: "error", message: "missing parameter" }).end();
        }
        var recipient = req.body.recipient;
        if (!/^[a-z][a-z0-9]*$/i.test(recipient)) { //make sure it's a valid username. Should make a module for this.
            return res.status(409).json({ status: "error", message: "Invalid recipient." }).end();
        }
        //get userid
        var getUseridQuery = "SELECT UserID FROM Users WHERE UserName = " + config.pool.escape(recipient);
        config.pool.query(getUseridQuery, function(err, result) {
            if (err) {
                console.log(err);
                return res.end();
            }
            else if (result.length) {
                var recipientID = result[0].UserID;
                //insert invitation into database
                var insertQuery = "INSERT INTO Invites (GroupID, InviterID, RecipientID) VALUES (" + req.params.groupid + ", " + req.body.decoded.UserID + ", " + recipientID + ");";
                config.pool.query(insertQuery, function(err, result) {
                    if (err) {
                        console.log(err);
                        return res.end();
                    } else {
                        return res.status(200).json({ status: "success", message: "invitation created" }).end();
                    }
                });
            }
            else {
                return res.status(409).json({ status: "error", message: "user you tried inviting doesn't exist" }).end();
            }
        });
    });
}