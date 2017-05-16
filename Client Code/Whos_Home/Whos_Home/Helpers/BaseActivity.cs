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
using Android.Graphics;

namespace Whos_Home.Helpers
{
    public class BaseActivity : Activity
    {
        protected void InitializeToolbars()
        {
            //initialize top toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);

            InitializeTabs();
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

        protected ImageButton tab1Button, tab2Button, tab3Button, tab4Button;
        protected TextView tab1Text, tab2Text, tab3Text, tab4Text, headingText;
        protected Color selectedColor, deselectedColor;

        void InitializeTabs()
        {

            tab1Button = FindViewById<ImageButton>(Resource.Id.tab1_icon);
            tab2Button = this.FindViewById<ImageButton>(Resource.Id.tab2_icon);
            tab3Button = this.FindViewById<ImageButton>(Resource.Id.tab3_icon);
            tab4Button = this.FindViewById<ImageButton>(Resource.Id.tab4_icon);

            tab1Text = this.FindViewById<TextView>(Resource.Id.tab1_text);
            tab2Text = this.FindViewById<TextView>(Resource.Id.tab2_text);
            tab3Text = this.FindViewById<TextView>(Resource.Id.tab3_text);
            tab4Text = this.FindViewById<TextView>(Resource.Id.tab3_text);

            selectedColor = Resources.GetColor(Resource.Color.theme_blue);
            deselectedColor = Resources.GetColor(Resource.Color.white);

            deselectAll();

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
        }

        protected void deselectAll()
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

        protected void showTab1()
        {
            deselectAll();

            tab1Button.SetColorFilter(selectedColor);
            tab1Text.SetTextColor(selectedColor);

            this.StartActivity(typeof(Locations));
        }

        protected void showTab2()
        {
            deselectAll();

            tab2Button.SetColorFilter(selectedColor);
            tab2Text.SetTextColor(selectedColor);

            this.StartActivity(typeof(BulletinBoard));
        }

        protected void showTab3()
        {
            deselectAll();

            tab3Button.SetColorFilter(selectedColor);
            tab3Text.SetTextColor(selectedColor);

            this.StartActivity(typeof(Lists));
        }

        protected void showTab4()
        {
            deselectAll();

            tab4Button.SetColorFilter(selectedColor);
            tab4Text.SetTextColor(selectedColor);

            this.StartActivity(typeof(Bills));
        }
    }
}