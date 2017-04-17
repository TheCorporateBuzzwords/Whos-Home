-- USE WHOSHOME;
-- use jptest;

-- Drop statements for everything, allows for everything to be executed at once
Drop Procedure If Exists addGroup;
Drop Procedure If Exists addTopic;
Drop Procedure If Exists deleteTopic;
Drop Procedure If Exists deleteList;
Drop Procedure If Exists removeUserFromGroup;
Drop Procedure If Exists updateUserLocations;

-- Procedure creation scripts

/*
  Procedure for creating a new group and adding the creating user to it through the linking table
  Perams:
  gName = Name of the created group (does not have to be unique)
  uID = userID of the user creating the group
  */
Delimiter //
Create Procedure addGroup
  (In gName char(56), In uID bigint)
Begin
  -- Declare any vars
  Declare gID bigint;

  -- Add new group
  Insert Into Groups (GroupName) Values (gName);

  -- Get ID of new group
  Select @gID:=LAST_INSERT_ID();

  -- Add user/group to linking table
  Insert Into User_Groups (UserID, GroupID) Values (uID, @gID);

  -- Add the the user to user_locations linking table for that group
  Insert Into User_Locations (UserID, GroupID) Values (uID, @gID);

End //
Delimiter;

/*
  Procedure for creating a new message board topic and adds the first post to the topis
  Params:
    gID = groupID of which group to add the messageboard topic to
    title = Title of the messageboard topic
    uID = userID of the user creating the topic
    msg = content of the post
*/
Delimiter //
Create Procedure addTopic
  (In gID bigint, In title varchar(50), In uID bigint, In message varchar(1024))
Begin
  -- Declare any vars
  Declare tID bigint;

  -- Add the new message topic
  Insert Into Message_Topics (GroupID, Title) Values (gID, title);

  -- Get the ID of the topic added
  Select @tID:=LAST_INSERT_ID();

  -- Add the post for the new topic
  Insert Into Posts (TopicID, UserID, Msg, PostTime) Values (@tID, uID, message, NOW());

End //
Delimiter;

/*
  Procedure for deleting a message board topic and all posts in it
  Params:
    tID = ID of the message board topic to delete
*/
Delimiter //
Create Procedure deleteTopic
  (In tID bigint)
Begin
  -- Delete all message board posts with the passed in topicID
  Delete from Posts
  Where TopicID = tID;

  -- Delete the message board topic with the passed in topicID
  Delete from Message_Topics
  Where TopicID = tID;

End //
Delimiter;

/*
  Procedure for deleting a list and all items within the list
  Params:
    lID = ID of the list to delete
*/
Delimiter //
Create Procedure deleteList
  (In lID bigint)
Begin
  -- Delete all list items connected to the passed in listID
  Delete From Items
  Where ListID = lID;

  -- Delete the list with the passed in listID
  Delete From Lists
  Where ListID = lID;

End //
Delimiter;

/*
  Procedure for removing a user from a group.
  This procedure will remore a user from a group without effecting any of the groups content
  created by that user.
  
  NOTE: If the removed user is the last user in a group, then the group and all of its
  content is removed
*/
Delimiter //
Create Procedure removeUserFromGroup
  (In gID bigint, In uID bigint)
Begin

  Declare UsersLeft bigint;

  -- Remove the user from the group
  Delete From User_Groups
  Where UserID = uID
    And GroupID = gID;
  
  -- REMOVE THE USER FROM THE GROUP LOCATIONS LINKING TABLE
  Delete From User_Locations
  Where UserID = uID
    And GroupID = gID;

  -- Check if the group has any members left
  -- If there are no members left, delete the group and all things connected to the group
  Select count(*) Into UsersLeft
  From User_Groups
  Where GroupID = gID;
  
  -- If there are members left, don't do anything
  If (UsersLeft = 0) Then
    Begin
    -- Delete all bills where groupid = groupid
      Delete From Bills
      Where GroupId = gID;
    -- For each message topic where groupid = groupid, delete all its posts
      Delete From Posts
      Where TopicID In (Select TopicID 
                        From Message_Topics
                        Where GroupID = gID);
    -- Delete all message topics where groupid = groupid
      Delete From Message_Topics
      Where GroupID = gID;
    -- For each list where groupid = groupid, delete all its items
      Delete From Items
      Where ListID In (Select ListID
                        From Lists
                        Where GroupID = gID);
    -- Delete all lists where groupid = groupid
      Delete From Lists
      Where GroupID = gID;
    -- Delete all user_groups? maybe not needed because this will already be empty of that group
      -- Not Needed
    -- Delete all invites to the group NOTE: is this correct behavior?
      Delete From Invites
      Where GroupID = gID;
    -- Delete all group locations where groupid = groupid
      Delete From Group_Locations
      Where GroupID = gID;
    -- Delete the group where groupid = groupid
      Delete From Groups
      Where GroupID = gID;
    End;
  End If;

End //
Delimiter;

/*
  Procedure for updated a user's locations in all groups that they are part off
*/
Delimiter //
Create Procedure updateUserLocations
  (In uID bigint, In newSSID varchar(32))
Begin

Declare bDone Int;

Declare foundUID bigint;
Declare foundGID bigint;
Declare foundLID bigint;

Declare updateLocationFound int;

Declare curs Cursor For (
  Select UserID, GroupID, LocationID
  From User_Locations
  Where UserID = uID);
Declare Continue Handler For Not Found Set bDone = 1;

Open curs;

