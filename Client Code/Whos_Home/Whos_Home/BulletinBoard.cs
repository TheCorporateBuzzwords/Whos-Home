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

namespace Whos_Home
{
    [Activity(Label = "MessageBoard")]
    class BulletinBoard : Activity
    {
        private List<BulletinPostObj> posts = new List<BulletinPostObj>();
        private Button NewPostButton;
        private ListView listView;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the MessageBoard layout resource
            SetContentView(Resource.Layout.MessageBoard);

            InitializeToolbars();
            InitializeFormat();

            
        }
        //titles and messsages will be stored and can be accessed when loading
        //a bulletin in a separate window.
        List<string>titles = new List<string>();
        List<string>messages = new List<string>();

 
        private async void InitializeFormat()
        {
            await UpdatePosts();

            NewPostButton = FindViewById<Button>(Resource.Id.NewPostButton);
            NewPostButton.Click += NewPostButton_Click;
        }

        public async Task UpdatePosts()
        {
            posts = new List<BulletinPostObj>();

            RequestHandler request = new RequestHandler(this);
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;
            var response = await request.GetMessages(token, groupid);
            //Checks to make sure response is OK
            if ((int)response.StatusCode == 200)
            {
                //Check to make sure List is not empty
                if (response.Content != "[]")
                {
                    //If response.Content is empty .Parse will break
                    JArray JPosts = JArray.Parse(response.Content);

                    foreach (JToken post in JPosts)
                    {
                        string author = (string)post["PosterName"];
                        string time = (string)post["DatePosted"];
                        string title = (string)post["Title"];
                        string topicid = (string)post["TopicID"];
                        string message = (string)post["Message"];

                        posts.Add(new BulletinPostObj(author, time, topicid, title, message));
                        titles.Add(title);
                        messages.Add(message);
                    }
                }
            }
            else
                Toast.MakeText(this, "Error Getting Comments", ToastLength.Long);

            listView = FindViewById<ListView>(Resource.Id.messagelistview);

            //reverse titles and messages so they are shown correctly in bulletinboard
            posts.Reverse();

            listView.Adapter = new BulletinListAdapter(this, posts);
       
            listView.ItemClick += OnMessageItemClick;
            listView.ItemLongClick += OnMessageLongClick;
        }

        //Click method for NewPostButton
        //Handles creating a new post for the message board
        private void NewPostButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            BulletinNew NewMessageDialog = new BulletinNew();
            NewMessageDialog.Show(transaction, "dialog fragment new message");

            listView.Adapter = new BulletinListAdapter(this, posts);

           
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
            var response = await request.DeletePost(token, db.GetActiveGroup().GroupID, posts[position].Topicid);
        }
        //function is called when a message title is selected from the message board
        //the function opens the message in a new activity (Bulletin.cs)
        void OnMessageItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            //creates an intent for a Bulletin activity
            Intent i = new Intent(Application.Context, typeof(Bulletin));

            //Serializes the title and message of the selected list item into json
            //This will be later deserialized in the bulletin.cs activity
            //i.PutExtra("Title", JsonConvert.SerializeObject(titles[position]));
            //i.PutExtra("Message", JsonConvert.SerializeObject(messages[position]));

            i.PutExtra("PostObject", JsonConvert.SerializeObject(posts[position]));

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
            builder.SetSmallIcon(Resource.Drawable.ic_action_content_save);

            // Build the notification
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            notificationManager.Notify(notification_id, notification);
        }

        private void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Bulletins";


            //initialize bottom toolbar
            var editToolbar = FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            //editToolbar.Title = "Navigate";
            editToolbar.InflateMenu(Resource.Menu.edit_menus);
            editToolbar.MenuItemClick += NavigateMenu;
                
                //(sender, e) => {
                //Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
            //};

        
        }

        //Method is used to navigate between activities using the bottom toolbar
        private void NavigateMenu(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            //Start the bulletin activity
            if(e.Item.ToString() == "Bulletins")
                this.StartActivity(typeof(BulletinBoard));

            //Start the Locations activity
            if (e.Item.ToString() == "Locations")
                this.StartActivity(typeof(Locations));

            //Start the Lists activity
            if (e.Item.ToString() == "Lists")
                this.StartActivity(typeof(Lists));

            //Start the Lists activity
            if (e.Item.ToString() == "Bills")
                this.StartActivity(typeof(Bills));
        }

        //called to specify menu resources for an activity
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        //called when a menu item is tapped
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();

            //loads notifications
            if (item.ToString() == "Notifications")
                this.StartActivity(typeof(Notifications));

            //Loads settings menu if preferences is selected
            if (item.ToString() == "Preferences")
                this.StartActivity(typeof(SettingsMenu));

            //Loads Groups menu if selected
            if (item.ToString() == "Groups")
                this.StartActivity(typeof(Groups));

            return base.OnOptionsItemSelected(item);
        }
    }
}