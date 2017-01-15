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
    class DB_Singleton
    {
        private static DB_Singleton instance = null;
        private static readonly object db_lock = new object();
        DB_Singleton()
        {

        }
        public static DB_Singleton Instance
        {
            get
            {
                lock(db_lock)
                {
                    if(instance == null)
                    {
                        instance = new DB_Singleton();
                    }
                    return instance;
                }
            }
        }
    }
}