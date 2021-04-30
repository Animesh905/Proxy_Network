using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Proxy_NetChecker.Models;
namespace Proxy_NetChecker
{
    class Program
    {
        static int ConsoleLoad = 0;
        static string proxyAddress;
        static Uri weburl;
        static string UserName;
        static string Password;
        static void Main()
        {
            try 
            { 
                string Proxyurl;
                int port;
                ConsoleLoad++;
                if (ConsoleLoad == 1)
                {
                    Console.WriteLine($"Net Checker [Version:{Assembly.GetEntryAssembly().GetName().Version}]\n");
                }
                Console.Write("Please provide url:");
                weburl = new Uri(Console.ReadLine());
                Console.Write("Please provide proxy:");
                Proxyurl = Console.ReadLine().ToString();
                Console.Write("Port No : ");
                port = Convert.ToInt32(Console.ReadLine());
            
                    CheckAutoGlobalProxyForRequest(weburl, Proxyurl, port);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CallingApp();
            }
        }

        public static void CheckAutoGlobalProxyForRequest(Uri resource, string Proxyurl, int Portno)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(resource);
            WebProxy proxy = new WebProxy();
            proxyAddress = $"http://{Proxyurl}:{Portno}";
            try
            {
                if (proxyAddress.Length > 0)
                {
                    Console.WriteLine("\nPlease enter the Credential may not be used");
                    Uri newUri = new Uri(proxyAddress);
                    proxy.Address = newUri;
                    Console.Write("Username :");
                    UserName = Console.ReadLine();
                    Console.Write(" Password:");
                    Password = Console.ReadLine();
                    proxy.Credentials = new NetworkCredential(UserName, Password);
                    webRequest.Proxy = proxy;
                }
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                Console.WriteLine($"=====================================Web data using {proxy.Address} ===========================================");
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                string responseString = streamRead.ReadToEnd();
                var regex = new Regex(
                           "(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)",
                           RegexOptions.Singleline | RegexOptions.IgnoreCase
                                      );
                System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*");
                responseString = regex.Replace(responseString, "");
                responseString = rx.Replace(responseString, "");
                responseString = responseString.Replace(">", "");
                responseString = responseString.Replace("&nbsp;", "");
                Console.WriteLine(responseString);
                IWebProxy webProxy = webRequest.Proxy;
                if (proxy != null)
                {
                    Console.WriteLine($"Proxy : {webProxy.GetProxy(webRequest.RequestUri)}");
                }
                streamResponse.Close();
                streamRead.Close();
                response.Close();
                CallingApp();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Connection refused! .. {proxyAddress}. Plz try with different proxy");
                CallingApp();
            }
        }
        public static void CallingApp()
        {
            Console.ResetColor();
            Console.WriteLine($"=====================================Web Surfing Using Transparent Proxy===========================================");
            Main();
        }
        
       
    }
}

