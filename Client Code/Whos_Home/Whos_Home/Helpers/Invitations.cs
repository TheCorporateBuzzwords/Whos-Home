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
    class Invitations
    {
        private string groupname;
        private string groupid;
        private string invitee;

        public string Groupname
        {
            get
            {
                return groupname;
            }

            set
            {
                groupname = value;
            }
        }

        public string Groupid
        {
            get
            {
                return groupid;
            }

            set
            {
                groupid = value;
            }
        }

        public string Invitee
        {
            get
            {
                return invitee;
            }

            set
            {
                invitee = value;
            }
        }
    }
}