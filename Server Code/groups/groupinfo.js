var config = require("./../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');
var validator = require('validator');
var jwt = require('jsonwebtoken');
var auth = require('./../middlewares/auth');

module.exports = function (app) {
    app.get('/groups/:id', auth.CheckAuthToken, function (req, res) {
        var groupid = req.params.id;
        console.log(req.body.decoded);
        if (req.body.decoded)
        {
            console.log("test1");
            res.send(req.body.decoded);
            res.end();
        }
        else
            console.log("test2");
    });
}