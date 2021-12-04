using System;
using System.Collections.Generic;
using System.Linq;
using EMPILab1.Models;

namespace EMPILab1.Helpers
{
    public static class MathHelpers
    {
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

            // получаю кол-во элементов в изначальной выборке
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
    }
}
