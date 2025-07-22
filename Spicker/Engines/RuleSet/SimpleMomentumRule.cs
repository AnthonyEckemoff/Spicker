using System;
using System.Collections.Generic;
using System.Linq;
using Spicker.Engines.RuleSet;

namespace Spicker.Rules
{
    public class SimpleMomentumRule : IRule
    {
        public RuleResult Evaluate(string ticker, List<HistoricalPricePoint> history)
        {
            if (history == null || history.Count < 2)
            {
                return new RuleResult
                {
                    Passed = false,
                    Score = 0,
                    Reason = "Not enough historical data"
                };
            }

            var firstClose = history.First().Close;
            var lastClose = history.Last().Close;

            bool passed = lastClose > firstClose;
            return new RuleResult
            {
                Passed = passed,
                Score = passed ? 1 : 0,
                Reason = passed
                    ? $"Price rose from {firstClose} to {lastClose}"
                    : $"Price dropped or stayed the same from {firstClose} to {lastClose}"
            };
        }
    }
}
