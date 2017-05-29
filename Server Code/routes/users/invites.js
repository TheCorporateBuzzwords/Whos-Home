var config = require("./../../config");
var mysql = require("mysql");
var auth = require('./../../middlewares/auth');
var router = require('express').Router();

router.get('/users/invites', auth.CheckAuthToken, function (req, res) {
    //Get connection 
    

    //Check that everything needed is passed in
    if (req.body.decoded) {
        var getRequest = "Select (Select UserName from Users as t where t.UserID = u.InviterID) as UserName \
                            , (Select FirstName From Users as t Where t.UserID = u.InviterID) as FirstName \
                            , (Select LastName From Users as t Where t.UserID = u.InviterID) as LastName \
                            , u.GroupID \
                            , (Select GroupName From Groups as t Where t.GroupID = u.GroupID) as GroupName \
                            From Invites as u \
                             Where RecipientID = " + req.body.decoded.UserID + ";";

        config.pool.query(getRequest, function (err, result) {
            if (err) {
                console.log(err);
            }
            else {
                return res.json(result);
            }
        });
    }
    else {
        return res.status(400).json({ status: "error", message: "missing parameter in GET request" });
    }
});
module.exports = router;