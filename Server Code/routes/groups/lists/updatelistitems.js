var config = require("./../../../config");
var mysql = require("mysql");
var auth = require('./../../../middlewares/auth');
var router = require('express').Router();

router.put('/groups/:groupid(\\d+)/lists/:listid(\\d+)', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //var con = mysql.createConnection(config.connectionInfo);
    if (req.body.itemid && req.body.completed) {
        if (req.body.completed != 1 && req.body.completed != 0) {
            return res.status(400).json({ status: "error", message: "invalid completed value" });
        }
        var updateRequest = "UPDATE Items SET Completed = " + req.body.completed + " WHERE ItemID = " + req.body.itemid;
        config.pool.query(updateRequest, function (err, result) {
            if (err) {
                console.log(err);
                return res.end();
            }
            else {
                return res.status(200).json({ status: "success", message: "successfully updated values" });
            }
        });
    }
    else {
        return res.status(400).json({ status: "error", message: "missing parameter in POST request" });
    }
});
module.exports = router;