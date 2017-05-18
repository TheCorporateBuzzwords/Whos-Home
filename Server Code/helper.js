var config = require("./config");
var mysql = require("mysql");
var router = require('express').Router();
var admin = require("firebase-admin");

module.exports = {
    getRegToken: function (userid, callback) {
        var selectStatement = "SELECT FirebaseToken FROM Users WHERE UserID = " + userid + ";";
        config.pool.query(selectStatement, function (err, result) {
            if(err) {
                return callback(err);
            }
            else if (result.length) {
                callback(null, result[0].FirebaseToken);
            }
            else {
                callback(new Error("User does not exist"));
            }
        });
    },
    getUsernameFromId: function (userid, callback) {
        var selectStatement = "SELECT UserName FROM Users WHERE UserID = " + userid + ";";
        config.pool.query(selectStatement, function(err, result) {
            if(err) {
                return callback(err);
            }
            else if(result.length) {
                return callback(null, result[0].UserName);
            }
            else {
                callback(new Error("User does not exist"));
            }
        });
    },
    getUserIdFromUsername: function (username, callback) {
        var selectStatement = "SELECT UserID FROM Users WHERE UserName = " + config.pool.escape(username) + ";";
        config.pool.query(selectStatement, function(err, result) {
            if(err) {
                return callback(err);
            }
            else if(result.length) {
                return callback(null, result[0].UserID);
            }
            else {
                return callback(new Error("Username doesn't exist"));
            }
        }); 
    },
    sendNotification: function (userid, title, message) {
        module.exports.getRegToken(userid, function(err, token) {
            var payload = {
                notification: {
                    title: title,
                    body: message
                }
            };
            var options = {
                priority: "high",
                timeToLive: 60 * 60 * 24
            };
            if(!err)
            {
                admin.messaging().sendToDevice(token, payload, options)
                    .then(function(response) {
                        console.log("Successfully sent message:", response);
                    })
                    .catch(function(error) {
                        console.log("Error sending message:", error);
                    });
            }
        });
    }
};