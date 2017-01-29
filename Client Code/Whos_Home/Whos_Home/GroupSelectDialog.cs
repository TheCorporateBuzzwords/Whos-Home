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
using Newtonsoft.Json;

namespace Whos_Home
{
    class GroupSelectDialog : DialogFragment
    {
        private Button Select;
        private Button Cancel;
        private Button AddUser;
        private TextView textviewGroupName;
        private string groupname;
        public GroupSelectDialog(string group_name)
        {
            this.groupname = group_name;
            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.GroupDialog, container, false);

            //find buttons in layout 
            Select = view.FindViewById<Button>(Resource.Id.buttonSelectGroup);
            Cancel = view.FindViewById<Button>(Resource.Id.buttonCancelSelectGroup);
            AddUser = view.FindViewById<Button>(Resource.Id.buttonAddUserToGroup);

            //find textview that contains group name and set it to value passed in constructor
            textviewGroupName = view.FindViewById<TextView>(Resource.Id.textviewGroupDialog);
            textviewGroupName.Text = groupname;

            //Add click functions for buttons
            Cancel.Click += Cancel_Click;
            AddUser.Click += AddUser_Click;
            Select.Click += Select_Click;

            return view;

        }

        private void Select_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Make " + groupname + " your current group?");

            //Set default group
            alert.SetPositiveButton("Confirm", (senderAlert, args) => 
            {
                //This is where current group is set
            });

            //Close dialog
            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                Dismiss();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
            Dismiss();
        }

        private void AddUser_Click(object sender, EventArgs e)
        {
            Dismiss();
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            AddUserToGroupDialog Dialog = new AddUserToGroupDialog();
            Dialog.Show(transaction, "dialog fragment add user");
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}
