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
    class BillObj
    {
        string billid;
        string sendername;
        string categoryid;
        string amount;
        string description;
        string title;
        string date;
        string recipientname;

        public string Billid
        {
            get
            {
                return billid;
            }

            set
            {
                billid = value;
            }
        }

        public string Sendername
        {
            get
            {
                return sendername;
            }

            set
            {
                sendername = value;
            }
        }

        public string Categoryid
        {
            get
            {
                return categoryid;
            }

            set
            {
                categoryid = value;
            }
        }

        public string Amount
        {
            get
            {
                return amount;
            }

            set
            {
                amount = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
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

        public string Recipientname
        {
            get
            {
                return recipientname;
            }

            set
            {
                recipientname = value;
            }
        }

        public BillObj(JToken token)
        {
            Billid = (string)token["BillID"];
            Sendername = (string)token["SenderName"];
            Title = (string)token["Title"];
            Description = (string)token["Description"];
            Amount = (string)token["Amount"];
            Date = (string)token["DateDue"];
            Recipientname = (string)token["RecipientName"];
        }
    }
}