using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using FGIAFG.Scraper.GOG.Results;
using FGIAFG.Scraper.GOG.Scraping.JsonData;
using FluentResults;
using Newtonsoft.Json;

namespace FGIAFG.Scraper.GOG.Scraping;

internal class GoodOldGamesScraper
{
    private readonly ILogger<GoodOldGamesScraper> logger;
    private readonly IHttpClientFactory httpClientFactory;

    public GoodOldGamesScraper(ILogger<GoodOldGamesScraper> logger, IHttpClientFactory httpClientFactory)
    {
        this.logger = logger;
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<Result<IEnumerable<FreeGame>>> Scrape(CancellationToken ct)
    {
        HttpClient httpClient = httpClientFactory.CreateClient("gog");
        HttpResponseMessage response = await httpClient.GetAsync(string.Empty, ct);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            return Result.Fail(new ExceptionalError(e));
        }

        string body = await response.Content.ReadAsStringAsync(ct);

        Result<string> jsonResult = await GetJson(httpClient.BaseAddress, body, ct);
        if (jsonResult.HasError<NoGiveawayError>())
            return Result.Ok(Array.Empty<FreeGame>().AsEnumerable()).WithReason(new NoGiveawaySuccess());

        if (jsonResult.IsFailed)
            return jsonResult.ToResult();

        GoodOldGamesData? data;

        try
        {
            data = JsonConvert.DeserializeObject<GoodOldGamesData>(jsonResult.Value);
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }

        if (data == null)
            return Result.Fail(new Error("Data is null"));

        List<FreeGame> freeGames = new()
        {
            new FreeGame(data.Title,
                $"https://www.gog.com{data.GameUrl.Replace("/de/", "/en/")}",
                $"https://images-1.gog-statics.com/{data.Logo.Image}.png",
                DateTime.UtcNow,
                DateTimeOffset.FromUnixTimeMilliseconds(data.EndTime).UtcDateTime)
        };

        return Result.Ok(freeGames.AsEnumerable());
    }

    private static async Task<Result<string>> GetJson(Uri baseAddress, string body, CancellationToken ct)
    {
        IBrowsingContext context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        IDocument document = await context.OpenAsync(x => x.Address(baseAddress).Content(body), ct);

        IHtmlAnchorElement giveawayAnchor = document.Body.QuerySelector<IHtmlAnchorElement>(".giveaway-banner");
        if (giveawayAnchor == null)
        {
            return Result.Fail(new NoGiveawayError());
        }

        string onClickContent = giveawayAnchor.GetAttribute("onclick");
        Match match = Regex.Match(onClickContent, "'.+'");
        if (!match.Success)
        {
            return Result.Fail("No onclick found");
        }

        string encodedJson = match.Value.Trim('\'');
        return Result.Ok(Regex.Unescape(encodedJson));
    }
}
