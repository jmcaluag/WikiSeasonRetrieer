using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;

namespace WikiSeasonRetriever
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.Write("Enter Wikipedia URL: " );
            string wikiURL = Console.ReadLine();

            string wikiURI = GetWikiUri(wikiURL);
            WikiSection retrievedJson = await RetrieveWikiJSON(wikiURI);
            
            List<Section> sectionSeason = retrievedJson.parse.sections;

            Console.WriteLine("Index Number: {0}", GetEpisodeSectionIndex(sectionSeason));
                        
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

        private static int GetEpisodeSectionIndex(List<Section> wikiSections)
        {
            //Checks whether the page contains a single season of all seasons of the series

            int sectionSize = wikiSections.Count;
            int indexPos = 0;

            while(indexPos < sectionSize)
            {
                Section section = wikiSections[indexPos];

                if(section.line.Equals("Episode list"))
                {
                    Console.WriteLine("Single season page");
                    return Convert.ToInt32(section.index);
                }
                else if(section.line.Equals("Episodes"))
                {
                    Console.WriteLine("Multi season page");
                    return Convert.ToInt32(section.index);
                }
                
                indexPos++;
            }

            return 0;
        }

    }
}