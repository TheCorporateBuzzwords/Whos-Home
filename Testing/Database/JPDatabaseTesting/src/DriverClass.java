import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.Statement;

import com.mysql.jdbc.Driver;

/**************************************************************************
Date Created: 11/9/16
Purpose:
  The program is a test suite to verify the functionality of the tables in WHOSHOME database.
 The tests with cover the database’s tables functionality and constraints.

Changes:
11/12/16 - Added phototyping for tests, Added output for each section
11/13/16 - Added code for tests and pass/fail conditions
  
**************************************************************************/
public class DriverClass {

	private final static String conURL = "jdbc:mysql://127.0.0.1:3306/jptest?autoReconnect=true&useSSL=false";
	private final static String userName = "TestUser";
	private final static String password = "password";
	
	public static void main(String[] args) {
		
		System.out.println("Welcome to Who'sHome database test program.");
		System.out.println("Attempting to connect to database...");
		
		try{
		//Class.forName("com.mysql.jdbc.Driver");
		Driver driver = new Driver();
		DriverManager.registerDriver(driver);
		
		//Connect to the datebase
		Connection conn = DriverManager.getConnection(conURL, userName, password);
		
		//Create a statement
		Statement stmt = conn.createStatement();
		
		System.out.println("Connection successful, beginning tests.");
		
		runTests(stmt);
		
		conn.close();
		}catch (Exception e){
			System.out.println("Connection failed.");
			System.out.println("Any error occured: " + e);
		}
	}
	
	private static void runTests(Statement stmt)
	{
		boolean finalResult = true;
		
		/**********************************
		  SharedLocations
		**********************************/
		if(SharedLocationsTableTest(stmt)){
			System.out.println("Table \"SharedLocation\" passed testing.");
		}
		else{
			System.out.println("Table \"SharedLocation\" failed testing.");
			finalResult = false;
		}
		
		/**********************************
		  Users
		**********************************/
		if(UsersTableTest(stmt)){
			System.out.println("Table \"Users\" passed testing.");
		}
		else{
			System.out.println("Table \"Users\" failed testing.");
			finalResult = false;
		}
		
		/**********************************
		  UserLocations
		**********************************/
		if(UserLocationsTableTest(stmt)){
			System.out.println("Table \"UserLocations\" passed testing.");
		}
		else{
			System.out.println("Table \"UserLocations\" failed testing.");
			finalResult = false;
		}
		
		/**********************************
		  Posts
		**********************************/
		if(PostsTableTest(stmt)){
			System.out.println("Table \"Posts\" passed testing.");
		}
		else{
			System.out.println("Table \"Posts\" failed testing.");
			finalResult = false;
		}
		
		/**********************************
		  Items
		**********************************/
		if(ItemsTableTest(stmt)){
			System.out.println("Table \"Items\" passed testing.");
		}
		else{
			System.out.println("Table \"Items\" failed testing.");
			finalResult = false;
		}
		
		/**********************************
		  Lists
		**********************************/
		if(ListsTableTest(stmt)){
			System.out.println("Table \"Lists\" passed testing.");
		}
		else{
			System.out.println("Table \"Lists\" failed testing.");
			finalResult = false;
		}
		
		/**********************************
		  Groups
		**********************************/
		if(GroupsTableTest(stmt)){
			System.out.println("Table \"Groups\" passed testing.");
		}
		else{
			System.out.println("Table \"Groups\" failed testing.");
			finalResult = false;
		}
		
		/**********************************
		  User_Group
		**********************************/
		if(User_GroupTableTest(stmt)){
			System.out.println("Table \"User_Group\" passed testing.");
		}
		else{
			System.out.println("Table \"User_Group\" failed testing.");
			finalResult = false;
		}
		
		/**********************************
		  Bills
		**********************************/
		if(BillsTableTest(stmt)){
			System.out.println("Table \"Bills\" passed testing.");
		}
		else{
			System.out.println("Table \"Bills\" failed testing.");
			finalResult = false;
		}
		
		if(finalResult){
			System.out.println("All test passed, database is probable working correctly.");
		}
		else{
			System.out.println("Not all test passed, fix the database.");
		}
	}
	
