namespace Miruken.Mediate.Tests
{
    using System;
    using Api;
    using FluentValidation;

    public class StockQuote
    {
        public string  Symbol { get; set; }
        public decimal Value  { get; set; }
    }

    public class GetStockQuote : IRequest<StockQuote>, IEquatable<GetStockQuote>
    {
        public GetStockQuote()
        {           
        }

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

    public class SellStock
    {
        public SellStock()
        {
            
        }

        public SellStock(string symbol, int numShares)
        {
            Symbol       = symbol;
            NumberShares = numShares;
        }

        public string Symbol       { get; set; }
        public int    NumberShares { get; set; }
    }

    public class GetStockQuoteIntegrity : AbstractValidator<GetStockQuote>
    {
        public GetStockQuoteIntegrity()
        {
            RuleFor(q => q.Symbol).NotEmpty();
        }
    }
}