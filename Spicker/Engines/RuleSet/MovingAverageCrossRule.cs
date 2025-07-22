using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spicker.Engines.RuleSet
{
    public class MovingAverageCrossRule : IRule
    {
        public RuleResult Evaluate(string ticker, List<HistoricalPricePoint> history)
        {
            var shortMA = history.TakeLast(5).Average(p => p.Close);
            var longMA = history.TakeLast(20).Average(p => p.Close);

            if (shortMA > longMA)
            {
                return new RuleResult
                {
                    Passed = true,
                    Score = 25,
                    Reason = "Short-term moving average crossed above long-term"
                };
            }

            return new RuleResult
            {
                Passed = false,
                Score = 0,
                Reason = "Moving average crossover not found"
            };
        }
    }

}
