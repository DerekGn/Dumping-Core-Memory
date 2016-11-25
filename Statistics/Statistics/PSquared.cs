/**
* MIT License
*
* Copyright (c) 2016 Derek Goslin < http://corememorydump.blogspot.ie/ >
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace DumpingCoreMemory.Statistics
{
    /// <summary>
    ///     An implementation of the P2 psquared algorithim
    ///     http://pierrechainais.ec-lille.fr/Centrale/Option_DAD/IMPACT_files/Dynamic%20quantiles%20calcultation%20-%20P2%20Algorythm.pdf
    /// </summary>
    public class PSquared
    {
        private readonly List<double> _dn;
        private readonly List<int> _n;
        private readonly List<double> _np;
        private readonly List<double> _q;

        private int _sampleCount;

        /// <summary>
        ///     Create instance of <see cref="PSquared" /> type
        /// </summary>
        /// <param name="quantiles">
        ///     The list of quantiles that this instance of <see cref="PSquared" /> will calculate.
        ///     The quantiles are specified as double values less than 1. For example 0.1, 0.2 etc
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="quantiles" /> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Throw if <paramref name="quantiles" /> is empty or contains duplicates
        ///     values or values > 1
        /// </exception>
        public PSquared(IList<double> quantiles)
        {
            Quantiles = ValidateQuantiles(quantiles);

            _dn = new List<double>();
            _np = new List<double>();
            _q = new List<double>();
            _n = new List<int>();

            // Add end markers
            _dn.AddRange(new List<double> {0.0, 1});
            _np.AddRange(new List<double> {0.0, 0.0});
            _q.AddRange(new List<double> {0.0, 0.0});
            _n.AddRange(new List<int> {0, 0});

            UpdateMarkers();

            foreach (var quantile in quantiles)
            {
                AddQuantile(quantile);
            }
        }

        /// <summary>
        ///     The list of quantiles that this instance is performing calculations for.
        /// </summary>
        public IList<double> Quantiles { get; }

        /// <summary>
        ///     Add a sample value to the quantile calculation
        /// </summary>
        /// <param name="sample">The sample value to add to the distribution</param>
        public void AddSample(double sample)
        {
            int i;

            if (_sampleCount >= _dn.Count)
            {
                _sampleCount++;
                var k = 0;

                // Find cell k such that qk =< Xj < qk+i and adjust extreme values (ql and qs) 
                if (sample < _q[0])
                {
                    _q[0] = sample;
                    k = 1;
                }
                else if (sample >= _q[_dn.Count - 1])
                {
                    _q[_dn.Count - 1] = sample;
                    k = _dn.Count - 1;
                }
                else
                {
                    for (i = 1; i < _dn.Count; i++)
                    {
                        if (sample < _q[i])
                        {
                            k = i;
                            break;
                        }
                    }
                }

                // Increment positions of markers k + 1 through to count of percentiles
                for (i = k; i < _dn.Count; i++)
                {
                    _n[i]++;
                    _np[i] = _np[i] + _dn[i];
                }

                // Update desired positions for all markers
                for (i = 0; i < k; i++)
                {
                    _np[i] = _np[i] + _dn[i];
                }

                // Adjust heights of markers if necessary
                for (i = 1; i < _dn.Count - 1; i++)
                {
                    var d = _np[i] - _n[i];
                    if ((d >= 1.0 && _n[i + 1] - _n[i] > 1)
                        || (d <= -1.0 && _n[i - 1] - _n[i] < -1.0))
                    {
                        var newq = Parabolic(i, Math.Sign(d));

                        if (_q[i - 1] < newq && newq < _q[i + 1])
                        {
                            _q[i] = newq;
                        }
                        else
                        {
                            _q[i] = Linear(i, Math.Sign(d));
                        }
                        _n[i] += Math.Sign(d);
                    }
                }
            }
            else
            {
                _q[_sampleCount] = sample;
                _sampleCount++;

                if (_sampleCount == _dn.Count)
                {
                    _q.Sort();

                    for (i = 0; i < _dn.Count; i++)
                    {
                        _n[i] = i + 1;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the calculated quantile value for the <paramref name="quantile" />
        /// </summary>
        /// <param name="quantile">The quantile to get the result</param>
        /// <returns>The calculated quantile value</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If <paramref name="quantile" /> is not one of the quantiles calculated by this instance
        /// </exception>
        public double Result(double quantile)
        {
            double result = 0;

            if (Quantiles.Contains(quantile))
            {
                if (_sampleCount < _dn.Count)
                {
                    var closest = 1;
                    _q.Sort();
                    for (var i = 2; i < _sampleCount; i++)
                    {
                        if (Math.Abs((double) i/_sampleCount - quantile) <
                            Math.Abs((double) closest/_sampleCount - quantile))
                        {
                            closest = i;
                        }
                    }
                    result = _q[closest];
                }
                else
                {
                    // Figure out which quantile is the one we're looking for by nearest dn
                    var closest = 1;
                    for (var i = 2; i < _dn.Count - 1; i++)
                    {
                        if (Math.Abs(_dn[i] - quantile) < Math.Abs(_dn[closest] - quantile))
                        {
                            closest = i;
                        }
                    }
                    result = _q[closest];
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(quantile), "invalid quantiles");
            }

            return result;
        }

        private IList<double> ValidateQuantiles(IList<double> quantiles)
        {
            if (quantiles == null)
            {
                throw new ArgumentNullException(nameof(quantiles));
            }

            if (quantiles.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantiles), "is empty");
            }

            if (quantiles.Distinct().Count() != quantiles.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(quantiles), "contains duplicates");
            }

            foreach (var quantile in quantiles)
            {
                if (quantile > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(quantiles), "contains values > 1");
                }
            }

            return quantiles;
        }

        private void AddQuantile(double quantile)
        {
            var markerCount = _dn.Count;

            _dn.AddRange(new List<double> {0.0, 0.0, 0.0});
            _np.AddRange(new List<double> {0.0, 0.0, 0.0});
            _q.AddRange(new List<double> {0.0, 0.0, 0.0});
            _n.AddRange(new List<int> {0, 0, 0});

            _dn[markerCount++] = quantile;
            _dn[markerCount++] = quantile/2.0;
            _dn[markerCount] = (1.0 + quantile)/2.0;

            UpdateMarkers();
        }

        private void UpdateMarkers()
        {
            _dn.Sort();

            for (var i = 0; i < _n.Count; i++)
            {
                _np[i] = (_n.Count - 1)*_dn[i] + 1;
            }
        }

        private double Parabolic(int i, int d)
        {
            return _q[i] +
                   d/(double) (_n[i + 1] - _n[i - 1])*
                   ((_n[i] - _n[i - 1] + d)*(_q[i + 1] - _q[i])/(_n[i + 1] - _n[i]) +
                    (_n[i + 1] - _n[i] - d)*(_q[i] - _q[i - 1])/(_n[i] - _n[i - 1]));
        }

        private double Linear(int i, int d)
        {
            return _q[i] + d*(_q[i + d] - _q[i])/(_n[i + d] - _n[i]);
        }
    }
}
