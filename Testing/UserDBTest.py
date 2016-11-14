#!/usr/bin/python
import csv
import MySQLdb
import pdb
from threading import Thread
from time import sleep

pdb.set_trace()
#cursor.execute("SELECT * FROM Users")
db = None
cursor = None

class User:
	def __init__(self):
		user_name = ""
		first_name = ""
		last_name = ""
		email = ""
		passw = ""
		active = "NULL"
		pushnot = "NULL"

	def SetValues(self, uName, fName, lName, eEmail, pPassw, aActive = "NULL", pPushnot = "NULL"):
		self.user_name = uName
		self.first_name = fName
		self.last_name = lName
		self.email = eEmail
		self.passw = pPassw
		self.active = aActive
		self.pushnot = pPushnot

	def GetAttr(self):
		return [self.user_name, self.first_name, self.last_name, self.email, self.passw, self.active, self.pushnot]
	
class Group:
	def __init__(self):
		group_name = ""
		post_id = "NULL"
		lists_id = "NULL"

	def SetValues(self, Gname, Pid = "NULL", Lid = "NULL"):
		self.group_name = Gname
		self.posts_id = Pid
		self.lists_id = Lid
	
	def GetAttr(self):
		return [self.group_name, self.posts_id, self.lists_id]

def TryExec(sql):
	try:
		cursor.execute(sql)
		db.commit()
		print("Commit succesful")
	except:
		db.rollback()
		print("Commit unsuccesful")

def InitDB(_host, _user, _passwd, _db):
	global db
	global cursor
	db = MySQLdb.connect(host=_host, user=_user, passwd=_passwd, db=_db) 
	cursor = db.cursor()

def AddUser(locations_id, user_name, first_name, last_name, email, passw, active, pushnot):
	db = MySQLdb.connect("192.168.1.111", "limited", "Speci@login$$$69$$$", "WHOSHOME") 
	cursor = db.cursor()

	insert_string = '(' + locations_id + ', ' + '\'' + user_name + '\', ' + '\'' + first_name + '\', ' + '\'' + last_name + '\', ' + '\'' + email + '\', ' + '\'' + passw + '\', ' + str(active).upper() + ', ' + str(pushnot).upper() + ')'
	#Add User1

	sql = """INSERT INTO Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot)
	VALUES """ + insert_string + ';'
	try:
		cursor.execute(sql)
		db.commit()
		print("Commit succesful")
	except:
		db.rollback()
		print("Commit unsuccesful")
	cursor.execute("SELECT UserID FROM Users WHERE UserName = '%s';" % (user_name))
	row = cursor.fetchone()
	db.close()
	return row[0]
	

#Add Group1
def AddGroup(group_name, post_id, lists_id):
	db = MySQLdb.connect("192.168.1.111", "limited", "Speci@login$$$69$$$", "WHOSHOME") 
	cursor = db.cursor()
	
	insert_string = '(\'' + group_name + '\', ' + post_id + ', ' + lists_id + ')'
	sql = """INSERT INTO Groups (GroupName, PostID, ListsID)
	VALUES """ + insert_string + ';'
	try:
		cursor.execute(sql)
		db.commit()
		print("Commit succesful")
	except:
		db.rollback()
		print("Commit unsuccesful")
	cursor.execute("SELECT GroupID FROM Groups WHERE GroupName = '%s';" % (group_name))
	row = cursor.fetchone()
	db.close()
	return row[0]

#Add User1 to Group1
def User_Group(user_id, group_id):
#	insert_string = '(' + user_id + ', ' + group_id + ')'
	db = MySQLdb.connect("192.168.1.111", "limited", "Speci@login$$$69$$$", "WHOSHOME") 
	cursor = db.cursor()
	
	insert_string = '(' + str(user_id) + ', ' + str(group_id) + ')'
	sql = ("""INSERT INTO User_Group
	SET UserID = %s, GroupID = %s;""" % (str(user_id), str(group_id)))
	try:
		cursor.execute(sql)
		db.commit()
		print("Commit succesful")
	except:
		db.rollback()
		print("Commit unsuccesful")
	db.close()


#def User_Location(UserID, LocationsID, InRange, Active):
#	sql = """INSERT INTO UserLocations (UserID, LocationsID, InRange, Active) VALUES ('%s', '%s', %s, %s);""" % (UserID, LocationsID, str(InRange).upper(), str(Active).upper())
#	TryExec(sql)
#def SharedLocations(SSID, NetName):
#	sql = """INSERT INTO SharedLocations(SSID, NetName) VALUES ('%s', '%s');""" % (SSID, NetName) 
#	TryExec(sql)



def OrderedAdd(thread_num, User1, User2, Group):

	UVals1 = User1.GetAttr()
	UID1 = AddUser('NULL', str(UVals1[0]), str(UVals1[1]), str(UVals1[2]), str(UVals1[3]), str(UVals1[4]), 'NULL', 'NULL')
	UVals2 = User2.GetAttr()
	UID2 = AddUser('NULL', str(UVals2[0]), str(UVals2[1]), str(UVals2[2]), str(UVals2[3]), str(UVals2[4]), 'NULL', 'NULL')

	GVals = Group.GetAttr()
	GID = AddGroup(str(GVals[0]), 'NULL', 'NULL')
	
	User_Group(UID1, GID)
	User_Group(UID2, GID)

	print("Completed Thread: %d", thread_num)

	

def main():
	global db

	with open('users.csv', 'r') as user_file:
		reader1 = csv.reader(user_file)
		user_list = list(reader1)

	with open('groups.csv', 'r') as groups_file:
		reader2 = csv.reader(groups_file)
		groups_list = list(reader2)
	
	UserArr = []
	GArray = []
	for i in range(1, 996):
		User1 = User()
		User1.SetValues(user_list[i][0], user_list[i][1], user_list[i][2], user_list[i][3], user_list[i][4])
		UserArr.append(User1)
		
	for i in range(500):
		Group1 = Group()
		Group1.SetValues(groups_list[i][0], 'NULL', 'NULL')
		GArray.append(Group1)
	
	all_threads = []
	for i in range(min(len(UserArr), len(GArray))):
		thread = Thread(target = OrderedAdd, args = (i, UserArr[i], UserArr[i+1], GArray[i]))
		thread.start()
		all_threads.append(thread)
		i+=2

	for t in all_threads:
		t.join()

	print("Complete")

main()
