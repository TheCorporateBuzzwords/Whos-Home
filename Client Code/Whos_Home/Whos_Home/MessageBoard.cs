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
using Android.Content.Res;
using System.IO;
using Newtonsoft.Json;

namespace Whos_Home
{
    [Activity(Label = "MessageBoard")]
    class MessageBoard : Activity
    {
        Button NewPostButton;
        ListView listView;
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

        //Assigns values to NewPostButton and ListView
        private void InitializeFormat()
        {
            //mock title data used for testing list
            
            for (int i = 0; i < 50; ++i)
            {
                titles.Add("Title" + i.ToString());
                messages.Add("Message" + i.ToString());


            }
            NewPostButton = FindViewById<Button>(Resource.Id.NewPostButton);
            NewPostButton.Click += NewPostButton_Click;


            listView = FindViewById<ListView>(Resource.Id.messagelistview);
            listView.Adapter = new MessageBoardListAdapter(this, titles, messages);
            
            listView.ItemClick += OnMessageItemClick;

        }

        //Click method for NewPostButton
        //Handles creating a new post for the message board
        private void NewPostButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            NewMessage_Dialog NewMessageDialog = new NewMessage_Dialog();
            NewMessageDialog.Show(transaction, "dialog fragment new message");

            listView.Adapter = new MessageBoardListAdapter(this, titles, messages);

           
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
            i.PutExtra("Title", JsonConvert.SerializeObject(titles[position]));
            i.PutExtra("Message", JsonConvert.SerializeObject(messages[position]));

            StartActivity(i);

        }

        private void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Options";

            //initialize bottom toolbar
            var editToolbar = FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            //editToolbar.Title = "Navigate";
            editToolbar.InflateMenu(Resource.Menu.edit_menus);
            editToolbar.MenuItemClick += (sender, e) => {
                Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
            };
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
            return base.OnOptionsItemSelected(item);
        }
    }
}