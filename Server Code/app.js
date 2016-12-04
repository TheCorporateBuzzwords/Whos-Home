var morgan = require('morgan');
var bodyParser = require('body-parser');
var express = require('express');
var app = express();
app.use(morgan('combined'));
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

var login = require('./session/login');

var signup = require('./users/signup');

app.listen(3000);

login(app);

signup(app);