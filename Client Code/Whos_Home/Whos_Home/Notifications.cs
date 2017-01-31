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
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Whos_Home
{
    [Activity(Label = "Notifications")]
    public class Notifications : Activity
    {
        private Button Brefresh;
        private List<Invitations> activeInvites;
        private ListView notificationslistview;
        private List<string> message = new List<string>();
        private List<string> notif_type = new List<string>();
        private int prevent_duplicates = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Notifications);

            Brefresh = FindViewById<Button>(Resource.Id.ButtonRefresh);
            Brefresh.Click += Brefresh_Click;

            notificationslistview = FindViewById<ListView>(Resource.Id.notificationslistview);
            notificationslistview.ItemClick += Notificationslistview_ItemClick;

            InitializeToolbars();


        }

        private void Notificationslistview_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            //If the notification is a group invitation
            if(notif_type.ElementAt<string>(position).ToString() == "Group Invite")
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                AcceptInviteDialog Dialog = new AcceptInviteDialog(activeInvites.ElementAt<Invitations>(position));
                Dialog.Show(transaction, "dialog fragment accept invite");

                //After dialog box closes, remove invitations
                message.RemoveAt(position);
                activeInvites.RemoveAt(position);
                notif_type.RemoveAt(position);
                notificationslistview.Adapter = new GroupListAdapter(this, notif_type, message);
                prevent_duplicates--;


            }
        }

        private async void Brefresh_Click(object sender, EventArgs e)
        {
            DB_Singleton db = DB_Singleton.Instance;

            RequestHandler request = new RequestHandler(this.BaseContext);

            var response = await request.GetInvitations(db.Retrieve("Token"));

            activeInvites = ConvertJson(response);

            //Prevents duplicating notifications when refreshing
            if (prevent_duplicates != activeInvites.Count)
            {
                //create messages for the listview
                for (int i = 0; i < activeInvites.Count; ++i)
                {
                    message.Insert(i, activeInvites.ElementAt<Invitations>(i).Invitee +
                        " has invited you to join " + activeInvites.ElementAt<Invitations>(i).Groupname);

                    notif_type.Insert(i, "Group Invite");
                }
                notificationslistview.Adapter = new GroupListAdapter(this, notif_type, message);
            }

            prevent_duplicates = activeInvites.Count;


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

        private void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Notifications";


            //initialize bottom toolbar
            var editToolbar = FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            //editToolbar.Title = "Navigate";
            editToolbar.InflateMenu(Resource.Menu.edit_menus);
            editToolbar.MenuItemClick += NavigateMenu;

            //(sender, e) => {
            //Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
            //};


        }

        //Method is used to navigate between activities using the bottom toolbar
        private void NavigateMenu(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            //Start the bulletin activity
            if (e.Item.ToString() == "Bulletins")
                this.StartActivity(typeof(BulletinBoard));

            //Start the Locations activity
            if (e.Item.ToString() == "Locations")
                this.StartActivity(typeof(Locations));

        }

        //called to specify menu resources for an activity
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        //called when a menu item is tapped
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();

            //loads notifications
            if (item.ToString() == "Notifications")
                this.StartActivity(typeof(Notifications));

            //Loads settings menu if preferences is selected
            if (item.ToString() == "Preferences")
                this.StartActivity(typeof(SettingsMenu));

            //Loads Groups menu if selected
            if (item.ToString() == "Groups")
                this.StartActivity(typeof(Groups));

            return base.OnOptionsItemSelected(item);
        }
    }
}