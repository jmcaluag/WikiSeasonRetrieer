using System;

namespace WikiSeasonRetriever
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter Wikipedia URL: " );
            string wikiURL = Console.ReadLine();

            Console.WriteLine(GetWikiUri(wikiURL));
        }

        static string GetWikiUri(string wikiURL)
        {
            wikiURL = wikiURL.Trim();
            string wikiUriFormat = "https://en.wikipedia.org/w/api.php?action=parse&format=json&prop=sections&page=";
            string subDirectory = GetWikiSubdirectory(wikiURL);
            
            return wikiUriFormat + subDirectory;
        }

        static string GetWikiSubdirectory(string wikiURL)
        {
            int indexShift = 5; //Takes account of "wiki/" spaces.

            int number = wikiURL.IndexOf("wiki/") + indexShift;
            
            return wikiURL.Substring(number);
        }

    }
}
