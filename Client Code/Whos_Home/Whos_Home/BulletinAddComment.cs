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
using Whos_Home.Helpers;

namespace Whos_Home
{
    class BulletinAddComment : DialogFragment
    {
        private string m_message;
        private Button B_submit;
        private EditText m_MessageText;
        //post object
        private BulletinPostObj m_post;

        public BulletinAddComment(BulletinPostObj bulletinPost)
        {
            //post object
            m_post = bulletinPost;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.BulletinCommentAdd, container, false);

            B_submit = view.FindViewById<Button>(Resource.Id.buttonCreateComment);
            B_submit.Click += BSubmit_Click;

            m_MessageText = view.FindViewById<EditText>(Resource.Id.edittextcomment);

            return view;
        }


        private  void BSubmit_Click(object sender, EventArgs e)
        {
            m_message = m_MessageText.Text;

            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Submit Comment?");

            alert.SetPositiveButton("Confirm", (senderAlert, args) => {
                MakeReq();
                ((Bulletin)Activity).UpdateComments();
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                ((Bulletin)Activity).UpdateComments();
                Dismiss();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private async void MakeReq()
        {
                //submit comment logic
                RequestHandler request = new RequestHandler(Context);
                DB_Singleton db = DB_Singleton.Instance;
                string token = db.Retrieve("Token");
                string groupid = db.GetActiveGroup().GroupID;

                var response = await request.PostMessageReply(token, groupid, m_post.Topicid, m_message);

                if((int)response.StatusCode == 200)
                {
                    Toast.MakeText(Context, "Post Succesful", ToastLength.Long);
                }
                else
                {
                }
            Dismiss();
        }
    }
}