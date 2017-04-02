var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');
var router = require('express').Router();

router.get('/users/groups', auth.CheckAuthToken, function (req, res) {
    //Get a connection
    //var con = mysql.createConnection(config.connectionInfo);

    //Check that the needed stuff is in the JSON
    if (req.body.decoded) {
        var getRequest = "Select g.GroupID, g.GroupName \
                                From Users as u Inner Join User_Groups as ug \
                                On u.UserID = ug.UserID \
                                Inner Join Groups as g \
                                on ug.GroupID = g.GroupID \
                                Where u.UserID = " + req.body.decoded.UserID + ";";

        config.pool.query(getRequest, function (err, result) {
            if (err) {
                //If error, log and handle
                console.log(err);
            }
            else {
                return res.json(result);
            }
        });
    }
    else {
        return res.status(400).json({ status: "error", message: "missing parameter in GET request" });
    }
});
module.exports = router;