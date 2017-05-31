var config = require("./../config");
var mysql = require("mysql");
var async = require('async');
var crypto = require('crypto');
var validator = require('validator');
var auth = require('./../middlewares/auth');
var jwt = require('jsonwebtoken');
var router = require('express').Router();
var admin = require("firebase-admin");
var helper = require('./../helper');

var serviceAccount = require("../etc/firebase/whos-home-fcb1b-firebase-adminsdk-mm1oa-7a54483f9e.json");

admin.initializeApp({
    credential: admin.credential.cert(serviceAccount),
    databaseURL: "firebase-adminsdk-mm1oa@whos-home-fcb1b.iam.gserviceaccount.com"
});

/**********************
 * Group Route Handlers
 *********************/

//route for creating a new group
router.post('/', auth.CheckAuthToken, function (req, res) {
    //Check for valid information in incoming JSON
    if (req.body.groupName) {
        //Call the add group procedure
        var insertRequest = "Call addGroup(" + config.pool.escape(req.body.groupName) + ", " + config.pool.escape(req.body.decoded.UserID) + ")";

        // //Perform the request
        // config.pool.query(insertRequest, function (err, result) {
        //     if (err) {
        //         //If error, log and handle
        //         console.log(err);
        //     }
        //     else {
        //         config.pool.query("SELECT LAST_INSERT_ID() AS id", function (err, result, field) {
        //             GroupID = result[0].id;
        //             if (err) {
        //                 console.log(err);
        //             } else {
        //                 res.status(200).json({ status: "success", groupID: GroupID });
        //             }
        //         });
        //     }
        // });

        //Perform the request
        config.pool.query(insertRequest, function (err, result) {
            if (err) {
                //If error, log and handle
                console.log(err);
            }
            else {
                // config.pool.query("SELECT LAST_INSERT_ID() AS id", function (err, result, field) {
                //     GroupID = result[0].id;
                //     if (err) {
                //         console.log(err);
                //     } else {
                //         res.status(200).json({ status: "success", groupID: GroupID });
                //     }
                // });

                res.status(200).json(result[1]);
            }
        });
    }
    //If the request does not have the correct info, send back error message
    else {
        res.status(400).json({ status: "error", message: "missing parameter in POST request" });
    }
});

//Authenticated route to get group information from group id
//(\\d+) makes sure :id is an integer.
router.get('/:groupid(\\d+)', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    if (req.body.decoded) { //Had to do a left join in this query or else nothing would be returned if a group had no locations.
        // var request = "SELECT UserName, GroupName, NetName, Users.UserID" +
        //                     "FROM Users" +
        //                     "JOIN User_Groups ON Users.UserID = User_Groups.UserID" +
        //                     "JOIN Groups ON User_Groups.GroupID = Groups.GroupID" +
        //                     "left join Group_Locations ON Group_Locations.GroupID = Groups.GroupID AND Users.LocationID = Group_Locations.LocationID" +
        //                     "WHERE Groups.GroupID = " + config.pool.escape(req.params.groupid);
        //var checkHome = "SELECT LocationID FROM Group_Locations WHERE GroupID = ";
        var request = "SELECT UserName, GroupName, IF(EXISTS(SELECT UserId FROM Users WHERE Home = Users.Home AND UserID = UL.UserID), 'Home', (select NetName From Group_Locations Where LocationID = UL.LocationID)) As NetName, Users.UserID " +
            "FROM Users " +
            "JOIN User_Groups ON Users.UserID = User_Groups.UserID " +
            "JOIN Groups ON User_Groups.GroupID = Groups.GroupID " +
            "left join User_Locations as UL " +
            "ON UL.GroupID = Groups.GroupID " +
            "AND Users.UserID = UL.UserID " +
            "AND UL.GroupID = User_Groups.GroupID " +
            "WHERE Groups.GroupID = " + config.pool.escape(req.params.groupid);

        config.pool.query(request, function (err, result) {
            return res.json(result);
        });
    }
});

//Endpoint for editing the name of the group
router.put('/:groupid(\\d+)/group/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed info
    if (req.body.newName) {
        var editRequest = "Update Groups" +
            " Set GroupName = " + config.pool.escape(req.body.newName) +
            " Where GroupID = " + config.pool.escape(req.params.groupid) + ";";

        config.pool.query(editRequest, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200).json({ status: "success", message: "Edit successfully made to Group name." });
            }
        });
    }
    else {
        res.status(400).json({ status: "error", message: "missing parameter in PUT request" });
    }
});

