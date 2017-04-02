var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');
var router = require('express').Router();

router.put('/users/location', auth.CheckAuthToken, function (req, res) {
    //var con = mysql.createConnection(config.connectionInfo);
    if (!req.body.bssid || req.body.bssid == null) {
        return res.status(409).json({ status: "error", message: "missing bssid in request." });
    }
    var locationID = "NULL";
    var getLocationIDRequest = "SELECT LocationID \
                                    FROM Group_Locations \
                                    WHERE SSID = " + config.pool.escape(req.body.bssid);
    config.pool.query(getLocationIDRequest, function (err, locationIDResult) {
        if (err) {
            console.log(err);
        } else {
            if (locationIDResult.length > 0) {
                locationID = locationIDResult[0].LocationID;
            }
            var updateRequest = "UPDATE Users \
                                     SET LocationID = " + locationID + " \
                                     WHERE UserID = " + req.body.decoded.UserID;
            config.pool.query(updateRequest, function (err, updateResult) {
                if (err) {
                    console.log(err);
                } else {
                    return res.status(200).json({ status: "success", message: "updated user's location." });
                }
            });
        }
    });
});
module.exports = router;