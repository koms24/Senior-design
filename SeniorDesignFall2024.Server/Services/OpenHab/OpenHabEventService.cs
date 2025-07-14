using Microsoft.OpenApi.Extensions;
using SeniorDesignFall2024.Server.Services.OpenHab.Types;
using System.Text.Json;
using System.Threading.Channels;

namespace SeniorDesignFall2024.Server.Services.OpenHab
{
    public class OpenHabEventService : IDisposable
    {
        public static readonly string[] EventStreamLineTypes = Enum.GetValues<EventStreamLineType>().Select(v => v.GetDisplayName()).ToArray();
        public static readonly TimeSpan Timeout = new TimeSpan(0, 0, 12);

        private string? _streamUid = null;
        private bool _eventStreamOpened = false;
        public string? StreamUid { get { return _streamUid; } }
        public bool EventStreamOpened { get { return _eventStreamOpened; } }

        private OpenHabComService _openHabComService;
        private OpenHabService _openHabService;
        private Stream _stream;
        private StreamReader _reader;
        private char[] buf = new char[64];

        public OpenHabEventService(OpenHabComService openHabComService, OpenHabService openHabService)
        {
            _openHabComService = openHabComService;
            _openHabService = openHabService;
        }

        public async Task Run(Stream stream, CancellationToken cancellationToken) {
            _streamUid = null;
            _eventStreamOpened = false;
            _stream = stream;
            //_stream.ReadTimeout = (int)Timeout.TotalMilliseconds;
            _reader = new StreamReader(stream);
            try {
                var sr = ReadStreamInit(cancellationToken).WaitAsync(Timeout, cancellationToken);
                var us = _openHabService.UpdateEventFilterList(() => this._streamUid, cancellationToken);
                await Task.WhenAll(sr, us);
            } catch (TaskCanceledException ex) {
                return;
            } catch (TimeoutException ex) {
                return;
            } catch (Exception ex) {
                return;
            }
            if (_streamUid == null)
                throw new FormatException("Stream id not found");
            _eventStreamOpened = true;
            
            try {
                await ReadStream(cancellationToken);
            } catch (TaskCanceledException ex) {
                return;
            } catch (TimeoutException ex) {
                return;
            } catch (Exception ex) {
                return;
            }
        }

        //protected enum EventStreamState
        //{
        //    Idle,
        //    Processing
        //}

        //public async Task<EventStreamAbstract> ParseDataLine(EventStreamLine l, List<EventStreamAbstract> msgLines)
        //{
        //    return l.type == EventStreamLineType.data && msgLines.Count <= 0 ? EventStreamStates.Parse(l) : l;
        //}

        //public async Task ProcessEventStream(CancellationToken cancellationToken)
        //{
        //    EventStreamAbstract line;
        //    DateTime lastReceived = DateTime.Now;
        //    Task? tout = null;
        //    EventStreamState state = EventStreamState.Idle;
        //    List<EventStreamAbstract> msgLines = new List<EventStreamAbstract>(3);
        //    do
        //    {
        //        do
        //        {
        //            if (cancellationToken.IsCancellationRequested)
        //                return;
        //            line = await ReadLine(cancellationToken);
        //            if (state == EventStreamState.Idle)
        //            {
        //                if (line.GetType() == typeof(EventStreamLine))
        //                {
        //                    EventStreamLine l = (EventStreamLine)line;
        //                    msgLines.Add(await ParseDataLine(l, msgLines));
        //                }
        //                else if (line is EventStreamLine)
        //                    msgLines.Add(line);
        //            }
        //            else if (state == EventStreamState.Processing)
        //            {
        //                if (line is EventStreamEndOfEvent)
        //                {
        //                    //emit event to channel
        //                    msgLines.Clear();
        //                }
        //                else if (line is EventStreamLine)
        //                {
        //                    EventStreamLine l = (EventStreamLine)line;
        //                    if (l.type == EventStreamLineType.data)
        //                        msgLines.Add(await ParseDataLine(l, msgLines));
        //                    else
        //                        msgLines.Add(l);
        //                }
        //                else
        //                    throw new Exception();
        //            }
        //            else
        //                throw new InvalidOperationException();

