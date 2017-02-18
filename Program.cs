using System;
using System.Net;
using System.IO;

using JWT;
using Newtonsoft.Json.Linq;

namespace JWT_dotNET {
    class Program {
        // Set these constants to your 
        const int PARTNERID = 0;
        const string APISECRET = "";
        const string SITEID = "";
        const string EMPLOYEEID = "";
        const string AUTHSERVICE = "https://clock.payrollservers.us/AuthenticationService/oauth2/userToken";

        static void Main(string[] args) {

            var token = new {
                iss = PARTNERID,
                product = "twpemp",
                sub = "partner",
                siteInfo = new {
                    type = "id",
                    id = SITEID
                },
                user = new {
                    type = "id",
                    id = EMPLOYEEID
                },
                exp = (Int32)DateTime.UtcNow.Add(new TimeSpan(0, 4, 30)).Subtract(new DateTime(1970, 1, 1)).TotalSeconds
            };

            var jwt = JsonWebToken.Encode(token, APISECRET, JwtHashAlgorithm.HS256);

            WebRequest request = WebRequest.Create(AUTHSERVICE);

            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers.Set("Authorization", string.Format("Bearer {0}", jwt));

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.Created) {
                JObject result = null;

                using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                    result = JObject.Parse(sr.ReadToEnd());
                }

                Console.WriteLine(string.Format("JWT: {0}", result["token"].ToString()));
            } else {
                Console.WriteLine("Error getting signed JWT.");
            }

            Console.ReadLine();
        }
    }
}
