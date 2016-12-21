using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Scrapper
{
    class Program
    {
        public static void Main()
        {
            string reference = "BSS000DHKH";
            string proxyIP = "212.20.63.36:8080";
            crawlPage(reference, proxyIP);
        }
        public static void crawlPage(string reference, string proxyIP)
        {
            // Create a request for the URL.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ficheinfoterre.brgm.fr/InfoterreFiche/ficheBss.action?id=" + reference);
            //request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Timeout = 60000;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20130401 Firefox/31.0";
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            WebProxy myProxy = new WebProxy();
            myProxy.Address = new Uri("http://" + proxyIP);
            myProxy.UseDefaultCredentials = true;
            request.Proxy = myProxy;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    string responseFromServer;
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        responseFromServer = reader.ReadToEnd();
                    }
                    using (StreamWriter outfile = new StreamWriter(Shared.RAW_FILE_PATH))
                    {
                        outfile.Write(responseFromServer);//.Replace("\\\"", "\""));//
                    }

                }
            }
        }
    }
}
