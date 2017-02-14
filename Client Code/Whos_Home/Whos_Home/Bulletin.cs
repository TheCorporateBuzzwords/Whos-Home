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
using Newtonsoft.Json.Linq;
using Whos_Home.Helpers;

namespace Whos_Home
{
    [Activity(Label = "Bulletin")]
    public class Bulletin : Activity
    {
        private ListView commentlistview;
        private TextView message, tvTitle;
        private string msg, title;
        private Button bAddComment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Bulletin);
            InitializeToolbars();
            InitializeFormat();
        }

        private void InitializeFormat()
        {
            List<string> comments = new List<string>();
            List<string> usernames = new List<string>();
            List<CommentObj> comment_objs = new List<CommentObj>();

            //deserializes the title and message that were converted to json in the messageboard.cs
            title = JsonConvert.DeserializeObject<string>(Intent.GetStringExtra("Title"));
            msg = JsonConvert.DeserializeObject<string>(Intent.GetStringExtra("Message"));

            /*
            DB_Singleton db = DB_Singleton.Instance;
            RequestHandler request = new RequestHandler(this);
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;
            var response = request.GetMessageReply(token, groupid, topicid);

            if (response.Content != "[]")
            {
                JArray JPosts = JArray.Parse(response.Content);

                foreach (JToken post in JPosts)
                {
                    string author = (string)post["PosterName"];
                    string time = (string)post["DatePosted"];
                    string message = (string)post["msg"];

                    comment_objs.Add(new CommentObj(author, time, title, topicid, message));
                    comments.Add(message);
                    comments.Add(message);
                }
            }
            */
            
            //generate fake comments
            /*for (int i = 1; i < 16; ++i)
            {
                comments.Add("Comment " + i.ToString());
                usernames.Add("User " + i.ToString());
            }*/


            tvTitle = FindViewById<TextView>(Resource.Id.textviewBulletinTitle);
            tvTitle.Text = title;

            //find the two views for message body and comment listview
            message = FindViewById<TextView>(Resource.Id.textviewBulletinMessage);
            commentlistview = FindViewById<ListView>(Resource.Id.BulletinCommentsListView);

            //find the add comment button
            bAddComment = FindViewById<Button>(Resource.Id.NewCommentButton);
            bAddComment.Click += BAddComment_Click;

            //set values for testing
            message.Text = msg;
            commentlistview.Adapter = new BulletinCommentListAdapter(this, usernames, comments);


            //set onClick method for message that will open the full message text in another window
            message.Click += TextViewClick;

        }

        private void BAddComment_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            BulletinAddComment NewCommentDialog = new BulletinAddComment();
            NewCommentDialog.Show(transaction, "dialog fragment new message");
        }

        private void TextViewClick(object sender, System.EventArgs e)
        {
            //create the dialog fragment
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            //pass in the title and message of the bulletin to be displayed
            BulletinFragment FullMessage= new BulletinFragment(title, msg);
            FullMessage.Show(transaction, "dialog fragment show full message");
        }


        private void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = title;


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
            if (e.Item.ToString() == "Bulletins")
                this.StartActivity(typeof(BulletinBoard));

            //Start the Locations activity
            if (e.Item.ToString() == "Locations")
                this.StartActivity(typeof(Locations));

            //Start the Lists activity
            if (e.Item.ToString() == "Lists")
                this.StartActivity(typeof(Lists));
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