        //            if (DateTime.Now.Subtract(lastReceived) < Timeout)
        //                continue;
        //            break;
        //        } while (false);
        //        //throw new TimeoutException();
        //        //log timeout
        //        tout = _openHabService.EventStreamTimeout();
        //    } while (!cancellationToken.IsCancellationRequested);
        //}

        public async Task ReadStreamInit(CancellationToken cancellationToken) {
            _streamUid = null;
            int charsRead = 0;
            _openHabComService.StartTimer();
            //read from stream to start of message
            int retryCnt = 0;
            do {
                try {
                    charsRead = await _reader.ReadAsync(buf.AsMemory(0, 1), cancellationToken);
                } catch(TimeoutException ex) {
                    if (++retryCnt > 1)
                        throw new TimeoutException($"Timed out after retry #: {retryCnt}", ex);
                }
                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();
            } while (charsRead > 0 && buf[0] != 'e');
            if (charsRead <= 0)
                throw new FormatException("Start of event stream not found");
            retryCnt = 0;
            try {
                charsRead += await _reader.ReadAsync(buf.AsMemory(1, 12), cancellationToken);
            } catch(TimeoutException ex) {
                if (++retryCnt > 1)
                    throw new TimeoutException($"Timed out after retry #: {retryCnt}", ex);
            }
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();
            if (charsRead < 12 || !buf.Take(12).SequenceEqual("event: ready"))
                throw new FormatException("Start of event stream not found");
            EventStreamAbstract[] msg = [];
            try {
                msg = await ReadStreamMessage(cancellationToken);
            } catch(TimeoutException ex) {
                throw new TimeoutException("Timeout", ex);
            }
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();
            EventStreamLine? suid = null;
            if (msg.Length < 2 || msg.FirstOrDefault(m=>m is EventStreamId)==null || (suid = (EventStreamLine?)msg.FirstOrDefault(m=>m.GetType()==typeof(EventStreamLine)&&((EventStreamLine)m).type==EventStreamLineType.data))==null)
                throw new FormatException();
            _streamUid = suid.data;
        }
        public async Task ReadStream(CancellationToken cancellationToken)
        {
            EventStreamAbstract[] msg = [];
            do {
                try {
                    msg = await ReadStreamMessage(cancellationToken);
                } catch (TimeoutException ex) {
                    throw new TimeoutException("Timeout", ex);
                }
                if (msg.Length == 1 && msg[0] is EventStreamLine) {
                    await _openHabComService.ReceivedEventFromOpenHab((EventStreamLine)msg[0]);
                }
            } while (!cancellationToken.IsCancellationRequested);
            throw new TaskCanceledException();
        }

        public async Task<EventStreamAbstract[]> ReadStreamMessage(CancellationToken cancellationToken) {
            EventStreamAbstract line;
            List<EventStreamAbstract> msgLines= new List<EventStreamAbstract>(3);
            _openHabComService.ResetTimer();
            do { 
                try {
                    line = await ReadLine(cancellationToken);
                } catch (TimeoutException ex) {
                    throw new TimeoutException("Timeout", ex);
                }
                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();
                if(line is EventStreamEndOfEvent)
                    return msgLines.ToArray();
                if(!(line is EventStreamLine))
                    msgLines.Add(line);
                else { 
                    EventStreamLine l = (EventStreamLine)line;
                    if (l.type != EventStreamLineType.data)
                        msgLines.Add(l);
                    else {
                        if (msgLines.Count > 0) {
                            if(msgLines.FirstOrDefault(v => (v is EventStreamEvent) && ((EventStreamEvent)v).IsAlive) != null)
                                msgLines.Add(EventStreamKeepAlive.Parse(l));
                            else
                                msgLines.Add(l);
                        } else {
                            msgLines.Add(EventStreamStates.Parse(l));
                        }
                    }
                }
            } while(!cancellationToken.IsCancellationRequested);
            throw new TaskCanceledException();
        }

