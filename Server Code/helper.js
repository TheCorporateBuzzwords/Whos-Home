var config = require("./config");
var mysql = require("mysql");
var router = require('express').Router();

module.exports = {
    getRegToken: function (userid, callback) {
        var selectStatement = "SELECT FirebaseToken FROM Users WHERE UserID = " + userid + ";";
        config.pool.query(selectStatement, function (err, result) {
            if(err) {
                return callback(err);
            }
            callback(null, result[0].FirebaseToken);
        });
    },
    getUsernameFromId: function (userid, callback) {
        var selectStatement = "SELECT UserName FROM Users WHERE UserID = " + userid + ";";
        config.pool.query(selectStatement, function(err, result) {
            if(err) {
                return callback(err);
            }
            callback(null, result[0].UserName);
        });
    },
    getUserIdFromUsername: function (username, callback) {
        var selectStatement = "SELECT UserID FROM Users WHERE UserName = " + config.pool.escape(username) + ";";
        config.pool.query(selectStatement, function(err, result) {
            if(err) {
                return callback(err);
            }
            callback(null, result[0].UserID);
        }); 
    },
    sendNotification: function (userid, title, message) {
        getRegToken(userid, function(err, token) {
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
            if(err)
            {
                console.log(err);
            }
            else
            {
                admin.messaging().sendToDevice(registrationToken, payload, options)
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