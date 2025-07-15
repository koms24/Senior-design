using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.Tf.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.Extension
{
    public struct BboxIntType
    {
        public int bottom;
        public int left;
        public int top;
        public int right;
        public BboxIntType(IEnumerable<int> values)
        {
            using(var iter = values.GetEnumerator())
            {
                bottom = iter.Current;
                iter.MoveNext();
                left = iter.Current;
                iter.MoveNext();
                top = iter.Current;
                iter.MoveNext();
                right = iter.Current;
            }
        }
        public BboxIntType(Mat m)
        {
            bottom = m.At<int>(0);
            left = m.At<int>(1);
            top = m.At<int>(2);
            right = m.At<int>(3);
        }
    }
    public struct ImageInfoType
    {
        public BboxIntType ImageRange { get; set; }
        public BboxIntType MaskRange { get; set; }
        public ImageInfoType(Mat d)
        {
            ImageRange = new BboxIntType(d.Row(0));
            MaskRange = new BboxIntType(d.Row(1));
        }
    }
    public struct BboxFloatType
    {
        public double bottom;
        public double left;
        public double top;
        public double right;
        public BboxFloatType(Mat m)
        {
            bottom = m.At<float>(2);
            left = m.At<float>(1);
            top = m.At<float>(0);
            right = m.At<float>(3);
        }
        public Rect ToRect(int srcHeight, int srcWidth, int dstHeight, int dstWidth, int offsetX = 0, int offsetY = 0) {
            double h = top - bottom;
            double w = right - left;
            double cy = h/2 + bottom;
            double cx = w/2 + left;
            double sy = (double)dstHeight / (double)srcHeight;
            double sx = (double)dstWidth / (double)srcWidth;
            return new Rect()
            {
                Y = offsetY + (int)Math.Floor((bottom - cy)*sy + cy),
                X = offsetX + (int)Math.Floor((left - cx)*sx + cx),
                Height = (int)Math.Ceiling(h*sy),
                Width = (int)Math.Ceiling(w*sx)
            };
        }
    }

    public class DecodedInferResult
    {
        private SdTfInferReturnType _data;
        public int DetectionCnt => _data.detection_cnt;
        private IEnumerable<BboxFloatType>? _bboxes = null;
        public IEnumerable<BboxFloatType> BBoxes {
            get {
                if (_bboxes == null)
                    _bboxes = Enumerable.Range(0, 100).Select(i => new BboxFloatType(_data.detection_boxes.Row(i)));
                return _bboxes;
            }
        }
        private float[]? _scores = null;
        public float[] Scores {
            get {
                if(_scores == null)
                    _data.detection_scores.GetArray<float>(out _scores);
                return _scores;
            }
        }
        private ImageInfoType? _imageInfo = null;
        public ImageInfoType ImageInfo {
            get {
                if (_imageInfo == null)
                    _imageInfo = new ImageInfoType(_data.image_info);
                return (ImageInfoType)_imageInfo;
            }
        }
        private int[]? _classes = null;
        public int[] Classes {
            get {
                if(_classes == null)
                    _data.detection_classes.GetArray<int>(out _classes);
                return _classes;
            }
        }
        private IEnumerable<Mat>? _masks = null;
        public IEnumerable<Mat> Masks {
            get {
                if (_masks == null)
                    _masks = Enumerable.Range(0, 100).Select(i => _data.detection_masks.Row(i));
                return _masks;
            }
        }

        public DecodedInferResult(SdTfInferReturnType d)
        {
            _data = d;
        }
    }
    public static class DataConverterExtension
    {
        public static BboxIntType ToIntBbox(this KeyPoint kp) 
        {
            double diameter = kp.Size;
            double radius = diameter / 2;
            return new BboxIntType() {
                bottom = (int)Math.Floor(kp.Pt.Y - radius),
                left = (int)Math.Floor(kp.Pt.X - radius),
                top = (int)Math.Ceiling(kp.Pt.Y + radius),
                right = (int)Math.Ceiling(kp.Pt.X + radius)
            };
        }

        public static IEnumerable<BboxIntType> ToIntBbox(this IEnumerable<KeyPoint> kp)
        {
            return kp.Select(ToIntBbox);
        }

        public static Rect ToRect(this BboxIntType bbox)
        {
            return new Rect()
            {
                Y = bbox.bottom,
                X = bbox.left,
                Width = (bbox.right - bbox.left),
                Height = (bbox.top - bbox.bottom)
            };
        }

        public static IEnumerable<Rect> ToRect(this IEnumerable<BboxIntType> bboxes)
        {
            return bboxes.Select(ToRect);
        }

        public static Rect Scale(this Rect rect, double scaleY, double scaleX, int minY = -1, int minX = -1, int maxY = -1, int maxX = -1)
        {
            double cx = ((double)rect.Width / 2) + (double)rect.X;
            double cy = ((double)rect.Height / 2) + (double)rect.Y;
            Rect scaled = new Rect()
            {
                Y = (int)(((double)rect.Y - cy) * scaleY + cy),
                X = (int)(((double)rect.X - cx) * scaleX + cx),
                Width = (int)((double)rect.Width * scaleX),
                Height = (int)((double)rect.Height * scaleY)
            };
            if (minY >= 0 && scaled.Y < minY) {
                scaled.Height -= (minY - scaled.Y);
                scaled.Y = minY;
            }
            if(maxY > 0 && (scaled.Y + scaled.Height) > maxY)
                scaled.Height -= ((scaled.Y + scaled.Height) - maxY);

            if (minX >= 0 && scaled.X < minX)
            {
                scaled.Width -= (minX - scaled.X);
                scaled.X = minX;
            }
            if (maxX > 0 && (scaled.X + scaled.Width) > maxX)
                scaled.Width -= ((scaled.X + scaled.Width) - maxX);
            return scaled;
        }

        public static IEnumerable<Rect> Scale(this IEnumerable<Rect> rects, double scaleY, double scaleX, int minY = -1, int minX = -1, int maxY = -1, int maxX = -1)
        {
            return rects.Select(r => r.Scale(scaleY, scaleX, minY, minX, maxY, maxX));
        }

        public static Rect ScaleTo(this Rect rect, int height, int width)
        {
            return rect.Scale((double)height/(double)rect.Height, (double)width/(double)rect.Width);
        }

        public static IEnumerable<Rect> ScaleTo(this IEnumerable<Rect> rects, int height, int width)
        {
            return rects.Select(r => r.ScaleTo(height, width));
        }

        public static IEnumerable<Rect> ToRect(this IEnumerable<float[]> bboxes)
        {
            return bboxes.Select(v => new Rect()
            {
                Y = (int)Math.Floor(v[0]),
                X = (int)Math.Floor(v[1]),
                Width = (int)Math.Ceiling(v[3] - v[1]),
                Height = (int)Math.Ceiling(v[2] - v[0])
            });
        }

        public static Rect SizeTo(this Rect rect, int height, int width)
        {
            double cx = ((double)rect.Width / 2) + (double)rect.X;
            double cy = ((double)rect.Height / 2) + (double)rect.Y;
            double sx = ((double)rect.Width / (double)width);
            double sy = ((double)rect.Height / (double)height);
            return new Rect()
            {
                Y = (int)Double.Round(((double)rect.Y - cy) * sy + cy),
                X = (int)Double.Round(((double)rect.X - cx) * sx + cx),
                Height = height,
                Width = width
            };
        }
        // 256 -> 4, 8, 16, 32, 64, 128       256
        // 320 ->    8, 16, 32, 64, 128,           320
        // 384 ->       16, 32, 64, 128, 192,           384
        // 448 ->           32, 64, 128, 192,                448
        // 512 ->               64, 128, 192, 256,                512

        //         0    1    2    3    4    5    6
        //     
        // 256 ->  4,   8,  16,  32,  64, 128, 256
        // 320 ->  8,  16,  32,  64,           320
        // 384 -> 16,  32,  64, 128, 192,      384
        // 448 -> 32,  64, 128, 192,           448
        // 512 -> 64, 128, 192, 256,           512
        public static IEnumerable<Rect> AutoPadForStrideSq(this Rect rect, int minWindow, int maxWindow, int baseStep, int minStrideMult = 2)
        {
            if (baseStep % 16 != 0 || baseStep % minStrideMult != 0 || minStrideMult%2 != 0 || minWindow % baseStep != 0 || maxWindow % baseStep != 0)
                throw new ArgumentException("Check Inputs");
            if(rect.Height <= minWindow && rect.Width <= minWindow)
                yield return rect.SizeTo(minWindow, minWindow);
            else if(rect.Height <= minWindow)
            {
                int newHeight = minWindow;
            }
            else if(rect.Width <= minWindow)
            {
                int newWidth = minWindow;
            }
            else
            {
                
            }
        }

        public static Rect ScaleToTileWindow(this KeyPoint kp, int windowSize = 512)
        {
            int diameter = (int)Double.Ceiling(kp.Size/windowSize)*windowSize;
            int radius = diameter / 2;
            return new Rect()
            {
                Y = (int)Math.Round(kp.Pt.Y - radius),
                X = (int)Math.Round(kp.Pt.X - radius),
                Height = diameter,
                Width = diameter
            };
        }

        public static IEnumerable<Mat> ToTiledMat(this Mat m, Rect rect, int windowSize)
        {
            if (m.Dims!=2)
                throw new ArgumentException(nameof(m));
            for (int i = 0, il = rect.Height / windowSize; i < il; i++)
            {
                for(int j=0, jl = rect.Width / windowSize; j < jl; j++)
                {
                    yield return new Mat(m, new Rect()
                    {
                        Y = rect.Y + windowSize*i,
                        X = rect.X + windowSize*j,
                        Height = windowSize,
                        Width = windowSize
                    });
                }
            }
        }

        public static IEnumerable<Mat> BatchMatToMats(this Mat m)
        {
            if(m.Dims < 3)
                throw new ArgumentException(nameof(m));
            int batchSize = m.Size(0);
            return Enumerable.Range(0, batchSize).Select(i => m.Row(i));
        }

        public static Rect PadForStride(this Rect rect, int windowY, int windowX, int strideY, int strideX)
        {
            if(strideY%2!=0)
                throw new ArgumentOutOfRangeException($"{nameof(strideY)} must be divisable by 2");
            if(strideX % 2 != 0)
                throw new ArgumentOutOfRangeException($"{nameof(strideX)} must be divisable by 2");
            if (windowY % strideY != 0)
                throw new ArgumentOutOfRangeException($"{nameof(windowY)} must be divisable by {nameof(strideY)}");
            if (windowX % strideX != 0)
                throw new ArgumentOutOfRangeException($"{nameof(windowX)} must be divisable by {nameof(strideX)}");
            var padded = new Rect()
            {
                Y = rect.Y,
                X = rect.X,
                Height = rect.Height,
                Width = rect.Width
            };
            bool chkH = padded.Height < windowY;
            bool chkW = padded.Width < windowX;
            int rH = padded.Height % strideY;
            int rW = padded.Width % strideY;
            if(chkH || chkW || rH!=0 || rW!=0)
            {
                int newHeight = padded.Height;
                if (chkH) {
                    newHeight = windowY;
                    rH = 0;
                }
                int newWidth = padded.Width;
                if (chkW) { 
                    newWidth = windowX;
                    rW = 0;
                }
                if(rH!=0) {
                    if (Double.Round((double)rH / (double)strideY) == 1)
                        newHeight += (strideY - rH);
                    else
                        newHeight -= rH;
                }
                if(rW!=0) { 
                    if(Double.Round((double)rW / (double)strideX) == 1)
                        newWidth += (strideX - rW);
                    else
                        newWidth -= rW;
                }
                padded = padded.SizeTo(newHeight, newWidth);
            }
            return padded;
        }

        public static IEnumerable<Rect> PadForStride(this IEnumerable<Rect> rects, int windowY, int windowX, int strideY, int strideX)
        {
            return rects.Select(r => r.PadForStride(windowY, windowX, strideY, strideX));
        }

        public static void DebugInfer(this SdTfInferReturnArray result, Mat img, string title = "Debug")
        {
            var smat = result.Data[0].detection_scores;
            var bmat = result.Data[0].detection_boxes;
            float[] scores = Enumerable.Range(0, 100).Select(i => smat.At<float>(i)).Where(v => v > 0.85).ToArray();
            float[][] boxes = new float[scores.Length][];
            for (int i = 0; i < scores.Length; i++)
            {
                boxes[i] = new float[] { bmat.At<float>(i, 0), bmat.At<float>(i, 1), bmat.At<float>(i, 2), bmat.At<float>(i, 3) };
            }
            var rects = boxes.ToRect();
            foreach (var rect in rects)
                Cv2.Rectangle(img, rect, new Scalar(0, 0, 255), 2);
            //img = img.CvtColor(ColorConversionCodes.RGB2BGR);
            Cv2.ImShow(title, img);
            //Cv2.WaitKey(0);
        }

        public static Mat ToBatchMat(this IEnumerable<Mat> mats, int batchHeight = 0, int batchWidth = 0)
        {
            int batchSize = mats.Count();
            if(batchSize <= 0)
                throw new ArgumentOutOfRangeException($"Error: {nameof(batchSize)} empty");
            Mat first = mats.First();
            int height = batchHeight<=0 ? first.Height : batchHeight;
            int width = batchWidth<=0 ? first.Width : batchWidth;
            MatType mtype = first.Type();
            Mat batch = new Mat(new[] { batchSize, height, width }, mtype);
            int i = 0;
            foreach(var m in mats) {
                if (m.Height != height || m.Width != width || m.Type() != mtype)
                    throw new Exception("Matrix mismatch, all mats must have matching dims and type");

                Mat mtmp = batch.Row(i);
                //m.Resize(new Size(width, height), 0,0,InterpolationFlags.Area).CopyTo(mtmp);
                m.CopyTo(mtmp);
                i++;
            }
            return batch;
        }

        public static DecodedInferResult Decode(this SdTfInferReturnType r)
        {
            return new DecodedInferResult(r);
        }
    }
}
