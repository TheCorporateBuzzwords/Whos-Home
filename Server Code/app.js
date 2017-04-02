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
var morgan = require('morgan');
var bodyParser = require('body-parser');

app.use(morgan('combined'));
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

//app.use('/session/login', require('./routes/session/login'));
app.use('/groups/:groupid(\\d+)', require('./routes/groups'));
/*app.use('/users', require('./routes/users/signup'));
app.use('/groups', require('./routes/groups/groupinfo'));
app.use('/groups/invitation', require('./routes/groups/invitation/sendinvite'));
app.use('/groups/locations', require('./routes/groups/locations/newlocation'));
app.use('/groups', require('./routes/groups/newgroup'));
app.use('/groups/locations', require('./routes/groups/locations/getgrouplocations'));
app.use('/groups/messageboard', require('./routes/groups/messageboard/messagetopics'));
app.use('/groups/messageboard', require('./routes/groups/messageboard/messageposts'));
app.use('/users', require('./routes/users/groups'));
app.use('/groups', require('./routes/users/location'));
app.use('/users', require('./routes/groups/invitation/acceptinvite'));
app.use('/users', require('./routes/users/invites'));
app.use('/groups/lists', require('./routes/groups/lists/newlist'));
app.use('/groups/lists', require('./routes/groups/lists/newlistitem'));
app.use('/groups/lists', require('./routes/groups/lists/getlists'));
app.use('/groups/lists', require('./routes/groups/lists/getlistitems'));
app.use('/groups/lists', require('./routes/groups/lists/updatelistitems'));
app.use('/groups/messageboard', require('./routes/groups/messageboard/messageboardedit'));
app.use('/groups/lists', require('./routes/groups/lists/listsedits'));
app.use('/groups', require('./routes/groups/groupedit'));
app.use('/groups/bills', require('./routes/groups/bills/createbill'));
app.use('/groups/bills', require('./routes/groups/bills/getbill'));
app.use('/teapot', require('./routes/teapot/teapot'));*/


app.listen(3000);