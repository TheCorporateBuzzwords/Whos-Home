var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');
var router = require('express').Router();

//Export for creating a new group
router.post('/groups/', auth.CheckAuthToken, function (req, res) {
    //Create a connection to the database
    //var con = mysql.createConnection(config.connectionInfo);

    //Check for valid information in incoming JSON
    if (req.body.groupName) {
        //Call the add group procedure
        var insertRequest = "Call addGroup(" + config.pool.escape(req.body.groupName) + ", " + config.pool.escape(req.body.decoded.UserID) + ")";

        //Perform the request
        config.pool.query(insertRequest, function (err, result) {
            if (err) {
                //If error, log and handle
                console.log(err);
            }
            else {
                config.pool.query("SELECT LAST_INSERT_ID() AS id", function (err, result, field) {
                    GroupID = result[0].id;
                    if (err) {
                        console.log(err);
                    } else {
                        res.status(200).json({ status: "success", groupID: GroupID });
                    }
                });
            }
        })
    }
    //If the request does not have the correct info, send back error message
    else {
        res.status(400).json({ status: "error", message: "missing parameter in POST request" });
    }
});
module.exports = router;