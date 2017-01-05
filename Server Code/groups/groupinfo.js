var config = require("./../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');
var validator = require('validator');
var jwt = require('jsonwebtoken');
var auth = require('./../middlewares/auth');

module.exports = function (app) {
    //Authenticated route to get group information from group id
    //(\\d+) makes sure :id is an integer.
    app.get('/groups/:id(\\d+)', auth.CheckAuthToken, function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        var request = "SELECT u.UserName, g.GroupName, sl.NetName \
                       FROM Users u \
                       INNER JOIN User_Group ug ON u.UserID = ug.UserID \
                       INNER JOIN Groups g ON ug.GroupID = g.GroupID \
                       INNER JOIN UserLocations ul ON u.LocationsID = ul.LocationsID \
                       INNER JOIN SharedLocations sl ON ul.LocationsID = sl.LocationsID \
                       WHERE g.GroupID = " + con.escape(req.params.id); + " \
                       AND u.UserID = " + req.body.decoded.
        if (req.body.decoded)
        {
            console.log("test1");
            res.send(req.body.decoded);
            res.end();
        }
    });
}