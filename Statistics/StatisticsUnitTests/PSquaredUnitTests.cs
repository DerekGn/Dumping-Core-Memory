using System;
using System.Collections.Generic;
using System.Diagnostics;
using DumpingCoreMemory.Statistics;
using NUnit.Framework;

namespace DumpingCoreMemory.StatisticsUnitTests
{
    [TestFixture]
    public class PSquaredUnitTests
    {
        [Test]
        public void TestConstructorValidationDuplicates()
        {
            Assert.That(() => new PSquared(new List<double> {0.1, 0.1}), Throws.TypeOf<ArgumentOutOfRangeException>()
                .With.Property("ParamName")
                .EqualTo("quantiles")
                .With.Property("Message")
                .Contains("contains duplicates"));
        }

        [Test]
        public void TestConstructorValidationEmpty()
        {
            Assert.That(() => new PSquared(new List<double>()), Throws.TypeOf<ArgumentOutOfRangeException>()
                .With.Property("ParamName").EqualTo("quantiles").With.Property("Message").Contains("is empty"));
        }

        [Test]
        public void TestConstructorValidationGreaterThanOne()
        {
            Assert.That(() => new PSquared(new List<double> {1.1, 0.1}), Throws.TypeOf<ArgumentOutOfRangeException>()
                .With.Property("ParamName")
                .EqualTo("quantiles")
                .With.Property("Message")
                .Contains("contains values > 1"));
        }

        [Test]
        public void TestConstructorValidationNull()
        {
            Assert.That(() => new PSquared(null), Throws.TypeOf<ArgumentNullException>()
                .With.Property("ParamName").EqualTo("quantiles"));
        }

        [Test]
        public void TestQuantilesTestCaseA()
        {
            var psquared = new PSquared(new List<double> {0d, 0.1d, 0.2d, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1});

            foreach (var d in TestCaseDataA.Data)
            {
                psquared.AddSample(d);
            }

            Assert.That(psquared.Result(0), Is.EqualTo(-2.42825039724691340000d));
            Assert.That(psquared.Result(0.1), Is.EqualTo(-1.17237756161733090000d));
            Assert.That(psquared.Result(0.2), Is.EqualTo(-0.78527849244109615000d));
            Assert.That(psquared.Result(0.3), Is.EqualTo(-0.54342135383426704000d));
            Assert.That(psquared.Result(0.4), Is.EqualTo(-0.23365458282504820000d));
            Assert.That(psquared.Result(0.5), Is.EqualTo(0.02240886950031340700d));
            Assert.That(psquared.Result(0.6), Is.EqualTo(0.23713921834760349000d));
            Assert.That(psquared.Result(0.7), Is.EqualTo(0.55275891548148226000d));
            Assert.That(psquared.Result(0.8), Is.EqualTo(0.88012701134409244000d));
            Assert.That(psquared.Result(0.9), Is.EqualTo(1.31936120666738650000d));
            Assert.That(psquared.Result(1), Is.EqualTo(2.10491355499534190000d));
        }
        
        [Test]
        public void TestQuantilesTestCaseDataRandomDistributionA()
        {
            var psquared = new PSquared(new List<double> {0d, 0.25d, 0.5d, 0.75d, 1d});

            foreach (var d in TestCaseDataRandomDistributionA.Data)
            {
                psquared.AddSample(d);
            }

            Assert.That(psquared.Result(0), Is.EqualTo(0.0097687190699588689d));
            Assert.That(psquared.Result(0.25d), Is.EqualTo(0.26429842848232071d));
            Assert.That(psquared.Result(0.5d), Is.EqualTo(0.49620747801544807d));
            Assert.That(psquared.Result(0.75d), Is.EqualTo(0.74817333592384194d));
            Assert.That(psquared.Result(1d), Is.EqualTo(0.99644118661550751d));

            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistributionA.Percentiles[0]} Actual: {psquared.Result(0)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistributionA.Percentiles[0], psquared.Result(0))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistributionA.Percentiles[1]} Actual: {psquared.Result(0.25d)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistributionA.Percentiles[1], psquared.Result(0.25d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistributionA.Percentiles[2]} Actual: {psquared.Result(0.5d)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistributionA.Percentiles[2], psquared.Result(0.5d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistributionA.Percentiles[3]} Actual: {psquared.Result(0.75d)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistributionA.Percentiles[3], psquared.Result(0.75d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistributionA.Percentiles[4]} Actual: {psquared.Result(1d)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistributionA.Percentiles[4], psquared.Result(1d))}");
        }

        [Test]
        public void TestQuantilesTestCaseDataRandomDistributionB()
        {
            var psquared = new PSquared(new List<double> { 0d, 0.25d, 0.5d, 0.75d, 1d });
            
            foreach (var d in TestCaseDataRandomDistributionB.Data)
            {
                psquared.AddSample(d);
            }

            Assert.That(psquared.Result(0), Is.EqualTo(0.0047192229265285461d));
            Assert.That(psquared.Result(0.25d), Is.EqualTo(0.24741203439543097d));
            Assert.That(psquared.Result(0.5d), Is.EqualTo(0.50894857119117332d));
            Assert.That(psquared.Result(0.75d), Is.EqualTo(0.74736639199760668d));
            Assert.That(psquared.Result(1d), Is.EqualTo(0.99292525864317671d));

            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistributionB.Percentiles[0]} Actual: {psquared.Result(0)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistributionB.Percentiles[0], psquared.Result(0))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistributionB.Percentiles[1]} Actual: {psquared.Result(0.25d)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistributionB.Percentiles[1], psquared.Result(0.25d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistributionB.Percentiles[2]} Actual: {psquared.Result(0.5d)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistributionB.Percentiles[2], psquared.Result(0.5d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistributionB.Percentiles[3]} Actual: {psquared.Result(0.75d)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistributionB.Percentiles[3], psquared.Result(0.75d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistributionB.Percentiles[4]} Actual: {psquared.Result(1d)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistributionB.Percentiles[4], psquared.Result(1d))}");
        }

        [Test]
        public void TestQuantilesTestCaseDataRandomDistributionBPerformance()
        {
            var psquared = new PSquared(new List<double> { 0d, 0.25d, 0.5d, 0.75d, 1d });

            Stopwatch sw = new Stopwatch();

            sw.Start();
            foreach (var d in TestCaseDataRandomDistributionB.Data)
            {
                psquared.AddSample(d);
            }
            sw.Stop();

            Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} Avg: {sw.ElapsedTicks / (Stopwatch.Frequency / (1000L*1000L))} us");
        }

        private static double CalculatePercentageDifference(double expected, double actual)
        {
            return ((expected - actual)/expected)*100;
        }
    }
}