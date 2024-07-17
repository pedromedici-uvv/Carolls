﻿using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        Console.WriteLine(" --> Consuming auction deleting: " + context.Message.Id);

        var result = await DB.DeleteAsync<Item>(context.Message.Id);

        if(!result.IsAcknowledged) throw new ArgumentException("Could not delete auction");

    }


}
