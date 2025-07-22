using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spicker.Models;
using Spicker.Services;

namespace Spicker.Engines.RuleSet
{
    public class VolatilityRule
    {
        public void Apply(List<PriceBar> data, AnalysisResult result)
        {
            var dailyRanges = data.Select(d => d.High - d.Low).ToList();
            var avgVolatility = dailyRanges.Average();

            if (avgVolatility > 3.0m)
            {
                result.Labels.Add("High Volatility");
                result.Score += 20;
            }
        }
    }

}
