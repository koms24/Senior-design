using MachineVision;
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.DataType;
using SeniorDesignFall2024.MachineVision.Extension;
using SeniorDesignFall2024.MachineVision.ImageOp;
using SeniorDesignFall2024.MachineVision.Tf;
using SeniorDesignFall2024.MachineVision.Tf.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MachineVisionTest
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var bgr2hsvOp = new CvtColorOp(null, new SeniorDesignFall2024.MachineVision.ImageOp.Options.CvtColorOpOptions() { EmitResult = false });
            IServiceCollection servicesBuilder = new ServiceCollection();
            servicesBuilder.AddSingleton(bgr2hsvOp);
            servicesBuilder.AddSingleton<MeanStdDevOp>();
            IServiceProvider services = servicesBuilder.BuildServiceProvider();

            Mat kernel = new Mat(9, 9, MatType.CV_8U, new Scalar(1));

            Mat ref_img_orig = Cv2.ImRead("plant130_rgb.png");
            Mat ref_img = new(ref_img_orig.Size(), ref_img_orig.Type());
            ref_img_orig.CopyTo(ref_img);
            //MeanStdDevOp meanStdOp = services.GetRequiredService<MeanStdDevOp>();
            //ImageData ref_idat = new ImageData(new[] { ref_img }, null);
            //meanStdOp.Run(ref_idat);

            
            //Mat ref_img_hsv = new Mat(ref_img.Size(), ref_img.Type());
            //Cv2.CvtColor(ref_img, ref_img_hsv, ColorConversionCodes.BGR2HSV_FULL);
            //Mat[] ref_img_hsv_split = ref_img_hsv.Split();
            //Mat _r_mean = new();
            //Mat _r_std = new();
            //Cv2.MeanStdDev(ref_img_hsv, _r_mean, _r_std);
            //double[] __r_mean;
            //double[] __r_std;
            //_r_mean.GetArray<double>(out __r_mean);
            //_r_std.GetArray<double>(out __r_std);
            //double r_mean = (double)ref_img_hsv_split[2].Mean();

            Cv2.CopyMakeBorder(ref_img, ref_img, 512, 512, 512+15, 512+15, BorderTypes.Constant, new Scalar(0, 0, 0));
            //Console.WriteLine($"Ref Mean - V Channel: {r_mean}");

            Mat img = Cv2.ImRead("test.jpeg");
            img = img.CopyMakeBorder(512,512,512,512,BorderTypes.Constant,new Scalar(0, 0, 0));

            ImageData idat = new ImageData(new[] { img }, null);
            var op = new ExtractPlantBlobsOp(null);
            op.Run(idat, out var bmap);
            var blobData = bmap.First();
            var lf = new LeafInferMaskRcnnOp(null);
            Dictionary<double, int> dcnt = new();
            for (double i = 156; i <= 180; i += 1) {
                if (((int)i) % 20 == 0) Console.WriteLine($"idx: {i}");
                using (Mat blobAdj = new Mat())
                {
                    blobData.SourceMat.MatchToRefMean(i, blobAdj);

                    var bmats = blobAdj.ToTiledMat(blobData.SourceRoiRect, 256).Select(m => { /*m.MatchToRefMean(r_mean, m);*/ return m; }).ToArray();

                    var rdat = new ImageData(bmats, null);
                    lf.Run(rdat);
                    SdTfInferReturnArray result = rdat.GetOpDataLastForKeyStrict<LeafInferMaskRcnnOp, SdTfInferReturnArray>(LeafInferMaskRcnnOp.OpDataKey_InferBatchResult);
                    int s = result.Data.Sum(o => { float[] v; o.detection_scores.GetArray<float>(out v); return v.Select(a => a > 0.85 ? 1 : 0).Append(0).Sum(); });
                    if (s > 0)
                    {
                        Console.WriteLine($"idx: {i}");
                        rdat.ShowBatchInferResults((float)0.85);
                    }
                    dcnt[i] = s;
                }
            }
            foreach(var kp in dcnt.OrderBy(o=>o.Value)) {
                Console.WriteLine($"mean: {kp.Key} :: cnt: {kp.Value}");
            }
            
            Console.WriteLine("Done");
        }
    }
}
