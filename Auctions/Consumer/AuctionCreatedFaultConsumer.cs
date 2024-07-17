using Contracts;
using MassTransit;

namespace AuctionService.Consumer
{
    public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
    {
        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {
            Console.WriteLine("Consuming faulty creation");

            var exception = context.Message.Exceptions.First();

            if(exception.ExceptionType == typeof(ArgumentException).FullName)
            {
                context.Message.Message.Model = "FooBar";
                await context.Publish(context.Message.Message);
            }else
            {
                await Console.Out.WriteLineAsync("Not an argument exception - update eeor dashboard somewhere");
            }
        }
    }
}
