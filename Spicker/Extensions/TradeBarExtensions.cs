using QuantConnect.Data.Market;
using Spicker.Engines.RuleSet;
using System.Collections.Generic;
using System.Linq;

namespace Spicker.Extensions
{
    public static class TradeBarExtensions
    {
        public static List<HistoricalPricePoint> ToHistoricalPricePoints(this IEnumerable<TradeBar> bars)
        {
            if (bars == null) return new List<HistoricalPricePoint>();

            return bars
                .Where(tb => tb != null)
                .Select(tb => new HistoricalPricePoint
                {
                    Time = tb.Time,
                    Open = tb.Open,
                    High = tb.High,
                    Low = tb.Low,
                    Close = tb.Close,
                    Volume = tb.Volume
                })
                .ToList();
        }

    }
}
