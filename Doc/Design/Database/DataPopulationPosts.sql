insert into Posts
SET UserID = 1, ResponseID = NULL, Title = 'title1', Msg = 'hello world', PostTime = CURRENT_TIME();

insert into Posts
SET UserID = 1, ResponseID = NULL, Title = 'title2', Msg = 'goodbye world', PostTime = CURRENT_TIME();

insert into Posts
SET UserID = 79, ResponseID = NULL, Title = 'title3', Msg = 'I am hungry', PostTime = CURRENT_TIME();

insert into Posts
SET UserID = 400, ResponseID = NULL, Title = 'title4', Msg = 'I need food from the store', PostTime = CURRENT_TIME();

insert into Posts
SET UserID = 937, ResponseID = NULL, Title = 'Seriously though', Msg = 'I am very hungry', PostTime = CURRENT_TIME();

insert into Posts
SET UserID = 56, ResponseID = NULL, Title = 'Football', Msg = 'I would like to be watching football right now', PostTime = CURRENT_TIME();

insert into Posts
SET UserID = 199, ResponseID = NULL, Title = 'psst', Msg = 'just kidding nothing', PostTime = CURRENT_TIME();

insert into Posts
SET UserID = 573, ResponseID = NULL, Title = 'title2', Msg = 'goodbye world', PostTime = CURRENT_TIME();

SELECT * FROM Posts;