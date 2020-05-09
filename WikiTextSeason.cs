using System.Text.Json.Serialization;

class WikiTextSeason
{
    [JsonPropertyName("parse")]
    public SeasonParse SeasonParse { get; set; }
}

class SeasonParse
{
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("pageid")]
        public long Pageid { get; set; }

        [JsonPropertyName("wikitext")]
        public SeasonWikitext SeasonWikitext { get; set; }
}

class SeasonWikitext
{
    [JsonPropertyName("*")]
    public string Content { get; set; }
}