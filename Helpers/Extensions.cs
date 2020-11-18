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

    }
}