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

namespace Scrapper
{
    // MAIN CLASS: load the reference list to crawl, the list of references already crawled in previous iterations and run the crawling agents
    class Scheduler
    {

        public static ConcurrentQueue<String> toDoList = new ConcurrentQueue<String>(); // a concurrent queue is protected against conflicts in case of multiple threads accessing to it
        // If the number of successive failures by all the crawling agents reaches a threshold , the scrapper is stopped (probably dues to an internet connection problem or if the website is down)
        public static int number_of_successive_crawling_failures = 0;

        public static void Main()
        {
            var key = new ConsoleKeyInfo();


            // Load file containing references already crawled and create alreadyDoneList HashSet
            var alreadyCrawledList = new HashSet<String>(); // Using a HashSet here
            Console.WriteLine("Loading AlreadyCrawled file...");
            try
            {
                using (var tempStreamReader = new StreamReader(Shared.ALREADY_CRAWLED_FILE_PATH))
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

            // Load BSS file and create toDoList Queue of references to crawl
            Console.WriteLine("Loading BSS file...");
            using(var tempStreamReader = new StreamReader(Shared.BSS_FILE_PATH))
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


            // Creating Crawling Agents
            Console.WriteLine("Creating " + Shared.NUMBER_OF_CRAWLER_AGENTS + " crawler agents...");
            for (int i = 1;i <= Shared.NUMBER_OF_CRAWLER_AGENTS;i++)
            {
                new CrawlingAgent();
            }
            key = Console.ReadKey(true);

        }

    }
}
