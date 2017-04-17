var config = require("./../config");
var mysql = require("mysql");
var auth = require('./../middlewares/auth');

module.exports = function (app) {
    app.get('/users/groups', auth.CheckAuthToken, function (req, res) {
        //Get a connection
        //var con = mysql.createConnection(config.connectionInfo);

        //Check that the needed stuff is in the JSON
        if(req.body.decoded)
        {
            var getRequest = "CALL get_group_info(" + req.body.decoded.UserID + ");";

            config.pool.query(getRequest, function (err, result) {
                if(err) {
                    //If error, log and handle
                    console.log(err);
                }
                else {
                    return res.json(result);
                }
            });
        }
        else
        {
            return res.status(400).json({ status: "error", message: "missing parameter in GET request" });   
        }
    });
}