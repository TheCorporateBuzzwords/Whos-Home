var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    app.post('/groups/:groupid(\\d+)/lists/:listid(\\d+)', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        //var con = mysql.createConnection(config.connectionInfo);
        if(req.params.groupid && req.body.content)
        {
            var insertRequest = "INSERT INTO Items (UserID, ListID, ItemText, PostTime) values (" + req.body.decoded.UserID + ", " + req.params.listid +  ", " + config.pool.escape(req.body.content) + ", " + "CURRENT_TIME()" + ");";
            config.pool.query(insertRequest, function(err, result) {
                if(err) {
                    console.log(err);
                    return res.end();
                }
                else {
                    config.pool.query("SELECT LAST_INSERT_ID() AS id", function (err, result, field) {
                        GroupID = result[0].id;
                        if (err) {
                            console.log(err);
                        } else {
                            return res.status(200).json({ status: "success", ListItemID: result[0].id });
                        }
                    });
                }
            });
        }
        else {
            return res.status(400).json({ status: "error", message: "missing parameter in POST request" });
        }
    });
}