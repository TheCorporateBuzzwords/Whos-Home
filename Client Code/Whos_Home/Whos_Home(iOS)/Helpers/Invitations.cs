using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Whos_Home.Helpers
{
    class Invitations
    {
        private string groupname;
        private string groupid;
        private string invitee;

        public Invitations(string groupName, string groupID, string inviteep)
        {
            Groupname = groupName;
            Groupid = groupID;
            Invitee = inviteep;
        }

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