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

namespace Whos_Home
{
    [Activity(Label = "SettingsMenu")]
    public class SettingsMenu : Activity
    {
        private List<String> items = new List<String>();
        private ListView listview;
        private string appendUsername, appendEmail;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SettingsMenu);

            InitializeToolbars();
            InitializeLocations();
            
        }

        private void InitializeLocations()
        {
            TextView username = FindViewById<TextView>(Resource.Id.textviewSettingsMenuUsername);
            TextView email = FindViewById<TextView>(Resource.Id.textviewSettingsMenuEmail);

            appendUsername = "CalebIsCool";
            appendEmail = "Caleb@email.com";

            //This is where the text values for the username and email are set
            username.Text = "Username: " + appendUsername;
            email.Text = "Email: " + appendEmail;

            //populate the items list which is used to create the list of locations
            //used for testing to show proof of concept
            for (int i = 1; i < 16; ++i)
            {
                items.Add("Location " + i.ToString());
            }

            //create a listview and assign its adapter to create a list with checklist characteristics
            listview = FindViewById<ListView>(Resource.Id.SettingsMenuListView);
            listview.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItemMultipleChoice, items);

            //sets the selection mode for the listview to multiple choice
            listview.ChoiceMode = Android.Widget.ChoiceMode.Multiple;

            //Sets the function to be called on click to the custom function OnLocationItemClick
            //This Function will select and deselect location values based on the item clicked.
            listview.ItemClick += OnLocationItemClick;

            //initializes a switch object to the one found in the settings page
            Switch s = FindViewById<Switch>(Resource.Id.SwitchSettingsMenu);

            //sets the function used when toggle is checked or unchecked
            s.CheckedChange += ToggleLocations;
        }

        //Function is called when a location is selected in the settings menu
        void OnLocationItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var position = e.Position;

            bool selected = listView.IsItemChecked(position);
            
            if(selected)
                listView.SetItemChecked(position, true);
            else
                listView.SetItemChecked(position, false);
        }

        private void ToggleLocations(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            
            int length = items.Count;
            //if the toggle is set to on
            if (e.IsChecked)
            {
                //set all locations to true if toggled on
                for (int i = 0; i < length; ++i)
                    listview.SetItemChecked(i, true);
            }
            else
            {
                //set all locations to false
                for (int i = 0; i < length; ++i)
                    listview.SetItemChecked(i, false);
            }
        }

        private void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Bulletins";


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