using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Couchbase.Lite;

namespace Whos_Home
{
    public class DB_Singleton
    {
        private static DB_Singleton instance = null;
        private static readonly object db_lock = new object();
        private static Database db = null;
        DB_Singleton()
        {

        }

        public static DB_Singleton Instance
        {
            get
            {
                lock(db_lock)
                {
                    if(instance == null)
                    {
                        instance = new DB_Singleton();
                    }
                    return instance;
                }
            }
        }

        //Should only be called once to initialize db
        public void InitDB()
        {
            if (db != null)
                throw new Exception("Database already initialized");
            else
            {
                db = Manager.SharedInstance.GetDatabase("userinfo"); //DB name must be all lowercase

                db.Delete();

                db = Manager.SharedInstance.GetDatabase("userinfo"); //DB name must be all lowercase

            }
        }
        
        //Should only be called once to initialize db
        public void InitialInsert(string token, string username, string email, string firstname)
        {
            if (db == null)
                throw new Exception("Database does not yet exist");
            else
            {
                var vals = new Dictionary<String, Object>
                {
                    {"username", username },
                    {"email", email },
                    {"firstname", firstname },
                    {"token", token }
                };

                var doc = db.CreateDocument();

                try
                {
                    doc.PutProperties(vals);
                } catch(CouchbaseLiteException)
                {
                    throw new Exception("Database Insert Failure");
                }
            }
        }

        public string Retrieve(string key)
        {
            //probably needs to be refactored at some point
            //This is kind of weird for a nosql db, but basically
            //this function assumes that you will only get one result back i.e.
            //The user token. I can change it to return a list instead
            //if this becomes an issue.
            var view = db.GetView(key);

            view.SetMap((docret, emit) =>
            {
                emit(docret[key], docret[key]);
            }, "1");

            var query = db.GetView(key).CreateQuery();

            query.Descending = true;
            query.Limit = 1;
            var rows = query.Run();
            string result = "";
            foreach(var row in rows)
            {
                result = row.Value.ToString();
                break;
            }

            return result;
        }


    }
}