using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.Res;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Whos_Home.Helpers;
using Whos_Home.Helpers.ListObjects;

namespace Whos_Home
{
    [Activity(Label = "MessageBoard")]
    class BulletinBoard : BaseActivity
    {
        private List<BulletinPostObj> m_posts = new List<BulletinPostObj>();
        private Button B_post;
        private ListView m_listView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the MessageBoard layout resource
            SetContentView(Resource.Layout.MessageBoard);

            InitializeToolbars();
            InitializeFormat();

            tab2Button.SetColorFilter(selectedColor);
            ActionBar.Title = "Bulletins";
        }
        //titles and messsages will be stored and can be accessed when loading
        //a bulletin in a separate window.
 
        private async void InitializeFormat()
        {
            B_post = FindViewById<Button>(Resource.Id.NewPostButton);
            B_post.Click += NewPostButton_Click;

            m_listView = FindViewById<ListView>(Resource.Id.messagelistview);
            m_listView.ItemClick += OnMessageItemClick;
            m_listView.ItemLongClick += OnMessageLongClick;

            await UpdatePosts();
        }

        public async Task UpdatePosts()
        {
            try
            {
                m_posts = await new BulletinList().UpdateList();
            }
            catch(Exception e)
            {
                m_posts = new List<BulletinPostObj>();
                Console.WriteLine(e.ToString());
            }

            //reverse titles and messages so they are shown correctly in bulletinboard

            m_listView.Adapter = new BulletinListAdapter(this, m_posts);
        }

        //Click method for NewPostButton
        //Handles creating a new post for the message board
        private void NewPostButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            BulletinNew NewMessageDialog = new BulletinNew();
            NewMessageDialog.Show(transaction, "dialog fragment new message");

            m_listView.Adapter = new BulletinListAdapter(this, m_posts);
        }

        void OnMessageLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            BuildAlert(e.Position);
        }

        private async void BuildAlert(int position)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Delete Bulletin?");

            //alert.SetMessage("Would you like to submit your comment?");
            alert.SetPositiveButton("Delete", async (senderAlert, args) => {
                await DeleteItem(position);
                await UpdatePosts();
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        async Task DeleteItem(int position)
        {
            RequestHandler request = new RequestHandler(this);
            
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            var response = await request.DeletePost(token, db.GetActiveGroup().GroupID, m_posts[position].Topicid);
        }

        //function is called when a message title is selected from the message board
        //the function opens the message in a new activity (Bulletin.cs)
        void OnMessageItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            //creates an intent for a Bulletin activity
            Intent i = new Intent(Application.Context, typeof(Bulletin));

            i.PutExtra("PostObject", JsonConvert.SerializeObject(m_posts[position]));

            StartActivity(i);
        }

        private void Notification(Intent activity, string text, string group, int intent_id, int notification_id)
        {
            //creates a Pending intent with the activity sent to the notification
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(this, intent_id, activity, PendingIntentFlags.OneShot);

            //creates a notification based on the intent and message of the notification
            Notification.Builder builder = new Notification.Builder(this);
            builder.SetContentIntent(pendingIntent);
            builder.SetContentTitle("Who's Home?");
            builder.SetContentText(text + group);
            builder.SetSmallIcon(Resource.Drawable.notification);

            // Build the notification
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            notificationManager.Notify(notification_id, notification);
        }
    }
}