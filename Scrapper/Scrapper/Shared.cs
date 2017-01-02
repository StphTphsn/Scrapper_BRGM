using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapper
{
    // This class contains all the parameters and file paths useful for the program to run
     static class Shared
    {
         // CRAWLING PARAMETERS
         public static int NUMBER_OF_CRAWLER_AGENTS = 1; // Number of crawling agents (the more the faster but beware of CPU saturation and website protections)
         public static bool USE_PROXY = false; //  Proxies are usefull to bypass some websites protections against scrapping (by detection of frequent requests from same IP)
         public static int ONE_CYCLE_TIME_MIN = 5000; // minimal time between two missions of the same crawler in milliseconds (avoid saturating website bandpass or being detected as a scrapper bot by the website)
         public static int RECEPTION_TIMEOUT = 60000; // maximimum reception time (avoid loosing to much time with a slow proxy)
         public static int NUMBER_OF_SUCCESSIVE_FAILURES_TOLERATED_FOR_ONE_PROXY = 3; // number of failures authorized before changing proxy (avoid staying stuck with a broken/malfunctionning/slow proxy)
         public static int NUMBER_OF_SUCCESSIVE_FAILURES_TOLERATED_FOR_ALL_PROXIES = 30; // number of failures authorized before stopping Scrapper (if the website is down or if the internet connection is down)


         // FILE PATHS
         public static string RAW_FILE_PATH = "../../../../RAW_WEB_PAGES/";
         public static string ALREADY_CRAWLED_FILE_PATH = "../../../../AlreadyCrawled.csv"; // INPUT file with BSS identifiers of references already crawled
         public static string BSS_FILE_PATH = "../../../../BSS_ID_20161114.csv"; // INPUT file with BSS identifiers of references to crawl
         public static string ERROR_LOG_FILE_PATH = "../../../../LOG_ERRORS.txt"; 


    }
}
