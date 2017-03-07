var config = require("./../config");
var mysql = require("mysql");
var auth = require('./../middlewares/auth');

module.exports = function (app) {
    //Authenticated route to get group information from group id
    //(\\d+) makes sure :id is an integer.
    app.get('/groups/:groupid(\\d+)', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        //var con = mysql.createConnection(config.connectionInfo);
        if (req.body.decoded) { //Had to do a left join in this query or else nothing would be returned if a group had no locations.
            var request = "SELECT UserName, GroupName, NetName\
                            FROM Users\
                            JOIN User_Groups ON Users.UserID = User_Groups.UserID\
                            JOIN Groups ON User_Groups.GroupID = Groups.GroupID\
                            LEFT JOIN Group_Locations ON Group_Locations.GroupID = Groups.GroupID\
                            WHERE Groups.GroupID = " + config.pool.escape(req.params.groupid);
            config.pool.query(request, function (err, result) {
                return res.json(result);
            });
        }
    });
}