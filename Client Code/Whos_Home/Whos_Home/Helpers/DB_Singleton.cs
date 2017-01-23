using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Couchbase.Lite;
using SQLite;
namespace Whos_Home.Helpers
{
    public class DB_Singleton
    {
        private static DB_Singleton instance = null;
        private static readonly object db_lock = new object();
        private static SQLiteConnection db = null;
        DB_Singleton()
        {

        }

        public static DB_Singleton Instance
        {
            get
            {
                lock (db_lock)
                {
                    if (instance == null)
                    {
                        instance = new DB_Singleton();
                    }
                    return instance;
                }
            }
        }

        //Should only be called once to initialize db
        //cris is dumb
        public void InitDB()
        {
            if (db != null)
                throw new Exception("Database already initialized");
            else
            {
                try
                {
                    db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "userdb.db"));

                    db.CreateTable<UserDB>();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        //Should only be called once to initialize db
        public void InitialInsert(string token, string username, string email, string firstname)
        {
            UserDB user = new UserDB(firstname, username, email, token);
            if (db == null)
                throw new Exception("Database does not yet exist");
            else
            {
                try
                {
                    db.Execute("DELETE FROM UserDB WHERE 1=1");
                    db.Insert(new UserDB(firstname, username, email, token));
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

        

        public string Retrieve(string key)
        {
            //This is messy, bad, and brute forced and NEEDS to be refactored at some point
            try
            {
                UserDB user = new UserDB();
                List<UserDB> result = db.Query<UserDB>("SELECT * FROM UserDB WHERE 1=1");
                if (key == "Email")
                    return result.First().Email;
                if (key == "UserName")
                    return result.First().UserName;
                if (key == "Token")
                    return result.First().Token;
                if (key == "FirstName")
                    return result.First().FirstName;

                return null;

            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
                return "null";
            }
        }

    }
}