﻿using System;
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

            PrintLogo();

            Console.Write("Is there ONLY ONE season in the series page? [Y/N]: ");
            char oneSeason = Convert.ToChar(Console.ReadLine());

            Console.Write("Enter Wikipedia URL: " );
            string wikiURL = Console.ReadLine();

            string wikiURI = GetWikiUri(wikiURL);
            WikiSection retrievedSectionJson = await GetWikiSectionJson(wikiURI);
            
            List<Section> sectionSeason = retrievedSectionJson.SectionParse.Sections;

            //This if..else can be turned into a method
            if(CheckSingleOrMultiSeason(sectionSeason))
            {
                Console.WriteLine("Single season page");
                //Find index of "line": "Episode list", use index to access section and get season wikitext.

                int episodeListIndex = GetEpisodeListIndex(sectionSeason);

                string wikiSectionURI = GetSeasonSection(GetWikiSubdirectory(wikiURL), episodeListIndex);

                WikiTextSeason seasonJson = await GetSeasonJson(wikiSectionURI);

                Console.WriteLine("Wiki Text Season: {0}", seasonJson.SeasonParse.SeasonWikitext.Content);
            }
            else
            {
                ShowIndexAndSeason(sectionSeason);

                Console.Write("Enter index of chosen season: ");
                int indexOfSeason = Convert.ToInt32(Console.ReadLine());

                string wikiSectionURI = GetSeasonSection(GetWikiSubdirectory(wikiURL), indexOfSeason);

                WikiTextSeason seasonJson = await GetSeasonJson(wikiSectionURI);

                Console.WriteLine("Wiki Text Season: {0}", seasonJson.SeasonParse.SeasonWikitext.Content);
            }
                                    
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
            
            return wikiURL.Substring(number);
        }

        private static async Task<WikiSection> GetWikiSectionJson(string wikiURI)
        {
            string response = await client.GetStringAsync(wikiURI);
            WikiSection wikiSectionJson = JsonSerializer.Deserialize<WikiSection>(response);

            return wikiSectionJson;
        }

        private static Boolean CheckSingleOrMultiSeason(List<Section> wikiSections)
        {
            //Checks whether the page contains a single season of all seasons of the series

            int sectionSize = wikiSections.Count;
            int indexPos = 0;

            while(indexPos < sectionSize)
            {
                Section section = wikiSections[indexPos];
                
                switch(section.Line)
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
        private static int GetEpisodeListIndex(List<Section> wikiSections)
        {
            int sectionSize = wikiSections.Count;
            int indexPos = 0;

            while(indexPos < sectionSize)
            {
                Section section = wikiSections[indexPos];
                
                if(section.Line.Equals("Episode list"))
                {
                    return Convert.ToInt32(section.Index);
                }
                
                indexPos++;
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

        private static string GetSeasonSection(string subDirectory, int indexSection)
        {
            string selectedSectionURI = String.Format("https://en.wikipedia.org/w/api.php?action=parse&format=json&page={0}&prop=wikitext&section={1}", subDirectory, indexSection);

            return selectedSectionURI;
        }

        private static async Task<WikiTextSeason> GetSeasonJson(string wikiURI)
        {
            string response = await client.GetStringAsync(wikiURI);
            WikiTextSeason wikiSeason = JsonSerializer.Deserialize<WikiTextSeason>(response);

            return wikiSeason;
        }
    }
}