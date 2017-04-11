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
    /***************************************
     * Class: DB_Singleton
     * 
     * Description: This class handles all of
     * the CRUD operations for the client side
     * Database. Currently just a JSON Doc
     * 
     * TODO: Change db from a JSON file to
     * to an actual NoSQL DB
     * *************************************/
    public class DB_Singleton
    {
        private static DB_Singleton instance = null;
        private static readonly object db_lock = new object();
        private static string fileName = "userinfo.json";
        private static string filePath = null;
        private static string fullPath = null;
        DB_Singleton(){ }

        /**********************************
         * DB is a thread safe singleton
         * to prevent stale data from old
         * Activities
         * ********************************/
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
        }

        public bool IsOnline()
        {
            bool online = true;
            try
            {
                online = bool.Parse(Instance.Retrieve("IsOnline")); 
            }
            catch
            {
                Console.WriteLine("Error finding online mode");
            }
            return online;
        }


        //Should only be called once to initialize db
        public void InitialInsert(string token, string username, string email, string firstname)
        {
            string json = JsonConvert.SerializeObject(new UserDB(firstname, username, email, token, true));

            File.WriteAllText(fullPath, json);
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

        /****************************************
         * Function: ChangeActiveGroup
         * Description: Changes the current active
         * Group
         * **************************************/
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

        /********************************
         * Function: GetActiveGroup
         * Description: Gets the current active
         * group. Use this instead of Retrieve
         * When getting the active Group
         * ******************************/
        public UserGroup GetActiveGroup()
        {
            string json = null;
            using (var streamReader = new StreamReader(fullPath))
            {
                json = streamReader.ReadToEnd();
                Console.WriteLine(json);
            }
            JObject userDBObj = JObject.Parse(json);

            return new UserGroup((string)userDBObj["ActiveGroup"]["GroupName"], (string)userDBObj["ActiveGroup"]["GroupID"]);

        }
    }
}