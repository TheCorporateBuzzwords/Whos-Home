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
    class BulletinNew : DialogFragment
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

                Intent intent = new Intent(this.Activity.ApplicationContext, typeof(BulletinBoard));
                Notification(intent, "A new message has been posted in ", "groupname", 0, 0);

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

        private void Notification(Intent activity, string text, string group, int intent_id, int notification_id)
        {
            //creates a Pending intent with the activity sent to the notification
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(this.Activity.ApplicationContext, intent_id, activity, PendingIntentFlags.OneShot);

            //creates a notification based on the intent and message of the notification
            Notification.Builder builder = new Notification.Builder(this.Activity.ApplicationContext);
            builder.SetContentIntent(pendingIntent);
            builder.SetContentTitle("Who's Home?");
            builder.SetContentText(text + group);
            builder.SetSmallIcon(Resource.Drawable.ic_action_content_save);

            // Build the notification
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                this.Activity.ApplicationContext.GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            notificationManager.Notify(notification_id, notification);
        }
    }
}