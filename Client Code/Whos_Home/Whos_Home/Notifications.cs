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
    public class Notifications : Activity
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


            //InitializeFormat();
            TestTabs();
            //InitializeToolbars();
        }

        private async void InitializeFormat()
        {
            //msgText = FindViewById<TextView>(Resource.Id.msgText);
            //B_Refresh = FindViewById<Button>(Resource.Id.ButtonRefresh);
            B_Refresh.Click += Brefresh_Click;

            bool isavail = await IsPlayServicesAvailable();

            //m_notificationslistview = FindViewById<ListView>(Resource.Id.notificationslistview);
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

            //Start the Lists activity
            if (e.Item.ToString() == "Lists")
                this.StartActivity(typeof(Lists));

            //Start the Lists activity
            if (e.Item.ToString() == "Bills")
                this.StartActivity(typeof(Bills));

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

        private ImageButton tab1Button, tab2Button, tab3Button, tab4Button;
        private TextView tab1Text, tab2Text, tab3Text, tab4Text, headingText;
        private Color selectedColor, deselectedColor;

        void TestTabs()
        {
            tab1Button = FindViewById<ImageButton>(Resource.Id.tab1_icon);
            tab2Button = this.FindViewById<ImageButton>(Resource.Id.tab2_icon);
            tab3Button = this.FindViewById<ImageButton>(Resource.Id.tab3_icon);
            tab4Button = this.FindViewById<ImageButton>(Resource.Id.tab4_icon);

            //headingText = this.FindViewById<TextView>(Resource.Id.heading_text);

            tab1Text = this.FindViewById<TextView>(Resource.Id.tab1_text);
            tab2Text = this.FindViewById<TextView>(Resource.Id.tab2_text);
            tab3Text = this.FindViewById<TextView>(Resource.Id.tab3_text);
            tab4Text = this.FindViewById<TextView>(Resource.Id.tab3_text);

            selectedColor = Resources.GetColor(Resource.Color.white);
            deselectedColor = Resources.GetColor(Resource.Color.theme_blue);

            tab1Button.Click += delegate {
                showTab1();
            };

            tab2Button.Click += delegate {
                showTab2();
            };

            tab3Button.Click += delegate {
                showTab3();
            };

            tab4Button.Click += delegate {
                showTab4();
            };

            //showTab1();

        }

        private void deselectAll()
        {
            tab1Button.SetColorFilter(deselectedColor);
            tab2Button.SetColorFilter(deselectedColor);
            tab3Button.SetColorFilter(deselectedColor);
            tab4Button.SetColorFilter(deselectedColor);

            tab1Text.SetTextColor(deselectedColor);
            tab2Text.SetTextColor(deselectedColor);
            tab3Text.SetTextColor(deselectedColor);
            tab4Text.SetTextColor(deselectedColor);

        }

        private void showFragment(Fragment fragment)
        {
            //var ft = FragmentManager.BeginTransaction();
            //ft.Replace(Resource.Id.container, fragment);
            //ft.Commit();
        }


        private void showTab1()
        {
            deselectAll();

            //headingText.Text = "Tab 1";

            tab1Button.SetColorFilter(selectedColor);
            tab1Text.SetTextColor(selectedColor);

            //this.StartActivity(typeof(Locations));

        }

        private void showTab2()
        {
            deselectAll();

            //headingText.Text = "Tab 1";

            tab2Button.SetColorFilter(selectedColor);
            tab2Text.SetTextColor(selectedColor);

            //this.StartActivity(typeof(Locations));
        }

        private void showTab3()
        {
            deselectAll();

            //headingText.Text = "Tab 1";

            tab3Button.SetColorFilter(selectedColor);
            tab3Text.SetTextColor(selectedColor);

            //this.StartActivity(typeof(Locations));
        }

        private void showTab4()
        {
            deselectAll();

            //headingText.Text = "Tab 1";

            tab4Button.SetColorFilter(selectedColor);
            tab4Text.SetTextColor(selectedColor);

            //this.StartActivity(typeof(Locations));
        }
    }
}