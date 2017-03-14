var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    app.get('/groups/:groupid(\\d+)/bills/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        if (req.query.recipient)
            var selectQuery = "SELECT BillID, GroupID, u1.UserName AS Sender, u2.UserName AS Recipient, CategoryID, Title, Description, Amount, DATE_FORMAT(DateDue, '%c/%d/%Y %r:%h:%s') AS DateDue \
                               FROM Bills b \
                               INNER JOIN Users u1 ON b.RecipientID = u1.UserID \
                               INNER JOIN Users u2 ON b.SenderID = u2.UserID \
                               WHERE GroupId = " + req.params.groupid + " AND RecipientId = " + req.query.recipient;
        else
            var selectQuery = "SELECT BillID, GroupID, u1.UserName AS Sender, u2.UserName AS Recipient, CategoryID, Title, Description, Amount, DATE_FORMAT(DateDue, '%c/%d/%Y %r:%h:%s') AS DateDue \
                               FROM Bills b \
                               INNER JOIN Users u1 ON b.RecipientID = u1.UserID \
                               INNER JOIN Users u2 ON b.SenderID = u2.UserID \
                               WHERE GroupId = " + req.params.groupid;
        console.log(selectQuery);
        config.pool.query(selectQuery, function(err, result) {
            if(err) {
                console.log(err);
                return res.end();
            }
            else {
                return res.status(200).json(result);
            }
        });
    });
}