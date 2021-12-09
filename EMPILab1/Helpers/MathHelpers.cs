using System;
using System.Collections.Generic;
using System.Linq;
using EMPILab1.Extensions;
using EMPILab1.Models;

namespace EMPILab1.Helpers
{
    public static class MathHelpers
    {
        private const int DECIMALS_COUNT = 8;

        #region -- Kernel Density Estimation (for task 5) --

        public static double GetDispersion(List<VariantItemViewModel> variants)
        {
            var result = 0d;

            if (variants is not null && variants.Any())
            {
                var expectedValueOfXSquared = 0d;
                var expectedValueOfX = 0d;

                foreach (var variant in variants)
                {
                    expectedValueOfXSquared += Math.Pow(variant.Value, 2) * variant.RelativeFrequency;
                    expectedValueOfX += variant.Value * variant.RelativeFrequency;
                }

                var squaredExpectedValueOfX = Math.Pow(expectedValueOfX, 2);

                result = expectedValueOfXSquared - squaredExpectedValueOfX;
            }

            return result;
        }

        public static double GetStandardDeviation(List<VariantItemViewModel> variants)
        {
            return Math.Sqrt(GetDispersion(variants));
        }

        public static double GetScottBandwidth(List<VariantItemViewModel> variants)
        {
            var stDev = GetStandardDeviation(variants);

            // getting the initial dataset count
            var n = variants.Sum(v => v.Frequency);

            return stDev * Math.Pow(n, -0.2);
        }

        public static double GaussianKernelFunc(double u)
        {
            return 1 / Math.Sqrt(2 * Math.PI) * Math.Exp(-(Math.Pow(u, 2) / 2));
        }

        public static List<Tuple<double, double>> GetGaussianKdePoints(List<double> initialDataset, double bandwidth)
        {
            var result = new List<Tuple<double, double>>();

            foreach (var x in initialDataset)
            {
                var sum = 0d;

                foreach (var xi in initialDataset)
                {
                    sum += GaussianKernelFunc((x - xi) / bandwidth);
                }

                var fx = (1 / (initialDataset.Count() * bandwidth)) * sum;

                result.Add(new Tuple<double, double>(x, fx));
            }

            return result;
        }

        #endregion

        #region -- Quantitative Characteristic (task 6) --

        public static double Mean(List<double> x)
        {
            var n = x.Count;
            var sum = x.Sum();

            return Math.Round(sum / n, DECIMALS_COUNT);
        }

        public static double MeanStandardError(List<double> x)
        {
            var stdDev = GetStandardDeviation(x.ToVariantsList());
            var stdErr = stdDev / Math.Sqrt(x.Count);

            return Math.Round(stdErr, DECIMALS_COUNT);
        }

        public static Tuple<double, double> MeanConfidenceInterval(double alpha, List<double> x)
        {
            var mean = Mean(x);
            var stdErr = MeanStandardError(x);
            var v = (double)x.Count() - 1;
            var t = QuantileT(1 - alpha / 2, v);

            var low = mean - t * stdErr;
            var high = mean + t * stdErr;

            return new Tuple<double, double>(Math.Round(low, DECIMALS_COUNT), Math.Round(high, DECIMALS_COUNT));
        }

        public static double Median(List<double> x)
        {
            var n = x.Count;

            if (n == 1)
            {
                return x[0];
            }

            x.Sort();

            double med;

            if (n % 2 == 0)
            {
                med = (x[n / 2] + x[n / 2 - 1]) / 2;
            }
            else
            {
                med = x[n / 2];
            }

            return med;
        }

        // https://www-users.york.ac.uk/~mb55/intro/cicent.htm
        public static Tuple<double, double> MedianConfidenceInterval(double alpha, List<double> x)
        {
            x.Sort();

            var u = QuantileU(1 - alpha / 2);

            var i = (int)Math.Floor((x.Count * 0.5) - u * Math.Sqrt(x.Count) / 2);
            var k = (int)Math.Floor((x.Count * 0.5) + 1 + u * Math.Sqrt(x.Count) / 2);

            if (k > x.Count - 1)
            {
                k = x.Count - 1;
            }

            return new Tuple<double, double>(x[i], x[k]);
        }

        public static double StandardDeviation(List<double> x)
        {
            var stdDev = GetStandardDeviation(x.ToVariantsList());

            return Math.Round(stdDev, DECIMALS_COUNT);
        }

        public static double StandardDeviationStandardError(List<double> x)
        {
            var stdDev = StandardDeviation(x);
            var stdDevStdErr = stdDev / Math.Sqrt(2 * x.Count);

            return Math.Round(stdDevStdErr, DECIMALS_COUNT);
        }

        public static Tuple<double, double> StandardDeviationConfidenceInterval(double alpha, List<double> x)
        {
            var stdDev = StandardDeviation(x);
            var stdDevOfStdDev = stdDev / Math.Sqrt(2 * x.Count);

            var v = (double)(x.Count - 1);
            var t = QuantileT(1 - alpha / 2, v);

            var low = stdDev - t * stdDevOfStdDev;
            var high = stdDev + t * stdDevOfStdDev;

            return new Tuple<double, double>(Math.Round(low, DECIMALS_COUNT), Math.Round(high, DECIMALS_COUNT));
        }

        public static double Skewness(List<double> x)
        {
            var sum = 0d;
            var mean = Mean(x);

            foreach (var val in x)
            {
                sum += Math.Pow(val - mean, 3);
            }

            var stdDev = StandardDeviationBiased(x);

            var skewness = sum / (x.Count * Math.Pow(stdDev, 3));

            return Math.Round(skewness, DECIMALS_COUNT);
        }

