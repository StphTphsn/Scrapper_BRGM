using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    // This class describes the crawling routing for one agent
    // BEWARE: The parsing is not done in this class but in the PARSER class
    class CrawlingAgent
    {
        public bool needNewProxy = true;
        string proxyIP = "None";
        string reference= "None";

        // Constructor: essentially starting thread
        public CrawlingAgent()
        {
            Console.WriteLine("Crawler is born");
            Task task = new Task(CrawlingRoutine);
            task.Start();
        }


        // Crawling routine: while there is references to crawl, crawl, pause, and loop agin until no reference is left
        // If the proxy IP needs to be changed, too many failures with this adress, ask for a new proxy IP to proxy Manager
        public void CrawlingRoutine()
        {

            int successiveFailuresByThisProxy = 0;
            string pageContent = "";

            // while there are still references to crawl in the toDoList and the crawlers succeed in retrieving web pages from the website
            while (Scheduler.toDoList.TryDequeue(out reference) && Scheduler.number_of_successive_crawling_failures<Shared.NUMBER_OF_SUCCESSIVE_FAILURES_TOLERATED_FOR_ALL_PROXIES)
            {
                // use a watch to measure the time of one cycle
                var watch = new Stopwatch();
                // reset variable 'crashed'
                var crashed = false;

                // Write in Console the reference being crawled
                Console.WriteLine(reference);

                // If we are using proxies and proxy needs to be changed, then choose a new proxy
                if (Shared.USE_PROXY & needNewProxy)
                {
                    var NewProxyIsFound = ProxyManager.proxyList.TryDequeue(out proxyIP);
                    // if proxy list is empty, crash the crawling agent at the end of this iteration
                    if (!NewProxyIsFound)
                    {
                        Logs.Write(Shared.ERROR_LOG_FILE_PATH, "DATE:"+ DateTime.Now + "  ERROR: Proxy list is empty");
                        crashed = true;
                    }
                    successiveFailuresByThisProxy = 0;
                    needNewProxy = false;
                }

                // Crawl webpage and return string containing page source
                // TIP: comment try and catch to efficiently debug web exceptions
                try
                {
                    // crawl page and keep page content in a string
                    pageContent = CrawlPage(reference, proxyIP);
                    // save page content in a .txt file
                    Logs.Write(Shared.RAW_FILE_PATH + reference + ".txt", pageContent);
                }
                // Catch any exceptions (can be due to a bad or disconnected proxy, the website protection, the website maintenance/saturation, or an internet connection issue)
                catch (Exception e)
                {
                    // Write error message in error log
                    Logs.Write(Shared.ERROR_LOG_FILE_PATH, "DATE:"+ DateTime.Now +"  PROXY IP:" + proxyIP + "  REFERENCE:" + reference + "  ERROR:" + e.Message);
                    crashed = true;
                }

                // Sometimes there is no exception but the page content does not contain the required information (redirection to a "check for Robot page", other mysterious errors)
                crashed = crashed || !PageContentIsCorrect(pageContent);

                //If there was no problem in the crawling procedure and if the page contains some required keywords...
                if (!crashed)
                {
                    // save page ref in 'Already Crawled' File 
                    Logs.Write(Shared.ALREADY_CRAWLED_FILE_PATH, reference);
                    // reset number_of_successive_crawling_failures to 0
                    successiveFailuresByThisProxy = 0;
                    Scheduler.number_of_successive_crawling_failures = 0;
                }
                else
                {
                    // increment the number of successive errors for this proxyIP
                    successiveFailuresByThisProxy++;
                    Scheduler.number_of_successive_crawling_failures++;
                }

                int timeLeft = (Shared.ONE_CYCLE_TIME_MIN - (int)watch.ElapsedMilliseconds);
                // if the crawling procedure took less time than the minimum cycle time (GOAL: ping the server at a reasonnable rate), wait
                if (timeLeft > 0)
                    System.Threading.Thread.Sleep(timeLeft);

                // if the Proxy is bad, change proxyIP next time
                if (successiveFailuresByThisProxy>= Shared.NUMBER_OF_SUCCESSIVE_FAILURES_TOLERATED_FOR_ONE_PROXY)
                    needNewProxy = true;
            }


        }


        public bool PageContentIsCorrect(string pageContent)
        {
            if (!pageContent.Contains("<div id=\"content_document\" class=\"bloc_content\">"))
            {
                Logs.Write(Shared.ERROR_LOG_FILE_PATH, "DATE:" + DateTime.Now + "  PROXY IP:" + proxyIP + "  REFERENCE:" + reference + "  ERROR: Page Content Is Incorrect");
                return false;
            }else{
                return true;
            }
        }

        public static string CrawlPage(string reference, string proxyIP)
        {
            // Create a request for the URL.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ficheinfoterre.brgm.fr/InfoterreFiche/ficheBss.action?id=" + reference);
            //request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Timeout = Shared.RECEPTION_TIMEOUT;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20130401 Firefox/31.0";
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            if (Shared.USE_PROXY)
            {
                WebProxy myProxy = new WebProxy();
                myProxy.Address = new Uri("http://" + proxyIP);
                myProxy.UseDefaultCredentials = true;
                request.Proxy = myProxy;
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        // Read the content bit by bit to allow breaking if time exceeds
                        string pageContent = "";
                        char[] buffer;
                        var watch = new Stopwatch();
                        while (!reader.EndOfStream && watch.ElapsedMilliseconds <= Shared.RECEPTION_TIMEOUT)
                        {
                            buffer = new char[4096 / 2];
                            reader.Read(buffer, 0, buffer.Length);
                            pageContent += new string(buffer);
                        }
                        return pageContent;
                    }
                    // This line replaces the entire previous paragraph but does not allox timeout due to slow reception
                    //pageContent = reader.ReadToEnd();
                }

            }

        }
    }
}

//string reference = "BSS000DHKH";
//string proxyIP = "212.20.63.36:8080";
//crawlPage(reference, proxyIP);

        //public string ChooseNewProxyIP()
        //{
        //    return "212.20.63.36:8080";
        //}
