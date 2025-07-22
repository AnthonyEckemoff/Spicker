using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spicker.Services
{
    public class LeanDataService
    {
        public List<PriceBar> GetHistoricalData(string ticker)
        {
            // Stub - you’ll later wire this to Lean’s backtest or local data
            return new List<PriceBar>
        {
            new PriceBar { Open = 100, High = 104, Low = 98, Close = 102 },
            new PriceBar { Open = 102, High = 107, Low = 101, Close = 105 },
            // ...
        };
        }
    }

    public class PriceBar
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }

}
