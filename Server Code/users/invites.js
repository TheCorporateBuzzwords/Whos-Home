var config = require("./../config");
var mysql = require("mysql");
var auth = require('./../middlewares/auth');

module.exports = function (app) {
    app.get('/users/invites', auth.CheckAuthToken, function (req, res) {
        //Get connection 
        var con = mysql.createConnection(config.connectionInfo);

        //Check that everything needed is passed in
        if(req.body.decoded)
        {
            var getRequest = "Select (Select UserName from Users as t where t.UserID = u.InviterID) as UserName \
                            , (Select FirstName From Users as t Where t.UserID = u.InviterID) as FirstName \
                            , (Select LastName From Users as t Where t.UserID = u.InviterID) as LastName \
                            , u.GroupID \
                            , (Select GroupName From Groups as t Where t.GroupID = u.GroupID) as GroupName \
                            From Invites as u \
                             Where RecipientID = " + req.body.decoded.UserID + ";";

            con.query(getRequest, function (err, result) {
                if(err)
                {
                    console.log(err);
                }
                else
                {
                    res.json(result);
                    res.end();
                }
            });
        }
        else
        {
            res.status(400);
            res.json({
                status: "error",
                message: "missing parameter in GET request"
            });  
        }
    });
}