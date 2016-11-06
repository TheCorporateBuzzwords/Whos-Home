USE WHOSHOME;

DROP TABLE IF EXISTS SharedLocations, UserLocations, User_Group, Bills, Groups, Lists, Items, MessageBoard,
  Posts, Users;

CREATE TABLE SharedLocations(
	LocationsID INT NOT NULL AUTO_INCREMENT,
	SSID varchar(32),
	NetName varchar(32),
  PRIMARY KEY (LocationsID)
) ENGINE=INNODB;

CREATE TABLE Users(
	UserID INT NOT NULL AUTO_INCREMENT,
	LocationsID INT,
	UserName varchar(20) NOT NULL UNIQUE,
	FirstName varchar(56),
	LastName varchar(56),
	Email varchar(50) NOT NULL UNIQUE,
	Pass varchar(50),
	Active BOOL,
	PushNot BOOL,
  PRIMARY KEY (UserID)
) ENGINE=INNODB;

CREATE TABLE UserLocations(
  UserID INT,
  FOREIGN KEY (UserID)
    REFERENCES Users (UserID),
  LocationsID INT,
  FOREIGN KEY (LocationsID)
    REFERENCES SharedLocations (LocationsID),
	InRange BOOL,
  Active  BOOL,
  PRIMARY KEY(UserID, LocationsID)
) ENGINE=INNODB;

CREATE TABLE Posts(
	PostID INT NOT NULL AUTO_INCREMENT,
	UserID INT,
  FOREIGN KEY (UserID) REFERENCES Users (UserID),
	ResponseID INT,
  FOREIGN KEY (ResponseID) REFERENCES Posts (PostID),
	Title varchar(20),
	Msg varchar(1024),
	PostTime datetime,
  PRIMARY KEY (PostID)
) ENGINE=INNODB;

CREATE TABLE Items(
	ItemID INT NOT NULL AUTO_INCREMENT,
	ItemText varchar(20),
	Completed BOOL,
  PRIMARY KEY(ItemID)
) ENGINE=INNODB;

CREATE TABLE Lists(
	ListsID INT NOT NULL AUTO_INCREMENT,
	UserID INT,
  FOREIGN KEY (UserID) References Users (UserID),
	Items INT,
  FOREIGN KEY (Items) References Items (ItemID),
	PostTime datetime,
  PRIMARY KEY (ListsID)
) ENGINE=INNODB;

CREATE TABLE Groups(
	GroupID INT NOT NULL AUTO_INCREMENT,
	GroupName varchar(56),
	PostID INT,
  FOREIGN KEY (PostID) REFERENCES Posts (PostID),
	ListsID INT,
  FOREIGN KEY (ListsID) REFERENCES Lists (ListsID),
  PRIMARY KEY (GroupID)
) ENGINE=INNODB;
 
CREATE TABLE User_Group(
	UserID INT,
  FOREIGN KEY (UserID) REFERENCES Users (UserID),
	GroupID INT,
  FOREIGN KEY (GroupID) REFERENCES Groups (GroupID),
	PRIMARY KEY(UserID, GroupID)
) ENGINE=INNODB;

CREATE TABLE Bills(
	BillID INT NOT NULL AUTO_INCREMENT,
	UserID INT,
  FOREIGN KEY (UserID) REFERENCES Users (UserID),
	GroupID INT,
  FOREIGN KEY (GroupID) REFERENCES Groups (GroupID),
	BillName varchar(26),
	BillDescription varchar(450),
	BillAmount DECIMAL(13, 2),
  PRIMARY KEY(BillID)
) ENGINE=INNODB;

