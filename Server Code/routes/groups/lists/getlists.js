var config = require("./../../../config");
var mysql = require("mysql");
var auth = require('./../../../middlewares/auth');
var router = require('express').Router();

router.get('/groups/:groupid(\\d+)/lists/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    var insertRequest = "SELECT Lists.ListID, Lists.GroupID, Lists.UserID, Lists.Title, Lists.PostTime, Users.UserName, Users.FirstName, Users.LastName FROM Lists INNER JOIN Users ON Lists.UserID = Users.UserID WHERE GroupID = " + req.params.groupid;
    config.pool.query(insertRequest, function (err, result) {
        if (err) {
            console.log(err);
            return res.end();
        }
        else {
            res.json(result);
        }
    });
});
module.exports = router;