var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    app.get('/groups/:groupid(\\d+)/bills/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
        if (req.query.recipient)
            var selectQuery = "SET @GROUPID =" + req.params.groupid + "; SET @RID= " + req.query.recipient + "; CALL get_personal_bills(@GROUPID, @RID);"
        else
            var selectQuery = "CALL get_group_bills(" + req.params.groupid + ");"
        config.pool.query(selectQuery, function(err, result) {
            if(err) {
                console.log(err);
                return res.end();
            }
            else {
                return res.status(200).json(result);
            }
        });
    });
}