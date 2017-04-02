var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');
var router = require('express').Router();

//Authenticated route to get group information from group id
//(\\d+) makes sure :id is an integer.
router.get('/groups/:groupid(\\d+)', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //var con = mysql.createConnection(config.connectionInfo);
    if (req.body.decoded) { //Had to do a left join in this query or else nothing would be returned if a group had no locations.
        var request = "SELECT UserName, GroupName, NetName, Users.UserID \
                            FROM Users \
                            JOIN User_Groups ON Users.UserID = User_Groups.UserID \
                            JOIN Groups ON User_Groups.GroupID = Groups.GroupID \
                            left join Group_Locations ON Group_Locations.GroupID = Groups.GroupID AND Users.LocationID = Group_Locations.LocationID \
                            WHERE Groups.GroupID = " + config.pool.escape(req.params.groupid);
        config.pool.query(request, function (err, result) {
            return res.json(result);
        });
    }
});
module.exports = router;