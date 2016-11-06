insert into Lists
  SET Items = null,
  PostTime = CURRENT_TIME(),
  UserID = (SELECT UserID 
            FROM Users
            WHERE UserName = 'rtorresc');

insert into Lists
  SET Items = null,
  PostTime = CURRENT_TIME(),
  UserID = 10;


  insert into Lists
  SET Items = null,
  PostTime = CURRENT_TIME(),
  UserID = 250;

   insert into Lists
  SET Items = null,
  PostTime = CURRENT_TIME(),
  UserID = 250;

 insert into Lists
  SET Items = null,
  PostTime = CURRENT_TIME(),
  UserID = 15;

 insert into Lists
  SET Items = null,
  PostTime = CURRENT_TIME(),
  UserID = 79;

   insert into Lists
  SET Items = null,
  PostTime = CURRENT_TIME(),
  UserID = 450;

   insert into Lists
  SET Items = null,
  PostTime = CURRENT_TIME(),
  UserID = 200;

SELECT * FROM Lists;
 