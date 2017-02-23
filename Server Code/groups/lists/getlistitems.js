var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    app.get('/groups/:groupid(\\d+)/lists/:listid(\\d+)', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
            var selectQuery = "SELECT Items.ItemID, Items.ListID, Items.UserID, Items.ItemText, Items.Completed, Items.PostTime, Users.UserName, Users.FirstName, Users.LastName FROM Items INNER JOIN Users ON Items.UserID = Users.UserID WHERE ListID = " + req.params.listid;
            con.query(selectQuery, function(err, result) {
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