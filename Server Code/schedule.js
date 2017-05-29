var cron = require('cron');
var helper = require('./helper');

var usersAtLocation = [ ]; 

function startLocationChecking() {
    cron.job("0 */5 * * * *", function () {
        usersAtLocation.forEach(checkLocation);
    }).start();
}

function checkLocation(userid) {
    console.info('cron job completed');
    helper.sendMessage(1, { data: { test: "test" } }, { timeToLive: 0 }, function (err, response) {
        console.log(response);
        if (err) {
            console.log("error");
            console.log(err);
        }
        else if (response.successCount > 0) {
            console.log("user still at location");
            //still at location
        }
        else {
            console.log("User is offline");
            removeUser(userid);
            changeLocation(userid);
        }
    });
}

function addUser(userid) {
    if (usersAtLocation.indexOf(userid) == -1)
    {
        usersAtLocation.push(userid);
    }
}

function removeUser(userid) {
    var index = usersAtLocation.indexOf(userid);
    if (index != -1) {
        usersAtLocation.splice(index, 1);
    }
}

function changeLocation(userid) {
    helper.updateUserLocation(userid, -1, function(err) {
        if (err) {
            console.log(err);
        }   
    });
}

module.exports = { startLocationChecking: startLocationChecking, addUserToLocationList: addUser };