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
    class GroupSelectDialog : DialogFragment
    {
        private Button Select;
        private Button Cancel;
        private Button AddUser;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.GroupDialog, container, false);

            Select = view.FindViewById<Button>(Resource.Id.buttonSelectGroup);
            Cancel = view.FindViewById<Button>(Resource.Id.buttonCancelSelectGroup);
            AddUser = view.FindViewById<Button>(Resource.Id.buttonAddUserToGroup);

            //Add click functions for buttons
            Cancel.Click += Cancel_Click;
            AddUser.Click += AddUser_Click;

            return view;

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