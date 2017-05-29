var app = require('express')();
var config = require("./config");
var morgan = require('morgan');
var bodyParser = require('body-parser');
var http = require('http');
var https = require('https');
var schedule = require('./schedule');

app.use(morgan('combined'));
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

app.use('/session', require('./routes/session'));
app.use('/groups', require('./routes/groups'));
app.use('/users', require('./routes/users'));
app.use('/', require('./routes'));

app.use('/ping/', require('./routes/ping'));
/*app.use('/users', require('./routes/users/signup'));
app.use('/users', require('./routes/users/groups'));
app.use('/users', require('./routes/groups/invitation/acceptinvite'));
app.use('/users', require('./routes/users/invites'));
app.use('/teapot', require('./routes/teapot/teapot'));*/

http.createServer(app).listen(3000);
https.createServer(config.httpsOptions, app).listen(4433);
schedule.startLocationChecking();

