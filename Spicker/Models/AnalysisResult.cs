using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spicker.Models
{
    public class AnalysisResult
    {
        public string Ticker { get; set; }
        public List<string> Labels { get; set; } = new();
        public int Score { get; set; } = 0;

        public override string ToString()
        {
            return $"Ticker: {Ticker}\nScore: {Score}/100\nLabels: {string.Join(", ", Labels)}";
        }
    }

}