//Endpoint for removing a single user from the group
router.delete('/:groupid(\\d+)/:userid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed info
    var deleteRequest = "Call removeUserFromGroup(" + config.params.groupid + ", " + req.body.decoded.UserID + ")";

    config.pool.query(deleteRequest, function (err, result) {
        //If there is an error, log it
        if (err) {
            console.log(err);
        }
        //If the response was made, return a status indicating success
        else {
            res.status(200);
            res.json({
                status: "success",
                message: "User successfully removed from group."
            });
        }
    });
});

/*********************
 * Bill Route Handlers
 ********************/

//Get Bills for group
router.get('/:groupid(\\d+)/bills/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    var selectQuery = "";
    if (req.query.recipient) {
        selectQuery = "SELECT BillID, GroupID, u1.UserName AS Sender, u2.UserName AS Recipient, CategoryID, Title, Description, Amount, DATE_FORMAT(DateDue, '%c/%d/%Y %r:%h:%s') AS DateDue" +
            " FROM Bills b" +
            " INNER JOIN Users u1 ON b.RecipientID = u1.UserID" +
            " INNER JOIN Users u2 ON b.SenderID = u2.UserID" +
            " WHERE GroupId = " + req.params.groupid + " AND RecipientId = " + req.query.recipient;
    }
    else {
        selectQuery = "SELECT BillID, GroupID, u1.UserName AS Sender, u2.UserName AS Recipient, CategoryID, Title, Description, Amount, DATE_FORMAT(DateDue, '%c/%d/%Y %r:%h:%s') AS DateDue" +
            " FROM Bills b" +
            " INNER JOIN Users u1 ON b.RecipientID = u1.UserID" +
            " INNER JOIN Users u2 ON b.SenderID = u2.UserID" +
            " WHERE GroupId = " + req.params.groupid;
    }
    config.pool.query(selectQuery, function (err, result) {
        if (err) {
            console.log(err);
            return res.end();
        }
        else {
            return res.status(200).json(result);
        }
    });
});

//Create bill
router.post('/:groupid(\\d+)/bills/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    if (!(req.body.recipient && req.body.category && req.body.title && req.body.description && req.body.amount && req.body.date)) {
        return res.status(400).json({ status: "error", message: "missing parameter in POST request" });
    }
    //verify data types
    if (isNaN(req.body.category) || isNaN(req.body.amount) || isNaN(req.body.recipient)) {
        return res.status(400).json({ status: "error", message: "invalid data type for one or more parameters" });
    }
    var insertQuery = "INSERT INTO Bills (GroupID, SenderID, RecipientID, CategoryID, Title, Description, Amount, DateDue)" +
        " VALUES (" + req.params.groupid + "," + req.body.decoded.UserID +
        "," + req.body.recipient + "," + req.body.category + "," + config.pool.escape(req.body.title) + "," +
        config.pool.escape(req.body.description) + "," + req.body.amount + "," + "STR_TO_DATE(" + config.pool.escape(req.body.date) +
        ", '%c/%d/%Y %r:%h:%s'));";
    config.pool.query(insertQuery, function (err, result) {
        if (err) {
            return res.status(500).json({ status: "error", message: "error processing request" });
        }
        else {
            helper.sendNotification(req.body.recipient, "New bill", "Someone has sent you a bill");

            return res.status(200).json({ status: "success", message: "successfully added bill" });
        }
    });
});

/***************************
 * Invitation Route Handlers
 **************************/

