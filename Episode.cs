using System;
class Episode
{
    public string season { get; set; }
    public float episodeNumberOverall { get; set; } //EpisodeNumber
    public float episodeNumberInSeason { get; set; } //EpisodeNumber2
    public string title { get; set; }
    public string titleRomaji { get; set; }
    public string titleKanji { get; set; }
    public DateTime originalAirDate { get; set; }

    public Episode()
    {
        //Null valued
        this.titleRomaji = null;
        this.titleKanji = null;
    }

}