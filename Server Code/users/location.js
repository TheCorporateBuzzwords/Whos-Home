var config = require("./../config");
var mysql = require("mysql");
var auth = require('./../middlewares/auth');

module.exports = function (app) {
    app.put('/users/location', auth.CheckAuthToken, function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        if(!req.body.bssid || req.body.bssid == null) {
            res.status(409);
            res.json({
                status: "error",
                message: "missing bssid in request."
            });
        }
        var locationID = "NULL";
        var getLocationIDRequest = "SELECT LocationID \
                                    FROM Group_Locations \
                                    WHERE SSID = " + con.escape(req.body.bssid);
        con.query(getLocationIDRequest, function (err, locationIDResult) {
            if (err) {
                console.log(err);
            } else {
                if (locationIDResult.length) {
                    locationID = locationIDResult[0].LocationID;
                }
                var updateRequest = "UPDATE Users \
                                     SET LocationID = " + locationID + " \
                                     WHERE UserID = " + req.body.decoded.UserID;
                con.query(updateRequest, function (err, updateResult) {
                    if (err) {
                        console.log(err);
                    } else {
                        res.status(200);
                        res.json({
                            status: "success",
                            message: "updated user's location."
                        });
                    }
                });
            }
        });
    });
}