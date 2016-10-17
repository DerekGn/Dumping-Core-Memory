using System;
using System.Collections.Generic;
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
        public void TestQuantilesTestCaseDataRandomDistribution()
        {
            var psquared = new PSquared(new List<double> {0d, 0.25d, 0.5d, 0.75d, 1d});

            foreach (var d in TestCaseDataRandomDistribution.Data)
            {
                psquared.AddSample(d);
            }

            Assert.That(psquared.Result(0), Is.EqualTo(0.0074893045760936749d));
            Assert.That(psquared.Result(0.25d), Is.EqualTo(0.28504496967193316d));
            Assert.That(psquared.Result(0.5d), Is.EqualTo(0.48137186347987881d));
            Assert.That(psquared.Result(0.75d), Is.EqualTo(0.70650331141191547d));
            Assert.That(psquared.Result(1d), Is.EqualTo(0.98608532658385972d));

            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistribution.Percentiles[0]} Actual: {psquared.Result(0)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistribution.Percentiles[0], psquared.Result(0))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistribution.Percentiles[1]} Actual: {psquared.Result(0.25d)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistribution.Percentiles[1], psquared.Result(0.25d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistribution.Percentiles[2]} Actual: {psquared.Result(0.5d)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistribution.Percentiles[2], psquared.Result(0.5d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistribution.Percentiles[3]} Actual: {psquared.Result(0.75d)} Error: {CalculatePercentageDifference(TestCaseDataRandomDistribution.Percentiles[3], psquared.Result(0.75d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomDistribution.Percentiles[4]} Actual: {psquared.Result(1d)} Error: Error: {CalculatePercentageDifference(TestCaseDataRandomDistribution.Percentiles[4], psquared.Result(1d))}");
        }

        [Test]
        public void TestQuantilesTestCaseDataRandomNormalDistribution()
        {
            var psquared = new PSquared(new List<double> { 0d, 0.25d, 0.5d, 0.75d, 1d });
            
            foreach (var d in TestCaseDataRandomNormalDistribution.Data)
            {
                psquared.AddSample(d);
            }

            Assert.That(psquared.Result(0), Is.EqualTo(-1.8734831276988309d));
            Assert.That(psquared.Result(0.25d), Is.EqualTo(-0.70305951196317573d));
            Assert.That(psquared.Result(0.5d), Is.EqualTo(-0.065403671463218238d));
            Assert.That(psquared.Result(0.75d), Is.EqualTo(0.78953029041255129d));
            Assert.That(psquared.Result(1d), Is.EqualTo(2.1478607563987957d));

            Console.WriteLine(
                $"Expected: {TestCaseDataRandomNormalDistribution.Percentiles[0]} Actual: {psquared.Result(0)} Error: {CalculatePercentageDifference(TestCaseDataRandomNormalDistribution.Percentiles[0], psquared.Result(0))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomNormalDistribution.Percentiles[1]} Actual: {psquared.Result(0.25d)} Error: {CalculatePercentageDifference(TestCaseDataRandomNormalDistribution.Percentiles[1], psquared.Result(0.25d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomNormalDistribution.Percentiles[2]} Actual: {psquared.Result(0.5d)} Error: {CalculatePercentageDifference(TestCaseDataRandomNormalDistribution.Percentiles[2], psquared.Result(0.5d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomNormalDistribution.Percentiles[3]} Actual: {psquared.Result(0.75d)} Error: {CalculatePercentageDifference(TestCaseDataRandomNormalDistribution.Percentiles[3], psquared.Result(0.75d))}");
            Console.WriteLine(
                $"Expected: {TestCaseDataRandomNormalDistribution.Percentiles[4]} Actual: {psquared.Result(1d)} Error: Error: {CalculatePercentageDifference(TestCaseDataRandomNormalDistribution.Percentiles[4], psquared.Result(1d))}");
        }

        private double CalculatePercentageDifference(double original, double newValue)
        {
            return ((original - newValue)/original)*100;
        }
    }
}