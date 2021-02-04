using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ServiceBranch.Controllers
{
    public class GlobalController : Controller
    {

        public static SelectList GenerateYearsDDL(int selected_year = -1)
        {
            var items = new List<SelectListItem>();
            for (int i = 2019; i <= 2050; i++)
            {

                SelectListItem tmp = new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                };
                if (selected_year != -1 && Int32.Parse(tmp.Value) == selected_year)
                {
                    tmp.Selected = true;
                }
                items.Add(tmp);



            }
            return new SelectList(items, "Value", "Text", selected_year);
        }
        //return a selectedlist of months
        public static SelectList GenerateMonthsDDL(int selected_month = -1)
        {
            var items = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                SelectListItem tmp = new SelectListItem
                {
                    Value = i.ToString(),
                    Text = ArabicMonthName(i)
                };
                if (selected_month != -1 && Int32.Parse(tmp.Value) == selected_month)
                {
                    tmp.Selected = true;
                }
                items.Add(tmp);
            }
            return new SelectList(items, "Value", "Text", selected_month);
        }
        public static string ArabicMonthName(int month_number)
        {
            IDictionary<int, string> monthsList = new Dictionary<int, string>() {
                {1,"كانون الثاني" },
                {2,"شباط" },
                {3,"آذار" },
                {4,"نيسان" },
                {5,"أيار" },
                {6,"حزيران" },
                {7,"تموز" },
                {8,"آب" },
                {9,"أيلول" },
                {10,"تشرين الأول" },
                {11,"تشرين الثاني" },
                {12,"كانون الأول" },
            };
            return monthsList[month_number];
        }
        public static bool IsValidDate(int year, int month, int day)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
                return false;

            if (month < 1 || month > 12)
                return false;

            return day > 0 && day <= DateTime.DaysInMonth(year, month);
        }
    }
}