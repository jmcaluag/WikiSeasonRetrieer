using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WikiSeasonRetriever
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {

            Console.Write("Is there ONLY ONE season in the series page? [Y/N]: ");
            char oneSeason = Convert.ToChar(Console.ReadLine());

            Console.Write("Enter Wikipedia URL: " );
            string wikiURL = Console.ReadLine();

            string wikiURI = GetWikiUri(wikiURL);
            WikiSection retrievedJson = await RetrieveWikiJSON(wikiURI);
            
            List<Section> sectionSeason = retrievedJson.parse.sections;

            if(CheckSingleOrMultiSeason(sectionSeason))
            {
                Console.WriteLine("Single season page");
            }
            else
            {
                ShowIndexAndSeason(sectionSeason);

                Console.Write("Enter index of chosen season: ");
                int indexOfSeason = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Wiki URI: {0}", GetSeasonSection(GetWikiSubdirectory(wikiURL), indexOfSeason));
            }
                                    
        }

        private static string GetWikiUri(string wikiURL)
        {
            wikiURL = wikiURL.Trim();
            string wikiUriFormat = "https://en.wikipedia.org/w/api.php?action=parse&format=json&prop=sections&page=";
            string subDirectory = GetWikiSubdirectory(wikiURL);
            
            return wikiUriFormat + subDirectory;
        }

        private static string GetWikiSubdirectory(string wikiURL)
        {
            int indexShift = 5; //Takes account of "wiki/" spaces.

            int number = wikiURL.IndexOf("wiki/") + indexShift;
            
            return wikiURL.Substring(number);
        }

        private static async Task<WikiSection> RetrieveWikiJSON(string wikiURI)
        {
            string response = await client.GetStringAsync(wikiURI);
            WikiSection wikiJsonSection = JsonSerializer.Deserialize<WikiSection>(response);

            return wikiJsonSection;
        }

        private static Boolean CheckSingleOrMultiSeason(List<Section> wikiSections)
        {
            //Checks whether the page contains a single season of all seasons of the series

            int sectionSize = wikiSections.Count;
            int indexPos = 0;

            while(indexPos < sectionSize)
            {
                Section section = wikiSections[indexPos];
                
                switch(section.line)
                {
                    case "Episode list":
                        return true;
                    case "Episodes":
                        return false;
                }
                
                indexPos++;
            }

            return false;
        }

        //Single-season page methods
        

        //End of single-season page methods

        //Multi-season page methods

        private static void ShowIndexAndSeason(List<Section> wikiSections)
        {
            int sectionSize = wikiSections.Count;
            int indexPos = 0;

            Regex rgxPattern = new Regex(@"Season \d[\W]+");

            while(indexPos < sectionSize)
            {
                Section section = wikiSections[indexPos];

                if(rgxPattern.IsMatch(section.line))
                {
                    Console.WriteLine("Index: {0} - {1}", section.index, section.line);
                }
                
                indexPos++;
            }
        }

        private static string GetSeasonSection(string subDirectory, int indexSection)
        {
            string selectedSectionUri = String.Format("https://en.wikipedia.org/w/api.php?action=parse&format=json&page={0}&prop=wikitext&section={1}", subDirectory, indexSection);

            return selectedSectionUri;
        }

        //End of multi-season page methods

    }
}