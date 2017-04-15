-- USE WHOSHOME;
-- use jptest;

-- Drop statements for everything, allows for everything to be executed at once
Drop Procedure If Exists addGroup;
Drop Procedure If Exists addTopic;
Drop Procedure If Exists deleteTopic;
Drop Procedure If Exists deleteList;
Drop Procedure If Exists removeUserFromGroup;

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

  -- Check if the group has any members left
  -- If there are no members left, delete the group and all things connected to the group
  Select count(*) Into UsersLeft
    From User_Groups
    Where GroupID = gID;

  -- REMOVE THE USER FROM THE GROUP LOCATIONS LINKING TABLE
  
  -- If there are members left, don't do anything
  -- If(UsersLeft == 0) Then
    -- Add the deletes to purge the group from the DB
  -- End If;

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

