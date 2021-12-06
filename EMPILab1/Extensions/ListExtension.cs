using System;
using System.Collections.Generic;
using System.Linq;
using EMPILab1.Models;

namespace EMPILab1.Extensions
{
    public static class ListExtension
    {
        public static List<VariantItemViewModel> ToVariantsList(this List<double> valuesList)
        {
            var sortedValues = valuesList.OrderBy(v => v).ToList();

            var uniqueValues = sortedValues.GroupBy(v => v);

            var variantsList = new List<VariantItemViewModel>();

            var i = 1;
            var empiricalDistrFuncValue = 0d;
            foreach (var group in uniqueValues)
            {
                var variant = new VariantItemViewModel
                {
                    Index = i,
                    Value = group.Key,
                    Frequency = group.Count(),
                    RelativeFrequency = (double)group.Count() / valuesList.Count(),
                    EmpiricalDistrFuncValue = Math.Round(empiricalDistrFuncValue += (double)group.Count() / valuesList.Count(), 5),
                };

                variantsList.Add(variant);

                ++i;
            }

            return variantsList;
        }
    }
}
