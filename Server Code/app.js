var morgan = require('morgan');
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
userGroups(app);
updateLocation(app);
acceptInvite(app);
userInites(app);
newlist(app);
newlistitem(app);
getlists(app);