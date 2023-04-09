using FluentResults;

namespace FGIAFG.Scraper.GOG.Results
{
    public class NoGiveawaySuccess : Success
    {
        public NoGiveawaySuccess() : base("No giveaway has been found, this could be a false-positive")
        {

        }
    }
}
