var config = require("./../config");
var mysql = require("mysql");
var auth = require('./../middlewares/auth');

module.exports = function (app) {
    app.get('/users/groups', auth.CheckAuthToken, function (req, res) {
        //Get a connection
        var con = mysql.createConnection(config.connectionInfo);

        //Check that the needed stuff is in the JSON
        if(req.body.decoded)
        {
            var getRequest = "Select g.GroupID, g.GroupName \
                                From Users as u Inner Join User_Groups as ug \
                                On u.UserID = ug.UserID \
                                Inner Join Groups as g \
                                on ug.GroupID = g.GroupID \
                                Where u.UserID = " + req.body.decoded.UserID + ";";

            con.query(getRequest, function (err, result) {
                if(err) {
                    //If error, log and handle
                    console.log(err);
                }
                else {
                    res.json(result);
                    res.end();
                }
            });
        }
        else
        {
            res.status(400);
            res.json({
                status: "error",
                message: "missing parameter in GET request"
            });   
        }
    });
}