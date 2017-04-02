var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');
var router = require('express').Router();

//Endpoint for editing the name of the group
router.put('/groups/:groupid(\\d+)/egroup/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed info
    if (req.body.decoded.UserID && req.params.groupid && req.body.newName) {
        var editRequest = "Update Groups \
                               Set GroupName = " + config.pool.escape(req.body.newName)
            + "Where GroupID = " + config.pool.escape(req.params.groupid) + ";";

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
                    message: "Edit successfully made to Group name."
                });
                res.end();
            }
        });
    }
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in PUT request"
        });
    }
});

//Endpoint for removing a single user from the group
router.delete('/groups/:groupid(\\d+)/duser/:userid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed info
    if (req.body.decoded.UserID && req.params.groupid && req.params.userid) {
        var deleteRequest = "";

        config.pool.query(deleteRequest, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200);
                res.json({
                    status: "Error",
                    message: "Endpoint not finished."
                    //status: "success",
                    //message: "User successfully removed from Group."
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