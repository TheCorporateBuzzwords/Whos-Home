using System;
using Android.App;
using Firebase.Iid;
using Android.Util;
using Whos_Home.Helpers;

namespace Whos_Home 
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        public override void OnTokenRefresh()
        {
            string refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            SendRegistrationToServer(refreshedToken);
        }
        void SendRegistrationToServer(string fcmToken)
        {
            // Add custom implementation, as needed.
            RequestHandler request = new RequestHandler();
            string jToken = DB_Singleton.Instance.Retrieve("Token");
            var response = request.FCMRegister(jToken, fcmToken);
        }

    }
}