using System.Collections.Generic;
using System.Text.Json.Serialization;

class WikiSection
{
    [JsonPropertyName("parse")]
    public SectionParse SectionParse { get; set; }
}

class SectionParse
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("pageid")]
    public int PageId { get; set; }
    
    [JsonPropertyName("sections")]
    public List<Section> Sections { get; set; }
}

 public class Section
{
    [JsonPropertyName("toclevel")]
    public int TocLevel { get; set; }
    
    [JsonPropertyName("level")]
    public string Level { get; set; }
    
    [JsonPropertyName("line")]
    public string Line { get; set; }
    
    [JsonPropertyName("number")]
    public string Number { get; set; }
    
    [JsonPropertyName("index")]
    public string Index { get; set; }
    
    [JsonPropertyName("fromtitle")]
    public string fromtitle { get; set; }
    
    [JsonPropertyName("byteoffset")]
    public int ByteOffSet { get; set; }
    
    [JsonPropertyName("anchor")]
    public string Anchor { get; set; }
}