        public static double SkewnessStandardError(List<double> x)
        {
            var N = x.Count;
            var stdErr = Math.Sqrt((double)(6 * (N - 2)) / ((N + 1) * (N + 3)));

            return Math.Round(stdErr, DECIMALS_COUNT);
        }

        public static Tuple<double, double> SkewnessConfidenceInterval(double alpha, List<double> x)
        {
            var skewness = Skewness(x);
            var t = QuantileT(1 - alpha / 2, x.Count - 1);
            var skewnessStdErr = SkewnessStandardError(x);

            var low = skewness - t * skewnessStdErr;
            var high = skewness + t * skewnessStdErr;

            return new Tuple<double, double>(Math.Round(low, DECIMALS_COUNT), Math.Round(high, DECIMALS_COUNT));
        }

        public static double Kurtosis(List<double> x)
        {
            var sum = 0d;
            var mean = Mean(x);

            foreach (var val in x)
            {
                sum += Math.Pow(val - mean, 4);
            }

            var stdDev = StandardDeviationBiased(x);

            var kurtosis = (sum / (x.Count * Math.Pow(stdDev, 4))) - 3;

            return Math.Round(kurtosis, DECIMALS_COUNT);
        }

        public static double KurtosisStandardError(List<double> x)
        {
            var N = x.Count;
            var stdErr = Math.Sqrt(24d * N * (N - 2) * (N - 3) / (Math.Pow(N + 1, 2) * ((N + 3) * (N + 5))));

            return Math.Round(stdErr, DECIMALS_COUNT);
        }

        public static Tuple<double, double> KurtosisConfidenceInterval(double alpha, List<double> x)
        {
            var kurtosis = Kurtosis(x);
            var t = QuantileT(1 - alpha / 2, (double)x.Count - 1);
            var kurtosisStdErr = KurtosisStandardError(x);

            var low = kurtosis - t * kurtosisStdErr;
            var high = kurtosis + t * kurtosisStdErr;

            return new Tuple<double, double>(Math.Round(low, DECIMALS_COUNT), Math.Round(high, DECIMALS_COUNT));
        }

        public static double AntiKurtosis(List<double> x)
        {
            var kurtosis = Kurtosis(x);
            var antiKurtosis = 1 / Math.Sqrt(kurtosis + 3);

            return Math.Round(antiKurtosis, DECIMALS_COUNT);
        }

        public static double VarianceBiased(List<double> x)
        {
            var sum = 0d;
            var mean = Mean(x);

            foreach (var val in x)
            {
                sum += Math.Pow(val - mean, 2);
            }

            return sum / x.Count;
        }

        /// <summary>
        /// Смещенное стандартное отклонение
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double StandardDeviationBiased(List<double> x)
        {
            var variance = VarianceBiased(x);
            var stdDev = Math.Sqrt(variance);
            return stdDev;
        }

        /// <summary>
        /// Квантиль U(p) нормального розподілу 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double QuantileU(double p)
        {
            static double phi(double a)
            {
                const double c0 = 2.515517;
                const double c1 = 0.802853;
                const double c2 = 0.010328;
                const double d1 = 1.432788;
                const double d2 = 0.1892659;
                const double d3 = 0.001308;

                var t = Math.Sqrt(-2 * Math.Log(a));

                return t - ((c0 + c1 * t + c2 * Math.Pow(t, 2)) / (1 + d1 * t + d2 * Math.Pow(t, 2) + d3 * Math.Pow(t, 3)));
            }

            double result;

            if (p <= 0.5)
            {
                result = -phi(p);
            }
            else
            {
                result = phi(1 - p);
            }

            return result;
        }

        /// <summary>
        /// Квантиль T(p,ν) розподілу Стьюдента
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double QuantileT(double p, double v)
        {
            var u = QuantileU(p);

            var g1 = (Math.Pow(u, 3) + u) / 4;
            var g2 = (5 * Math.Pow(u, 5) + 16 * Math.Pow(u, 3) + 3 * u) / 96;
            var g3 = (3 * Math.Pow(u, 7) + 19 * Math.Pow(u, 5) + 17 * Math.Pow(u, 3) - 15 * u) / 384;
            var g4 = (79 * Math.Pow(u, 9) + 779 * Math.Pow(u, 7) + 1482 * Math.Pow(u, 5) - 1920 * Math.Pow(u, 3) - 945 * u) / 92160;

            return u + g1 / v + g2 / Math.Pow(v, 2) + g3 / Math.Pow(v, 3) + g4 / Math.Pow(v, 4);
        }

        #endregion

        #region -- Recovered Implerical distribution and density functions for Exponential distribution (task 8.2) --

        public static double ImplericalFuncExponential(double x, double lambda)
        {
            return x < 0
                ? 0d
                : 1 - Math.Exp(-lambda * x);
        }

        public static double DensityFuncForExponential(double x, double lambda)
        {
            return x < 0
                ? 0d
                : lambda * Math.Exp(-lambda * x);
        }

        public static List<double> GetXs(List<double> x,int count)
        {
            var min = x.Min();
            var max = x.Max();

            double step = (max - min) / count;

            var result = new List<double>();

            for (var i = min; i <= max; i += step)
            {
                result.Add(i);
            }

            return result;
        }

        #endregion
    }
}
