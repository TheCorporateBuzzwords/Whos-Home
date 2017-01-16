var config = require("./../../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');
var validator = require('validator');
var jwt = require('jsonwebtoken');
var auth = require('./../../middlewares/auth');

//Export for creating a new group
module.exports = function (app) {
    app.post('/groups/', auth.CheckAuthToken, function (req, res) {
        //Create a connection to the database
        var con = mysql.createConnection(config.connectionInfo);

        //Check for valid information in incoming JSON
        if (req.body.groupName && req.body.userID) {
            //Call the add group procedure
            var insertRequest = "Call addGroup(" + con.escape(req.body.groupName) + ", " + con.escape(req.body.userID) + ")";

            //Question: Validate the token before creating a group? Or is that done in the app.post auth.CheckAuthToken?
            //Perform the request
            con.query(insertRequest, function(err, result) {
                if(err) {
                    //If error, log and handle
                    console.log(err);
                }
                else {
                    //If the group was added, pass back success message
                    res.status(200);
                    res.json({
                        status: "success",
                        message: "Successfully created group and added founding member"
                    });
                    res.end();
                }
            })
        }
        //If the request does not have the correct info, send back error message
        else {
            res.status(400);
            res.json({
                status: "error",
                message: "missing paramater in POST request"
            });            
        }
    });
}