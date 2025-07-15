using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.Extension
{
    public static class MatExtension
    {
        public static string DebugDimsToString(this Mat m)
        {
            string o = "[ ";
            int channels = m.Channels();
            for (int i=0,l=m.Dims; i<l; i++) {
                if(i!=0)
                    o += " x ";
                o += m.Size(i);
            }
            o += $" ] Channels={channels}";
            return o;
        }

        public static Mat EnsureMatBatch(this Mat m)
        {
            if(m.Dims > 2)
                return m;
            var d = Enumerable.Empty<int>().Append(1);
            int i = 0;
            for(int l=m.Dims; i<l;i++)
                d = d.Append(m.Size(i));
            if(i <= 2) d = d.Append(1);
            return m.Reshape(m.Channels(), d.ToArray());
        }

        public static void DebugShowImage(this Mat m, string windowTitle = "Debug", ColorConversionCodes? ccc = null)
        {
            if (ccc != null) {
                Cv2.ImShow(windowTitle, m.CvtColor((ColorConversionCodes)ccc));
            } else
                Cv2.ImShow(windowTitle, m);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
        public static void DebugShowImage(this IEnumerable<Mat> em, IEnumerable<string>? windowTitle = null, ColorConversionCodes? ccc = null)
        {
            IEnumerable<string> titles = windowTitle == null ? Enumerable.Range(0, em.Count()).Select(i => $"Debug{i}") : windowTitle;
            if (titles.Count() < em.Count())
                titles = titles.Concat(Enumerable.Range(titles.Count(), em.Count()).Select(i => $"Debug{i}"));
            Mat[] _em = em.ToArray();
            string[] _t = titles.ToArray();
            int i = 0;
            foreach (Mat m in _em)
            {
                if (ccc != null)
                {
                    Cv2.ImShow(_t[i], m.CvtColor((ColorConversionCodes)ccc));
                }
                else
                    Cv2.ImShow(_t[i], m);
                i++;
            }
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        public static void MatchToRefMean(this Mat m, double refMean, Mat outMat)
        {
            Mat k = new Mat(new Size(13,13), MatType.CV_8UC1, new Scalar(1));

            using (var hsv_img = new Mat(m.Size(), m.Type()))
            {
                //Mat m_id = new Mat(m.Size(), MatType.CV_8UC1, new Scalar(1));
                Cv2.CvtColor(m, hsv_img, ColorConversionCodes.BGR2HSV_FULL);
                Mat[] hsv_split = hsv_img.Split();
                Mat mask = new();
                Cv2.Threshold(hsv_split[1], mask, 20, 255, ThresholdTypes.Binary);
                //Cv2.ImShow("DebugS", mask);
                //Cv2.WaitKey(0);
                Cv2.MorphologyEx(mask, mask, MorphTypes.Close, k, null, 1);
                //Cv2.ImShow("DebugS", mask);
                //Cv2.WaitKey(0);
                //Cv2.BitwiseAnd(hsv_split[0], mask, hsv_split[0]);
                //Cv2.ImShow("DebugH", hsv_split[0]);
                //Cv2.WaitKey(0);
                //Cv2.Threshold(hsv_split[0], mask, 14.5, 44.5, ThresholdTypes.Binary);
                //Cv2.ImShow("DebugH", mask);
                //Cv2.WaitKey(0);
                ////Cv2.ImShow("Debug", hsv_split[0]);
                ////Cv2.WaitKey(0);
                ////Cv2.ImShow("Debug", hsv_split[1]);
                ////Cv2.WaitKey(0);
                //Cv2.ImShow("DebugV", hsv_split[2]);
                //Cv2.WaitKey(0);
                //Cv2.Threshold(hsv_split[2], mask, 135, 255, ThresholdTypes.Binary);
                //Cv2.ImShow("DebugV", mask);
                //Cv2.WaitKey(0);
                //Cv2.DestroyAllWindows();
                Mat _mean = new();
                Mat _std = new();
                hsv_img.MeanStdDev(_mean, _std, mask);
                double[] __m;
                double[] __s;
                _mean.GetArray<double>(out __m);
                _std.GetArray<double>(out __s);
                double minV = -1;
                double maxV = -1;
                hsv_split[0].MinMaxIdx(out minV, out maxV, new int[] { }, new int[] { }, mask);
                hsv_split[1].MinMaxIdx(out minV, out maxV, new int[] { }, new int[] { }, mask);
                hsv_split[2].MinMaxIdx(out minV, out maxV, new int[] { }, new int[] { }, mask);
                //Mat v_adj = new Mat();
                //hsv_split[2].ConvertTo(v_adj, MatType.CV_32FC1, 1, __s[2]);
                //v_adj.ConvertTo(v_adj, -1, 58.98 / __s[2], 157.0);
                //v_adj.ConvertTo(hsv_split[2], MatType.CV_8UC1, 1, 0);
                //double mean = (double)hsv_split[2].Mean().Val0;
                double dif = refMean - __m[2];

                //if(dif > 0) dif *= -1.2;
                //else dif /= -1.2;
                hsv_split[2].ConvertTo(hsv_split[2], -1, 1, dif);
                Cv2.Merge(hsv_split, outMat);
                Cv2.CvtColor(outMat, outMat, ColorConversionCodes.HSV2BGR_FULL);
                //Cv2.ImShow("Debug", outMat);
                //Cv2.WaitKey(0);
                //Cv2.DestroyAllWindows();
                //Cv2.GaussianBlur(outMat, outMat, new Size(3, 3), 1);
                foreach (Mat s in hsv_split)
                    s.Dispose();
            }
        }
    }
}