Set bDone = 0;
-- For each entry in the User_Locations that matchs the UserID
Repeat
  Fetch curs Into foundUID, foundGID, foundLID;

  -- Grab the groupID from the entry and seach for a group_location that matchs the SSID and groupID
  Select count(*) Into updateLocationFound
  From Group_Locations
  Where GroupID = foundGID
    and SSID = newSSID;

  -- If one is found, set the LocationID in the User_Locations to the ID for that location
  If (updateLocationFound != 0) Then
    Update User_Locations
    Set LocationID = (Select LocationID
                      From Group_Locations
                      Where GroupID = foundGID
                        and SSID = newSSID)
    Where UserID = uID
      and GroupID = foundGID;
  -- Else, Set the LocationID in the User_Location to null
  Else
    Update User_Locations
    Set LocationID = NULL
    Where UserID = uID
      and GroupID = foundGID;
  End If;

Until bDone End Repeat;
Close curs;

End //
Delimiter;

DELIMITER //
CREATE PROCEDURE get_personal_bills
  (IN gId BIGINT, IN rId BIGINT)
BEGIN
  SELECT BillID, GroupID, u1.UserName AS Sender, u2.UserName AS Recipient, CategoryID, Title, Description, Amount, DATE_FORMAT(DateDue, '%c/%d/%Y %r:%h:%s') AS DateDue
  FROM Bills b
  INNER JOIN Users u1 ON b.RecipientID = u1.UserID
  INNER JOIN Users u2 ON b.SenderID = u2.UserID
  WHERE GroupId = gId AND RecipientId = rId;
END//
DELIMITER;

DELIMITER //
CREATE PROCEDURE get_group_bills
  (IN gId BIGINT)
BEGIN
  SELECT BillID, GroupID, u1.UserName AS Sender, u2.UserName AS Recipient, CategoryID, Title, Description, Amount, DATE_FORMAT(DateDue, '%c/%d/%Y %r:%h:%s') AS DateDue
  FROM Bills b
  INNER JOIN Users u1 ON b.RecipientID = u1.UserID
  INNER JOIN Users u2 ON b.SenderID = u2.UserID
  WHERE GroupId = gId;
END//
DELIMITER;

SET @GROUPID = 43;
SET @RID = 4;
CALL get_personal_bills(4, 43);
CALL get_group_bills(43);


DELIMITER //
CREATE PROCEDURE get_invites
  (IN uId BIGINT, IN gId BIGINT)
BEGIN
  SELECT * 
  FROM Invites 
  WHERE RecipientID = uId AND GroupID = gId;
END//
DELIMITER;
CALL get_invites(4, 49)



DELIMITER //
CREATE PROCEDURE accept_invite
  (IN uId BIGINT, IN gId BIGINT)
BEGIN
  INSERT INTO User_Groups(UserID, GroupID) VALUES (uid, gid);
  DELETE FROM Invites WHERE GroupId = gid AND RecipientID = uid;
END//
DELIMITER;

CALL accept_invite(4, 49);

DELIMITER //
CREATE PROCEDURE get_user_group
  (IN uId BIGINT, IN gId BIGINT)
BEGIN
  SELECT * FROM User_Groups WHERE UserID = uid AND GroupID = gid;
END//
DELIMITER;

CALL get_user_group(1, 43);

DELIMITER //
CREATE PROCEDURE delete_invite
  (IN gId BIGINT, IN rId BIGINT)
BEGIN
  DELETE FROM Invites WHERE GroupID = gId AND RecipientID = rId;
END//
DELIMITER;

DELIMITER //
CREATE PROCEDURE get_userid
  (IN uName VARCHAR(20))
BEGIN
  SELECT UserID
  FROM Users 
  WHERE UserName = uName;
END//
DELIMITER;

CALL get_userid('ogcris');

DELIMITER //
CREATE PROCEDURE insert_invite
  (IN gId BIGINT, IN sId BIGINT, rId BIGINT) /*where gId = groupid, sId = senderid, and rId = recipientid*/
BEGIN
  INSERT INTO Invites (GroupID, InviterID, RecipientID) VALUES (gId, sId, rId);
END//
DELIMITER;

CALL insert_invite(43, 2, 1);

DELIMITER //
CREATE PROCEDURE get_list_items
(IN lId BIGINT)
BEGIN
SELECT Items.ItemID, Items.ListID, Items.UserID, Items.ItemText, Items.Completed, Items.PostTime, Users.UserName, Users.FirstName, Users.LastName 
  FROM Items 
  INNER JOIN Users 
  ON Items.UserID = Users.UserID 
  WHERE ListID = lId;
END//
DELIMITER;

CALL get_list_items(34);
SELECT * FROM Lists;


DELIMITER //
CREATE PROCEDURE get_username
(IN uName VARCHAR(20))
BEGIN
SELECT UserName FROM Users WHERE UserName = uName;
END//
DELIMITER;

CALL get_username('ogcris');

DELIMITER //
CREATE PROCEDURE get_email
(IN uEmail VARCHAR(50))
BEGIN
SELECT UserName, Email
FROM Users 
WHERE Email = uemail;
END//
DELIMITER;

CALL get_email('Email@test.com');

DELIMITER //
CREATE PROCEDURE insert_user
(IN uName VARCHAR(20), IN fName VARCHAR(56), IN lName VARCHAR(56), IN uEmail VARCHAR(50), IN pass CHAR(128), IN salt CHAR(32))
BEGIN
INSERT INTO Users (UserName, FirstName, LastName, Email, Pass, Salt, Active, PushNot, LocationID, LocationActive)
    VALUES (uName, fName, lName, uEmail, pass, salt, false, false, null, false);
END//
DELIMITER;

CALL insert_user('mynameismyname', 'firstname', 'lastname', 'kdsjfs@email.nill', '9a6808581ea032be04b9b1502717885f8554f5e17775ef8ab2925fd51a07f1d74ac2d4e94e5e043a552e82de048db45827e39d8daf08434622d8d3c25cd85bac', '7f3e1cd079b87bba89b62870bed7b9db');

