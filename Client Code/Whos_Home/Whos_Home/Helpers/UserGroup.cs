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

namespace Whos_Home.Helpers
{
    public class UserGroup
    {
        private string groupName;
        private string groupID;

        UserGroup()
        {   }
        public UserGroup(string groupname, string groupid)
        {
            GroupName = groupname;
            GroupID = groupid;
        }

        public string GroupName
        {
            get
            {
                return groupName;
            }

            set
            {
                groupName = value;
            }
        }

        public string GroupID
        {
            get
            {
                return groupID;
            }

            set
            {
                groupID = value;
            }
        }
    }
}