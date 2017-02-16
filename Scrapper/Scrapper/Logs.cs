using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    // This class deals with all written logs
    static class Logs
    {
        static StreamWriter logFile;
        // This lock is there to avoid conflicts between threads (i.e. CrawlerAgents)
        static Object myLock = new Object();


        public static void Write(string path, string line)
        {
            lock (myLock)
            {
                using (logFile = File.AppendText(path))
                {
                    logFile.WriteLine(line);
                    Console.WriteLine(line);
                }
            }
        }
    }
}