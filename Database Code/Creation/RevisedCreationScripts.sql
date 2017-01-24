USE WHOSHOME;
-- use jptest;

 -- Drop table statements
  Drop Table If Exists Bills, Posts, Message_Topics, Items, Lists, User_Locations, User_Groups, Users, Group_Locations, Groups;

 -- Table creaiton
/*
  Table: Groups
  Purpose: Table for the groups of who's home.

  Maybe add a discription column?
*/
Create Table Groups (
  GroupID           bigint        not null auto_increment
  , GroupName       varchar(56)   not null
  , primary key (GroupID)
  ) ENGINE = INNODB;

 /*
  Table: Group_Locations
  Purpose: Table for holding the locations for each group
*/
Create Table Group_Locations (
  LocationID        bigint        not null auto_increment
  , GroupID         bigint        not null
  , SSID            varchar(32)   not null
  , NetName         varchar(32)   not null
  , foreign key (GroupID) references Groups(GroupID)
  , primary key (LocationID)
  ) ENGINE = INNODB;

/*
  Table: Users
  Purpose: Table for the users of who's home, most everything is required
  so many columns are not null

  Should first and last name be not null?
  Default values for active/pushnot?
*/
Create Table Users (
  UserID            bigint        not null auto_increment
  , UserName        varchar(20)   not null unique
  , FirstName       varchar(56)
  , LastName        varchar(56)
  , Email           varchar(50)   not null unique
  , Pass            char(128)     not null
  , Salt            char(32)      not null
  , Active          bool
  , PushNot         bool
  , LocationID      bigint
  , LocationActive  bool
  , foreign key (LocationID) references Group_Locations(LocationID)
  , primary key (UserID)
  ) ENGINE = INNODB;

/*
  Table:  User_Groups
  Purpose: Linking table for groups and users
*/
Create Table User_Groups (
  UserID            bigint        not null
  , GroupID         bigint        not null
  , foreign key (UserID) references Users(UserID)
  , foreign key (GroupID) references Groups(GroupID)
  , primary key (UserID, GroupID)
  ) ENGINE = INNODB;

/*
  Table: Lists
  Purpose: Table for group's lists, does not hold the list items only the list metadata
*/
Create Table Lists (
  ListID            bigint        not null auto_increment
  , GroupID         bigint        not null
  , UserID          bigint        not null
  , Title           varchar(60)
  , PostTime        datetime
  , foreign key (GroupID) references Groups(GroupID)
  , foreign key (UserID) references Users(UserID)
  , primary key (ListID)
  ) ENGINE = INNODB;

/*
  Table: Items
  Purpose: Table for items in a list
*/
Create Table Items (
  ItemID            bigint        not null auto_increment
  , ListID          bigint        not null
  , UserID          bigint        not null
  , ItemText        varchar(40)
  , Completed       bool
  , PostTime        datetime
  , foreign key (ListID) references Lists(ListID)
  , foreign key (UserID) references Users(UserID)
  , primary key (ItemID)
  ) ENGINE = INNODB;

/*
  Table: Message_Topics
  Purpose: Table for the message board part of who's home. Holds
  the metadata of topics for a group's message board
*/
Create Table Message_Topics (
  TopicID           bigint        not null auto_increment
  , GroupID         bigint        not null
  , Title           varchar(50)
  , foreign key (GroupID) references Groups(GroupID)
  , primary key (TopicID)
  ) ENGINE = INNODB;

/*
  Table: Posts
  Purpose: Teble for posts within a message board topic
*/
Create Table Posts (
  PostID            bigint        not null auto_increment
  , TopicID         bigint        not null
  , UserID          bigint        not null
  , Msg             varchar(1024)
  , PostTime        datetime
  , foreign key (TopicID) references Message_Topics(TopicID)
  , foreign key (UserID) references Users(UserID)
  , primary key (PostID)
  ) ENGINE = INNODB;

/*
  Table: Bills
  Purpose: Table for the bills part of who's home groups.
  
  May need some changes later when that part is added but
  works for now
*/
Create Table Bills (
  BillID            bigint        not null auto_increment
  , GroupID         bigint        not null
  , UserID          bigint        not null
  , Name            varchar(26)
  , Description     varchar(1024)
  , Amount          decimal(13,2)
  , foreign key (GroupID) references Groups(GroupID)
  , foreign key (UserID) references Users(UserID)
  , primary key (BillID)
  ) ENGINE = INNODB;