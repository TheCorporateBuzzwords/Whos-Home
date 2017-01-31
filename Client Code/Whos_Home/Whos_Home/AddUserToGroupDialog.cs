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
    class AddUserToGroupDialog : DialogFragment
    {
        private Button Confirm;
        private Button Cancel;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.AddUserToGroupDialog, container, false);

            Confirm = view.FindViewById<Button>(Resource.Id.buttonConfirmAddUserToGroup);
            Cancel = view.FindViewById<Button>(Resource.Id.buttonCancelAddUserToGroup);

            //add click functions for buttons
            Confirm.Click += Confirm_Click;
            Cancel.Click += Cancel_Click;

            return view;
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            /*RequestHandler request = new RequestHandler();
            request.InviteToGroup(groupid, DB_Singleton.Instance.Retrieve("Token"), username);
            */

        }
        

        private void Cancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private async void Invite(string username, string groupname)
        {
            RequestHandler request = new RequestHandler(Context);
            DB_Singleton db = DB_Singleton.Instance;
            await request.InviteToGroup(db.SearchGroup(groupname).GroupID, username, db.Retrieve("Token"));
        }
    }
}