//Authenticated route to accept an invite to a group
router.get('/:groupid(\\d+)/invitation/', auth.CheckAuthToken, function (req, res) {

    var checkInvite = "SELECT * FROM Invites WHERE RecipientID = " + req.body.decoded.UserID + " AND GroupID = " + req.params.groupid;
    var insertQuery = "INSERT INTO User_Groups (UserID, GroupID) VALUES (" + req.body.decoded.UserID + ", " + req.params.groupid + ");" +
        " DELETE FROM Invites WHERE GroupID = " + req.params.groupid + " AND RecipientID = " + req.body.decoded.UserID + ";" +
        " Insert Into User_Locations (UserID, GroupID) Values (" + req.body.decoded.UserID + ", " + req.params.groupid + ");";
    var checkDupeQuery = "SELECT * FROM User_Groups WHERE UserID = " + req.body.decoded.UserID + " AND GroupID = " + req.params.groupid;
    var deleteInvite = "DELETE FROM Invites WHERE GroupID = " + req.params.groupid + " AND RecipientID = " + req.body.decoded.UserID;
    config.pool.query(checkInvite, function (err, checkInviteResult) {
        if (err) {
            console.log(err);
            return res.end();
        }
        else if (!checkInviteResult.length) {
            //User hasn't been invited to the group
            return res.status(403).json({ status: "error", message: "you have not been invited to this group" });
        }
        //ability to deny the invitation
        if (req.query.deny === "true") {
            config.pool.query(deleteInvite, function (err, denyResult) {
                if (err) {
                    console.log(err);
                    return res.end();
                }
                return res.status(200).json({ status: "success", message: "invitation denied" });
            });
        }
        else {
            config.pool.query(checkDupeQuery, function (err, checkDupeResult) {
                if (err) {
                    console.log(err);
                    return res.end();
                }
                else if (checkDupeResult.length) {
                    //User has invitation, but is already part of the group.
                    return res.status(409).json({ status: "error", message: "you are already part of this group" });
                }
                config.pool.query(insertQuery, function (err, insertResult) {
                    if (err) {
                        console.log(err);
                        return res.end();
                    }
                    return res.status(200).json({ status: "success", message: "you are now part of this group" });
                });
            });
        }
    });
});

//Authenticated route to create an invitation to a group
router.post('/:groupid(\\d+)/invitation/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    if (!req.body.recipient) {
        return res.status(400).json({ status: "error", message: "missing parameter" });
    }
    var recipient = req.body.recipient;
    if (!/^[a-z][a-z0-9]*$/i.test(recipient)) { //make sure it's a valid username. Should make a module for this.
        return res.status(409).json({ status: "error", message: "Invalid recipient." });
    }
    //get userid
    var getUseridQuery = "SELECT UserID FROM Users WHERE UserName = " + config.pool.escape(recipient);
    config.pool.query(getUseridQuery, function (err, result) {
        if (err) {
            console.log(err);
            return res.end();
        }
        else if (result.length) {
            var recipientID = result[0].UserID;
            //insert invitation into database
            var insertQuery = "INSERT INTO Invites (GroupID, InviterID, RecipientID) VALUES (" + req.params.groupid + ", " + req.body.decoded.UserID + ", " + recipientID + ");";
            config.pool.query(insertQuery, function (err, result) {
                if (err) {
                    console.log(err);
                    return res.end();
                } else {
                    helper.sendNotification(recipientID, "New group invite", "You have a new invitation to a group");
                    return res.status(200).json({ status: "success", message: "invitation created" });
                }
            });
        }
        else {
            return res.status(409).json({ status: "error", message: "user you tried inviting doesn't exist" });
        }
    });
});

/**********************
 * Lists Route Handlers
 *********************/

//Get lists
router.get('/:groupid(\\d+)/lists/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    var insertRequest = "SELECT Lists.ListID, Lists.GroupID, Lists.UserID, Lists.Title, Lists.PostTime, Users.UserName, Users.FirstName, Users.LastName FROM Lists INNER JOIN Users ON Lists.UserID = Users.UserID WHERE GroupID = " + req.params.groupid;
    config.pool.query(insertRequest, function (err, result) {
        if (err) {
            console.log(err);
            return res.end();
        }
        else {
            res.json(result);
        }
    });
});

//create a list
router.post('/:groupid(\\d+)/lists/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    if (req.params.groupid && req.body.title) {
        var insertRequest = "INSERT INTO Lists (GroupID, UserID, Title, PostTime) values (" + config.pool.escape(req.params.groupid) + ", " + req.body.decoded.UserID + ", " + config.pool.escape(req.body.title) + ", " + "CURRENT_TIME()" + ");";
        config.pool.query(insertRequest, function (err, result) {
            if (err) {
                console.log(err);
                return res.end();
            }
            else {
                config.pool.query("SELECT LAST_INSERT_ID() AS id", function (err, result, field) {
                    if (err) {
                        console.log(err);
                    }
                    else {
                        return res.status(200).json({ status: "success", listid: result[0].id });
                    }
                });
            }
        });
    }
    else {
        return res.status(400).json({ status: "error", message: "missing parameter in POST request" });
    }
});

