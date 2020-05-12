using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Google.Apis.Auth;
using Google.Apis.Http;
using Google.Apis.Auth.OAuth2;
using System.Net.Http;
using System.Net.Http.Headers;
using Google.Apis.Logging;
namespace Program
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {                  
                //Google.ApplicationContext.RegisterLogger(new ConsoleLogger(LogLevel.All,true));
                var targetAudience = "https://myapp-6w42z6vi3q-uc.a.run.app";
                string uri = "https://httpbin.org/get";
                string CREDENTIAL_FILE_JSON = "/path/to/svc_accuont.json";
                new Program().Run(targetAudience, CREDENTIAL_FILE_JSON, uri).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var err in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + err.Message);
                }
            }

        }


        public async Task<string> Run(string targetAudience, string credentialsFilePath, string uri)
        {
            ServiceAccountCredential saCredential;

            using (var fs = new FileStream(credentialsFilePath, FileMode.Open, FileAccess.Read))
            {
                saCredential = ServiceAccountCredential.FromServiceAccountData(fs);
            }
            OidcToken oidcToken = await saCredential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(targetAudience).WithTokenFormat(OidcTokenFormat.Standard)).ConfigureAwait(false);            
            string token = await oidcToken.GetAccessTokenAsync().ConfigureAwait(false);
            using (var httpClient = new HttpClient())
            {
               
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string response = await httpClient.GetStringAsync(uri).ConfigureAwait(false);
                Console.WriteLine(response);
                return response;
            }
        }
    }
}

