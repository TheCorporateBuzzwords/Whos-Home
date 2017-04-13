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

using RestSharp;
using Couchbase.Lite;

namespace Whos_Home.Helpers
{
    /***********************************
     * Class: Request Handler
     * 
     * Description: This class handles all
     * requests to the server. 
     * 
     * TODO: Remove Constructor that accepts
     * a context. The reason is to make it 
     * non-Android specific. This requires 
     * changing the current calls to the default
     * constructor
     * **********************************/
    class RequestHandler
    {
        private string url = null;
        IRestRequest request = null;
        RestClient client = null;
        public RequestHandler(Context context)
        {
            url = context.Resources.GetString(Resource.String.url);
            client = new RestClient(url);

            //Needed, because the Certificates are self signed
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
        }
        public RequestHandler()
        {
            url = "https://75.142.141.235:4433";
            client = new RestClient(url);
        }
        //Initial Request for login and signup
        public async Task<IRestResponse> SignIn(User user)
        {
            request = new RestRequest("/session", Method.POST);
            request.AddObject(user);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }

        public async Task<IRestResponse> SignUp(User user)
        {
            request = new RestRequest("/users", Method.POST);
            request.AddObject(user);

            return await client.ExecuteTaskAsync(request);
        }

        public async Task<IRestResponse> CreateGroup(string groupName, string token)
        {
            request = new RestRequest("/groups", Method.POST);

            request.AddParameter("groupName", groupName);
            request.AddHeader("x-access-token", token);
            var response = await client.ExecuteTaskAsync(request);
            return response;
        }

