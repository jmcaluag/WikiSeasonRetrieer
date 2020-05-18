using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace WikiSeasonRetriever
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static List<Section> wikipediaPageSections;
        private static string subDirectory;
        private static List<Episode> seasonList = new List<Episode>();

        static async Task Main(string[] args)
        {

            PrintLogo();

            Console.Write("Enter Wikipedia URL: " );
            string wikiURL = Console.ReadLine();

            Console.Write("Is there ONLY ONE season? [Y/N]: ");
            char oneSeason = Char.ToUpper(Convert.ToChar(Console.ReadLine()));

            //Creates the REST API URI
            string wikiURI = GetWikiURI(wikiURL);

            string wikitextContentOfSeasonSection = await GetWikitextSeason(wikiURI, oneSeason);

            if(CheckForSeasonPage(wikitextContentOfSeasonSection))
            {
                string seasonPageURL = CreateSeasonPageURL(wikitextContentOfSeasonSection);
                string seasonPageURI = GetWikiURI(seasonPageURL);

                wikitextContentOfSeasonSection = await GetWikitextSeason(seasonPageURI, 'Y');
            }

            //Parsing and Scraping for Episode Details
            StartParser(wikitextContentOfSeasonSection);

            //Test
            Console.WriteLine("\n---------------");
            Console.WriteLine("Test");
            Console.Write("Enter an episode number: ");
            int episodeNumber = Convert.ToInt32(Console.ReadLine()) - 1;

            Console.WriteLine();

            Console.WriteLine("Season: {0}", seasonList[episodeNumber].season);
            Console.WriteLine("Series Number: {0}", seasonList[episodeNumber].episodeNumberOverall);
            Console.WriteLine("Season Episode Number: {0}", seasonList[episodeNumber].episodeNumberInSeason);
            Console.WriteLine("Title: {0}", seasonList[episodeNumber].title);
            if(seasonList[episodeNumber].titleRomaji != null)
            {
                Console.WriteLine("Title Romaji: {0}", seasonList[episodeNumber].titleRomaji);
            }
            if(seasonList[episodeNumber].titleKanji != null)
            {
                Console.WriteLine("Title Kanji: {0}", seasonList[episodeNumber].titleKanji);
            }
            Console.WriteLine("Date: {0}", seasonList[episodeNumber].originalAirDate.GetDateTimeFormats('d'));
        }

        private static void PrintLogo()
        {
            string retrieverLogo = "\n\n\n                  -.`  -sys+:`                                                 \n                  hMMMNNMMMMMMMd/                                               \n                  yMMMMMMMMMMMMMMh                                              \n                 -yNMMMMMMMMMMMMMMs                                             \n                 -dMMMMMMMMMMMMMMMN.                                            \n                   `-dMMMMMMMMMMMMN-                                            \n                     yMMMMMMMMMMMM.                                             \n                     `NMMMMMMMMMMMs:                                            \n                      yMMMMMMMMMMMMMd+`                                         \n                      hMMMMMMMMMMMMMMMMy:                                       \n                     oMMMMMMMMMMMMMMMMMMMd:                                     \n                     yMMMMMMMMMMMMMMMMMMMMMo                                    \n                     :MMMMMMMMMMMMMMMMMMMMMMy`                                  \n                      hMMMMMMMMMMMMMMMMMMMMMMm`                                 \n                      .MMMMMMMMMMMMMMMMMMMMMMMm-                                \n                       MMMMMMMMMMMMMMMMMMMMMMMMN+                               \n                      `MMMMMMMMMMMMMMMMMMMMMMMMMM.                              \n                      yMMMMMMMMMMMMMMMMMMMMMMMMMMs                              \n                      NMMMMo/MMMMMMMMMMMMMMMMMMMMy                              \n                     /MMMMNsdMMMMMMMMMMMMMMMMMMMMm/.`                           \n                  :ydMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNNmdhs:                  \n                 `syyyso+/::::::///++oooo++////:://///++osssss:                 \n\n\n";

            Console.WriteLine("Golden Wiki Season Retriever");
            Console.WriteLine(retrieverLogo);
        }
        
        //Work in Progress:
        private static async Task<string> GetWikitextSeason(string wikiURI, char oneSeason)
        {
            WikiSection retrievedSectionJSON = await GetWikiSectionJSON(wikiURI);

            wikipediaPageSections = retrievedSectionJSON.SectionParse.Sections;

            string wikipediaEpisodeListURI = RetrieveSeriesSeason(oneSeason);
            WikitextSeason seasonSection = await GetSeasonSectionAsJSON(wikipediaEpisodeListURI);

            string wikitextContentOfSeasonSection = seasonSection.SeasonParse.SeasonWikitext.Content;

            return wikitextContentOfSeasonSection;
        }

        private static string GetWikiURI(string wikiURL)
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

        private static async Task<WikiSection> GetWikiSectionJSON(string wikiURI)
        {
            string response = await client.GetStringAsync(wikiURI);
            WikiSection wikiSectionJson = JsonSerializer.Deserialize<WikiSection>(response);

            return wikiSectionJson;
        }

        private static string RetrieveSeriesSeason(char oneSeason)
        {
            string episodeListSectionURI = null;

            switch(oneSeason)
            {
                case 'Y':
                    //Find index number of "Episode list" or "Episodes"
                    int episodeListIndexPosition = GetEpisodesIndex();

                    //Uses index number to get section of the one season.
                    episodeListSectionURI = CreateEpisodeListSectionURI(episodeListIndexPosition);

                    break;

                case 'N':
                    ShowIndexAndSeason();
                    Console.Write("\nEnter INDEX of season:");
                    int chosenIndexNumber = Convert.ToInt32(Console.ReadLine());

                    episodeListSectionURI = CreateEpisodeListSectionURI(chosenIndexNumber);

                    break;
            }

            return episodeListSectionURI;
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
        private static void ShowIndexAndSeason()
        {
            int sectionSize = wikipediaPageSections.Count;
            int indexPos = 0;

            Regex regexPattern = new Regex(@"Season \d[\W]+");

            while(indexPos < sectionSize)
            {
                Section section = wikipediaPageSections[indexPos];

                if(regexPattern.IsMatch(section.Line))
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

        private static Boolean CheckForSeasonPage(string contentOfSeasonSection)
        {
            Regex regexPattern = new Regex(@"{:[\w\s()]+");
            Boolean seasonPageExist = false;

            if(regexPattern.IsMatch(contentOfSeasonSection))
            {
                seasonPageExist = true;
            }

            return seasonPageExist;
        }

        private static string CreateSeasonPageURL(string contentOfSeasonSection)
        {
            Regex regexPattern = new Regex(@"{:[\w\s()]+");

            string wikipediaURL = "https://en.wikipedia.org/wiki/";

            string seasonPageURL = null;

            if(regexPattern.IsMatch(contentOfSeasonSection))
            {
                string seasonPageSubdirectory = regexPattern.Match(contentOfSeasonSection).Value.Substring(2);

                seasonPageURL = wikipediaURL + seasonPageSubdirectory.Trim().Replace(' ', '_');
            }

            return seasonPageURL;

        }

    //Wikitext scraper and parser functionality
        private static void StartParser(string wikitextContentOfSeasonSection)
        {
            using (StringReader reader = new StringReader(wikitextContentOfSeasonSection))
            {
                Boolean collectStatus = false; //Ignores all lines until 

                while(reader.Peek() > -1)
                {
                    string currentLine = reader.ReadLine();

                    if(FindEpisode(currentLine))
                    {
                        seasonList.Add(new Episode());
                        collectStatus = true;
                    }
                    else if (FindEndOfEpisode(currentLine))
                    {
                        collectStatus = false;
                    }
                    else if (collectStatus)
                    {
                        if(CheckEpisodeDetail(currentLine))
                        {
                            //Ignores <hr> tags and other non-episode details under the collectStatus of "true".
                            CollectEpisodeDetails(seasonList[seasonList.Count - 1], currentLine);
                        }

                    }
                }
            }

            Console.WriteLine("Season list size: {0}", seasonList.Count);
        }

        //Finds an episode block
        private static Boolean FindEpisode(string wikitemplateLine)
        {
            Boolean episodeTemplate = wikitemplateLine.Contains("{{Episode list");

            return episodeTemplate;
        }

        //Finds the end of an episode block
        private static Boolean FindEndOfEpisode(string wikitemplateLine)
        {
            Boolean episodeTemplate = wikitemplateLine.Equals("}}");

            return episodeTemplate;
        }

        //Checks each line if it 
        private static Boolean CheckEpisodeDetail(string wikitemplateLine)
        {
            string readerLine = wikitemplateLine.Trim();
            Boolean validEpisodeDetail = readerLine[0].Equals('|');

            return validEpisodeDetail;
        }

        private static void CollectEpisodeDetails(Episode episode, string readerLine)
        {
            string[] partialLines = readerLine.Split('=');

            string episodeKey = ParseWikitext(partialLines[0]);
            string episodeValue = ParseWikitext(partialLines[1]);

            AssignValueToEpisode(episode, episodeKey, episodeValue);
        }

        private static string ParseWikitext(string readerLine)
        {
            string partialWikitext = readerLine.Trim();

            if(partialWikitext.Length == 0)
            {
                return null;
            }
            else if(partialWikitext[0].Equals('|')) //Looks for Episode value substring.
            {
                return readerLine.Substring(2).Trim();
            }
            else if ((partialWikitext.Contains("{{") && partialWikitext.Contains("}}")) || (partialWikitext.Contains("[[") && partialWikitext.Contains("]]")))
            {
                string wikitextValue = "";
                Regex pattern = new Regex(@"(?<=\[\[|\{\{).*?(?=\]\]|\}\})");

                if(partialWikitext.Contains("[[") && !partialWikitext.Contains("Start date"))
                {
                    wikitextValue = pattern.Match(partialWikitext).Value.Trim();

                    return ParseWikiLink(wikitextValue);
                }
                else
                {
                    wikitextValue = pattern.Match(partialWikitext).Value.Trim();
                    return wikitextValue;
                }

            }
            else
            {
                return readerLine.Trim();
            }
        }

        private static string ParseWikiLink(string readerLine)
        {
            string linkLabel = "";

            if(readerLine.Contains('|'))
            {
                string[] partialDetails = readerLine.Split('|');
                linkLabel = partialDetails[1];

                return linkLabel;
            }
            else
            {
                return linkLabel;
            }
        }

        private static void AssignValueToEpisode(Episode episode, string episodeKey, string episodeValue)
        {
            switch (episodeKey)
            {
                case "1":
                    episode.season = episodeValue;
                    break;
                case "EpisodeNumber":
                    episode.episodeNumberOverall = Convert.ToSingle(episodeValue);
                    break;
                case "EpisodeNumber2":
                    episode.episodeNumberInSeason = Convert.ToSingle(episodeValue);
                    break;
                case "Title":
                    episode.title = episodeValue;
                    break;
                case "TranslitTitle":
                    episode.titleRomaji = episodeValue;
                    break;
                case "NativeTitle":
                    episode.titleKanji = episodeValue;
                    break;
                case "OriginalAirDate":
                    episode.originalAirDate = ParseWikiDate(episodeValue);
                    break;
                default:
                    break;
            }
        }

        private static DateTime ParseWikiDate(string episodeValue) //Value being passed in has to be trimmed
        {
            //Format in the form of: Start date|2016|4|10
            string[] dateValues = episodeValue.Split("|");

            return new DateTime(Convert.ToInt32(dateValues[1]), Convert.ToInt32(dateValues[2]), Convert.ToInt32(dateValues[3]));
        }

    //End of Wikitext scraper and parser functionality
    }
}