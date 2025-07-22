using Moq;
using Spicker.DataProviders;
using Spicker.Engines.RuleSet;
using Spicker.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using QuantConnect.Data.Market;
using QuantConnect;

namespace Spicker.Tests
{
    public class RuleSuiteEngineTests
    {
        [Fact]
        public async Task AnalyzeAsync_ReturnsExpectedResult_WhenRulesPass()
        {
            var symbol = new Symbol(SecurityIdentifier.GenerateEquity("FAKE", Market.USA, mapSymbol: false), "FAKE");

            // Arrange
            var tradeBars = new List<TradeBar>
            {
                new TradeBar
                {
                    Time = DateTime.UtcNow.AddMinutes(-1),
                    Open = 100m,
                    High = 105m,
                    Low = 95m,
                    Close = 102m,
                    Volume = 1000m,
                    Symbol = symbol
                }
            };

            var dataProviderMock = new Mock<IExtendedHistoricalDataProvider>();
            dataProviderMock
                .Setup(p => p.GetHistoricalDataAsync("MSFT", It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Resolution>()))
                .ReturnsAsync(tradeBars);

            var ruleMock = new Mock<IRule>();
            ruleMock
                .Setup(r => r.Evaluate("MSFT", It.IsAny<List<HistoricalPricePoint>>()))
                .Returns(new RuleResult
                {
                    Passed = true,
                    Score = 70,
                    Reason = "Strong uptrend"
                });

            var rules = new List<IRule> { ruleMock.Object };
            var engine = new RuleSuiteEngine(dataProviderMock.Object, rules);

            // Act
            var results = await engine.AnalyzeAsync(new List<string> { "MSFT" });

            // Assert
            Assert.True(results.ContainsKey("MSFT"), "MSFT was not analyzed successfully. It may have failed inside AnalyzeAsync.");
            var result = results["MSFT"];

            Assert.Equal("MSFT", result.Ticker);
            Assert.Equal(70, result.Score);
            Assert.Equal("LONG", result.Direction);
            Assert.Contains("Strong uptrend", result.Reasons);
        }
    }
}
