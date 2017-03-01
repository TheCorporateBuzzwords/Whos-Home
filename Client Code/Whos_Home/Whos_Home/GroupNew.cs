using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Whos_Home.Helpers;
using Newtonsoft.Json.Linq;

namespace Whos_Home
{
    
    public class GroupNew : DialogFragment
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

        private async void BConfirm_Click(object sender, EventArgs e)
        {
            //CreateGroup(View.FindViewById<EditText>(Resource.Id.edittextNewGroup).Text);
            string groupname = View.FindViewById<EditText>(Resource.Id.edittextNewGroup).Text;
            RequestHandler request = new RequestHandler(Context);

            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            var response = await request.CreateGroup(groupname, token);
            int statusCode = (int)response.StatusCode;
            if ((int)response.StatusCode == 200)
            {
                db.AddGroup(groupname, (string)JObject.Parse(response.Content)["groupID"]);
                Success();
            }
            else
                Failure();
            /*
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Create the new group " + EditTextGroupName.Text + "?");

            //Send new location information to database
            alert.SetPositiveButton("Confirm", (senderAlert, args) => 
            {
//                Task create = CreateGroup().Text);
                Dismiss();
            });

            //Close dialog and cancel add
            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {});

            Dialog dialog = alert.Create();
            dialog.Show();
            */
        }
        public void Success()
        {
            Toast.MakeText(this.Context, "Group Successfully Created", ToastLength.Long).Show();
            ((Groups)Activity).UpdateGroups();
        }

        public void Failure()
        {
            Toast.MakeText(this.Context, "Group Creation Failed, Please Try Again Later", ToastLength.Long).Show();
        }
    }
}