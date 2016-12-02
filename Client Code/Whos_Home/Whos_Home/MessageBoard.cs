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

            InitializeFormat();


        }
        List<string> titles = new List<string>();
        //Assigns values to NewPostButton and ListView
        private void InitializeFormat()
        {
            //mock title data used for testing list
            
            for (int i = 0; i < 50; ++i)
            {
                titles.Add("Title" + i.ToString());

            }
            NewPostButton = FindViewById<Button>(Resource.Id.NewPostButton);
            NewPostButton.Click += NewPostButton_Click;


            listView = FindViewById<ListView>(Resource.Id.messagelistview);
            listView.Adapter = new MessageBoardListAdapter(this, titles);
            
        }

        //Click method for NewPostButton
        //Handles creating a new post for the message board
        private void NewPostButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            NewMessage_Dialog NewMessageDialog = new NewMessage_Dialog();
            NewMessageDialog.Show(transaction, "dialog fragment new message");

            listView.Adapter = new MessageBoardListAdapter(this, titles);

           
        }
    }
}