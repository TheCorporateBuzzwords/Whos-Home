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
            Cancel.Click += Cancel_Click;

            return view;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}