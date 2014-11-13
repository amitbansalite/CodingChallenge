using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace CodingChallenge
{
    public class Response
    {
        private String Url;
        private Regex regex;
        private int countCalls;
        private int totalSumResponseTime;
        private SortedDictionary<int, int> modes;
        private Dictionary<String, int> dynos = new Dictionary<string, int>();

        public Response(String url)
        {
            Url = url;
            modes = new SortedDictionary<int, int>();
            dynos = new Dictionary<string, int>();
        }

        public Response(String url, String regexPattern)
            : this(url)
        {
            regex = new Regex(regexPattern);
        }

        public bool AnalyseLog(String text, int lineNumber)
        {
            try
            {
                if ((regex != null && regex.Match(text).Success) || text.Contains(Url))
                {
                    countCalls++;
                    var content = text.Split(' ');

                    var connect = content[8].Split('=')[1];
                    var connectTime = connect.Remove(connect.Length - 2);
                    var service = content[9].Split('=')[1];
                    var serviceTime = service.Remove(service.Length - 2);

                    var responseTime = Convert.ToInt32(connectTime) + Convert.ToInt32(serviceTime);
                    totalSumResponseTime += responseTime;

                    int currentModeCount;
                    modes.TryGetValue(responseTime, out currentModeCount);
                    modes[responseTime] = currentModeCount + 1;

                    int currentDynoCount;
                    var currentDyno = content[7].Split('=')[1];
                    dynos.TryGetValue(currentDyno, out currentDynoCount);
                    dynos[currentDyno] = currentDynoCount + 1;

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error encountered while parsing the log file. The format at line number {0} is incorrect : ", lineNumber);
            }
            return false;
        }

        public string GetMostFrequentDyno()
        {
            if (dynos.Any())
            {
                KeyValuePair<string, int> mostOccuring = dynos.First();
                foreach (KeyValuePair<string, int> dyno in dynos)
                {
                    if (dyno.Value > mostOccuring.Value)
                        mostOccuring = dyno;
                }
                return mostOccuring.Key;
            }
            return "None";
        }

        public int GetMode()
        {
            if (modes.Any())
                return modes.Keys.Last();
            else
                return 0;
        }

        public int GetMean()
        {
            if (countCalls > 0)
                return totalSumResponseTime / countCalls;
            else
                return 0;
        }

        // The SortedDictionary stores the frequency distribution. 
        // To find the median, the group(key) which contains the median is being searched and returned
        public int GetMedian()
        {
            if (modes.Any())
            {
                var position = modes.Count/2;
                var elements = 0;
                foreach (KeyValuePair<int, int> elem in modes)
                {
                    elements += elem.Value;
                    if (elements >= position)
                        return elem.Key;                    
                }                
            }

            return 0;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append(String.Format("\n Url = {0} ", Url));
            str.Append(String.Format("\n Number of times accessed     = {0}", countCalls));
            str.Append(String.Format("\n Mean   response time         = {0}ms ", GetMean()));
            str.Append(String.Format("\n Median response time         = {0}ms ", GetMedian()));
            str.Append(String.Format("\n Mode   response time         = {0}ms ", GetMode()));
            str.Append(String.Format("\n dyno that responded the most = {0} ", GetMostFrequentDyno()));
            return str.ToString();
        }
    }
}
