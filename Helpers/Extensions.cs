using System;
using Microsoft.AspNetCore.Http;

namespace api.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse res, string msg)
        {
            res.Headers.Add("Application-Error", msg);
            res.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            res.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static int CalcAge(this DateTime dateTime)
        {
            var age = DateTime.Today.Year - dateTime.Year;
            if (dateTime.AddYears(age) > DateTime.Today)
            { age--; }

            return age;

        }

    }
}