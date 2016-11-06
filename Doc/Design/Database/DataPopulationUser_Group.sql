SELECT * FROM Groups


insert into User_Group
  SET UserID = (SELECT UserID
                FROM Users
                WHERE UserName = 'rtorresc'),
  GroupID = (SELECT GroupID
            FROM Groups
            WHERE GroupName = 'Welch and Sons');


  insert into User_Group
  SET UserID = 1, GroupID = 1;

  insert into User_Group
  SET UserID = 2, GroupID = 1;

  insert into User_Group
  SET UserID = 3, GroupID = 1;

insert into User_Group
  SET UserID = 4, GroupID = 1;

insert into User_Group
  SET UserID = 5, GroupID = 1;

insert into User_Group
  SET UserID = 6, GroupID = 1;

insert into User_Group
  SET UserID = 7, GroupID = 2;

insert into User_Group
  SET UserID = 8, GroupID = 2;

insert into User_Group
  SET UserID = 9, GroupID = 3;

insert into User_Group
  SET UserID = 10, GroupID = 3;

insert into User_Group
  SET UserID = 1, GroupID = 3;

SELECT * FROM User_Group