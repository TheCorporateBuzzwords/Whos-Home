var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    app.post('/groups/:groupid(\\d+)/bills', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        if(req.body.recipient, req.body.category, req.body.title, req.body.description, req.body.amount, req.body.date)
        {
            var insertQuery = "INSERT INTO Bills (GroupID, SenderID, RecipientID, CategoryID, Title, Description, Amount, DateDue) \
                               VALUES (" + req.params.groupid + "," + req.body.decoded.UserID
                                + "," + req.body.recipient + "," + req.body.category + "," + config.pool.escape(req.body.title) + "," 
                                + config.pool.escape(req.body.description) + "," + req.body.amount + "," + "STR_TO_DATE(" + config.pool.escape(req.body.date)
                                + ", '%c/%d/%Y %r:%h:%s'));";
            console.log(insertQuery);
            config.pool.query(insertQuery, function(err, result) {
                if(err) {
                    console.log(err);
                    return res.end();
                }
                else {
                    return res.status(200).json({ status: "success", message: "successfully added bill." });
                }
            });
        }
    });
}