using System.Collections.Concurrent;
using System.IO;
using Spicker.DataProviders;
using Spicker.Engines.RuleSet;
using Spicker.Extensions;

namespace Spicker.Engine
{
    public class RuleSuiteEngine
    {
        private readonly IExtendedHistoricalDataProvider _dataProvider;

        private readonly List<IRule> _rules;

        public RuleSuiteEngine(IExtendedHistoricalDataProvider dataProvider, List<IRule> rules)
        {
            _dataProvider = dataProvider;
            _rules = rules;
        }

        public async Task<ConcurrentDictionary<string, RuleAnalysisResult>> AnalyzeAsync(List<string> tickers)
        {
            var results = new ConcurrentDictionary<string, RuleAnalysisResult>();
            var end = DateTime.UtcNow;
            var start = end.AddDays(-30); // or configurable period

            var tasks = tickers.Select(async ticker =>
            {
                try
                {
                    var history = await _dataProvider.GetHistoricalDataAsync(ticker, start, end)
                                 .ConfigureAwait(false);

                    var pricePoints = history.Select(tb => new HistoricalPricePoint
                    {
                        Time = tb.Time,
                        Open = tb.Open,
                        High = tb.High,
                        Low = tb.Low,
                        Close = tb.Close,
                        Volume = tb.Volume
                    }).ToList();

                    var score = 0;
                    var reasons = new List<string>();

                    foreach (var rule in _rules)
                    {
                        var result = rule.Evaluate(ticker, pricePoints); // <-- correct now
                        if (result.Passed)
                        {
                            score += result.Score;
                            reasons.Add(result.Reason);
                        }
                    }

                    var direction = score > 60 ? "LONG" : score < 40 ? "SHORT" : "NEUTRAL";
                    var resultObject = new RuleAnalysisResult
                    {
                        Ticker = ticker,
                        Score = score,
                        Direction = direction,
                        Reasons = reasons
                    };

                    LogAnalysis(resultObject);
                    results[ticker] = resultObject;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error analyzing {ticker}: {ex.Message}");
                }
            });

            await Task.WhenAll(tasks);
            return results;
        }

        private void LogAnalysis(RuleAnalysisResult result)
        {
            var path = $"logs/{DateTime.Now:yyyyMMdd}/{result.Ticker}.log";
            var dir = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllLines(path, new[]
            {
                $"Ticker: {result.Ticker}",
                $"Score: {result.Score}",
                $"Direction: {result.Direction}",
                $"Reasons:",
                string.Join(Environment.NewLine, result.Reasons)
            });
        }
    }
}