	private static boolean SharedLocationsTableTest(Statement stmt){
		boolean testResult = true;
		
		System.out.println("------------------------Testing SharedLocations Table------------------------");
		
		// Insert new location
		System.out.println("Inserting new location.");
		try{
			//Add location
			stmt.execute("insert into SharedLocations (SSID, NetName) values ('132.69.192.14/31', 'OIT');");

			//Test location is in database
			ResultSet rs = stmt.executeQuery("select * from SharedLocations where NetName = 'OIT';");
			
			rs.next();
			if(!(rs.getString("NetName").equals("OIT"))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove location
				System.out.println("Passed");
				stmt.execute("Delete from SharedLocations Where NetName = 'OIT';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// Edit location
		System.out.println("Editing a location.");
		try{
			//Add location
			stmt.execute("insert into SharedLocations (SSID, NetName) values ('132.69.192.14/31', 'OIT');");

			//Edit location
			stmt.execute("update SharedLocations set NetName = 'LCC' where NetName = 'OIT';");
			
			//Test edit
			ResultSet rs = stmt.executeQuery("select * from SharedLocations where NetName = 'LCC';");

			rs.next();
			if(!(rs.getString("NetName").equals("LCC"))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove location
				System.out.println("Passed");
				stmt.execute("delete from SharedLocations where NetName = 'LCC'");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// Delete existing location
		System.out.println("Deleting a location.");
		try{
			//Add location
			stmt.execute("insert into SharedLocations (SSID, NetName) values ('132.69.192.14/31', 'OIT');");
			
			//Delete location
			stmt.execute("Delete from SharedLocations Where NetName = 'OIT';");

			//test delete
			ResultSet rs = stmt.executeQuery("select * from SharedLocations where NetName = 'OIT';");
			
			if(!rs.next()){
				System.out.println("Passed");
			}
			else{
				System.out.println("Failed.");
				testResult = false;
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		//Delete location with FK?
		
		return testResult;
	}
	
	private static boolean UsersTableTest(Statement stmt){
		boolean testResult = true;
		
		System.out.println("------------------------Testing Users Table------------------------");
		// Add a new user
		System.out.println("Adding a new user");
		try{
			//Add User
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'FireOnTop', 'Patrick', 'Caruana', 'fake@email.com', 'lHaNSBpS', false, true);");

			//Test user is in database
			ResultSet rs = stmt.executeQuery("select * from Users where UserName = 'FireOnTop';");
			
			rs.next();
			if(!(rs.getString("UserName").equals("FireOnTop"))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove user
				System.out.println("Passed");
				stmt.execute("Delete from Users Where UserName = 'FireOnTop';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// Add a new user with a repeated name or email
		System.out.println("Adding a new user with a repeated name");
		try{
			//Add User
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'FireOnTop', 'Patrick', 'Caruana', 'fake@email.com', 'lHaNSBpS', false, true);");

			//Test user is in database
			ResultSet rs = stmt.executeQuery("select * from Users where UserName = 'FireOnTop';");
			
			//Add repeated username
			try{
				rs = stmt.executeQuery("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'FireOnTop', 'Terry', 'Ward', 'twardl@skyrock.com', 'pb1ml9FGw', false, false);");
			}catch (Exception e){
				//Remove user
				System.out.println("Passed");
				stmt.execute("Delete from Users Where UserName = 'FireOnTop';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// Edit user to valid name
		System.out.println("Editing a user's username to something valid");
		try{
			//Add User
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'FireOnTop', 'Patrick', 'Caruana', 'fake@email.com', 'lHaNSBpS', false, true);");

			//Edit username
			stmt.execute("update Users set UserName = 'Pandora' where UserName = 'FireOnTop';");
			
			//Test user is in database
			ResultSet rs = stmt.executeQuery("select * from Users where UserName = 'Pandora';");
			
			rs.next();
			if(!(rs.getString("UserName").equals("Pandora"))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove user
				System.out.println("Passed");
				stmt.execute("Delete from Users Where UserName = 'Pandora';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// Edit user to invalid name
		System.out.println("Editing a user's username to something invalid");
		try{
			//Add User
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'FireOnTop', 'Patrick', 'Caruana', 'fake@email.com', 'lHaNSBpS', false, true);");
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'Pandora', 'Willie', 'Myers', 'wmyersf@chron.com', '6jmkUxQYqk', false, false);");
			
			//Edit username
			try{
				stmt.execute("update Users set UserName = 'Pandora' where UserName = 'FireOnTop';");
			}catch (Exception e){
				System.out.println("Passed");
				stmt.execute("Delete from Users Where UserName = 'Pandora';");
				stmt.execute("Delete from Users Where UserName = 'FireOnTop';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// Delete user? with FK?
		
		return testResult;
	}
	
	private static boolean UserLocationsTableTest(Statement stmt){
		boolean testResult = true;
		
		System.out.println("------------------------Testing UserLocations Table------------------------");
		
		//Add user location
		System.out.println("Adding a new UserLocation");
		try{
			int userID = 0;
			int locationID = 0;
			int tmpu = 0;
			int tmpl = 0;
			
			//Add User and sharedLocations
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'FireOnTop', 'Patrick', 'Caruana', 'fake@email.com', 'lHaNSBpS', false, true);");
			stmt.execute("insert into SharedLocations (SSID, NetName) values ('132.69.192.14/31', 'OIT');");

			ResultSet urs = stmt.executeQuery("select * from Users where UserName = 'FireOnTop';");
			urs.next();
			userID = urs.getInt("UserID");
			ResultSet lrs = stmt.executeQuery("select * from SharedLocations where NetName = 'OIT';");
			lrs.next();
			locationID = lrs.getInt("LocationsID");
			
			//Add UserLocation
			stmt.execute("insert into UserLocations (UserID, LocationsID, InRange, Active) values ( " + userID + ", " + locationID + " , false ,false);");
			
			//Test UserLocation is in database
			ResultSet rs = stmt.executeQuery("select * from UserLocations where UserID = " + userID + " and LocationsID = " + locationID + ";");
			
			rs.next();
			tmpu = rs.getInt("UserID");
			tmpl = rs.getInt("LocationsID");
			if(tmpu != userID && tmpl != locationID){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove user
				System.out.println("Passed");
				
				stmt.execute("Delete from UserLocations where UserID = " + userID + " && LocationsID = " + locationID + ";");
				stmt.execute("Delete from Users Where UserName = 'FireOnTop';");
				stmt.execute("Delete from SharedLocations Where NetName = 'OIT';");
				}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		//Edit user location? Only real things to edit are inrange and active. Only bools...
		
		return testResult;
	}
	
	private static boolean PostsTableTest(Statement stmt){
		boolean testResult = true;
		
		System.out.println("------------------------Testing Posts Table------------------------");
		
		// Add a post
		System.out.println("Adding a new post");
		try{
			int userID = 0;
			//Add FK User
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'cweaver0', 'Clarence', 'Weaver', 'cweaver0@paypal.com', 'q4mvIakXW2', false, false);");
			
			ResultSet urs = stmt.executeQuery("select * from Users where UserName = 'cweaver0';");
			urs.next();
			userID = urs.getInt("UserID");
			
			//Add post
			stmt.execute("insert into Posts (UserID, ResponseID, Title, Msg, PostTime) values (" + userID + ", NULL, 'Test post', 'please ignore', CURRENT_TIME());");

			//Test post is in database
			ResultSet rs = stmt.executeQuery("select * from Posts where Title = 'Test post';");
			
			rs.next();
			if(!(rs.getString("Title").equals("Test post"))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove post
				System.out.println("Passed");
				stmt.execute("Delete from Posts Where Title = 'Test post';");
				stmt.execute("Delete from Users where UserName = 'cweaver0';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// Add a response
		System.out.println("Adding a response to a post");
		try{
			int userID = 0;
			int postID = 0;
			//Add FK User
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'cweaver0', 'Clarence', 'Weaver', 'cweaver0@paypal.com', 'q4mvIakXW2', false, false);");
			
			ResultSet urs = stmt.executeQuery("select * from Users where UserName = 'cweaver0';");
			urs.next();
			userID = urs.getInt("UserID");
			
			//Add post
			stmt.execute("insert into Posts (UserID, ResponseID, Title, Msg, PostTime) values (" + userID + ", NULL, 'Test post', 'please ignore', CURRENT_TIME());");

			//Get post id
			ResultSet prs = stmt.executeQuery("select * from Posts where Title = 'Test post';");
			prs.next();
			postID = prs.getInt("PostID");
			
			//Add response
			stmt.execute("insert into Posts (UserID, ResponseID, Title, Msg, PostTime) values (" + userID + ", " + postID + ", NULL, 'ignoring post', CURRENT_TIME());");
			
			//Test post is in database
			ResultSet rs = stmt.executeQuery("select * from Posts where Title = 'Test post';");
			
			rs.next();
			if(!(rs.getString("Title").equals("Test post"))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove post
				System.out.println("Passed");
				stmt.execute("Delete from Posts Where Msg = 'ignoring post';");
				stmt.execute("Delete from Posts Where Title = 'Test post';");
				stmt.execute("Delete from Users where UserName = 'cweaver0';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// delete a post(with responses?)
		
		// Edit message
		System.out.println("Adding a new post");
		try{
			int userID = 0;
			//Add FK User
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'cweaver0', 'Clarence', 'Weaver', 'cweaver0@paypal.com', 'q4mvIakXW2', false, false);");
			
			ResultSet urs = stmt.executeQuery("select * from Users where UserName = 'cweaver0';");
			urs.next();
			userID = urs.getInt("UserID");
			
			//Add post
			stmt.execute("insert into Posts (UserID, ResponseID, Title, Msg, PostTime) values (" + userID + ", NULL, 'Test post', 'please ignore', CURRENT_TIME());");

			stmt.execute("update Posts set Msg = 'test change' where Title = 'Test post'");
			
			//Test post is in database
			ResultSet rs = stmt.executeQuery("select * from Posts where Title = 'Test post';");
			
			rs.next();
			if(!(rs.getString("Msg").equals("test change"))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove post
				System.out.println("Passed");
				stmt.execute("Delete from Posts Where Title = 'Test post';");
				stmt.execute("Delete from Users where UserName = 'cweaver0';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		return testResult;
	}
	
	private static boolean ItemsTableTest(Statement stmt){
		boolean testResult = true;
		
		System.out.println("------------------------Testing Items Table------------------------");
		
		// Add item
		System.out.println("Adding item.");
		try{
			//Add item
			stmt.execute("insert into Items (ItemText, Completed) values ('Eggs', false);");

			//Test location is in database
			ResultSet rs = stmt.executeQuery("select * from Items where ItemText = 'Eggs';");
			
			rs.next();
			if(!(rs.getString("ItemText").equals("Eggs"))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove item
				System.out.println("Passed");
				stmt.execute("Delete from Items Where ItemText = 'Eggs';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// edit item
		System.out.println("Editing existing item.");
		try{
			//Add item
			stmt.execute("insert into Items (ItemText, Completed) values ('Eggs', false);");

			//Edit item
			stmt.execute("update Items set ItemText = 'Milk' where ItemText = 'Eggs';");
			
			//Test location is in database
			ResultSet rs = stmt.executeQuery("select * from Items where ItemText = 'Milk';");
			
			rs.next();
			if(!(rs.getString("ItemText").equals("Milk"))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove item
				System.out.println("Passed");
				stmt.execute("Delete from Items Where ItemText = 'Milk';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		return testResult;
	}
	
	private static boolean ListsTableTest(Statement stmt){
		boolean testResult = true;
		
		System.out.println("------------------------Testing Lists Table------------------------");
		
		// Add list
		System.out.println("Creating a list.");
		try{
			int tmpu = 0;
			int uID = 0;
			//Add user
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'FireOnTop', 'Patrick', 'Caruana', 'fake@email.com', 'lHaNSBpS', false, true);");
			//Get userID
			ResultSet rs = stmt.executeQuery("select * from Users Where UserName = 'FireOnTop';");
			rs.next();
			tmpu = rs.getInt("UserID");
					
			//Add list
			stmt.execute("insert into Lists (UserID, Items, PostTime) values ('" + tmpu + "', null, CURRENT_TIME());");

			//Test list is in database
			rs = stmt.executeQuery("select * from Lists where UserID = " + tmpu + ";");
			
			rs.next();
			uID = rs.getInt("UserID");
			if(tmpu != uID){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove item
				System.out.println("Passed");
				stmt.execute("delete from Lists Where UserID = " + tmpu + ";");
				stmt.execute("delete from Users where UserName = 'FireOnTop';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// remove list
		System.out.println("Deleting a list.");
		try{
			int tmpu = 0;
			int uID = 0;
			//Add user
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'FireOnTop', 'Patrick', 'Caruana', 'fake@email.com', 'lHaNSBpS', false, true);");
			//Get userID
			ResultSet rs = stmt.executeQuery("select * from Users Where UserName = 'FireOnTop';");
			rs.next();
			tmpu = rs.getInt("UserID");
					
			//Add list
			stmt.execute("insert into Lists (UserID, Items, PostTime) values ('" + tmpu + "', null, CURRENT_TIME());");

			stmt.execute("delete from Lists Where UserID = " + tmpu + ";");
			
			//Test list is in database
			rs = stmt.executeQuery("select * from Lists where UserID = " + tmpu + ";");
			
			if(rs.next()){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove item
				System.out.println("Passed");
				stmt.execute("delete from Users where UserName = 'FireOnTop';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// Add items
		System.out.println("Adding items to a list.");
		try{
			int tmpu = 0;
			int tmpi = 0;
			int uID = 0;
			//Add user
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'FireOnTop', 'Patrick', 'Caruana', 'fake@email.com', 'lHaNSBpS', false, true);");
			//Get userID
			ResultSet rs = stmt.executeQuery("select * from Users Where UserName = 'FireOnTop';");
			rs.next();
			tmpu = rs.getInt("UserID");
			
			//Add item
			stmt.execute("insert into Items (ItemText, Completed) values ('Eggs', false);");
			
			rs = stmt.executeQuery("select * from Items where ItemText = 'Eggs';");
			rs.next();
			tmpi = rs.getInt("ItemID");
					
			//Add list
			stmt.execute("insert into Lists (UserID, Items, PostTime) values ('" + tmpu + "', " + tmpi + ", CURRENT_TIME());");

			//Test list is in database
			rs = stmt.executeQuery("select * from Lists where UserID = " + tmpu + ";");
			
			rs.next();
			uID = rs.getInt("UserID");
			if(tmpu != uID){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove item
				System.out.println("Passed");
				stmt.execute("delete from Lists Where UserID = " + tmpu + ";");
				stmt.execute("delete from Users where UserName = 'FireOnTop';");
				stmt.execute("Delete from Items Where ItemText = 'Eggs';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		return testResult;
	}
	
	private static boolean GroupsTableTest(Statement stmt){
		boolean testResult = true;
		
		System.out.println("------------------------Testing Groups Table------------------------");
		
		// Adding a group
		System.out.println("Creating a group.");
		try{
			//Add item
			stmt.execute("insert into Groups (GroupName, PostID, ListsID) values ('Stark LLC', null, null);");

			//Test location is in database
			ResultSet rs = stmt.executeQuery("select * from Groups where GroupName = 'Stark LLC';");
			
			rs.next();
			if(!(rs.getString("GroupName").equals("Stark LLC"))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove item
				System.out.println("Passed");
				stmt.execute("delete from Groups Where GroupName = 'Stark LLC';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// editing a group
		System.out.println("Editing a group.");
		try{
			//Add item
			stmt.execute("insert into Groups (GroupName, PostID, ListsID) values ('Stark LLC', null, null);");
			
			//Edit group
			stmt.execute("update Groups set GroupName = 'StoneBank' where GroupName = 'Stark LLC';");

			//Test location is in database
			ResultSet rs = stmt.executeQuery("select * from Groups where GroupName = 'StoneBank';");
			
			rs.next();
			if(!(rs.getString("GroupName").equals("StoneBank"))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove item
				System.out.println("Passed");
				stmt.execute("delete from Groups Where GroupName = 'StoneBank';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		return testResult;
	}
	
	private static boolean User_GroupTableTest(Statement stmt){
		boolean testResult = true;
		
		System.out.println("------------------------Testing User_Group Table------------------------");
		
		// Add a user_group to a user
		System.out.println("Inserting new User_Group.");
		try{
			int userID = 0;
			int groupID = 0;
			int tmpu = 0;
			int tmpg = 0;
			//Add group and user
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'cweaver0', 'Clarence', 'Weaver', 'cweaver0@paypal.com', 'q4mvIakXW2', false, false);");
			stmt.execute("insert into Groups (GroupName, PostID, ListsID) values ('The Nest', null, null);");
			
			ResultSet urs = stmt.executeQuery("select * from Users where UserName = 'cweaver0';");
			urs.next();
			userID = urs.getInt("UserID");
			
			ResultSet grs = stmt.executeQuery("select * from Groups where GroupName = 'The Nest';");
			grs.next();
			groupID = grs.getInt("GroupID");
			
			//Add User_Group
			stmt.execute("insert into User_Group (UserID, GroupID) values (" + userID + ", " + groupID + ");");

			//Test bill is in database
			ResultSet rs = stmt.executeQuery("select * from User_Group where UserID = " + userID + " and GroupID = " + groupID + ";");
			
			rs.next();
			tmpu = rs.getInt("UserID");
			tmpg = rs.getInt("GroupID");
			if(tmpu != (userID) && tmpg != (groupID)){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove User_Group
				System.out.println("Passed");
				stmt.execute("Delete from User_Group where UserID = " + userID + " and GroupID = " + groupID + ";");
				stmt.execute("Delete from Users where UserName = 'cweaver0';");
				stmt.execute("Delete from Groups where GroupName = 'The Nest';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		return testResult;
	}
	
	private static boolean BillsTableTest(Statement stmt){
		boolean testResult = true;
		
		System.out.println("------------------------Testing Bills Table------------------------");		
		
		// Adding a bill with valid group and user
		System.out.println("Inserting new bill.");
		try{
			int userID = 0;
			int groupID = 0;
			//Add group and user
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'cweaver0', 'Clarence', 'Weaver', 'cweaver0@paypal.com', 'q4mvIakXW2', false, false);");
			stmt.execute("insert into Groups (GroupName, PostID, ListsID) values ('The Nest', null, null);");
			
			ResultSet urs = stmt.executeQuery("select * from Users where UserName = 'cweaver0';");
			urs.next();
			userID = urs.getInt("UserID");
			
			ResultSet grs = stmt.executeQuery("select * from Groups where GroupName = 'The Nest';");
			grs.next();
			groupID = grs.getInt("GroupID");
			
			//Add Bill
			stmt.execute("insert into Bills (UserID, GroupID, BillName, BillDescription, BillAmount) values (" + userID + ", " + groupID + ", 'Snoop Dog Inc.', 'The goods and stuff', 420.69);");

			//Test bill is in database
			ResultSet rs = stmt.executeQuery("select * from Bills where UserID = " + userID + " And GroupID = " + groupID + " And BillName = 'Snoop Dog Inc.';");
			
			rs.next();
			if(!(rs.getString("BillName").equals("Snoop Dog Inc."))){
				testResult = false;
				System.out.println("Failed.");
			}
			else{
				//Remove bill
				System.out.println("Passed");
				stmt.execute("Delete from Bills where BillName = 'Snoop Dog Inc.'");
				stmt.execute("Delete from Users where UserName = 'cweaver0';");
				stmt.execute("Delete from Groups where GroupName = 'The Nest';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		//Add bill with invalid group/user
		System.out.println("Adding bill with invalid groupID and userID");
		try{
			//Add Bill
			stmt.execute("insert into Bills (UserID, GroupID, BillName, BillDescription, BillAmount) values (23, 2, 'Snoop Dog Inc.', 'The goods and stuff', 420.69);");

			//Test location is in database
			ResultSet rs = stmt.executeQuery("select * from Bills where UserID = 23 And GroupID = 2 And BillName = 'Snoop Dog Inc.';");

			testResult = false;
		}catch (Exception e){
			System.out.println("Passed.");
		}
		
		// removing a bill
		System.out.println("Removing a bill.");
		try{
			int userID = 0;
			int groupID = 0;
			//Add group and user
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'cweaver0', 'Clarence', 'Weaver', 'cweaver0@paypal.com', 'q4mvIakXW2', false, false);");
			stmt.execute("insert into Groups (GroupName, PostID, ListsID) values ('The Nest', null, null);");
			
			ResultSet urs = stmt.executeQuery("select * from Users where UserName = 'cweaver0';");
			urs.next();
			userID = urs.getInt("UserID");
			
			ResultSet grs = stmt.executeQuery("select * from Groups where GroupName = 'The Nest';");
			grs.next();
			groupID = grs.getInt("GroupID");
			
			//Add Bill
			stmt.execute("insert into Bills (UserID, GroupID, BillName, BillDescription, BillAmount) values (" + userID + ", " + groupID + ", 'Snoop Dog Inc.', 'The goods and stuff', 420.69);");

			//Remove bill
			stmt.execute("Delete from Bills where BillName = 'Snoop Dog Inc.'");
			
			//Test bill is in database
			ResultSet rs = stmt.executeQuery("select * from Bills where UserID = " + userID + " And GroupID = " + groupID + " And BillName = 'Snoop Dog Inc.';");
			
			if(!rs.next()){
				System.out.println("Passed");
				stmt.execute("Delete from Users where UserName = 'cweaver0';");
				stmt.execute("Delete from Groups where GroupName = 'The Nest';");
			}
			else{
				//Remove bill
				System.out.println("Failed");
				stmt.execute("Delete from Users where UserName = 'cweaver0';");
				stmt.execute("Delete from Groups where GroupName = 'The Nest';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		
		// Editing a bill (amount, name, description)
		System.out.println("Editing a bill.");
		try{
			int userID = 0;
			int groupID = 0;
			//Add group and user
			stmt.execute("insert into Users (LocationsID, UserName, FirstName, LastName, Email, Pass, Active, PushNot) values (null, 'cweaver0', 'Clarence', 'Weaver', 'cweaver0@paypal.com', 'q4mvIakXW2', false, false);");
			stmt.execute("insert into Groups (GroupName, PostID, ListsID) values ('The Nest', null, null);");
			
			ResultSet urs = stmt.executeQuery("select * from Users where UserName = 'cweaver0';");
			urs.next();
			userID = urs.getInt("UserID");
			
			ResultSet grs = stmt.executeQuery("select * from Groups where GroupName = 'The Nest';");
			grs.next();
			groupID = grs.getInt("GroupID");
			
			//Add Bill
			stmt.execute("insert into Bills (UserID, GroupID, BillName, BillDescription, BillAmount) values (" + userID + ", " + groupID + ", 'Snoop Dog Inc.', 'The goods and stuff', 420.69);");
			
			//Edit bill name
			stmt.execute("update Bills set BillName = 'Party fund' where BillName = 'Snoop Dog Inc.'");
			
			//Test updated bill is in database
			ResultSet rs = stmt.executeQuery("select * from Bills where UserID = " + userID + " And GroupID = " + groupID + " And BillName = 'Party fund';");
			
			rs.next();
			if(!(rs.getString("BillName").equals("Party fund"))){
				testResult = false;
				System.out.println("Failed.");
				stmt.execute("Delete from Users where UserName = 'cweaver0';");
				stmt.execute("Delete from Groups where GroupName = 'The Nest';");
			}
			else{
				//Remove bill
				System.out.println("Passed");
				stmt.execute("Delete from Bills where BillName = 'Party fund'");
				stmt.execute("Delete from Users where UserName = 'cweaver0';");
				stmt.execute("Delete from Groups where GroupName = 'The Nest';");
			}
		}catch (Exception e){
			System.out.println("Failed.");
			System.out.println("Any error happened: " + e);
			testResult = false;
		}
		return testResult;
	}
}
