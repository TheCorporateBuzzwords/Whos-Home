var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    app.post('/groups/:groupid(\\d+)/lists/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        if(req.params.groupid && req.body.title)
        {
            var insertRequest = "INSERT INTO Lists (GroupID, UserID, Title, PostTime) values (" + con.escape(req.params.groupid) + ", " + req.body.decoded.UserID + ", " + con.escape(req.body.title) + ", " + "CURRENT_TIME()" + ");";
            con.query(insertRequest, function(err, result) {
                if(err) {
                    console.log(err);
                    return res.end();
                }
                else {
                    con.query("SELECT LAST_INSERT_ID() AS id", function (err, result, field) {
                        if(err) {
                            console.log(err);
                        } 
                        else {
                            return res.status(200).json({ status: "success", listid: result[0].id });
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