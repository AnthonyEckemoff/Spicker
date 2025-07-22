// File: IExtendedHistoricalDataProvider.cs
using QuantConnect;
using QuantConnect.Data.Market;
using Spicker.Engines.RuleSet;

namespace Spicker.DataProviders
{
    public interface IExtendedHistoricalDataProvider : IHistoricalDataProvider
    {
        Task<IEnumerable<TradeBar>> GetHistoricalDataAsync(string ticker, DateTime start, DateTime end, Resolution resolution = Resolution.Minute);
    }
}