        public void CloseStream() {
            _streamUid = null;
            _eventStreamOpened = false;
            Dispose();
        }

        public async Task<EventStreamAbstract> ReadLine(CancellationToken cancellationToken)
        {
            int charsRead = 0;
            try {
                charsRead = await _reader.ReadAsync(buf.AsMemory(0, 1), cancellationToken);
            } catch (TimeoutException ex) {
                throw new TimeoutException("Timeout", ex);
            }
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();
            if (charsRead <= 0 || buf[0] == '\n')
                return new EventStreamEndOfEvent();
            if (buf[0] == 'i')
            {
                try {
                    charsRead += await _reader.ReadAsync(buf.AsMemory(1, 3), cancellationToken);
                } catch (TimeoutException ex) {
                    throw new TimeoutException("Timeout", ex);
                }
                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();
                if (charsRead == 4 && buf.Skip(1).Take(2).SequenceEqual("d:")) {
                    string? s;
                    try {
                        s = await _reader.ReadLineAsync(cancellationToken);
                    } catch (TimeoutException ex) {
                        throw new TimeoutException("Timeout", ex);
                    }
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();
                    return new EventStreamId
                    {
                        type = EventStreamLineType.id,
                        data = s
                    };
                } else
                    throw new FormatException($"No paser match for: '{buf}'");
            } else if (buf[0] == 'd') {
                try {
                    charsRead += await _reader.ReadAsync(buf.AsMemory(1, 5), cancellationToken);
                } catch (TimeoutException ex) {
                    throw new TimeoutException("Timeout", ex);
                }
                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();
                if (charsRead == 6 && buf.Skip(1).Take(4).SequenceEqual("ata:")) {
                    string? s;
                    try {
                        s = await _reader.ReadLineAsync(cancellationToken);
                    } catch (TimeoutException ex) {
                        throw new TimeoutException("Timeout", ex);
                    }
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();
                    return new EventStreamLine {
                        type = EventStreamLineType.data,
                        data = s
                    };
                } else
                    throw new FormatException($"No paser match for: '{buf}'");
            } else if (buf[0] == 'e' || buf[0] == 'r') {
                try {
                    charsRead += await _reader.ReadAsync(buf.AsMemory(1, 6), cancellationToken);
                } catch (TimeoutException ex) {
                    throw new TimeoutException("Timeout", ex);
                }
                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();
                if(charsRead != 7 || (buf[1] != 'e' && buf[1] != 'v'))
                    throw new FormatException($"No paser match for: '{buf.Take(7).ToString()}'");
                if (buf[1] == 'v') {
                    if (buf.Skip(2).Take(4).SequenceEqual("ent:")) {
                        string? s;
                        try {
                            s = await _reader.ReadLineAsync(cancellationToken);
                        } catch (TimeoutException ex) {
                            throw new TimeoutException("Timeout", ex);
                        }
                        if (cancellationToken.IsCancellationRequested)
                            throw new TaskCanceledException();
                        return new EventStreamEvent
                        {
                            type = EventStreamLineType.@event,
                            data = s
                        };
                    } else
                        throw new FormatException($"No paser match for: '{buf.Take(7).ToString()}'");
                } else {    //if (buf[1] == 'e')
                    if (buf.Skip(2).Take(4).SequenceEqual("try:")) {
                        string? s;
                        try {
                            s = await _reader.ReadLineAsync(cancellationToken);
                        } catch (TimeoutException ex) {
                            throw new TimeoutException("Timeout", ex);
                        }
                        if (cancellationToken.IsCancellationRequested)
                            throw new TaskCanceledException();
                        return new EventStreamRetry
                        {
                            type = EventStreamLineType.retry,
                            retry = int.Parse((s ?? "-1").Trim())
                        };
                    } else
                        throw new FormatException($"No paser match for: '{buf.Take(7).ToString()}'");
                }
            } else
                throw new FormatException($"No paser match for: '{buf.Take(7).ToString()}'");

        }

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader.Dispose();
            }
            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
            }
        }
    }
}
