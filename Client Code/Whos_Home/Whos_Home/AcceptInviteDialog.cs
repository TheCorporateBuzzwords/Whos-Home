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
    class AcceptInviteDialog : DialogFragment
    {
        private string groupname;
        private Button BAccpet;
        private Button BDecline;
        private TextView GroupName;

        public AcceptInviteDialog(Invitations invite)
        {
            //get any values from invite here
            groupname = invite.Groupname;
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

        private void BDecline_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void BAccpet_Click(object sender, EventArgs e)
        {
            //accept invitation here
        }
    }
}