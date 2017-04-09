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

using Newtonsoft.Json.Linq;

using Whos_Home.Helpers;
namespace Whos_Home
{
    class ListNew : DialogFragment
    {
        private Button B_Confrim, B_Cancel;
        private EditText m_editText;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.ListNew, container, false);

            //set values for private attributes
            B_Confrim = view.FindViewById<Button>(Resource.Id.buttonConfirmListNew);
            B_Cancel = view.FindViewById<Button>(Resource.Id.buttonCancelListNew);
            m_editText = view.FindViewById<EditText>(Resource.Id.edittextListNew);

            //set click functions
            B_Confrim.Click += BConfrim_Click;
            B_Cancel.Click += BCancel_Click;

            return view;
        }

        private void BCancel_Click(object sender, EventArgs e)
        {
            ((Lists)Activity).UpdateLists();
            Dismiss();
        }

        private async void BConfrim_Click(object sender, EventArgs e)
        {
            //Implement new list functionality
            string listname = m_editText.Text;
            await PostNewList();

            ((Lists)Activity).UpdateLists();
            Dismiss();
        }

        private async Task PostNewList()
        {
            RequestHandler request = new RequestHandler(Context);
            DB_Singleton db = DB_Singleton.Instance;

            string token = db.Retrieve("Token");
            string groupid = db.GetActiveGroup().GroupID;

            var response = await request.PostNewList(token, groupid, m_editText.Text);

            if((int)response.StatusCode == 200)
            {
                Toast.MakeText(Context, "Succesfully posted", ToastLength.Long);
            }
            else
            {
                Toast.MakeText(Context, "Error posting message", ToastLength.Long);
            }

            Dismiss();
        }
    }
}