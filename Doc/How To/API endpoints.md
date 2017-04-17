\* = required

Authentication token can be sent with the header: x-access-token

## Users
### Create a new account
POST /users

\*Username: desired username for user. Must only contain letters and numbers, can't begin with a number.

\*FirstName: User's first name

\*LastName: User's last name

\*Email: User's email

\*Password: User's password

\*Confirm: Repeated password to confirm it's matched

### Get information for all groups a user belongs to
GET /users/groups (authenticated)

### Update User's location
PUT /users/location (authenticated)

\*bssid: BSSID for location user is at (null if not connected).

### Get information for all invites targeted at a single user
GET /users/invites (authenticated)

### Update home location
PUT /users/home (authenticated)

\*ssid: ssid of home location

### Register user for FCM
POST /users/fcm (authenticated)

\*regToken: registration token for FCM

## Session
### Login
POST /session

\*Username: User's username

\*Password: User's password

## Groups
### Get group information
GET /groups/{groupid} (authenticated)

\*groupid: id of group

### Create a new group
POST /groups/ (authenticated)

\*groupName: Name of the new group

### Edit the name of a group
PUT /groups/{groupid} (authenticated)

\*newName: Text of the new group name

## Groups/invitation
### create a group invitation for a user
POST /groups/{groupid}/invitation/ (authenticated)

\*groupid: id of the group the invite is for

\*recipient: username of person being invited

### Accept an existing invite to a group
GET /groups/{groupid}/invitation

deny: true if invitation is to be declined.

## Groups/location
### Creates a new location for the group
POST /groups/{groupid}/location/ (authenticated)

\*ssid: ssid for connection

\*locationName: name of new location

### Get information on all locations of a group
GET /groups/{groupid}/locations/ (authenticated)

## Groups/messageboard
### Create a messageboard post topic
POST /groups/{groupid}/messageboard/ (authenticated)

\*title: Title of the message board post (50 char limit)

\*msg: Content of the message board post (1024 char limit)

### Get all messageboard post topics
GET /groups/{groupid}/messageboard/ (authenticated)

Returns: TopicID, Title, Date/Time posted, Message, and Username/FirstName/Lastname of original poster for each topic in a group

### Respond to a messageboard topic
POST /groups/{groupid}/messageboard/ (authenticated)

\*topicid: ID of the topic being responding to

\*msg: Content of the reponse post (1024 char) limit

### Get responses to a single post topic
GET /groups/{groupid}/messageboard/{topicid}/ (authenticated)

Returns: (For each post to the topic that matches the passed in topicID)

PostID: ID of the message topic response

Msg: Content of the post

PostTime: Time the post was made

PostersName: Username of the original poster

### Edit a messageboard topic
PUT /groups/{groupid}/messageboard/{topicid}/ (authenticated)

\*newTitle: New title of the message board post (50 char limit)

### Edit a messageboard response
PUT /groups/{groupid}/messageboard/{topicid}/{postid} (authenticated)

\*newMsg: New message of the response (1024 char limit)

### Delete a messageboard topic and all responses
DELETE /groups/{groupid}/messageboard/{topicid} (authenticated)

Note: This will delete the topic and all response posts to that topic

### Delete a single message board response
DELETE /groups/{groupid}/messageboard/{topicid}/{postid} (authenticated)

## Groups/Lists

### Add new list
POST /groups/{groupid}/lists/

\*title: title of the list

### Add new list item
POST /groups/{groupid}/lists/{listid}

\*content: content of list item

### Get all lists
GET /groups/{groupid}/lists/

Returns: All lists in your group

### Get all list items for a list
GET /groups/{groupid}/lists/{listid}

Returns: (For all items in the list that matchs the passed in listid)

ItemID: ID value for the list item

ListID: ID value of the list the item belong to (should be the same as the passed in id

UserID: ID of the user who added the item

ItemText: Text of the item

Completed: Bool representing if the item is completed or not

PostTime: Time the item was created

UserName: Username of the user who made the item

FirstName: First name of the user who made the item

LastName: Last name of the user who made the item

### Update list items to completed/not completed
PUT /groups/{groupid}/lists/{listid}

\*listid: id of list item to be updated

\*completed: 1 (completed) or 0 (not completed)

### Edit an item in a list
PUT /groups/{groupid}/lists/{itemid}/ (authenticated)

\*itemid: id of the list item to edit

\*newText: The new text of the time

### Delete an item in a list
DELETE /groups/{groupid}/lists/{itemid}/ (authenticated)

\*itemid: id of the list item to delete

### Edit the title of a list
PUT /groups/{groupid}/lists/{listid}/ (authenticated)

\*listid: id of the list to edit

\*newTitle: The new text for the title of the list

### Delete a list and all of the list's items
DELETE /groups/{groupid}/lists/{listid}/ (authenticated)

\*listid: id of the list to delete

## Groups/Bills

### Add a new bill
POST /groups/{groupid}/bills/

\* recipient: id of bill recipient

\* category: category id of bill

\* title: title of bill

\* description: description of bill

\* amount: dollar amount of bill (i.e. 18.33)

\* date: date in format "mm/dd/yyyy hh:mm:ss"

### Get bills
GET /groups/{groupid}/bills/

recipient: optional id of recipient to only get bills for that one group member
