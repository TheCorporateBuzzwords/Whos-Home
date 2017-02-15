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
    class ListAddItem : DialogFragment
    {
        private Button bConfrim, bCancel;
        private EditText editText;
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            view = inflater.Inflate(Resource.Layout.ListNew, container, false);

            //set values for private attributes
            bConfrim = view.FindViewById<Button>(Resource.Id.buttonConfirmListNew);
            bCancel = view.FindViewById<Button>(Resource.Id.buttonCancelListNew);
            editText = view.FindViewById<EditText>(Resource.Id.edittextListNew);

            //set click functions
            bConfrim.Click += BConfrim_Click;
            bCancel.Click += BCancel_Click;

            return view;

        }

        private void BCancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void BConfrim_Click(object sender, EventArgs e)
        {
            //Implement new list item functionality
            string itemname = editText.Text;

            Toast.MakeText(view.Context, "Item Added: " + itemname,
               ToastLength.Short).Show();

            editText.Text = "";
        }
    }
}