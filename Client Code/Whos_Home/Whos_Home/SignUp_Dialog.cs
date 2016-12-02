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
using Newtonsoft.Json;
using System.Net;
using RestSharp;

namespace Whos_Home
{
    class SignUp_Dialog : DialogFragment
    {
        private Button confirm;
        private string url = "http://96.41.173.205:3000";
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.sign_up, container, false);
            confirm = view.FindViewById<Button>(Resource.Id.buttonConfirm);

            //sets click function for the confirm button;
            confirm.Click += testDel;

            return view;

        }


        public async void testDel(object sender, System.EventArgs e)
        {
            //retrieves data from dialog box
                View view = this.View;
                var firstname = view.FindViewById<EditText>(Resource.Id.firstnametext).Text;
                var lastname = view.FindViewById<EditText>(Resource.Id.lastnametext).Text;
                var email = view.FindViewById<EditText>(Resource.Id.emailtext).Text;
                var username = view.FindViewById<EditText>(Resource.Id.usernametext).Text;
                var password = view.FindViewById<EditText>(Resource.Id.passwordtext).Text;
                var passCheck = view.FindViewById<EditText>(Resource.Id.repeatpasswordtext).Text;

                //create an alert box to show data that was entered (For testing)
                AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);

                Console.WriteLine("INFO:");
                Console.WriteLine("First Name: " + firstname);
                Console.WriteLine("Last Name: " + lastname);
                Console.WriteLine("Email: " + email);
                Console.WriteLine("Password1: " + password);
                Console.WriteLine("Password2: " + passCheck);

                if(firstname != null && lastname != null && email != null && password != null && password == passCheck)
                {
                User user = new User(firstname, lastname, username, email, password, passCheck);
                    string json = JsonConvert.SerializeObject(user);

                    var client = new RestClient(url);

                    var request = new RestRequest("/users", Method.POST);
                    request.AddObject(user);
                    var response = await client.ExecuteTaskAsync(request);
                    Console.WriteLine("RESPONSE: " + response.Content);
                }

                //create an instance of a user and initialize it
                Dialog dialog = alert.Create();
                dialog.Show();
            }
        
    }
}