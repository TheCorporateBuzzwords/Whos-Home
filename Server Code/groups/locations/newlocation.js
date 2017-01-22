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
                console.log("Executed");
                if(err) {
                    console.log(err);
                }
                else {
                    res.status(200);
                    res.json({
                        status: "success",
                        message: "succesfully added location" 
                    });
                    res.end();
                }
            });
        }
        else {
            res.status(400);
            res.json({
                status: "error",
                message: "missing parameter in POST request"
            });
            res.end();
        }
    });
}