//Endpoint for editing a list
router.put('/:groupid(\\d+)/lists/:listid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed information
    if (req.body.decoded.UserID && req.params.groupid && req.params.listid) {
        var editRequst = "Update Lists" +
            " Set Title = " + config.pool.escape(req.body.newTitle) +
            " Where ListID = " + config.pool.escape(req.params.listid) +
            " And GroupID = " + config.pool.escape(req.params.groupid) + ";";

        config.pool.query(editRequst, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200);
                res.json({
                    status: "success",
                    message: "Edit successfully made to list."
                });
            }
        });
    }
    //If the request is missing params, send back an error
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in PUT request"
        });
    }
});

//Endpoint for deleting a list and all items within that list
router.delete('/:groupid(\\d+)/lists/:listid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed info
    if (req.body.decoded.UserID && req.params.groupid && req.params.listid) {
        var deleteRequest = "Call deleteList(" + config.pool.escape(req.params.listid) + ");";

        config.pool.query(deleteRequest, function (err, result) {
            //Leg any errors that happened
            if (err) {
                console.log(err);
            }
            else {
                res.status(200);
                res.json({
                    status: "success",
                    message: "Successfully deleted list and all items in list."
                });
            }
        });
    }
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in DELETE request"
        });
    }
});

/**************************
 * List Item Route Handlers
 *************************/

//Get list items
router.get('/:groupid(\\d+)/lists/:listid(\\d+)', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    var selectQuery = "SELECT Items.ItemID, Items.ListID, Items.UserID, Items.ItemText, Items.Completed, Items.PostTime, Users.UserName, Users.FirstName, Users.LastName FROM Items INNER JOIN Users ON Items.UserID = Users.UserID WHERE ListID = " + req.params.listid;
    config.pool.query(selectQuery, function (err, result) {
        if (err) {
            console.log(err);
            return res.end();
        }
        else {
            res.json(result);
        }
    });
});

//create new list item
router.post('/:groupid(\\d+)/lists/:listid(\\d+)', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    if (req.params.groupid && req.body.content) {
        var insertRequest = "INSERT INTO Items (UserID, ListID, ItemText, PostTime) values (" + req.body.decoded.UserID + ", " + req.params.listid + ", " + config.pool.escape(req.body.content) + ", " + "CURRENT_TIME()" + ");";
        config.pool.query(insertRequest, function (err, result) {
            if (err) {
                console.log(err);
                return res.end();
            }
            else {
                config.pool.query("SELECT LAST_INSERT_ID() AS id", function (err, result, field) {
                    GroupID = result[0].id;
                    if (err) {
                        console.log(err);
                    } else {
                        return res.status(200).json({ status: "success", ListItemID: result[0].id });
                    }
                });
            }
        });
    }
    else {
        return res.status(400).json({ status: "error", message: "missing parameter in POST request" });
    }
});

//Endpoint for editing a single item in a list
router.put('/:groupid(\\d+)/lists/:listid(\\d+)/:itemid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed information
    if (req.body.newText || (req.body.completed && req.body.completed === "true" || req.body.completed === "false")) {
        var completed;

        if (req.body.completed) {
            if(req.body.completed === "true") 
                completed = 1;
            else   
                completed = 0;
        }

        var editRequest = "Update Items SET ";
        if (req.body.newText)
        {
            editRequest += "ItemText = " + config.pool.escape(req.body.newText) + " ";
        }
        if (req.body.completed)
        {
            editRequest += "Completed = " + completed + " ";
        }
        editRequest += "Where ItemID = " + config.pool.escape(req.params.itemid) + ";";

        config.pool.query(editRequst, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200).json({ status: "success", message: "Edit successfully made to list item." });
            }
        });
    }
    /*else if (req.params.itemid && req.body.completed) {
        if (req.body.completed !== 1 && req.body.completed !== 0) {
            return res.status(400).json({ status: "error", message: "invalid completed value" });
        }
        var updateRequest = "UPDATE Items SET Completed = " + req.body.completed + " WHERE ItemID = " + req.body.itemid;
        config.pool.query(updateRequest, function (err, result) {
            if (err) {
                console.log(err);
                return res.end();
            }
            else {
                return res.status(200).json({ status: "success", message: "successfully updated values" });
            }
        });
    }
    //If the request is missing params, send back an error
    else {
        res.status(400).json({ status: "error", message: "missing parameter in PUT request" });
    }*/
});

//Endpoint for deleting a single item in a list
//router.delete('/:groupid(\\d+)/listitem/:itemid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
router.delete('/:groupid(\\d+)/lists/:listid(\\d+)/:itemid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //Check for all needed information
    //Create the request
    var deleteRequest = "Delete From Items" +
        " Where ItemID = " + config.pool.escape(req.params.itemid) + ";";

    config.pool.query(deleteRequest, function (err, result) {
        //If there is an error, log it
        if (err) {
            console.log(err);
        }
        //If the response was made, return a status indicating success
        else {
            res.status(200);
            res.json({
                status: "success",
                message: "Successfully deleted list item."
            });
            res.end();
        }
    });
});

