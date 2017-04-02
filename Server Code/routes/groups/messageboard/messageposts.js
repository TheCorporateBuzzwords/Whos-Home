var config = require("./../../../config");
var mysql = require("mysql");
var auth = require('./../../../middlewares/auth');
var router = require('express').Router();

//Post for adding a response to a specific messageboard topic in a group
router.post('/groups/:groupid(\\d+)/messages/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //Get a connection
    //var con = mysql.createConnection(config.connectionInfo);

    //Check to make sure all required info is present
    if (req.params.groupid && req.body.topicid && req.body.msg && req.body.decoded.UserID) {
        //Creat the request
        var insertRequest = "Insert Into Posts (TopicID, UserID, Msg, PostTime) values (" + config.pool.escape(req.body.topicid) + ", " + config.pool.escape(req.body.decoded.UserID) + "," + config.pool.escape(req.body.msg) + ", NOW())";

        //Perform the request
        config.pool.query(insertRequest, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200);
                res.json({
                    status: "success",
                    message: "Response successfully added to message board topic."
                });
                res.end();
            }
        });
    }
    //If not all required info is present give an error
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in POST request"
        });
    }
});

//Get for retreiving all responses to a specific messageboard topic in a group
router.get('/groups/:groupid(\\d+)/messages/:topicid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //Get a connection
    //var con = mysql.createConnection(config.connectionInfo);

    //Check to make sure all required info is present
    if (req.params.groupid && req.params.topicid) {
        //Create the request
        var getRequest = "Select PostID, Msg, PostTime, \
                                (Select UserName \
                                    From Users \
                                    Where UserID = p.UserID) as PostersName \
                                From Posts as p \
                                Where TopicID = " + config.pool.escape(req.params.topicid);

        //Perform the request
        config.pool.query(getRequest, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the request was made, return the message topic responses
            else {
                res.json(result);
                res.end();
            }
        });
    }
    //If not all required info is present give an error
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in POST request"
        });
    }
});
module.exports = router;