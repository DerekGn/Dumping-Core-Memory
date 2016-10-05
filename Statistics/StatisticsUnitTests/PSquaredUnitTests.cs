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
        public void TestQuantilesTestCaseB()
        {
            var psquared = new PSquared(new List<double> {0d, 0.25d, 0.5d, 0.75d, 1d});

            foreach (var d in TestCaseDataB.Data)
            {
                psquared.AddSample(d);
            }

            Assert.That(psquared.Result(0), Is.EqualTo(0.010376343130563188d));
            Assert.That(psquared.Result(0.25d), Is.EqualTo(0.24177769775826496d));
            Assert.That(psquared.Result(0.5d), Is.EqualTo(0.46242674676685891d));
            Assert.That(psquared.Result(0.75d), Is.EqualTo(0.83040990858645203d));
            Assert.That(psquared.Result(1d), Is.EqualTo(0.98174957638606375d));
        }
    }
}