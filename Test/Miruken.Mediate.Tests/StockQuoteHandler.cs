namespace Miruken.Mediate.Tests
{
    using System;
    using Concurrency;
    using Mediate;

    public class StockQuoteHandler : PipelineHandler
    {
        public static int Called;

        private readonly Random random = new Random();

        [Mediates]
        public Promise<StockQuote> GetQuote(GetStockQuote quote)
        {
            ++Called;

            if (quote.Symbol == "EX")
                throw new Exception("Stock Exchange is down");

            return Promise.Resolved(new StockQuote
            {
                Symbol = quote.Symbol,
                Value = Convert.ToDecimal(random.NextDouble() * 10.0)
            });
        }

        [Mediates]
        public Promise SellStock(SellStock sell)
        {
            ++Called;

            if (sell.Symbol == "EX")
                throw new Exception("Stock Exchange is down");

            return Promise.Empty;
        }
    }
}