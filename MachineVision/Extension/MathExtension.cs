using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.Extension
{
    public static class MathExtension
    {
        static double StandardDeviation(this IEnumerable<double> vals) {
            double mean = vals.Average();
            double stddev = Math.Sqrt(vals.Average(n=> { double d = n - mean; return d * d; }));
            return stddev;
        }
        static double StandardDeviation<T>(this IEnumerable<T> vals, Func<T, double> selector) where T : class
        {
            return vals.Select(selector).StandardDeviation();
        }
    }
}
