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
using Android.Content.Res;
using Whos_Home.Helpers;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Whos_Home
{
    public class DB_Singleton
    {
        private static DB_Singleton instance = null;
        private static readonly object db_lock = new object();
        private static string fileName = "userinfo.json";
        private static string filePath = null;
        private static string fullPath = null;
        DB_Singleton(){ }

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
        public void InitDB()
        {
            string getFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            filePath = getFilePath;

            fullPath = Path.Combine(filePath, fileName);
            File.Delete(fullPath);

            ChangeActiveGroup(null);
            /*
            Manager manager = Manager.SharedInstance;

            try
            {
                db = manager.GetDatabase("userinformation");
            }
            catch (CouchbaseLiteException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            */


        }

        //Should only be called once to initialize db
        public void InitialInsert(string token, string username, string email, string firstname)
        {
            string json = JsonConvert.SerializeObject(new UserDB(firstname, username, email, token));

            File.WriteAllText(fullPath, json);

            /*using (var streamWriter = new StreamWriter(fullPath, true))
            {
                streamWriter.WriteLine(json);
            }
            */

            /*
            Dictionary<string, object> properties = new Dictionary<string, object>
            {
                {"username", username },
                {"firstname", firstname },
                {"email", email },
                {"token", token },
                {"groups", new List<Tuple<string, string>>()}
            };

            Document document = db.CreateDocument();
            document.PutProperties(properties);
            docID = document.Id;
            */
        }



        public string Retrieve(string key)
        {
            string json = null;
            using (var streamReader = new StreamReader(fullPath))
            {
                json = streamReader.ReadToEnd();
                Console.WriteLine(json);
            }

            return (string)JObject.Parse(json)[key];
            /*
            var doc = db.GetDocument(docID);
            string result = (string)doc.Properties[key];
            Console.WriteLine(result);
            return result;
            */
        }

        public List<UserGroup> GetUserGroups()
        {
            string json = null;
            using (var streamReader = new StreamReader(fullPath))
            {
                json = streamReader.ReadToEnd();
                Console.WriteLine(json);
            }

            string teststring = (string)JObject.Parse(json)["UserGroups"].ToString();

            JArray usergroups = JArray.Parse(teststring);

            List<UserGroup> resultList = new List<UserGroup>();
            foreach(JObject idk in usergroups)
            {
                string key = Regex.Split(idk["GroupName"].ToString(), ", ")[0];
                string value = Regex.Split(idk["GroupID"].ToString(), ", ")[0];
                resultList.Add(new UserGroup(key, value));
            }

            return resultList;
        }

        public UserGroup SearchGroup(string groupname)
        {
            if (!GetUserGroups().Exists(check => check.GroupName == groupname)) 
                return null;

            return GetUserGroups().Find(find => find.GroupName == groupname);
        }

        

        public void AddGroup(string groupName, string groupID)
        {
            string json = null;
            using (var streamReader = new StreamReader(fullPath))
            {
                json = streamReader.ReadToEnd();
                Console.WriteLine(json);
            }

            UserDB tempUser = JsonConvert.DeserializeObject<UserDB>(json);
            tempUser.AddGroup(groupName, groupID);
            string jsonConversiton = JsonConvert.SerializeObject(tempUser);
            File.WriteAllText(fullPath, jsonConversiton);
        }

        public void ChangeActiveGroup(UserGroup activeGroup)
        {
            string json = null;
            using (var streamReader = new StreamReader(fullPath))
            {
                json = streamReader.ReadToEnd();
                Console.WriteLine(json);
            }
            JObject userDBObj = JObject.Parse(json);
            JObject groupJson = JObject.FromObject(activeGroup);
            userDBObj["ActiveGroup"] = groupJson;

            File.WriteAllText(fullPath, userDBObj.ToString());
        }
        public bool IsActiveSet()
        {
            string json = null;
            UserGroup group = new UserGroup(null, null);
            using (var streamReader = new StreamReader(fullPath))
            {
                json = streamReader.ReadToEnd();
                Console.WriteLine(json);
            }

            JObject userDBObj = JObject.Parse(json);
            if (userDBObj["ActiveGroup"]["GroupName"] == null)
                return false;

            return true;
        }
        public UserGroup GetActiveGroup()
        {
            string json = null;
            UserGroup group = new UserGroup(null, null);
            using (var streamReader = new StreamReader(fullPath))
            {
                json = streamReader.ReadToEnd();
                Console.WriteLine(json);
            }
            JObject userDBObj = JObject.Parse(json);
            group = new UserGroup((string)userDBObj["ActiveGroup"]["GroupName"], (string)userDBObj["ActiveGroup"]["GroupID"]);

            return group;

        }
    }
}