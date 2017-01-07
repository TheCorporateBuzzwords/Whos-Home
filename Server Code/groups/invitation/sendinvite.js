var config = require("./../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');
var validator = require('validator');
var jwt = require('jsonwebtoken');
var auth = require('./../middlewares/auth');

module.exports = function (app) {
    //Authenticated route to 
    app.get('/groups/invite/:id(\\d+)', auth.CheckAuthToken, function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        var recipient = req.params.recipient;
    });
}