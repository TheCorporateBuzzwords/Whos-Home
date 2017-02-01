var config = require("./../../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');
var validator = require('validator');
var jwt = require('jsonwebtoken');
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    app.post('/groups/:groupid(\\d+)/location/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        if(req.params.groupid && req.body.ssid && req.body.locationName)
        {
            console.log("executing");
            var insertRequest = "INSERT INTO Group_Locations (GroupID, SSID, NetName) values (" + con.escape(req.params.groupid) + ", " + con.escape(req.body.ssid) + ", " + con.escape(req.body.locationName) + ");";
            con.query(insertRequest, function(err, result) {
                if(err) {
                    console.log(err);
                    return res.end();
                }
                else {
                    return res.status(200).json({ status: "success", message: "succesfully added location"});
                }
            });
        }
        else {
            return res.status(400).json({ status: "error", message: "missing parameter in POST request" });
        }
    });
}