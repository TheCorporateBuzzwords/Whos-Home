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
using Whos_Home.Helpers;
using Newtonsoft.Json;

namespace Whos_Home
{
    class BillsGraphMonth : DialogFragment
    {
        private Spinner m_month_spinner, m_year_spinner;
        private List<BillObj> m_bills;

        public BillsGraphMonth(List<BillObj> bills)
        {
            m_bills = bills;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.BillsGraphMonth, container, false);

            m_month_spinner = view.FindViewById<Spinner>(Resource.Id.BillsGraphMonthSpinner);
            m_year_spinner = view.FindViewById<Spinner>(Resource.Id.BillsGraphYearSpinner);

            m_month_spinner.Adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleSpinnerItem, GetMonths());
            m_year_spinner.Adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleSpinnerItem, GetYears());


            return view;
        }

        private void CreateGraph()
        {
            List<Tuple<string, float>> graph_vals = new List<Tuple<string, float>>();
            float other = 0;
            float rent = 0;
            float utilities = 0;
            float groceries = 0;

            foreach (BillObj bill in m_bills)
            {
                switch (bill.Categoryid)
                {
                    case "1":
                        other += Convert.ToSingle(bill.Amount);
                        break;

                    case "2":
                        rent += Convert.ToSingle(bill.Amount);
                        break;

                    case "3":
                        utilities += Convert.ToSingle(bill.Amount);
                        break;

                    case "4":
                        groceries += Convert.ToSingle(bill.Amount);
                        break;

                    default:
                        Console.WriteLine("INVALID CATEGORY ID");
                        break;
                }
            }
            if (other != 0)
                graph_vals.Add(new Tuple<string, float>("Other", other));
            if (rent != 0)
                graph_vals.Add(new Tuple<string, float>("Rent", rent));
            if (utilities != 0)
                graph_vals.Add(new Tuple<string, float>("Utilities", utilities));
            if (groceries != 0)
                graph_vals.Add(new Tuple<string, float>("Groceries", groceries));

            Intent i = new Intent(Application.Context, typeof(BillsGraph));

            i.PutExtra("BillsList", JsonConvert.SerializeObject(graph_vals));

            StartActivity(i);
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

        private List<string> GetYears()
        {
            List<string> years = new List<string>();

            for (int i = 2000; i < 3000; ++i)
                years.Add(i.ToString());

            return years;
        }

        public static implicit operator BillsGraphMonth(BillsNew v)
        {
            throw new NotImplementedException();
        }
    }
}