using SeniorDesignFall2024.MachineVision.DataType;
using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.Tf.Internal;
using SeniorDesignFall2024.MachineVision.ImageOp;

namespace SeniorDesignFall2024.MachineVision.Extension
{
    public static class DebugExtension
    {
        public static void ShowBatchInferResults(this ImageData dat, float? minScore = null, HashSet<int>? classFilter = null)
        {
            SdTfInferReturnArray result = dat.GetOpDataLastForKeyStrict<LeafInferMaskRcnnOp, SdTfInferReturnArray>(LeafInferMaskRcnnOp.OpDataKey_InferBatchResult);
            IEnumerable<Mat> batch = dat.GetLatestHistory<LeafInferMaskRcnnOp>().Select(d=>d.Latest); //dat.Mats.First().BatchMatToMats();
            int winCnt = 0;
            var tmpOut = batch.Zip(result.Data, (m, r) =>
            {
                Mat tmp = new Mat(m.Size(), m.Type());
                m.CopyTo(tmp);
                int dstHeight = tmp.Height;
                int dstWidth = tmp.Width;
                DecodedInferResult d = new(r);
                using (var itBoxes = d.BBoxes.GetEnumerator())
                using (var itMasks = d.Masks.GetEnumerator())
                {
                    int srcHeight = 256;    //d.ImageInfo.ImageRange.top
                    int srcWidth = 256;     //d.ImageInfo.ImageRange.right
                    for (int i = 0, l = d.DetectionCnt; i < l; i++)
                    {
                        if ((minScore == null || d.Scores[i] >= minScore) && (classFilter == null || classFilter.Contains(d.Classes[i])))
                        {
                            Cv2.Rectangle(tmp, itBoxes.Current.ToRect(srcHeight, srcWidth, dstHeight, dstWidth), new Scalar(0, 0, 255), 2);
                        }
                        itBoxes.MoveNext();
                        itMasks.MoveNext();
                    }
                }
                return tmp;
            }).ToArray();
            for(int i = 0, l=tmpOut.Count(); i<l; i++)
                Cv2.ImShow($"Debug:ShowBatchInferResults:{winCnt++}", tmpOut[i]);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
            foreach(var m in tmpOut)
                m.Dispose();
        }
    }
}
