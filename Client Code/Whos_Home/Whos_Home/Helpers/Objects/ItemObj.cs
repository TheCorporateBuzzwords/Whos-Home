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

using Newtonsoft.Json.Linq;

namespace Whos_Home.Helpers
{
    class ItemObj
    {
        string author;
        string time;
        string message;
        string isDone;
        string id;

        public ItemObj(string author, string time, string message, string isDone)
        {
            Author = author;
            Time = time;
            Message = message;
            IsDone = isDone;
            Id = null;
        }

        public ItemObj(string author, string time, string message, string isDone, string id)
        {
            Author = author;
            Time = time;
            Message = message;
            IsDone = isDone;
            Id = id;
        }

        public ItemObj(JToken token)
        {
            Time = (string)token["PostTime"];
            Author = (string)token["UserName"];
            Message = (string)token["ItemText"];
            IsDone = (string)token["Completed"];
            Id = (string)token["ItemID"];

            if (IsDone == null || IsDone == "null")
                IsDone = false.ToString();
        }

        public bool IsItemChecked()
        {
            bool ret = false;
            if (isDone == true.ToString())
                ret = true;

            //this is bad I know
            return ret;
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

        public string IsDone
        {
            get
            {
                return isDone;
            }

            set
            {
                isDone = value;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }
    }
}