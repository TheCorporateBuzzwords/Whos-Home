USE WHOSHOME;
-- use jptest;

-- Drop statements for everything, allows for everything to be executed at once
Drop Procedure If Exists addGroup;
Drop Procedure If Exists addTopic;

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
  Perams:
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

