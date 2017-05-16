using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Whos_Home.Helpers;
using Newtonsoft.Json.Linq;
using RestSharp;
using Android.Gms.Common;
using Firebase.Iid;
using Android.Util;
using Android.Graphics;

namespace Whos_Home
{
    [Activity(Label = "Notifications")]
    public class Notifications : BaseActivity
    {
        private Button B_Refresh;
        private List<Invitations> m_activeInvites;
        private ListView m_notificationslistview;
        private List<string> m_message = new List<string>();
        private List<string> m_notif_type = new List<string>();
        private int m_prevent_duplicates = 0;
        TextView msgText;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Notifications);
            //Console.WriteLine(FirebaseInstanceId.Instance.Token);
            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.GetString(key);
                    Console.WriteLine("Key: {0} Value: {1}", key, value);
                }
            }

            InitializeFormat();
            InitializeToolbars();
            ActionBar.Title = "Notifications";
        }

        private async void InitializeFormat()
        {
            msgText = FindViewById<TextView>(Resource.Id.msgText);
            B_Refresh = FindViewById<Button>(Resource.Id.ButtonRefresh);
            B_Refresh.Click += Brefresh_Click;

            bool isavail = await IsPlayServicesAvailable();

            m_notificationslistview = FindViewById<ListView>(Resource.Id.notificationslistview);
            m_notificationslistview.ItemClick += Notificationslistview_ItemClick;
        }

        private void Notificationslistview_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            //If the notification is a group invitation
            if(m_notif_type.ElementAt<string>(position).ToString() == "Group Invite")
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                InviteAcceptDialog Dialog = new InviteAcceptDialog(m_activeInvites.ElementAt<Invitations>(position));
                Dialog.Show(transaction, "dialog fragment accept invite");

                //After dialog box closes, remove invitations
                m_message.RemoveAt(position);
                m_activeInvites.RemoveAt(position);
                m_notif_type.RemoveAt(position);
                m_notificationslistview.Adapter = new GroupListAdapter(this, m_notif_type, m_message);
                m_prevent_duplicates--;
            }
        }

        public async Task<bool> IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    msgText.Text = "This device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                msgText.Text = "Google Play Services is available.";
                RequestHandler request = new RequestHandler();
                string fcmToken = FirebaseInstanceId.Instance.Token;
                Console.WriteLine(fcmToken);
                //await request.FCMRegister(DB_Singleton.Instance.Retrieve("Token"), fcmToken);
                return true;
            }
        }

        private async void Brefresh_Click(object sender, EventArgs e)
        {
            DB_Singleton db = DB_Singleton.Instance;

            RequestHandler request = new RequestHandler(this.BaseContext);

            var response = await request.GetInvitations(db.Retrieve("Token"));

            m_activeInvites = ConvertJson(response);

            //Prevents duplicating notifications when refreshing
            if (m_prevent_duplicates != m_activeInvites.Count)
            {
                //create messages for the listview
                for (int i = 0; i < m_activeInvites.Count; ++i)
                {
                    m_message.Insert(i, m_activeInvites.ElementAt<Invitations>(i).Invitee +
                        " has invited you to join " + m_activeInvites.ElementAt<Invitations>(i).Groupname);

                    m_notif_type.Insert(i, "Group Invite");
                }
                m_notificationslistview.Adapter = new GroupListAdapter(this, m_notif_type, m_message);
            }

            m_prevent_duplicates = m_activeInvites.Count;
        }

        private List<Invitations> ConvertJson(IRestResponse response)
        {
            List<Invitations> invites = new List<Invitations>();
            JArray JInvites = JArray.Parse(response.Content);

            foreach(var invite in JInvites)
            {

                //Console.WriteLine(invite.ToString());
                invites.Add(new Invitations((string)invite["GroupName"], (string)invite["GroupID"], (string)invite["UserName"]));
            }

            return invites;
        }
    }
}