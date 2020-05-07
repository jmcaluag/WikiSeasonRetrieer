using System.Collections.Generic;

class WikiSection
{
    public Parse parse { get; set; }
}

class Parse
{
    public string title { get; set; }
    public int pageid { get; set; }
    public List<Section> sections { get; set; }
}

 public class Section
{
    public int toclevel { get; set; }
    public string level { get; set; }
    public string line { get; set; }
    public string number { get; set; }
    public string index { get; set; }
    public string fromtitle { get; set; }
    public int byteoffset { get; set; }
    public string anchor { get; set; }
}