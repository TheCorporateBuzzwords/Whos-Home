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

##Groups/messageboard
### Create a messageboard post topic
POST /groups/{groupid}/messagetopic/ (authenticated)

\*title: Title of the message board post (50 char limit)

\*msg: Content of the message board post (1024 char limit)

### Get all messageboard post topics
GET /groups/{groupid}/messagetopic/ (authenticated)

Returns: TopicID, Title, Date/Time posted, and Username of original poster for each topic in a group

### Respond to a messageboard topic
POST /groups/{groupid}/messageposts/ (authenticated)

\*topicid: ID of the topic being responding to

\*msg: Content of the reponse post (1024 char) limit

### Get responses to a single post topic
GET /groups/{groupid}/{topicid}/messageposts/ (authenticated)

Returns: Response msg, Date/Time posted, and Username of the original poster for each response to a topic