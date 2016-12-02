var express = require('express');
var mysql = require("mysql");
var bodyParser = require('body-parser');
var async = require('async');
var crypto = require('crypto');
var app = express();
var morgan = require('morgan')

app.use(morgan('combined'))

app.use(bodyParser.urlencoded({ extended: false }))

app.use(bodyParser.json())

var config = {
    hashBytes: 64,
    saltBytes: 16,
    // more iterations means an attacker has to take longer to brute force an
    // individual password, so larger is better. however, larger also means longer
    // to hash the password. tune so that hashing the password takes about a
    // second
    iterations: 872791,
    digest: 'sha512'
};
var con = mysql.createConnection({
    host: "96.41.173.205",
    user: "limited",
    password: "Speci@login$$$69$$$",
    database: "WHOSHOME"
});    


app.listen(3000);


app.post('/login/', function (req, res) {
    con.connect(function(err) {
        console.log("connected");
    });
    async.waterfall([
        function checkValidData(callback) {
            if (req.body.Username && req.body.Password) {
                callback(null);
            }
            else {
                res.status(400);
                res.send('Missing parameter in POST request');
                return;
            }
        },
        function getSalt(callback) {
            getHashRequest = 'SELECT Salt FROM Users WHERE UserName = "' + req.body.Username + '"';
            con.query(getHashRequest, function(err, results) {
                if(results.length)
                    callback(err, results[0].Salt);
                else
                    res.status(409);
                    res.send("Incorrect username or password.");
                    return;
            });
        },
        function getHash(saltData, callback) {
            hashPassword(req.body.Password, saltData, function(err, hash) {
                callback(err, hash);
            });
        },
        function checkLogin(hash, callback) {
            checkLoginRequest = 'SELECT UserName FROM Users WHERE UserName = "' + req.body.Username + '" AND Pass = "' + hash + '"';
            con.query(checkLoginRequest, function(err, results) {
                if (!results.length) {
                    res.status(409);
                    res.send("Incorrect username or password.");
                    return;
                }
                else {
                    callback(err);
                }
            });
        }
    ],
    function complete(err)
    {
        if(err)
            console.log(err);
        else
            res.status(200);
            res.send("Sign in successful.");
        con.end(function (err) {
            console.log(err);
        });
    });
});

function hashPassword(password, salt, callback) {
    crypto.pbkdf2(password, new Buffer(salt, "hex"), config.iterations, config.hashBytes, config.digest, function (err, hash) {
        callback(err, hash.toString('hex'));
    });
}