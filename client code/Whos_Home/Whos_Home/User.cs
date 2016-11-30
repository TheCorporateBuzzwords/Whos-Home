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
    class User
    {
        private string firstname, lastname, username, email, password;

        public User(string fn, string ln, string un, string e, string pass)
        {
            firstname = fn;
            lastname = ln;
            username = un;
            email = e;
            password = pass;
        }

        //constructor used for login information
        public User(string user, string pass)
        {
            firstname = null;
            lastname = null;
            email = null;
            username = user;
            password = pass;
        }

        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
            }
        }

        public string Firstname
        {
            get
            {
                return firstname;
            }

            set
            {
                firstname = value;
            }
        }

        public string Lastname
        {
            get
            {
                return lastname;
            }

            set
            {
                lastname = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
            }
        }

        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
            }
        }
    }
}