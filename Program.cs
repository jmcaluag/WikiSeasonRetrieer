using System;

namespace WikiSeasonRetriever
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter Wikipedia URL: " );
            string wikiURL = Console.ReadLine().Trim();

            string pageAttribute = GetWikiSubdirectory(wikiURL);

            Console.WriteLine(pageAttribute);
        }

        static string GetWikiSubdirectory(string wikiURL)
        {
            int indexShift = 5; //Takes account of "wiki/" spaces.

            int number = wikiURL.IndexOf("wiki/") + indexShift;
            
            return wikiURL.Substring(number);
        }
    }
}
