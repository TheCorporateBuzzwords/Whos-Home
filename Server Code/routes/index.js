var router = require('express').Router();

router.get('/', function (req, res) {
    res.status(200).send("Welcome to the Who's Home API");
});

module.exports = router;