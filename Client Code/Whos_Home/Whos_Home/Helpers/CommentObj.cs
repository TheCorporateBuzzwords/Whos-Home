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

namespace Whos_Home.Helpers
{
    public class CommentObj
    {
        string message;
        string time;
        string author;
        string topicid;

        public CommentObj()
        {
            Message = null;
            Time = null;
            Author = null;
            Topicid = null;
        }
        public CommentObj(string auth, string msg, string tme, string topid)
        {
            Message = msg;
            Time = tme;
            Author = auth;
            Topicid = topid;
        }
        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }

        public string Time
        {
            get
            {
                return time;
            }

            set
            {
                time = value;
            }
        }

        public string Author
        {
            get
            {
                return author;
            }

            set
            {
                author = value;
            }
        }

        public string Topicid
        {
            get
            {
                return topicid;
            }

            set
            {
                topicid = value;
            }
        }
    }
}