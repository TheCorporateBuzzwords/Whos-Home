var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    //Authenticated route to get group information from group id
    //(\\d+) makes sure :id is an integer.
    app.get('/groups/:groupid(\\d+)/locations', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        //var con = mysql.createConnection(config.connectionInfo);
        var request = "SELECT g.GroupName, gl.SSID, gl.NetName \
                            FROM Groups g \
                            INNER JOIN Group_Locations gl ON g.GroupID = gl.GroupID \
                            WHERE g.GroupID = " + config.pool.escape(req.params.groupid);
        config.pool.query(request, function (err, result) {
            return res.json(result);
        });
    });
}