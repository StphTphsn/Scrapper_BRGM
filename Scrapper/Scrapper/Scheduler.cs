// Code written by Stephane Deny in December 2016. This is a multi-threaded crawler using proxies to crawl the BGRM database.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    // MAIN CLASS: load the reference list to crawl, the list of references already crawled in previous iterations and run the crawling agents
    class Scheduler
    {

        public static ConcurrentQueue<String> toDoList; // a concurrent queue is protected against conflicts in case of multiple threads accessing to it

        public static ConsoleKeyInfo key = new ConsoleKeyInfo(); // useful to promt key from user on the console

        public static int number_of_successive_crawling_failures = 0; // if the number of successive failures by all the crawling agents reaches a threshold , the Scraper is stopped (probably dues to an internet connection problem or if the website is down)


        public static void Main()
        {
            // Write in the log that this is the beginning of a new scrapping session
            Logs.Write(Shared.ERROR_LOG_FILE_PATH, System.Environment.NewLine + "DATE:" + DateTime.Now + "  NEW START");

            // Load file containing references already crawled and create alreadyDoneList HashSet
            Console.WriteLine("Loading AlreadyCrawled file...");
            var alreadyCrawledList = LoadAlreadyCrawledList(Shared.ALREADY_CRAWLED_FILE_PATH);

            // Load BSS file and create toDoList of references to crawl (the toDoList is a public field beacuse it will be used by the Crawling Agents)
            Console.WriteLine("Loading BSS file...");
            LoadToCrawlList(Shared.BSS_FILE_PATH, alreadyCrawledList);

            // If the proxy mode is activated, launch proxy mananger (will fetch proxy list from previous sessions or from the internet)
            if (Shared.USE_PROXY)
            {
                Console.WriteLine("Building list of proxies... ");
                ProxyManager.BuildProxyList();
            }

            // Creating Crawling Agents
            Console.WriteLine("Creating " + Shared.NUMBER_OF_CRAWLING_AGENTS + " crawling agent(s)...");
            for (int i = 1;i <= Shared.NUMBER_OF_CRAWLING_AGENTS;i++)
            {
                new CrawlingAgent();
            }

            // Press any key to interrupt program (you can interrupt the program and restart it safely at any time, it will keep in memory all references previously crawled)
            Console.WriteLine("Press any key to interrupt...");
            key = Console.ReadKey(true);

        }




        // Load list of references already crawled
        public static  HashSet<String> LoadAlreadyCrawledList(string filePath)
        {
            var key = new ConsoleKeyInfo(); // useful to promt key from user on the console
            var alreadyCrawledList = new HashSet<String>(); // Using a HashSet here
            try
            {
                using (var tempStreamReader = new StreamReader(filePath))
                {
                    string tempReference;
                    while ((tempReference = tempStreamReader.ReadLine()) != null)
                    {
                        alreadyCrawledList.Add(tempReference);
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("'Already Crawled' file not found. Assuming no references were crawled yet. Press any key to start crawling.");
                key = Console.ReadKey(true);
            }

            return alreadyCrawledList;
        }

        // Load list of references to crawl
        public static void LoadToCrawlList(string filePath, HashSet<String> alreadyCrawledList){
            toDoList = new ConcurrentQueue<String>();
            using(var tempStreamReader = new StreamReader(filePath))
            {
                string tempRow;
                tempStreamReader.ReadLine();
                while ((tempRow = tempStreamReader.ReadLine()) != null)
                 {
                     var tempColumnElements = tempRow.Split(new string[] {";"}, StringSplitOptions.None);
                     var tempReference = tempColumnElements[4];
                    if (!alreadyCrawledList.Contains(tempReference))
                        toDoList.Enqueue(tempReference);
                 }
             }
            Console.WriteLine(toDoList.Count() + " pages to crawl");
        }

    }



}

