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
### Invite a user to a group
GET /groups/{groupid}/invitation/?{recipient} (authenticated)

\*groupid: id of the group the invite is for

\*recipient: username of person being invited

### Accept an existing invite to a group
POST /groups/invitation

\*inviteToken: token received from POST to groups/invitation

## Groups/location
### Creates a new location for the group
POST /groups/{groupid}/location/ (authenticated)

\*ssid: ssid for connection

\*locationName: name of new location