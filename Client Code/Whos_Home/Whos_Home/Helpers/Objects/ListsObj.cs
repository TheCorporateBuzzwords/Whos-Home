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
    class ListsObj
    {
        private string date;
        private string author;
        private string title;
        private string topicid;
        private string authFirst;
        private string authLast;

        public ListsObj(string date, string author, string title, string topicid, string authFirst, string authLast)
        {
            Date = date;
            Author = author;
            Title = title;
            Topicid = topicid;
            AuthFirst = authFirst;
            AuthLast = authLast;
        }

        public ListsObj(JToken token)
        {
            Date = (string)token["PostTime"];
            Author = (string)token["UserName"];
            Title = (string)token["Title"];
            Topicid = (string)token["ListID"];
            AuthFirst = (string)token["FirstName"];
            AuthLast = (string)token["LastName"];
        }

        public string Date
        {
            get
            {
                return date;
            }

            set
            {
                date = value;
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

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
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

        public string AuthFirst
        {
            get
            {
                return authFirst;
            }

            set
            {
                authFirst = value;
            }
        }

        public string AuthLast
        {
            get
            {
                return authLast;
            }

            set
            {
                authLast = value;
            }
        }

    }
}