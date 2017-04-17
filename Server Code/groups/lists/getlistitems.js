var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');

module.exports = function (app) {
    app.get('/groups/:groupid(\\d+)/lists/:listid(\\d+)', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
            var selectQuery = "CALL get_list_items(" + req.params.listid + ");";
            config.pool.query(selectQuery, function(err, result) {
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