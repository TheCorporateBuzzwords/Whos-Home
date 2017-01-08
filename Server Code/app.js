var morgan = require('morgan');
var bodyParser = require('body-parser');
var express = require('express');
var app = express();
var login = require('./session/login');
var signup = require('./users/signup');
var groupinfo = require('./groups/groupinfo');
var sendinvite = require('./groups/invitation/sendinvite');
//var auth = require('./middlewares/auth');
app.use(morgan('combined'));
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());
//app.use(auth.CheckAuthToken());


app.listen(3000);

login(app);
signup(app);
groupinfo(app);
sendinvite(app);