/***********************
 * Group Location Routes
 **********************/

//get group location info
router.get('/:groupid(\\d+)/locations', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    var request = "SELECT g.GroupName, gl.SSID, gl.NetName" +
        " FROM Groups g" +
        " INNER JOIN Group_Locations gl ON g.GroupID = gl.GroupID" +
        " WHERE g.GroupID = " + config.pool.escape(req.params.groupid);
    config.pool.query(request, function (err, result) {
        return res.json(result);
    });
});

//add new group location
router.post('/:groupid(\\d+)/location/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    if (req.params.groupid && req.body.ssid && req.body.locationName) {
        //console.log("executing");
        var insertRequest = "INSERT INTO Group_Locations (GroupID, SSID, NetName) values (" + config.pool.escape(req.params.groupid) + ", " + config.pool.escape(req.body.ssid) + ", " + config.pool.escape(req.body.locationName) + ");";
        config.pool.query(insertRequest, function (err, result) {
            if (err) {
                console.log(err);
                return res.end();
            }
            else {
                return res.status(200).json({ status: "success", message: "succesfully added location" });
            }
        });
    }
    else {
        return res.status(400).json({ status: "error", message: "missing parameter in POST request" });
    }
});

/*********************
 * Messageboard Routes
 ********************/

//Get for retreiving all responses to a specific messageboard topic in a group
router.get('/:groupid(\\d+)/messageboard/:topicid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //Create the request
    var getRequest = "Select PostID, Msg, PostTime," +
        " (Select UserName" +
        " From Users" +
        " Where UserID = p.UserID) as PostersName" +
        " From Posts as p" +
        " Where TopicID = " + config.pool.escape(req.params.topicid);

    //Perform the request
    config.pool.query(getRequest, function (err, result) {
        //If there is an error, log it
        if (err) {
            console.log(err);
        }
        //If the request was made, return the message topic responses
        else {
            res.json(result);
        }
    });
});

//Get for retreiving all messageboard topics for a group
router.get('/:groupid(\\d+)/messageboard/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //Get a connection
    //var con = mysql.createConnection(config.connectionInfo);

    //Check that all required information is passed
    if (req.params.groupid) {
        //Super gross, clean up later
        //Create a var that has the call to the procedure
        var getRequest = "Select mt.TopicID, mt.Title," +
            " (Select MIN(p2.PostTime)" +
            " From Posts as p2" +
            " Where p2.TopicID = mt.TopicID) as DatePosted," +
            " (Select Msg" +
            " From Posts as p3" +
            " Where p3.TopicID = mt.TopicID" +
            " AND p3.PostTime = DatePosted) as Message," +
            " (Select UserName" +
            " From Users" +
            " Where UserID =" +
            " (Select UserID" +
            " From Posts as p" +
            " Where p.TopicID = mt.TopicID" +
            " And p.PostTime = DatePosted)) as PosterName," +
            " (Select FirstName" +
            " From Users" +
            " Where UserID =" +
            " (Select UserID" +
            " From Posts as p" +
            " Where p.TopicID = mt.TopicID" +
            " And p.PostTime = DatePosted)) as FirstName," +
            " (Select LastName" +
            " From Users" +
            " Where UserID =" +
            " (Select UserID" +
            " From Posts as p" +
            " Where p.TopicID = mt.TopicID" +
            " And p.PostTime = DatePosted)) as LastName" +
            " From Message_Topics as mt" +
            " Where mt.GroupID = " + config.pool.escape(req.params.groupid);

        //Change request to return topics and responses by "pages"? groups of 10 or something? grouped by date

        //Perform the request
        config.pool.query(getRequest, function (err, result) {
            //If an error happens, log
            if (err) {
                console.log(err);
            }
            //Return message board topics and meta information
            else {
                res.json(result);
            }
        });
    }
    //If all required information is not present, send back an error message
    else {
        res.status(400).json({ status: "error", message: "missing parameter in GET request" });
    }
});

