namespace Miruken.Mediator.Tests
{
    using System;
    using Miruken.Mediator;

    public class StockQuote
    {
        public string  Symbol { get; set; }
        public decimal Value  { get; set; }
    }

    public class GetStockQuote : IRequest<StockQuote>, IEquatable<GetStockQuote>
    {
        public GetStockQuote(string symbol)
        {
            Symbol = symbol;
        }

        public string Symbol { get; set; }

        public bool Equals(GetStockQuote other)
        {
            return Symbol == other?.Symbol;
        }
    }
}