using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantConnect;
using QuantConnect.Data.Market;
using Spicker.Models;
using Spicker.Services;

namespace Spicker.Engines.RuleSet
{
    public interface IHistoricalDataProvider
    {
        IEnumerable<TradeBar> GetHistoricalData(string ticker, DateTime start, DateTime end, Resolution resolution = Resolution.Minute);
    }

    public interface IRule
    {
        RuleResult Evaluate(string ticker, List<HistoricalPricePoint> history);
    }

    public class RuleResult
    {
        public bool Passed { get; set; }
        public int Score { get; set; }
        public string Reason { get; set; }
    }
    public class RuleEvaluationSummary
    {
        public string Ticker { get; set; } = string.Empty;
        public DateTime EvaluatedAt { get; set; }
        public List<RuleEvaluationResult> RuleResults { get; set; } = new();
    }

    public class RuleEvaluationResult
    {
        public string RuleName { get; set; } = string.Empty;
        public bool Passed { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class RuleAnalysisResult
    {
        public string Ticker { get; set; }
        public int Score { get; set; }
        public string Direction { get; set; }
        public List<string> Reasons { get; set; } = new();
    }

    public class HistoricalPricePoint
    {
        public DateTime Time { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }


}
