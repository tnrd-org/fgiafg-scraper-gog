using Newtonsoft.Json;

namespace FGIAFG.Scraper.GOG.Scraping.JsonData
{
    internal class GoodOldGamesData
    {
        [JsonProperty("id")] public Guid Id { get; set; }
        [JsonProperty("endTime")] public long EndTime { get; set; }
        [JsonProperty("background")] public string Background { get; set; }
        [JsonProperty("mobileBackground")] public string MobileBackground { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("logo")] public Logo Logo { get; set; }
        [JsonProperty("gameUrl")] public string GameUrl { get; set; }
        [JsonProperty("backgroundColour")] public string BackgroundColour { get; set; }
    }
}
