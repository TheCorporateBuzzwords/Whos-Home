using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class Bulletin : BaseActivity
    {
        private ListView m_listview;
        private TextView m_message, m_title, m_author, m_date;
        private Button bAddComment;
        private List<CommentObj> comment_objs = new List<CommentObj>();

        private BulletinPostObj post;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Bulletin);
            InitializeToolbars();
            InitializeFormat();
        }


        private async void InitializeFormat()
        {
            post = JsonConvert.DeserializeObject<BulletinPostObj>(Intent.GetStringExtra("PostObject"));
            m_listview = FindViewById<ListView>(Resource.Id.BulletinCommentsListView);

            //find view for title
            m_title = FindViewById<TextView>(Resource.Id.textviewBulletinTitle);
            m_title.Text = post.Title;

            //find views for author and date
            m_author = FindViewById<TextView>(Resource.Id.textviewBulletinAuthor);
            m_author.Text = "Posted by: " + post.Author;
            m_date = FindViewById<TextView>(Resource.Id.textviewBulletinDate);
            m_date.Text = "Posted: " + post.Time;

            //find the two views for message body and comment listview
            m_message = FindViewById<TextView>(Resource.Id.textviewBulletinMessage);

            //find the add comment button
            bAddComment = FindViewById<Button>(Resource.Id.NewCommentButton);
            bAddComment.Click += BAddComment_Click;

            m_message.Text = post.Message;

            await UpdateComments();

            m_message.Click += TextViewClick;
            m_listview.ItemLongClick += Commentlistview_LongClick;
        }

        public async Task UpdateComments()
        {
            List<string> comments = new List<string>();
            List<string> usernames = new List<string>();

            //deserializes the title and message that were converted to json in the messageboard.cs
            comment_objs = new List<CommentObj>();

            DB_Singleton db = DB_Singleton.Instance;
            RequestHandler request = new RequestHandler(this);
            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;

            var response = await request.GetMessageReplies(token, groupid, post.Topicid);

            if ((int)response.StatusCode == 200)
            {
                if (response.Content != "[]")
                {
                    JArray JPosts = JArray.Parse(response.Content);

                    foreach (JToken Jpost in JPosts)
                        comment_objs.Add(new CommentObj(Jpost));
                }
            }
            else
                Toast.MakeText(this, "Error getting comments", ToastLength.Long);

            var minus_first_comment = comment_objs;

            minus_first_comment.RemoveAt(0);
            m_listview.Adapter = new BulletinCommentListAdapter(this, minus_first_comment);
        }

        private void Commentlistview_LongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            BuildAlert(e.Position);
        }

        private async void BuildAlert(int position)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Delete Bulletin?");

            //alert.SetMessage("Would you like to submit your comment?");
            alert.SetPositiveButton("Delete", async (senderAlert, args) => {
                await DeleteItem(position);
                await UpdateComments();
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private async Task DeleteItem(int position)
        {
            RequestHandler request = new RequestHandler(this);

            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            var response = await request.DeleteMessageReply(token, db.GetActiveGroup().GroupID, comment_objs[position].Topicid);
        }

        private void BAddComment_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            BulletinAddComment NewCommentDialog = new BulletinAddComment(post);
            NewCommentDialog.Show(transaction, "dialog fragment new message");
            //commentlistview.RefreshDrawableState();
        }

        private void TextViewClick(object sender, System.EventArgs e)
        {
            //create the dialog fragment
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            //pass in the title and message of the bulletin to be displayed
            BulletinFragment FullMessage= new BulletinFragment(post.Title, post.Message);
            FullMessage.Show(transaction, "dialog fragment show full message");
        }
    }
}