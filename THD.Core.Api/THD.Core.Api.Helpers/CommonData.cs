using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace THD.Core.Api.Helpers
{
    public class CommonData
    {
        public static int GetYearOfPeriod()
        {
            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            int year = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
            int month = DateTime.Now.Month;
            int result = 2524;

            if (month >= 1 && month <= 9) result = (year);
            if (month >= 10 && month <= 12) result = (year + 1);

            return result;
        }

        public static int ConvertYearToThai(int year)
        {

            int result = (year >= 2500 ? year : (year + 543));

            return result;
        }

    }
}
