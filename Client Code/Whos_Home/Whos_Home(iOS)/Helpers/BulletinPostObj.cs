using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Whos_Home.Helpers
{
    public class BulletinPostObj
    {
        string author;
        string time;
        string topicid;
        string title;
        string message;

        public BulletinPostObj(string p_author, string p_time, string p_topicid, string p_title, string p_message)
        {
            Author = p_author;
            Time = p_time;
            Topicid = p_topicid;
            Title = p_title;
            Message = p_message;

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