var router = require('express').Router();

router.get('/teapot', function (req, res) {
    res.status(418).send("success");
});
module.exports = router;