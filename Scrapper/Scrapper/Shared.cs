using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    // This class contains all the parameters and file paths useful for the program to run
     static class Shared
    {
         // CRAWLING PARAMETERS
         public static bool USE_PROXY = false; //  Proxies are usefull to bypass some websites protections against scrapping (by detection of frequent requests from same IP)
         public static int NUMBER_OF_CRAWLING_AGENTS = 1; // Number of crawling agents (the more the faster but beware of CPU saturation and website protections)
         public static int ONE_CYCLE_TIME_MIN = 10000; // minimal time between two missions of the same crawler in milliseconds (avoid saturating website bandpass or being detected as a Scraper bot by the website)
         public static int RECEPTION_TIMEOUT = 60000; // maximimum reception time (avoid loosing to much time with a slow proxy)
         public static int NUMBER_OF_SUCCESSIVE_FAILURES_TOLERATED_FOR_ONE_PROXY = 3; // number of failures authorized before changing proxy (avoid staying stuck with a broken/malfunctionning/slow proxy)
         public static int NUMBER_OF_SUCCESSIVE_FAILURES_TOLERATED_FOR_ALL_PROXIES = 30; // number of failures authorized before stopping Scraper (if the website is down or if the internet connection is down)


         // FILE PATHS
         public static string RAW_FILE_PATH = "../../../../RAW_WEB_PAGES/"; // Folder wher all raw web page conent are stored (one file per page)
         public static string ALREADY_CRAWLED_FILE_PATH = "../../../../FILES/AlreadyCrawled.csv"; // INPUT file with BSS identifiers of references already crawled
         public static string BSS_FILE_PATH = "../../../../FILES/BSS_ID_20161114.csv"; // INPUT file with BSS identifiers of references to crawl
         public static string ERROR_LOG_FILE_PATH = "../../../../FILES/LOG_ERRORS.txt"; // Log where all errors are written (useful for debugging)
         public static string PROXY_LIST_FILE_PATH = "../../../../FILES/PROXY_LIST.txt";  // List of proxy IPs for hidden scraping
         public static string BROKEN_PROXY_LIST_FILE_PATH = "../../../../FILES/BROKEN_PROXY_LIST.txt";  // List of proxy IPs that didn't work during previous iteration of the scrapper

    }
}
