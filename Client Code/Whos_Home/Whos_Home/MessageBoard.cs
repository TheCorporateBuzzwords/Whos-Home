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
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the MessageBoard layout resource
            SetContentView(Resource.Layout.MessageBoard);
            MockData();

        }

        //temporary function that reads data from file
        //using to load text for testing
        private void MockData()
        {

            int num = 0;
            string title, message;
            for(int i = 0; i < 5; ++i)
            {
                title = "Title" + num.ToString();
                message = "Message" + num.ToString();
            }
        }
    }
}