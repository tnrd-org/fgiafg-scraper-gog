﻿using FGIAFG.Scraper.GOG.Database;
using FGIAFG.Scraper.GOG.Scraping;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Quartz;
using DbContext = FGIAFG.Scraper.GOG.Database.DbContext;

namespace FGIAFG.Scraper.GOG.Jobs;

internal class ScrapeAndStoreJob : IJob
{
    private readonly GoodOldGamesScraper scraper;
    private readonly DbContext dbContext;
    private readonly ILogger<ScrapeAndStoreJob> logger;

    public ScrapeAndStoreJob(GoodOldGamesScraper scraper, DbContext dbContext, ILogger<ScrapeAndStoreJob> logger)
    {
        this.scraper = scraper;
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        CancellationToken ct = context.CancellationToken;

        Result<IEnumerable<FreeGame>> result = await scraper.Scrape(ct);
        if (result.IsFailed)
        {
            logger.LogError("Scrape failed. Result: {Result}", result.ToString());
            return;
        }

        if (ct.IsCancellationRequested)
            return;

        foreach (FreeGame freeGame in result.Value)
        {
            if (ct.IsCancellationRequested)
                return;

            string hash = freeGame.CalculatePersistentHash();

            if (await dbContext.Games.AnyAsync(x => x.Hash == hash, ct))
                continue;

            EntityEntry<GameModel> entry = dbContext.Games.Add(new GameModel()
            {
                Title = freeGame.Title,
                EndDate = freeGame.EndDate,
                Url = freeGame.Url,
                ImageUrl = freeGame.ImageUrl,
                StartDate = freeGame.StartDate,
                Hash = hash
            });
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
