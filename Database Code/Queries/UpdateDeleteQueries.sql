/********************
/ Update password for the "change password" page. This is only if the user is logged in and knows their password
/ Checks what user entered as their "current password" against what the DB has stored
/ If the passwords match, it sets the DB password to the updated password.
********************/

IF Pass = /*current password entered */
  UPDATE Users
  SET Pass = /*updated password*/
  WHERE UserID = /*id of user who requested change*/



/********************
/ Update password for the "forgot password" feature
/ Updates password based on user's email.
********************/

UPDATE Users
SET Pass = /*updated password*/
WHERE Email = /*user email*/



/********************
/ Update query for when a user's location has changed
/ Update the user's current location based on the SSID they've connected to
/ Also can be used if a user moves out of range of a location
********************/

UPDATE Users
SET LocationsID = 
(SELECT LocationsID FROM SharedLocations WHERE SSID = /*current location*/)
WHERE UserID = /*current user*/



/********************
/ Query for when a user chooses to delete a location (connection) from a group's list of locations
********************/
DELETE FROM SharedLocations
WHERE LocationsID = /*selected location to be deleted*/
