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
    class InviteAcceptDialog : DialogFragment
    {
        private string groupname;
        private string groupid;
        private Button BAccpet;
        private Button BDecline;
        private TextView GroupName;

        public InviteAcceptDialog(Invitations invite)
        {
            //get any values from invite here
            groupname = invite.Groupname;
            groupid = invite.Groupid;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.AcceptInvite, container, false);

            //Find button instances in view
            BAccpet = view.FindViewById<Button>(Resource.Id.buttonAcceptInvite);
            BDecline = view.FindViewById<Button>(Resource.Id.buttonDeclineInvite);
            GroupName = view.FindViewById<TextView>(Resource.Id.textviewAcceptInvite);

            GroupName.Text = groupname;
            BAccpet.Click += BAccpet_Click;
            BDecline.Click += BDecline_Click;

            return view;

        }

        private async void BDecline_Click(object sender, EventArgs e)
        {
            //request.RespondInvitation(db.Retrieve("Token"), db.SearchGroup(groupname), false);
            RequestHandler request = new RequestHandler(Context);
            DB_Singleton db = DB_Singleton.Instance; 

            var response = await request.RespondInvitation(db.Retrieve("Token"), groupid, true);

            if((int)response.StatusCode == 200)
            {
                Toast.MakeText(Context, "Rejected Invite", ToastLength.Long);
            }
            else
            {
                Toast.MakeText(Context, "Invite response failed", ToastLength.Long);
            }
            Dismiss();
        }

        private async void BAccpet_Click(object sender, EventArgs e)
        {
            RequestHandler request = new RequestHandler(Context);
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            var response = await request.RespondInvitation(token, groupid, false);

            if((int)response.StatusCode == 200)
            {
                Toast.MakeText(Context, "Accepted Invite", ToastLength.Long);
                AddGroup(groupid, groupname);
            }
            else
            {
                Toast.MakeText(Context, "Invite response failed", ToastLength.Long);
            }
            Dismiss();
        }

        private void AddGroup(string groupid, string groupname)
        {
            DB_Singleton.Instance.AddGroup(groupname, groupid);
        }
    }
}