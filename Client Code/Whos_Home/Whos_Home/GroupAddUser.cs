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

using Whos_Home.Helpers;

namespace Whos_Home
{
    class GroupAddUser : DialogFragment
    {
        private Button B_Confirm;
        private Button B_Cancel;
        private string m_invitee;
        private string m_groupname; //here ya go

        public GroupAddUser(string group_name)
        {
            m_groupname = group_name;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.AddUserToGroupDialog, container, false);

            B_Confirm = view.FindViewById<Button>(Resource.Id.buttonConfirmAddUserToGroup);
            B_Cancel = view.FindViewById<Button>(Resource.Id.buttonCancelAddUserToGroup);

            //add click functions for buttons
            B_Confirm.Click += Confirm_Click;
            B_Cancel.Click += Cancel_Click;

            return view;
        }

        private async void Confirm_Click(object sender, EventArgs e)
        {
            RequestHandler request = new RequestHandler(Context);
            var db = DB_Singleton.Instance;
            var groupid = DB_Singleton.Instance.SearchGroup(m_groupname).GroupID;

            m_invitee = View.FindViewById<EditText>(Resource.Id.edittextAddUserToGroupDialog).Text;
            var token = db.Retrieve("Token");
            
            var response = await request.SendInvitation(token, groupid, m_invitee);
            
            if((int)response.StatusCode == 200)
            {
                Toast.MakeText(Context, "Invite Sent", ToastLength.Long);
            }
            else
            {
                Toast.MakeText(Context, "Error", ToastLength.Long);
            }
            Dismiss();
        }
        

        private void Cancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void Invite(string username, string groupname)
        {
            RequestHandler request = new RequestHandler(Context);
            DB_Singleton db = DB_Singleton.Instance;
            //await request.InviteToGroup(db.SearchGroup(groupname).GroupID, username, db.Retrieve("Token"));
        }
    }
}