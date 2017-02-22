var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    app.get('/groups/:groupid(\\d+)/lists/:listid(\\d+)', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
            var insertRequest = "SELECT ItemID, ListID, UserID, ItemText, Completed, PostTime, UserName, FirstName, LastName FROM Items INNER JOIN Users ON Items.UserID = Users.UserID WHERE ListID = " + req.params.listid;
            con.query(insertRequest, function(err, result) {
                if(err) {
                    console.log(err);
                    return res.end();
                }
                else {
                    res.json(result);
                }
            });
    });
}