var config = require("./../config");
var mysql = require("mysql");
var auth = require('./../middlewares/auth');

module.exports = function (app) {
    //Authenticated route to get group information from group id
    //(\\d+) makes sure :id is an integer.
    app.get('/groups/:groupid(\\d+)', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        if (req.body.decoded) { //Had to do a left join in this query or else nothing would be returned if a group had no locations.
            var request = "SELECT u.UserName, g.GroupName, (SELECT NetName FROM Group_Locations WHERE u.LocationID = gl.LocationID) AS LocationName FROM Users u \
                            INNER JOIN User_Groups ug ON u.UserID = ug.UserID \
                            INNER JOIN Groups g ON ug.GroupID = g.GroupID \
                            LEFT JOIN Group_Locations gl ON g.GroupID = gl.GroupID \
                            WHERE g.GroupID = " + con.escape(req.params.groupid);
            con.query(request, function (err, result) {
                return res.json(result);
            });
        }
    });
}