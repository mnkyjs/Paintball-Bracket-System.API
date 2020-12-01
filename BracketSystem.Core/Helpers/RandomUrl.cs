using System;
using System.Collections.Generic;

namespace il_y.BracketSystem.Core.Helpers
{
    /// <summary>
    ///     RandomURL class generates Random URLs for applications.
    /// </summary>
    public static class RandomUrl
    {
        // List of characters and numbers to be used...  
        private static readonly List<int> Numbers = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9, 0};

        private static readonly List<char> Characters = new List<char>
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B',
            'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '-', '_'
        };

        public static string GetUrl()
        {
            var url = "";
            var rand = new Random();
            // run the loop till I get a string of 10 characters  
            for (var i = 0; i < 11; i++)
            {
                // Get random numbers, to get either a character or a number...  
                var random = rand.Next(0, 3);
                if (random == 1)
                {
                    // use a number  
                    random = rand.Next(0, Numbers.Count);
                    url += Numbers[random].ToString();
                }
                else
                {
                    random = rand.Next(0, Characters.Count);
                    url += Characters[random].ToString();
                }
            }

            return url;
        }
    }
}