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

namespace Whos_Home
{
    
    public class NewGroup : DialogFragment
    {
        Button BConfirm;
        Button BCancel;
        EditText EditTextGroupName;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.NewGroup, container, false);

            BConfirm = view.FindViewById<Button>(Resource.Id.buttonCreateGroup);
            BCancel = view.FindViewById<Button>(Resource.Id.buttonCancelCreateGroup);
            EditTextGroupName = view.FindViewById<EditText>(Resource.Id.edittextNewGroup);

            //Set click functions for confirm and cancel buttons
            BConfirm.Click += BConfirm_Click;
            BCancel.Click += BCancel_Click;

            return view;
        }

        private void BCancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void BConfirm_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Create the new group " + EditTextGroupName.Text + "?");

            //Send new location information to database
            alert.SetPositiveButton("Confirm", (senderAlert, args) => 
            {
                Dismiss();
            });

            //Close dialog and cancel add
            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {});

            Dialog dialog = alert.Create();
            dialog.Show();
        }
    }
}