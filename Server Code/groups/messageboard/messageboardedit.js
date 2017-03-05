var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    //Endpoint for editing a message board topic
    app.put('/groups/:groupid(\\d+)/topicedit/:topicid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        //Get a connection
        var con = mysql.createConnection(config.connectionInfo);

        //Check for all needed info
        if(req.body.decoded.UserID && req.params.groupid && req.params.topicid)
        {
            //Check for if the user is the person who made the message board topic originaly

            
            }
            //If it gets to here, the request is missing params
            else
            {
                res.status(400);
                res.json({
                    status: "error",
                    message: "missing parameter in PUT request"
                });
            }
        }
        //If the request is missing params, send back an error
        else
        {
            res.status(400);
            res.json({
                status: "error",
                message: "missing parameter in PUT request"
            });
        }
    });
    //Ednpoint for editing a message board topic response

}