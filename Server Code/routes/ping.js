var config = require("./../config");
var mysql = require("mysql");
var router = require('express').Router();
var helper = require('./../helper');

router.get('/:userid', function (req, res) {
    helper.sendNotification(req.params.userid, "Ping", "You've been pinged");
    res.status(200).send("Sent");
});

module.exports = router;