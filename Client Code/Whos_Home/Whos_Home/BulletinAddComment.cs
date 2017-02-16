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
    class BulletinAddComment : DialogFragment
    {
        private string message;
        private Button bSubmit;
        private EditText MessageText;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.BulletinCommentAdd, container, false);

            bSubmit = view.FindViewById<Button>(Resource.Id.buttonCreateComment);
            bSubmit.Click += BSubmit_Click;

            MessageText = view.FindViewById<EditText>(Resource.Id.edittextcomment);

            return view;
        }

        private async void BSubmit_Click(object sender, EventArgs e)
        {
            message = MessageText.Text;

            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Submit Comment?");


            //alert.SetMessage("Would you like to submit your comment?");

            alert.SetPositiveButton("Confirm", (senderAlert, args) => {
                //submit comment logic
            RequestHandler request = new RequestHandler(Context);
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;
            /*
            var response = await request.PostMessageReply(token, groupid, topicid, message);

            if((int)response.StatusCode == 200)
            {
                Toast.MakeText(//success or whatever);
                //Refresh bulletin

            }
            else
            {

            }
            */

            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                Dismiss();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }
    }
}