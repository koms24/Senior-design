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
    public class ImageDataSingle : IDisposable
    {
        private Mat _orig;
        private Mat _current;
        private bool _ownsResourceTracker = false;
        private ResourcesTracker _resourcesTracker;
        private bool disposedValue;

        private ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> _ops_data = new();

        public Mat Latest => _current;
        public ResourcesTracker Tracker => _resourcesTracker;
        public ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> OpsData => _ops_data;

        public ImageDataSingle(Mat input, ResourcesTracker? resourcesTracker)
        {
            _orig = input;
            _current = input;
            _ownsResourceTracker = resourcesTracker == null;
            _resourcesTracker = resourcesTracker ?? new ResourcesTracker();
        }

        public ConcurrentDictionary<string, object> GetOpData<T>() where T : IImageOp
        {
            return _ops_data.GetOrAdd(typeof(T), t => new ConcurrentDictionary<string, object>());
        }

        public ConcurrentDictionary<string, object>? GetOpDataOrDefault<T>() where T : IImageOp
        {
            ConcurrentDictionary<string, object>? result = null;
            _ops_data.TryGetValue(typeof(T), out result);
            return result;
        }

        public bool HasOpData<T>() where T : IImageOp
        {
            return _ops_data.ContainsKey(typeof(T));
        }

        public R? GetOpDataForKeyOrDefault<T, R>(string k) where T : IImageOp where R : class
        {
            var d = _ops_data.GetOrAdd(typeof(T), t => throw new KeyNotFoundException(typeof(T).Name));
            object? o = null;
            d.TryGetValue(k, out o);
            //d.GetOrAdd(k, t => throw new KeyNotFoundException(k));
            return o as R;
        }

        public bool HasOpDataForKey<T>(string k) where T : IImageOp
        {
            ConcurrentDictionary<string, object>? d = null;
            _ops_data.TryGetValue(typeof(T), out d);
            if (d == null)
                return false;
            return d.ContainsKey(k);
        }

        public R GetOpDataForKeyStrict<T,R>(string k) where T : IImageOp where R : class
        {
            var d = _ops_data.GetOrAdd(typeof(T), t => throw new KeyNotFoundException(typeof(T).Name));
            object? o = d.GetOrAdd(k, t => throw new KeyNotFoundException(k));
            return o as R ?? throw new NullReferenceException();
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
        ~ImageDataSingle()
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
