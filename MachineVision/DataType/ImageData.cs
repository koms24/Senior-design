using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.DataType
{
    public class ImageData : IDisposable
    {
        private IEnumerable<ImageDataSingle> _orig;
        private int _historyIdx;
        private ConcurrentDictionary<Type,IEnumerable<IEnumerable<ImageDataSingle>>> _history = new();
        private IEnumerable<KeyValuePair<int, Type>> _historyIndex = Enumerable.Empty<KeyValuePair<int, Type>>();
        private IEnumerable<ImageDataSingle> _current;
        private bool _ownsResourceTracker = false;
        private ResourcesTracker _resourcesTracker;
        private bool disposedValue;

        private ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> _ops_data = new();
        private ConcurrentDictionary<Mat, ImageDataSingle> _img_map = new();

        public IEnumerable<ImageDataSingle> Latest => _current;
        public Mat LatestMat => _current.Last().Latest;
        public IEnumerable<Mat> Mats => _current.Select(o => o.Latest);
        public ResourcesTracker Tracker => _resourcesTracker;
        public Mat OrigFirstMat => _orig.First().Latest;

        public ImageData(IEnumerable<Mat> input, ResourcesTracker? resourcesTracker)
        {
            _resourcesTracker = resourcesTracker ?? new ResourcesTracker();
            _ownsResourceTracker = resourcesTracker == null;
            _orig = input.Select(m=> {
                var i = new ImageDataSingle(m, resourcesTracker);
                _img_map[m] = i;
                return i;
            }).ToArray();
            _current = _orig;
        }

        public void Result(IEnumerable<Mat> input, ResourcesTracker? resourcesTracker = null)
        {
            _current = input.Select(m =>
            {
                var i = new ImageDataSingle(m, resourcesTracker);
                _img_map[m] = i;
                return i;
            }).ToArray();
        }

        public void Result<T>(IEnumerable<Mat> input, ResourcesTracker? resourcesTracker = null, bool addCurrentToHistory = false) where T : IImageOp {
            if (addCurrentToHistory) {
                Type op_type = typeof(T);
                _history.AddOrUpdate(op_type, t => (new[] { _current }).AsEnumerable(), (t, c) => c.Append(_current));
                _historyIndex = _historyIndex.Append(new KeyValuePair<int, Type>(_historyIdx++, op_type));
            }
            Result(input, resourcesTracker);
        }

        public IEnumerable<ImageDataSingle> GetLatestHistory<T>() where T : IImageOp
        {
            Type op_type = typeof(T);
            int idx = _historyIndex.Reverse().First(kp => kp.Value == op_type).Key;
            IEnumerable<IEnumerable<ImageDataSingle>>? result = null;
            _history.TryGetValue(op_type, out result);
            if (result == null)
                throw new Exception();
            return result.Skip(idx).First();
        }

        public ConcurrentDictionary<string, object> GetOpDataLast<T>(Mat? img) where T : IImageOp
        {
            Type t = typeof(T);
            if (img != null)
                return _img_map.GetOrAdd(img, v => throw new InvalidDataException("Image not in internal image map"))
                    .OpsData.GetOrAdd(t, x => {
                        var new_dict = new ConcurrentDictionary<string, object>();
                        _ops_data.AddOrUpdate(t, new_dict, (y, z) => new_dict);
                        return new_dict;
                    });
            return _ops_data.GetOrAdd(t, v => throw new KeyNotFoundException(t.Name));
        }

        public IEnumerable<KeyValuePair<Mat, ConcurrentDictionary<string, object>>> GetOpData<T>() where T : IImageOp
        {
            Type t = typeof(T);
            if (!_ops_data.ContainsKey(t))
                throw new KeyNotFoundException(t.Name);
            return _img_map.Where(kp=>kp.Value.HasOpData<T>()).Select(kp => new KeyValuePair<Mat, ConcurrentDictionary<string, object>>(kp.Key, kp.Value.GetOpDataOrDefault<T>()));
        }

        public bool HasOpData<T>() where T : IImageOp
        {
            return _ops_data.ContainsKey(typeof(T));
        }

        public R GetOpDataLastForKeyStrict<T,R>(string k, Mat? img = null) where T : IImageOp where R : class
        {
            Type t = typeof(T);
            if(img != null)
                return _img_map.GetOrAdd(img, v => throw new InvalidDataException("Image not in internal image map"))
                    .GetOpDataForKeyOrDefault<T, R>(k) ?? throw new KeyNotFoundException(k);
            var d = _ops_data.GetOrAdd(t, v => throw new KeyNotFoundException(t.Name));
            object? o = d.GetOrAdd(k, v => throw new KeyNotFoundException(k));
            return o as R ?? throw new NullReferenceException();
        }

        public IEnumerable<KeyValuePair<Mat,R>> GetOpDataForKey<T, R>(string k, Mat? img) where T : IImageOp where R : class
        {
            Type t = typeof(T);
            if (!_ops_data.ContainsKey(t))
                throw new KeyNotFoundException(t.Name);
            return _img_map.Where(v => v.Value.HasOpDataForKey<T>(k)).Select(kp => new KeyValuePair<Mat, R>(kp.Key, kp.Value.GetOpDataForKeyStrict<T, R>(k)));
        }

        public R GetOpDataForKeyString<T, R>(Mat img, string k) where T : IImageOp where R : class
        {
            return _img_map.GetValueOrDefault(img)?.GetOpDataForKeyOrDefault<T,R>(k) ?? throw new KeyNotFoundException("image Mat not found");
        }

        public void AddOpDataForKeyLast<T, R>(Mat img, string k, R data) where T : IImageOp where R : class
        {
            Type t = typeof(T);
            var d = _img_map.GetOrAdd(img, i =>
            {
                var id = new ImageDataSingle(i, _resourcesTracker);
                _current = _current.Prepend(id);
                return id;
            });
            d.OpsData.AddOrUpdate(t,
                v => {
                    ConcurrentDictionary<string, object> tmp = new()
                    {
                        [k] = data
                    };
                    _ops_data.AddOrUpdate(t, tmp, (tv, v) =>
                    {
                        v.AddOrUpdate(k, data, (s, o) => data);
                        return v;
                    });
                    return tmp;
                },
                (tv, v) =>
                {
                    v.AddOrUpdate(k, data, (s, o) => data);
                    _ops_data.AddOrUpdate(t, v, (tv, v) =>
                    {
                        v.AddOrUpdate(k, data, (s, o) => data);
                        return v;
                    });
                    return v;
                });
        }

        public void AddOpDataForKeyLast<T, R>(IEnumerable<Mat> mats, string k, R data) where T : IImageOp where R : class
        {
            foreach (var img in mats)
            {
                Type t = typeof(T);
                var d = _img_map.GetOrAdd(img, i =>
                {
                    var id = new ImageDataSingle(i, _resourcesTracker);
                    _current = _current.Prepend(id);
                    return id;
                });
                d.OpsData.AddOrUpdate(t,
                    v =>
                    {
                        ConcurrentDictionary<string, object> tmp = new()
                        {
                            [k] = data
                        };
                        _ops_data.AddOrUpdate(t, tt => new ConcurrentDictionary<string, object>(tmp), (tv, vv) =>
                        {
                            vv.AddOrUpdate(k, data, (s, o) => data);
                            return vv;
                        });
                        return tmp;
                    },
                    (tv, v) =>
                    {
                        v.AddOrUpdate(k, data, (s, o) => data);
                        _ops_data.AddOrUpdate(t, tt => new ConcurrentDictionary<string, object>(v), (tv, vv) =>
                        {
                            vv.AddOrUpdate(k, data, (s, o) => data);
                            return vv;
                        });
                        return v;
                    });
            }
        }

        public void AddOpDataForKeyFirst<T, R>(Mat img, string k, R data) where T : IImageOp where R : class
        {
            Type t = typeof(T);
            var d = _img_map.GetOrAdd(img, i =>
            {
                var id = new ImageDataSingle(i, _resourcesTracker);
                _current = _current.Prepend(id);
                return id;
            });
            d.OpsData.AddOrUpdate(t,
                v => {
                    ConcurrentDictionary<string, object> tmp = new()
                    {
                        [k] = data
                    };
                    _ops_data.AddOrUpdate(t, tt => new ConcurrentDictionary<string, object>(tmp), (tv, vv) =>
                    {
                        vv.AddOrUpdate(k, data, (s, o) => o);
                        return vv;
                    });
                    return tmp;
                },
                (tv, v) =>
                {
                    v.AddOrUpdate(k, data, (s, o) => data);
                    _ops_data.AddOrUpdate(t, tt => new ConcurrentDictionary<string, object>(v), (tv, vv) =>
                    {
                        vv.AddOrUpdate(k, data, (s, o) => o);
                        return vv;
                    });
                    return v;
                });
        }

        private void freeUnmanaged()
        {
            if(_ownsResourceTracker)
                _resourcesTracker.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                freeUnmanaged();
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~ImageData()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
