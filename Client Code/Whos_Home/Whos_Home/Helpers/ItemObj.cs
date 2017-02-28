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
    class ItemObj
    {
        string author;
        string time;
        string message;
        string isDone;
        string id;

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

        public ItemObj(string author, string time, string message, string isDone)
        {
            this.Author = author;
            this.Time = time;
            this.Message = message;
            this.IsDone = isDone;
        }

        public ItemObj(string author, string time, string message, string isDone, string id)
        {
            this.author = author;
            this.time = time;
            this.message = message;
            this.isDone = isDone;
            this.id = id;
        }
    }
}