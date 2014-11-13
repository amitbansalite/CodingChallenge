using System;
using System.IO;
using System.Collections.Generic;

namespace CodingChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            // Validations
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid command line arguments. Please enter one one argument which is the path to the log file.");
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }

            var fileName = args[0];
            if (File.Exists(fileName) == false)
            {
                Console.WriteLine("File cannot be found at the specified location. Please enter a valid path to the log file");
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }

            // initial setup
            var responses = new List<Response>();
            responses.Add(new Response("GET /api/users/{user_id}", @"GET.*users\/\d+ "));
            responses.Add(new Response("POST /api/users/{user_id}", @"POST.*users\/\d+ "));
            responses.Add(new Response("count_pending_messages"));
            responses.Add(new Response("get_messages"));
            responses.Add(new Response("get_friends_progress"));
            responses.Add(new Response("get_friends_score"));

            // parsing the file, line by line and saving the analysis
            var lineNumber = 0;
            using (StreamReader reader = File.OpenText(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    lineNumber++;
                    for (int x = 0; x < responses.Count; x++)
                    {
                        var found = responses[x].AnalyseLog(line, lineNumber);
                        if (found)
                            break;
                    }
                }
            }

            foreach (var result in responses)
            {
                Console.WriteLine(result.ToString());
                Console.WriteLine();
            }

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
