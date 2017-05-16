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
using RestSharp;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using Firebase.Iid;

namespace Whos_Home.Helpers
{
    public static class CredentialHandler
    {
        static public async Task<bool> SignUp(User user)
        {
            if (await IsValidInput(user))
                return true;
            return false;
        }

        static public async Task<bool> SignIn(string username, string password)
        {
            if(await IsValidInput(username, password))
                return true;

            return false;
        }

        //SignUp Methods
        static public async Task<bool> IsValidInput(User user)
        {
            bool fname_valid = (user.Firstname != null && user.Firstname != "");
            bool lname_valid = (user.Lastname != null && user.Lastname != "");
            bool email_valid = (user.Email != null && user.Email != "");
            bool uname_valid = (user.Username != null && user.Username != "");
            bool pass_valid = (user.Password != null && user.Password != "");
            bool all_valid = (fname_valid && lname_valid && email_valid && uname_valid && pass_valid);
            //create an alert box to show data that was entered (For testing)
            if (all_valid && user.Password == user.Confirm)
            {
                RequestHandler request = new RequestHandler();

                IRestResponse response = await request.SignUp(user);
                HttpStatusCode code = response.StatusCode;
                string token;
                try
                {
                    JObject ar = JObject.Parse(response.Content);
                    token = (string)ar["token"];
                }
                catch
                {
                    return false;
                }

                int code_num = (int)code;
                if (code_num == 201)
                {
                    InsertInDB(new string[] { token, user.Username, user.Email, user.Firstname});
                    return true;
                }
                else
                    return false;
            }
            else
                return false;

        }
        //SignIn Methods
        static public async Task<bool> IsValidInput(string username, string password)
        {
            if (password != null && username != null && password != "" && username != "")
            {
                User user = new User(username, password);
                RequestHandler request = new RequestHandler();

                var response = await request.SignIn(user);

                HttpStatusCode code = response.StatusCode;
                int code_num = (int)code;
                if (code_num == 200)
                {
                    Success(response);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        static public async void Success(IRestResponse response)
        {
            InsertInDB(DecodeToken(response));
            UpdateGroups();
            //await new RequestHandler().FCMRegister(DB_Singleton.Instance.Retrieve("Token"), FirebaseInstanceId.Instance.Token);
        }

        static private async void UpdateGroups()
        {
            DB_Singleton db = DB_Singleton.Instance;
            string token = db.Retrieve("Token");
            RequestHandler request = new RequestHandler();
            IRestResponse response = await request.PullGroups(token);

            List<UserGroup> userGroupList = ReformatResponse(response.Content);

            foreach (UserGroup user in userGroupList)
            {
                db.AddGroup(user.GroupName, user.GroupID);
            }
            if (userGroupList.Count > 0)
                db.ChangeActiveGroup(userGroupList[0]);
            else
                db.ChangeActiveGroup(new UserGroup(null, null));
        }

        static private List<UserGroup> ReformatResponse(string content)
        {
            //REFACTOR
            if (content == "[]" || content == "")
                return new List<UserGroup>();

            List<UserGroup> groupList = new List<UserGroup>();
            JArray tempArr = JArray.Parse((string)content);
            foreach (var group in tempArr)
            {
                string groupid = (string)group["GroupID"];
                string groupname = (string)group["GroupName"];
                groupList.Add(new UserGroup(groupname, groupid));
            }

            return groupList;
        }

        static private string[] DecodeToken(IRestResponse response)
        {
            JObject respJson = JObject.Parse(response.Content);

            string token = (string)respJson["token"];

            JwtSecurityToken jwtToken = new JwtSecurityToken(token);

            string pattern = @"{.*?}";
            string decodedJwt = jwtToken.ToString();
            List<string> regResults = new List<string>();

            foreach (Match m in Regex.Matches(decodedJwt, pattern))
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

        static private void InsertInDB(string[] decodedToken)
        {

            DB_Singleton instance = DB_Singleton.Instance;

            instance.InitDB();

            instance.InitialInsert(decodedToken[0], decodedToken[1],
                decodedToken[2], decodedToken[3]);
        }
    }
}