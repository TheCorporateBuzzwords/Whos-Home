/*User and group information - Returns group names, usernames associated with the groups, and user’s locations*/
SELECT u.UserName, g.GroupName, sl.NetName
FROM Users u
INNER JOIN User_Group ug ON u.UserID = ug.UserID
INNER JOIN Groups g ON ug.GroupID = g.GroupID
INNER JOIN UserLocations ul ON u.LocationsID = ul.LocationsID
INNER JOIN SharedLocations sl ON ul.LocationsID = sl.LocationsID
ORDER BY g.GroupName, u.UserName, sl.NetName


/*Post information - Returns messages, the group their associate with, and their responses.*/
SELECT p.Msg, g.GroupName, u.UserName, p.PostID, p.ResponseID
FROM Posts p
INNER JOIN Users u ON p.UserID = u.UserID
INNER JOIN User_Group ug ON u.UserID = ug.UserID
INNER JOIN Groups g ON ug.GroupID = g.GroupID
ORDER BY g.GroupName, p.PostID, p.ResponseID


/*List Information - Returns list items, the group their associated with, time created, and user who created it.*/
SELECT l.Items, l.Time u.UserName, g.GroupName
FROM Lists l
INNER JOIN Users u ON l.UserID = u.UserID
INNER JOIN User_Group ug ON u.UserID = ug.UserID
INNER JOIN Groups g ON ug.GroupID = g.GroupID
ORDER BY g.GroupName, l.Time, l.Items, u.UserName


/*Bill Information - Returns bill information, user associated with bill, and group associated with bill*/
SELECT b.BillName, b.BillDescription, b.BillAmount, GroupName, UserName
FROM Bills b
INNER JOIN Groups g ON b.GroupID = g.GroupID
INNER JOIN User_Group ug ON g.GroupID = ug.GroupID
INNER JOIN Users u ON l.UserID = u.UserID
ORDER BY g.GroupName
