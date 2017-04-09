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
using Whos_Home.Helpers;

namespace Whos_Home
{
    class GroupSelectDialog : DialogFragment
    {
        private Button B_Select;
        private Button B_Cancel;
        private Button B_AddUser;
        private TextView m_textviewGroupName;
        private string m_groupname;

        public GroupSelectDialog(string group_name)
        {
            this.m_groupname = group_name;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.GroupDialog, container, false);

            //find buttons in layout 
            B_Select = view.FindViewById<Button>(Resource.Id.buttonSelectGroup);
            B_Cancel = view.FindViewById<Button>(Resource.Id.buttonCancelSelectGroup);
            B_AddUser = view.FindViewById<Button>(Resource.Id.buttonAddUserToGroup);

            //find textview that contains group name and set it to value passed in constructor
            m_textviewGroupName = view.FindViewById<TextView>(Resource.Id.textviewGroupDialog);
            m_textviewGroupName.Text = m_groupname;

            //Add click functions for buttons
            B_Cancel.Click += Cancel_Click;
            B_AddUser.Click += AddUser_Click;
            B_Select.Click += Select_Click;

            return view;
        }

        private void Select_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Make " + m_groupname + " your current group?");

            //Set current group
            alert.SetPositiveButton("Confirm", (senderAlert, args) => 
            {
                //This is where current group is set
                DB_Singleton db = DB_Singleton.Instance;
                UserGroup active = db.SearchGroup(m_groupname);
                db.ChangeActiveGroup(active);
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
            GroupAddUser Dialog = new GroupAddUser(m_groupname);
            Dialog.Show(transaction, "dialog fragment add user");
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}
