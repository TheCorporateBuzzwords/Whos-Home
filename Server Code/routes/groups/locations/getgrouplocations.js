var config = require("./../../../config");
var mysql = require("mysql");
var auth = require('./../../../middlewares/auth');
var router = require('express').Router();

//Authenticated route to get group information from group id
//(\\d+) makes sure :id is an integer.
router.get('/groups/:groupid(\\d+)/locations', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    
    var request = "SELECT g.GroupName, gl.SSID, gl.NetName \
                            FROM Groups g \
                            INNER JOIN Group_Locations gl ON g.GroupID = gl.GroupID \
                            WHERE g.GroupID = " + config.pool.escape(req.params.groupid);
    config.pool.query(request, function (err, result) {
        return res.json(result);
    });
});
module.exports = router;