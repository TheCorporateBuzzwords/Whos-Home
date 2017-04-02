var config = require("./../../../config");
var mysql = require("mysql");
var auth = require('./../../../middlewares/auth');
var router = require('express').Router();

//Endpoint for editing a message board topic
router.put('/groups/:groupid(\\d+)/topicedit/:topicid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //Get a connection
    //var con = mysql.createConnection(config.connectionInfo);

    //Check for all needed info
    if (req.body.decoded.UserID && req.params.groupid && req.params.topicid && req.body.newTitle) {
        //Create the request
        var editRequest = "Update Message_Topics \
                              Set Title = " + config.pool.escape(req.body.newTitle)
            + "Where TopicID = " + config.pool.escape(req.params.topicid)
            + "And GroupID = " + config.pool.escape(req.params.groupid) + ";";

        config.pool.query(editRequest, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200);
                res.json({
                    status: "success",
                    message: "Edit successfully made to message board topic."
                });
                res.end();
            }
        });
    }
    //If the request is missing params, send back an error
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in PUT request"
        });
    }
});

//Ednpoint for editing a message board topic response
router.put('/groups/:groupid(\\d+)/postedit/:postid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed info
    if (req.body.decoded.UserID && req.params.groupid && req.params.postid && req.body.newMsg) {

        //Create the request
        var editRequst = "Update Posts \
                              Set Msg = " + config.pool.escape(req.body.newMsg)
            + "Where PostID = " + config.pool.escape(req.params.postid) + ";";

        config.pool.query(editRequst, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200);
                res.json({
                    status: "success",
                    message: "Edit successfully made to the message board response."
                });
                res.end();
            }
        });
    }
    //If the request is missing params, send back an error
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in PUT request"
        });
    }
});

//Endpoint for deleting an entire message board topic and all responses
router.delete('/groups/:groupid(\\d+)/topicdelete/:topicid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //Check for all needed info
    if (req.body.decoded.UserID && req.params.groupid && req.params.topicid) {

        //Create the request
        var deleteRequest = "Call deleteTopic(" + config.pool.escape(req.params.topicid) + ");";

        config.pool.query(deleteRequest, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200);
                res.json({
                    status: "success",
                    message: "Successfully deleted message board topic."
                });
                res.end();
            }
        });
    }
    //If the request is missing params, send back an error
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in DELETE request"
        });
    }
});

//Endpoint for deleting a message board response
router.delete('/groups/:groupid(\\d+)/postdelete/:postid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //Check for all needed info
    if (req.body.decoded.UserID && req.params.groupid && req.params.postid) {
        var deleteRequest = "Delete from Posts \
                                 Where PostID = " + config.pool.escape(req.params.postid) + ";";

        config.pool.query(deleteRequest, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200);
                res.json({
                    status: "success",
                    message: "Successfully deleted message board response."
                });
                res.end();
            }
        });
    }
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in DELETE request"
        });
    }
});

module.exports = router;