var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    //Post for adding a new messageboard topic and the inital response to a group
    app.post('/groups/:groupid(\\d+)/messagetopic/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        //Get a connection
        var con = mysql.createConnection(config.connectionInfo);

        //check that all required information is passed
        if(req.params.groupid && req.body.title && req.body.decoded.UserID && req.body.msg)
        {
            //Create a var that has the call to the procedure
            var insertRequest = "Call addTopic(" + con.escape(req.params.groupid) + ", " + con.escape(req.body.title) + ", " + con.escape(req.body.decoded.UserID) + ", " + con.escape(req.body.msg) + ")";
            
            //Perform the request
            con.query(insertRequest, function(err, result) {
                //If an error happens, log
                if(err) {
                    console.log(err);
                }
                //If the message board topic was made, pass back success
                else {
                    res.status(200).json({ status: "success", message: "Topic successfully added to group's message board." });
                }
            });
        }
        //If the request does not have the correct info, send back error message
        else
        {
            res.status(400).json({ status: "error", message: "missing parameter in POST request" });   
        }
    });


    //Get for retreiving all messageboard topics for a group
    app.get('/groups/:groupid(\\d+)/messagetopic/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        //Get a connection
        var con = mysql.createConnection(config.connectionInfo);

        //Check that all required information is passed
        if(req.params.groupid)
        {
            //Super gross, clean up later
            //Create a var that has the call to the procedure
            var getRequest = "Select mt.TopicID, mt.Title, \
                                (Select MIN(p2.PostTime) \
                                    From Posts as p2 \
                                    Where p2.TopicID = mt.TopicID) as DatePosted, \
                                    (Select Msg \
                                        From Posts as p3 \
                                        Where p3.TopicID = mt.TopicID \
                                            AND p3.PostTime = DatePosted) as Message, \
                                    (Select UserName \
                                        From Users \
                                        Where UserID = \
                                        (Select UserID \
                                            From Posts as p \
                                            Where p.TopicID = mt.TopicID \
                                                And p.PostTime = DatePosted)) as PosterName, \
                                    (Select FirstName \
                                        From Users \
                                        Where UserID = \
                                        (Select UserID \
                                            From Posts as p \
                                            Where p.TopicID = mt.TopicID \
                                                And p.PostTime = DatePosted)) as FirstName, \
                                    (Select LastName \
                                        From Users \
                                        Where UserID = \
                                        (Select UserID \
                                            From Posts as p \
                                            Where p.TopicID = mt.TopicID \
                                                And p.PostTime = DatePosted)) as LastName \
                                From Message_Topics as mt \
                                Where mt.GroupID = " + con.escape(req.params.groupid);

            //Change request to return topics and responses by "pages"? groups of 10 or something? grouped by date

            //Perform the request
            con.query(getRequest, function(err, result) {
                //If an error happens, log
                if(err) {
                    console.log(err);
                }
                //Return message board topics and meta information
                else {
                    res.json(result);
                    res.end();
                }
            });
        }
        //If all required information is not present, send back an error message
        else
        {
            res.status(400).json({ status: "error", message: "missing parameter in GET request" });   
        }
    });
}