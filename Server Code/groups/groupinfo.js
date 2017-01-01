var config = require("./../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto'); 
var validator = require('validator');
var jwt = require('jsonwebtoken');

module.exports = function(app) {
    app.get('/groups/:id', function (req, res) {
       var groupid = req.params.id;
       console.log(groupid); 
    });
}