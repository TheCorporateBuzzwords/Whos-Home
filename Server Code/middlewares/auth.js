var config = require("./../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');
var jwt = require('jsonwebtoken');

module.exports =
    {
        CheckAuthToken: function (req, res, next) {
            var token = req.body.token || req.query.token || req.headers['x-access-token'];
            if (token) {
                jwt.verify(token, config.JWTInfo.secret, function (error, decoded) {
                    if (error) {
                        res.status(403);
                        res.json({
                            status: "Error",
                            message: "Failed to authenticate token."
                        });
                        res.end();
                    } else {
                        req.body.decoded = decoded;
                        next();
                    }
                });
            } 
            else {
                next();
            }
        }
    }