        public async Task<IRestResponse> PullGroups(string token)
        {
            request = new RestRequest("/users/groups", Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }

        public async Task<IRestResponse> EditGroupName(string token, string groupid, string newname)
        {
            request = new RestRequest("groups/{groupid}/egroup/", Method.PUT);
            request.AddParameter("newName", newname);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }

        //INVITATIONS Requests
        public async Task<IRestResponse> RespondInvitation(string token, string groupid, bool deny)
        {
            request = new RestRequest(string.Format("/groups/{0}/invitation/?deny={1}", groupid, deny.ToString().ToLower()), Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> SendInvitation(string token, string groupid, string username)
        {
            request = new RestRequest(string.Format("/groups/{0}/invitation/", groupid), Method.POST);
            request.AddUrlSegment("groupid", groupid);
            request.AddParameter("recipient", username);

            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);

            return response;
        }
        public async Task<IRestResponse> GetInvitations(string token)
        {
            request = new RestRequest("/users/invites", Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }

        //LOCATIONS Requests
        public async Task<IRestResponse> GetUserLocations(string token, string groupid)
        {
            request = new RestRequest(string.Format("/groups/{0}", groupid), Method.GET);
            request.AddHeader("x-access-token", token);
            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> AddLocation(string token, string SSID, string locationName, string groupid)
        {
            request = new RestRequest("groups/{groupid}/location/", Method.POST);
            request.AddUrlSegment("groupid", groupid);
            request.AddParameter("ssid", SSID);
            request.AddParameter("locationName", locationName);

            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);

            return response;
        }
        public async Task<IRestResponse> GetLocations(string token, string groupid)
        {
            request = new RestRequest("/groups/{groupid}/locations/", Method.GET);
            request.AddHeader("x-access-token", token);

            request.AddUrlSegment("groupid", groupid);

            var response = await client.ExecuteTaskAsync(request);

            return response;
        }
        public async Task<IRestResponse> UpdateLocation(string token, string SSID)
        {
            //SSID can be null if user is offline
            request = new RestRequest("/users/location", Method.PUT);

            request.AddHeader("x-access-token", token);
            request.AddParameter("bssid", SSID);

            var response = await client.ExecuteTaskAsync(request);

            return response;
        }

        //MESSAGE Requests
        public async Task<IRestResponse> GetMessages(string token, string groupid)
        {
            request = new RestRequest(string.Format("/groups/{0}/messagetopic", groupid), Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> PostMessages(string token, string groupid, string title, string message)
        {
            request = new RestRequest(string.Format("/groups/{0}/messagetopic", groupid), Method.POST);
            request.AddHeader("x-access-token", token);
            request.AddParameter("title", title);
            request.AddParameter("msg", message);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> GetMessageReplies(string token, string groupid, string topicid)
        {
            request = new RestRequest(string.Format("/groups/{0}/messages/{1}", groupid, topicid), Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;

        }
        public async Task<IRestResponse> PostMessageReply(string token, string groupid, string topicid, string message)
        {
            request = new RestRequest(string.Format("/groups/{0}/messages", groupid), Method.POST);
            request.AddHeader("x-access-token", token);
            request.AddParameter("topicid", topicid);
            request.AddParameter("msg", message);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> PostNewList(string token, string groupid, string title)
        {
            request = new RestRequest(string.Format("/groups/{0}/lists", groupid), Method.POST);
            request.AddHeader("x-access-token", token);
            request.AddParameter("title", title);

            var response = await client.ExecuteTaskAsync(request);
            return response;

        }
        public async Task<IRestResponse> EditPostName(string token, string groupid, string topicid, string newtitle)
        {
            request = new RestRequest(string.Format("/groups/{0}/topicedit/{1}", groupid, topicid), Method.PUT);
            request.AddHeader("x-access-token", token);
            request.AddParameter("newTitle", newtitle);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> DeleteMessageReply(string token, string groupid, string replyid)
        {
            request = new RestRequest(string.Format("/groups/{0}/postdelete/{1}", groupid, replyid), Method.DELETE);
            request.AddHeader("x-access-token", token);
            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> DeletePost(string token, string groupid, string postid)
        {
            request = new RestRequest(string.Format("/groups/{0}/topicdelete/{1}", groupid, postid), Method.DELETE);
            request.AddHeader("x-access-token", token);
            var response = await client.ExecuteTaskAsync(request);
            return response;
        }


        //LIST Requests
        public async Task<IRestResponse> PostNewListItem(string token, string groupid, string listid, string content)
        {
            request = new RestRequest(string.Format("/groups/{0}/lists/{1}", groupid, listid), Method.POST);
            request.AddHeader("x-access-token", token);
            request.AddParameter("content", content);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> GetLists(string token, string groupid)
        {
            request = new RestRequest(string.Format("/groups/{0}/lists/", groupid), Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> GetListItems(string token, string groupid, string listid)
        {
            request = new RestRequest(string.Format("/groups/{0}/lists/{1}", groupid, listid), Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> EditListName(string token, string groupid, string listid, string newtitle)
        {
            request = new RestRequest(string.Format("/groups/{0}/elist/{1}", groupid, listid), Method.POST);
            request.AddHeader("x-access-token", token);
            request.AddParameter("newTitle", newtitle);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        //public async Task<IRestResponse> UpdateListItem(string token, string groupid, string item) { }
        public async Task<IRestResponse> PutListItem(string token, string groupid, string listid, string itemid, bool completed)
        {
            request = new RestRequest(string.Format("/groups/{0}/lists/{1}", groupid, listid), Method.PUT);
            request.AddHeader("x-access-token", token);
            //this is kinda weird and breaking 
            //My design, but it's the way the api
            //is set up right now

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> DeleteList(string token, string groupid, string listid)
        {
            request = new RestRequest(string.Format("/groups/{0}/dlist/{1}", groupid, listid), Method.DELETE);
            request.AddHeader("x-access-token", token);
            //this is kinda weird and breaking 
            //My design, but it's the way the api
            //is set up right now

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
        public async Task<IRestResponse> DeleteListItem(string token, string groupid, string itemid)
        {
            request = new RestRequest(string.Format("/groups/{0}/dlistitem/{1}", groupid, itemid), Method.DELETE);
            request.AddHeader("x-access-token", token);
            //this is kinda weird and breaking 
            //My design, but it's the way the api
            //is set up right now

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }


        //BILLS Requests
        public async Task<IRestResponse> PutBill(string token, string groupid, string recipientid, string category, string title, string description, string ammount, DateTime date)
        {
            request = new RestRequest(string.Format("/groups/{0}/bills/", groupid), Method.POST);
            request.AddHeader("x-access-token", token);

            request.AddParameter("recipient", recipientid);
            request.AddParameter("category", category);
            request.AddParameter("title", title);
            request.AddParameter("description", description);
            request.AddParameter("amount", ammount);
            request.AddParameter("date", date);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }

        public async Task<IRestResponse> GetBills(string token, string groupid, string username = null)
        {
            request = new RestRequest(string.Format("/groups/{0}/bills/", groupid), Method.GET);
            request.AddHeader("x-access-token", token);

            var response = await client.ExecuteTaskAsync(request);
            return response;
        }
    }   
}