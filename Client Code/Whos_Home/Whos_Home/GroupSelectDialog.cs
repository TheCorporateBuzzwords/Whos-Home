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
    class GroupSelectDialog : DialogFragment
    {
        private Button Select;
        private Button Cancel;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.GroupDialog, container, false);

            Select = view.FindViewById<Button>(Resource.Id.buttonSelectGroup);
            Cancel = view.FindViewById<Button>(Resource.Id.buttonCancelSelectGroup);

            return view;

        }
    }
}