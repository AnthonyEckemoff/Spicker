using NodaTime;
using QuantConnect;
using QuantConnect.Configuration;
using QuantConnect.Data;
using QuantConnect.Data;
using QuantConnect.Data.Auxiliary;
using QuantConnect.Data.Auxiliary;
using QuantConnect.Data.Market;
using QuantConnect.Data.Market;
using QuantConnect.Interfaces;
using QuantConnect.Lean.Engine.DataFeeds;
using QuantConnect.Lean.Engine.HistoricalData;
using QuantConnect.Lean.Engine.Storage;
using QuantConnect.Securities;
using QuantConnect.Securities;
using QuantConnect.Securities.Equity;
using static QuantConnect.Messages;
using Market = QuantConnect.Market;
using SecurityExchangeHours = QuantConnect.Securities.SecurityExchangeHours;
using SecurityManager = QuantConnect.Securities.SecurityManager;
using Symbol = QuantConnect.Symbol;

namespace Spicker.DataProviders
{
    public class LeanHistoricalDataProvider : IExtendedHistoricalDataProvider
    {
        private readonly HistoryProviderManager _historyProviderManager;
        private readonly SecurityManager _securityManager;
        private readonly string _market = Market.USA;

        public LeanHistoricalDataProvider()
        {
            _securityManager = new SecurityManager(new TimeKeeper(DateTime.UtcNow));
            _historyProviderManager = new HistoryProviderManager();

            var dataProvider = new DefaultDataProvider();


            var mapFileProvider = new LocalDiskMapFileProvider();
            mapFileProvider.Initialize(dataProvider);

            var factorFileProvider = new LocalDiskFactorFileProvider();
            factorFileProvider.Initialize(mapFileProvider, dataProvider);

            var dataCacheProvider = new ZipDataCacheProvider(dataProvider, isDataEphemeral: false);

            var historyProviderParameters = new HistoryProviderInitializeParameters(
                job: null,
                api: null,
                dataProvider: dataProvider,
                dataCacheProvider: dataCacheProvider,
                mapFileProvider: mapFileProvider,
                factorFileProvider: factorFileProvider,
                statusUpdateAction: _ => { },
                parallelHistoryRequestsEnabled: false,
                dataPermissionManager: new DataPermissionManager(),
                objectStore: new LocalObjectStore(),
                algorithmSettings: null
            );

            _historyProviderManager.Initialize(historyProviderParameters);
        }

        public IEnumerable<TradeBar> GetHistoricalData(string ticker, DateTime start, DateTime end, Resolution resolution = Resolution.Minute)
        {
            var symbol = Symbol.Create(ticker, SecurityType.Equity, Market.USA);

            var request = new HistoryRequest(
                start,
                end,
                typeof(TradeBar),
                symbol,
                resolution,
                SecurityExchangeHours.AlwaysOpen(TimeZones.NewYork),
                TimeZones.NewYork,
                resolution,
                false,
                false,
                DataNormalizationMode.Adjusted,
                TickType.Trade
            );

            return _historyProviderManager
                .GetHistory(new[] { request }, DateTimeZone.Utc)
                .OfType<TradeBar>();
        }

        public Task<IEnumerable<TradeBar>> GetHistoricalDataAsync(string ticker, DateTime start, DateTime end, Resolution resolution = Resolution.Minute)
        {
            return Task.Run(() => GetHistoricalData(ticker, start, end, resolution));
        }
    }
}
