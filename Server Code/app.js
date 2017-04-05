/*var morgan = require('morgan');
var bodyParser = require('body-parser');
var express = require('express');
var app = express();
var login = require('./session/login');
var signup = require('./users/signup');
var groupinfo = require('./groups/groupinfo');
var sendinvite = require('./groups/invitation/sendinvite');
var newlocation = require('./groups/locations/newlocation');
var newgroup = require('./groups/newgroup');
var getgrouplocations = require('./groups/locations/getgrouplocations');
var messagetopics = require('./groups/messageboard/messagetopics');
var messageresponse = require('./groups/messageboard/messageposts');
var userGroups = require('./users/groups');
var updateLocation = require('./users/location');
var acceptInvite = require('./groups/invitation/acceptinvite');
var userInites = require('./users/invites');
var newlist = require('./groups/lists/newlist');
var newlistitem = require('./groups/lists/newlistitem');
var getlists = require('./groups/lists/getlists');
var getlistitems = require('./groups/lists/getlistitems');
var updatelists = require('./groups/lists/updatelistitems');
var messageboardedit = require('./groups/messageboard/messageboardedit');
var listsedit = require('./groups/lists/listsedits');
var groupedit = require('./groups/groupedit');
var createbill = require('./groups/bills/createbill');
var getbill = require('./groups/bills/getbill');
var teapot = require('./teapot/teapot');

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
newlocation(app);
newgroup(app);
getgrouplocations(app);
messagetopics(app);
messageresponse(app);
messageboardedit(app);
userGroups(app);
updateLocation(app);
acceptInvite(app);
userInites(app);
newlist(app);
newlistitem(app);
getlists(app);
updatelists(app);
getlistitems(app);
listsedit(app);
groupedit(app);
createbill(app);
getbill(app);
teapot(app);*/

var app = require('express')();
var config = require("./config");
var morgan = require('morgan');
var bodyParser = require('body-parser');
var http = require('http');
var https = require('https');

app.use(morgan('combined'));
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

app.use('/session', require('./routes/session'));
app.use('/groups', require('./routes/groups'));
app.use('/users', require('./routes/users'));
app.use('/', require('./routes'));
/*app.use('/users', require('./routes/users/signup'));
app.use('/users', require('./routes/users/groups'));
app.use('/users', require('./routes/groups/invitation/acceptinvite'));
app.use('/users', require('./routes/users/invites'));
app.use('/teapot', require('./routes/teapot/teapot'));*/

http.createServer(app).listen(3000);
https.createServer(config.httpsOptions, app).listen(4433);