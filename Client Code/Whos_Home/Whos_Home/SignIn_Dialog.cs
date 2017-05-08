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
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using Whos_Home.Helpers;
using Firebase.Iid;

namespace Whos_Home
{
    class SignIn_Dialog : DialogFragment
    {
        private Button B_SignIn;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.sign_in, container, false);

            B_SignIn = view.FindViewById<Button>(Resource.Id.buttonlogin);

            //sets click function for the sign in button;
            B_SignIn.Click += SignInAttempt;

            return view;
        }

        public async void SignInAttempt(object sender, EventArgs e)
        {
            //retrieves data from dialog box
            View view = this.View;
            var username = view.FindViewById<EditText>(Resource.Id.signinusername).Text;
            var password = view.FindViewById<EditText>(Resource.Id.signinpassword).Text;

            if (password != null && username != null && password != "" && username != "")
            {
                User user = new User(username, password);

                RequestHandler request = new RequestHandler(Context);
                
                var response = await request.SignIn(user);

                HttpStatusCode code = response.StatusCode;
                int code_num = (int)code;

                if (code_num == 200)
                    Success(response);

                else
                    InvalidResponse(response);
            }
            else
                InvalidInput();
        }

        public async void Success(IRestResponse response)
        {
            Toast.MakeText(this.Context, "Login Successful", ToastLength.Long).Show();
            InsertInDB(DecodeToken(response));
            UpdateGroups();
            //await new RequestHandler().FCMRegister(DB_Singleton.Instance.Retrieve("Token"), FirebaseInstanceId.Instance.Token);
            Activity.StartActivity(typeof(Groups));
        }

        private async void UpdateGroups()
        {
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            RequestHandler request = new RequestHandler(Context);
            IRestResponse response = await request.PullGroups(token);

            List<UserGroup> userGroupList = ReformatResponse(response.Content);

            foreach(UserGroup user in userGroupList)
            {
                db.AddGroup(user.GroupName, user.GroupID);
            }
            if (userGroupList.Count > 0)
                db.ChangeActiveGroup(userGroupList[0]);
            //CAUTION A NEW USER THAT TRIES TO ACCESS ELEMENTS 
            //WITHOUT A GROUP WILL CRASH THE APP
            else
                db.ChangeActiveGroup(new UserGroup(null, null));
        }

        private List<UserGroup> ReformatResponse(string content)
        {
            if (content == "[]" || content == "")
                return new List<UserGroup>();

            List<UserGroup> groupList = new List<UserGroup>();
            JArray tempArr = JArray.Parse((string)content);
            foreach(var group in tempArr)
            {
                string groupid = (string)group["GroupID"];
                string groupname = (string)group["GroupName"];
                groupList.Add(new UserGroup(groupname, groupid));
            }

            return groupList;
        }

        private string[] DecodeToken(IRestResponse response)
        {
            JObject respJson = JObject.Parse(response.Content);

            string token = (string)respJson["token"];

            JwtSecurityToken jwtToken = new JwtSecurityToken(token);

            string pattern = @"{.*?}";
            string decodedJwt = jwtToken.ToString();
            List<string> regResults = new List<string>();

            foreach(Match m in Regex.Matches(decodedJwt, pattern))
            {
                regResults.Add(m.ToString());
            }

            JObject json = JObject.Parse(regResults[1]);
            string username = (string)json["Username"];
            string email = (string)json["Email"];
            string firstname = (string)json["First"];

            string[] decodedToken = { token, username, email, firstname };

            return decodedToken;
        }

        private void InsertInDB(string[] decodedToken)
        {
            
            DB_Singleton instance = DB_Singleton.Instance;

            instance.InitDB();

            instance.InitialInsert(decodedToken[0], decodedToken[1],
                decodedToken[2], decodedToken[3]);
        }

        private void InvalidResponse(IRestResponse response)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Login Failed");

            if (response.Content != "")
                alert.SetMessage(response.Content);
            else
                alert.SetMessage("Connection Error");

            alert.SetPositiveButton("Retry", (senderAlert, args) => { });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                Dismiss();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void InvalidInput()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Login Failed");
            alert.SetMessage("Cannot leave either field blank");

            alert.SetPositiveButton("Retry", (senderAlert, args) => { });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                Dismiss();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }
    }
}