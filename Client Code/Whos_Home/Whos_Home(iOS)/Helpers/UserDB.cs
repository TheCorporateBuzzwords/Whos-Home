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

using Newtonsoft.Json.Linq;
using Whos_Home.Helpers;

namespace Whos_Home.Helpers
{
    public class UserDB
    {
        public UserDB()
        { }
        
        
        public UserDB(string firstname, string username, string email, string token, bool isonline)
        {
            FirstName = firstname;
            UserName = username;
            Email = email;
            Token = token;
            IsOnline = isonline;
        }

        public void AddGroup(string groupName, string groupID)
        {
            UserGroups.Add(new UserGroup(groupName, groupID));
        }

        //[PrimaryKey, AutoIncrement]
        public bool IsOnline { get; set; }
        public int ID { get; set; }

        public string FirstName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public UserGroup ActiveGroup
        {
            get
            {
                return activeGroup;
            }

            set
            {
                activeGroup = value;
            }
        }

        public List<UserGroup> UserGroups = new List<UserGroup>();

        public List<UserLocation> userLocations = new List<UserLocation>();

        private UserGroup activeGroup;

        public override string ToString()
        {
            return string.Format("[Person: ID={0}, FirstName={1}, UserName={2}, Email={3}, Token={4}", ID, FirstName, UserName, Email, Token);
        }

    }
}