var config = require("./../../../config");
var mysql = require("mysql");
var auth = require('./../../../middlewares/auth');
var router = require('express').Router();

router.post('/groups/:groupid(\\d+)/bills', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    if (!(req.body.recipient && req.body.category && req.body.title && req.body.description && req.body.amount && req.body.date)) {
        return res.status(400).json({ status: "error", message: "missing parameter in POST request" });
    }
    //verify data types
    if (isNaN(req.body.category) || isNaN(req.body.amount) || isNaN(req.body.recipient)) {
        return res.status(400).json({ status: "error", message: "invalid data type for one or more parameters" });
    }
    var insertQuery = "INSERT INTO Bills (GroupID, SenderID, RecipientID, CategoryID, Title, Description, Amount, DateDue) \
                           VALUES (" + req.params.groupid + "," + req.body.decoded.UserID
        + "," + req.body.recipient + "," + req.body.category + "," + config.pool.escape(req.body.title) + ","
        + config.pool.escape(req.body.description) + "," + req.body.amount + "," + "STR_TO_DATE(" + config.pool.escape(req.body.date)
        + ", '%c/%d/%Y %r:%h:%s'));";
    config.pool.query(insertQuery, function (err, result) {
        if (err) {
            return res.status(500).json({ status: "error", message: "error processing request" });
        }
        else {
            return res.status(200).json({ status: "success", message: "successfully added bill" });
        }
    });
});
module.exports = router;