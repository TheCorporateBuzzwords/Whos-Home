module.exports = function (app) {
    app.get('/teapot', function (req, res) {
        res.status(418).send("success");
    });
}