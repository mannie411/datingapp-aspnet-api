using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
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