var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    //Authenticated route to accept an invite to a group
    app.get('/groups/:groupid(\\d+)/invitation', auth.CheckAuthToken, function (req, res) {
        //var con = mysql.createConnection(config.connectionInfo);
        var checkInvite = "SELECT * FROM Invites WHERE RecipientID = " + req.body.decoded.UserID + " AND GroupID = " + req.params.groupid;
        var insertQuery = "INSERT INTO User_Groups (UserID, GroupID) VALUES (" + req.body.decoded.UserID + ", " + req.params.groupid + "); DELETE FROM Invites WHERE GroupID = " + req.params.groupid + " AND RecipientID = " + req.body.decoded.UserID;
        var checkDupeQuery = "SELECT * FROM User_Groups WHERE UserID = " + req.body.decoded.UserID + " AND GroupID = " + req.params.groupid;
        var deleteInvite = "DELETE FROM Invites WHERE GroupID = " + req.params.groupid + " AND RecipientID = " + req.body.decoded.UserID;
        config.pool.query(checkInvite, function (err, checkInviteResult) {
            if(err) {
                console.log(err);
                return res.end();
            }
            else if(!checkInviteResult.length) {
                //User hasn't been invited to the group
                return res.status(403).json({ status: "error", message: "you have not been invited to this group" });
            }
            //ability to deny the invitation
            if (req.query.deny === "true") {
                config.pool.query(deleteInvite, function(err, denyResult) {
                    if(err) {
                        console.log(err);
                        return res.end();
                    }
                    return res.status(200).json({ status: "success", message: "invitation denied" });
                });
            }
            config.pool.query(checkDupeQuery, function(err, checkDupeResult) {
                if (err) {
                    console.log(err);
                    return res.end();
                }
                else if (checkDupeResult.length) {
                    //User has invitation, but is already part of the group.
                    return res.status(409).json({ status: "error", message: "you are already part of this group" });
                }
                config.pool.query(insertQuery, function(err, insertResult) {
                    if(err) {
                        console.log(err);
                        return res.end();
                    }
                    return res.status(200).json({ status: "success", message: "you are now part of this group" });
                });
            });
        }); 
    });
}