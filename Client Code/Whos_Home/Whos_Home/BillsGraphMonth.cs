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
    class BillsGraphMonth : DialogFragment
    {
        private Spinner spinner;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.BillsGraphMonth, container, false);

            spinner = view.FindViewById<Spinner>(Resource.Id.BillsGraphMonthSpinner);
            spinner.Adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleSpinnerItem, GetMonths());

            return view;
        }

        private List<string> GetMonths()
        {
            List<string> months = new List<string>();

            months.Add("January");
            months.Add("February");
            months.Add("March");
            months.Add("April");
            months.Add("May");
            months.Add("June");
            months.Add("July");
            months.Add("August");
            months.Add("September");
            months.Add("October");
            months.Add("November");
            months.Add("December");

            return months;

        }

        public static implicit operator BillsGraphMonth(BillsNew v)
        {
            throw new NotImplementedException();
        }
    }
}