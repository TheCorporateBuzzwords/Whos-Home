var config = require("./../../../config");
var mysql = require("mysql");
var auth = require('./../../../middlewares/auth');
var router = require('express').Router();

//Endpoint for editing a single item in a list
router.put('/groups/:groupid(\\d+)/elistitem/:itemid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed information
    if (req.body.decoded.UserID && req.params.groupid && req.params.itemid && req.body.newText) {
        //Create the request
        var editRequst = "Update Items \
                              Set ItemText = " + config.pool.escape(req.body.newText)
            + "Where ItemID = " + config.pool.escape(req.params.itemid) + ";";

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
                    message: "Edit successfully made to list item."
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

//Endpoint for deleting a single item in a list
router.delete('/groups/:groupid(\\d+)/dlistitem/:itemid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed information
    if (req.body.decoded.UserID && req.params.groupid && req.params.itemid) {
        //Create the request
        var deleteRequest = "Delete From Items \
                                 Where ItemID = " + config.pool.escape(req.params.itemid) + ";";

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
                    message: "Successfully deleted list item."
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

//Endpoint for editing a list
router.put('/groups/:groupid(\\d+)/elist/:listid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed information
    if (req.body.decoded.UserID && req.params.groupid && req.params.listid && req.body.newTitle) {
        var editRequst = "Update Lists \
                              Set Title = " + config.pool.escape(req.body.newTitle)
            + "Where ListID = " + config.pool.escape(req.params.listid)
            + "And GroupID = " + config.pool.escape(req.params.groupid) + ";";

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
                    message: "Edit successfully made to list."
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

//Endpoint for deleting a list and all items within that list
router.delete('/groups/:groupid(\\d+)/dlist/:listid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed info
    if (req.body.decoded.UserID && req.params.groupid && req.params.listid) {
        var deleteRequest = "Call deleteList(" + config.pool.escape(req.params.listid) + ");";

        config.pool.query(deleteRequest, function (err, result) {
            //Leg any errors that happened
            if (err) {
                console.log(err);
            }
            else {
                res.status(200);
                res.json({
                    status: "success",
                    message: "Successfully deleted list and all items in list."
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