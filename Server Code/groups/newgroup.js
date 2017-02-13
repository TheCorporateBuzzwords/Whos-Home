var config = require("./../config");
var mysql = require("mysql");
var auth = require('./../middlewares/auth');

//Export for creating a new group
module.exports = function (app) {
    app.post('/groups/', auth.CheckAuthToken, function (req, res) {
        //Create a connection to the database
        var con = mysql.createConnection(config.connectionInfo);

        //Check for valid information in incoming JSON
        if (req.body.groupName) {
            //Call the add group procedure
            var insertRequest = "Call addGroup(" + con.escape(req.body.groupName) + ", " + con.escape(req.body.decoded.UserID) + ")";

            //Perform the request
            con.query(insertRequest, function (err, result) {
                if (err) {
                    //If error, log and handle
                    console.log(err);
                }
                else {
                    con.query("SELECT LAST_INSERT_ID() AS id", function (err, result, field) {
                        GroupID = result[0].id;
                        if (err) {
                            console.log(err);
                        } else {
                            res.status(200);
                            res.json({
                                status: "success",
                                groupID: GroupID
                            });
                            res.end();
                        }
                    });
                }
            })
        }
        //If the request does not have the correct info, send back error message
        else {
            res.status(400);
            res.json({
                status: "error",
                message: "missing parameter in POST request"
            });
        }
    });
}