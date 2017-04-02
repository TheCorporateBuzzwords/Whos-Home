var config = require("./../../../config");
var mysql = require("mysql");
var auth = require('./../../../middlewares/auth');
var router = require('express').Router();

router.post('/groups/:groupid(\\d+)/location/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //var con = mysql.createConnection(config.connectionInfo);
    if (req.params.groupid && req.body.ssid && req.body.locationName) {
        console.log("executing");
        var insertRequest = "INSERT INTO Group_Locations (GroupID, SSID, NetName) values (" + config.pool.escape(req.params.groupid) + ", " + config.pool.escape(req.body.ssid) + ", " + config.pool.escape(req.body.locationName) + ");";
        config.pool.query(insertRequest, function (err, result) {
            if (err) {
                console.log(err);
                return res.end();
            }
            else {
                return res.status(200).json({ status: "success", message: "succesfully added location" });
            }
        });
    }
    else {
        return res.status(400).json({ status: "error", message: "missing parameter in POST request" });
    }
});
module.exports = router;