//Post for adding a response to a specific messageboard topic in a group
router.post('/:groupid(\\d+)/messageboard/:topicid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check to make sure all required info is present
    if (req.body.msg) {
        //Creat the request
        var insertRequest = "Insert Into Posts (TopicID, UserID, Msg, PostTime) values (" + req.params.topicid + ", " + config.pool.escape(req.body.decoded.UserID) + "," + config.pool.escape(req.body.msg) + ", NOW())";

        //Perform the request
        config.pool.query(insertRequest, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200);
                res.json({
                    status: "success",
                    message: "Response successfully added to message board topic."
                });
            }
        });
    }
    //If not all required info is present give an error
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in POST request"
        });
    }
});

//add a new topic
router.post('/:groupid(\\d+)/messageboard/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //check that all required information is passed
    if (req.params.groupid && req.body.title && req.body.decoded.UserID && req.body.msg) {
        //Create a var that has the call to the procedure
        var insertRequest = "Call addTopic(" + config.pool.escape(req.params.groupid) +
            ", " + config.pool.escape(req.body.title) +
            ", " + config.pool.escape(req.body.decoded.UserID) +
            ", " + config.pool.escape(req.body.msg) + ")";

        //Perform the request
        config.pool.query(insertRequest, function (err, result) {
            //If an error happens, log
            if (err) {
                console.log(err);
            }
            //If the message board topic was made, pass back success
            else {
                res.status(200).json({ status: "success", message: "Topic successfully added to group's message board." });
            }
        });
    }
    //If the request does not have the correct info, send back error message
    else {
        res.status(400).json({ status: "error", message: "missing parameter in POST request" });
    }
});


//Endpoint for editing a message board topic
router.put('/:groupid(\\d+)/messageboard/:topicid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    //Check for all needed info
    if (req.body.newTitle) {
        //Create the request
        var editRequest = "Update Message_Topics" +
            " Set Title = " + config.pool.escape(req.body.newTitle) +
            " Where TopicID = " + config.pool.escape(req.params.topicid) +
            " And GroupID = " + config.pool.escape(req.params.groupid) + ";";

        config.pool.query(editRequest, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200);
                res.json({
                    status: "success",
                    message: "Edit successfully made to message board topic."
                });
                res.end();
            }
        });
    }
    //If the request is missing params, send back an error
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in PUT request"
        });
    }
});

//NEEDS MAJOR UPDATING FOR SECURITY!!!!!!!!!!
//Endpoint for editing a message board topic response
router.put('/:groupid(\\d+)/messageboard/:topicid(\\d+)/:postid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Check for all needed info
    if (req.body.newMsg) {

        //Create the request
        var editRequst = "Update Posts" +
            " Set Msg = " + config.pool.escape(req.body.newMsg) +
            " Where PostID = " + config.pool.escape(req.params.postid) + ";";

        config.pool.query(editRequst, function (err, result) {
            //If there is an error, log it
            if (err) {
                console.log(err);
            }
            //If the response was made, return a status indicating success
            else {
                res.status(200);
                res.json({
                    status: "success",
                    message: "Edit successfully made to the message board response."
                });
            }
        });
    }
    //If the request is missing params, send back an error
    else {
        res.status(400);
        res.json({
            status: "error",
            message: "missing parameter in PUT request"
        });
    }
});

//NEEDS MAJOR UPDATING FOR SECURITY!!!!!!!!!!
//Endpoint for deleting an entire message board topic and all responses
router.delete('/:groupid(\\d+)/messageboard/:topicid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {

    //Create the request
    var deleteRequest = "Call deleteTopic(" + config.pool.escape(req.params.topicid) + ");";

    config.pool.query(deleteRequest, function (err, result) {
        //If there is an error, log it
        if (err) {
            console.log(err);
        }
        //If the response was made, return a status indicating success
        else {
            res.status(200);
            res.json({
                status: "success",
                message: "Successfully deleted message board topic."
            });
        }
    });
});

//NEEDS MAJOR UPDATING FOR SECURITY!!!!!!!!!!
//Endpoint for deleting a message board response
router.delete('/:groupid(\\d+)/messageboard/:topicid(\\d+)/:postid(\\d+)/', [auth.CheckAuthToken, auth.CheckInGroup], function (req, res) {
    var deleteRequest = "Delete from Posts" +
        "Where PostID = " + config.pool.escape(req.params.postid) + ";";

    config.pool.query(deleteRequest, function (err, result) {
        //If there is an error, log it
        if (err) {
            console.log(err);
        }
        //If the response was made, return a status indicating success
        else {
            res.status(200);
            res.json({
                status: "success",
                message: "Successfully deleted message board response."
            });
        }
    });
});



module.exports = router;



