var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    //Authenticated route to accept an invite to a group
    app.get('/groups/:groupid(\\d+)/invitation', auth.CheckAuthToken, function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        var checkInvite = "SELECT * FROM Invites WHERE RecipientID = " + req.body.decoded.UserID + " AND GroupID = " + req.params.groupid;
        var insertQuery = "INSERT INTO User_Groups (UserID, GroupID) VALUES (" + req.body.decoded.UserID + ", " + req.params.groupid + ");";
        var checkDupeQuery = "SELECT * FROM User_Groups WHERE UserID = " + req.body.decoded.UserID + " AND GroupID = " + req.params.groupid;
        con.query(checkInvite, function (err, checkInviteResult) {
            if(err) {
                console.log(err);
                res.end();
            }
            else if(!checkInviteResult.length) {
                res.status(403).json({ status: "error", message: "you have not been invited to this group" });
            }
            con.query(checkDupeQuery, function(err, checkDupeResult) {
                if (err) {
                    console.log(err);
                    res.end();
                }
                else if (checkDupeResult.length) {
                    res.status(409).json({ status: "error", message: "you are already part of this group" });
                }
                con.query(insertQuery, function(err, insertResult) {
                    if(err) {
                        console.log(err);
                        res.end();
                    }
                    res.status(200).json({ status: "success", message: "you are now part of this group" });
                });
            });
        }); 
    });
}