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

        private static List<Section> wikipediaPageSections;

        private static string subDirectory;

        static async Task Main(string[] args)
        {

            PrintLogo();

            Console.Write("Enter Wikipedia URL: " );
            string wikiURL = Console.ReadLine();

            Console.Write("Is there ONLY ONE season? [Y/N]: ");
            char oneSeason = Char.ToUpper(Convert.ToChar(Console.ReadLine()));

            //Creates the REST API URI
            string wikiURI = GetWikiUri(wikiURL);

            //Retrieves the sections on a Wikipedia page as a JSON object.
            WikiSection retrievedSectionJson = await GetWikiSectionJson(wikiURI);

            //Gets the sections and turns it into a C# list.
            wikipediaPageSections = retrievedSectionJson.SectionParse.Sections;

            //Either finds the Episodes sections of lists out the season and their index
            string wikipediaEpisodeListURI = RetrieveSeriesSeason(oneSeason);
            WikitextSeason seasonSection = await GetSeasonSectionAsJSON(wikipediaEpisodeListURI);

            string contentOfSeasonSection = seasonSection.SeasonParse.SeasonWikitext.Content;

            Console.WriteLine(contentOfSeasonSection);
                                    
        }

        private static void PrintLogo()
        {
            string retrieverLogo = "\n\n\n                  -.`  -sys+:`                                                 \n                  hMMMNNMMMMMMMd/                                               \n                  yMMMMMMMMMMMMMMh                                              \n                 -yNMMMMMMMMMMMMMMs                                             \n                 -dMMMMMMMMMMMMMMMN.                                            \n                   `-dMMMMMMMMMMMMN-                                            \n                     yMMMMMMMMMMMM.                                             \n                     `NMMMMMMMMMMMs:                                            \n                      yMMMMMMMMMMMMMd+`                                         \n                      hMMMMMMMMMMMMMMMMy:                                       \n                     oMMMMMMMMMMMMMMMMMMMd:                                     \n                     yMMMMMMMMMMMMMMMMMMMMMo                                    \n                     :MMMMMMMMMMMMMMMMMMMMMMy`                                  \n                      hMMMMMMMMMMMMMMMMMMMMMMm`                                 \n                      .MMMMMMMMMMMMMMMMMMMMMMMm-                                \n                       MMMMMMMMMMMMMMMMMMMMMMMMN+                               \n                      `MMMMMMMMMMMMMMMMMMMMMMMMMM.                              \n                      yMMMMMMMMMMMMMMMMMMMMMMMMMMs                              \n                      NMMMMo/MMMMMMMMMMMMMMMMMMMMy                              \n                     /MMMMNsdMMMMMMMMMMMMMMMMMMMMm/.`                           \n                  :ydMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNNmdhs:                  \n                 `syyyso+/::::::///++oooo++////:://///++osssss:                 \n\n\n";

            Console.WriteLine("Golden Wiki Season Retriever");
            Console.WriteLine(retrieverLogo);
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
            
            subDirectory = wikiURL.Substring(number);
            
            return subDirectory;
        }

        private static async Task<WikiSection> GetWikiSectionJson(string wikiURI)
        {
            string response = await client.GetStringAsync(wikiURI);
            WikiSection wikiSectionJson = JsonSerializer.Deserialize<WikiSection>(response);

            return wikiSectionJson;
        }

        private static string RetrieveSeriesSeason(char oneSeason)
        {
            switch(oneSeason)
            {
                case 'Y':
                    //Find index number of "Episode list" or "Episodes"
                    int episodeListIndexPosition = GetEpisodesIndex();

                    //Uses index number to get section of the one season.
                    string episodeListSectionURI = CreateEpisodeListSectionURI(episodeListIndexPosition);

                    return episodeListSectionURI;

                case 'N':
                    return "Placeholder";
            }

            return "Won't reach here";
        }
    
    //Single-season page methods
        private static int GetEpisodesIndex()
        {
            int numberOfSections = wikipediaPageSections.Count;
            int indexPosition = 0;

            while(indexPosition < numberOfSections)
            {
                Section section = wikipediaPageSections[indexPosition];

                if(section.Line.Equals("Episode list") || section.Line.Equals("Episodes"))
                {
                    return Convert.ToInt32(section.Index);
                }

                indexPosition++;
            }

            return 0;
        }
        
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

                if(rgxPattern.IsMatch(section.Line))
                {
                    Console.WriteLine("Index: {0} - {1}", section.Index, section.Line);
                }
                
                indexPos++;
            }
        }
    //End of multi-season page methods

        private static string CreateEpisodeListSectionURI(int indexSection)
        {
            string selectedSectionURI = String.Format("https://en.wikipedia.org/w/api.php?action=parse&format=json&page={0}&prop=wikitext&section={1}", subDirectory, indexSection);

            return selectedSectionURI;
        }

        private static async Task<WikitextSeason> GetSeasonSectionAsJSON(string wikiURI)
        {
            string response = await client.GetStringAsync(wikiURI);
            WikitextSeason wikiSeason = JsonSerializer.Deserialize<WikitextSeason>(response);

            return wikiSeason;
        }
    }
}