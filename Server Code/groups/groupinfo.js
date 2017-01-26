var config = require("./../config");
var mysql = require("mysql");
var auth = require('./../middlewares/auth');

module.exports = function (app) {
    //Authenticated route to get group information from group id
    //(\\d+) makes sure :id is an integer.
    app.get('/groups/:id(\\d+)', auth.CheckAuthToken, function (req, res) {
        var con = mysql.createConnection(config.connectionInfo);
        if(req.body.decoded)
        {
            var request = "SELECT u.UserName, g.GroupName, (SELECT NetName FROM Group_Locations AS gl WHERE u.LocationID = gl.LocationID) AS LocationName \
                            FROM Users u \
                            INNER JOIN User_Groups ug ON u.UserID = ug.UserID \
                            INNER JOIN Groups g ON ug.GroupID = g.GroupID \
                            INNER JOIN Group_Locations gl ON g.GroupID = gl.GroupID \
                            WHERE g.GroupID = " + con.escape(req.params.id); + " \
                            AND u.UserID = " + req.body.decoded.UserID;
            con.query(request, function(err, result) {
                res.json(result);
                res.end();
            });
        }
    });
}