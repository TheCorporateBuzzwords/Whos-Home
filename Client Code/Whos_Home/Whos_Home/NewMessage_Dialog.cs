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
    class NewMessage_Dialog : DialogFragment
    {
        private Button Submit;
        private string title;
        private string message;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.NewMessage, container, false);

            //Set button instance, set click function
            Submit = view.FindViewById<Button>(Resource.Id.buttonCreateMessage);
            Submit.Click += new EventHandler(delegate (object sender, System.EventArgs e)
            {
                string MsgTitle = view.FindViewById<EditText>(Resource.Id.edittexttitle).Text;
                string MsgBody = view.FindViewById<EditText>(Resource.Id.edittextmessage).Text;

                //create dialog to show data that was taken from text fields
                //Currently serves a purpose in testing, may be left in as confirmation to the user
                AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
                alert.SetTitle("New Message Submitted:");
                alert.SetMessage(string.Format("Subject: " + MsgTitle + "\nMessage:\n" + MsgBody));
                
                Dialog dialog = alert.Create();
                dialog.Show();

                //send values to server??

                //set private values equal to equal values from dialog box
                title = MsgTitle;
                message = MsgBody;
                //closes message dialog box
                Dismiss();

            });
            return view;

        }
        public string GetTitle()
        {
            return title;
        }

        public string GetMessage()
        {
            return message;
        }
    }
}