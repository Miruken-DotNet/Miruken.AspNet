namespace Miruken.Mediate.Tests
{
    using System;
    using FluentValidation;
    using Miruken.Mediate;

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

    public class GetStockQuoteIntegrity : AbstractValidator<GetStockQuote>
    {
        public GetStockQuoteIntegrity()
        {
            RuleFor(q => q.Symbol).NotEmpty();
        }
    }
}