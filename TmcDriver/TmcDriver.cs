using Microsoft.Extensions.Options;
using SeniorDesignFall2024.TmcDriver;
using SeniorDesignFall2024.TmcDriver.DI;
using SeniorDesignFall2024.TmcDriver.HAL.Tmc2209.Registers;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.TmcDriver
{
    public class TmcDriver : IDisposable {
        private TmcDriverOptions options;
        private TmcUartProvider uartProvider;
        private GpioController gpioController;

        private TmcDriverIc[] tmcDriverIcs;
        private Dictionary<string, TmcDriverIc> tmcDriverIcNameMap;
        private Dictionary<string, TmcDriverIc> tmcDriverIcUidMap;
        private Dictionary<string, IEnumerable<TmcDriverIc>> tmcDriverIcGroupMap;
        private Stopwatch stopwatch = new();


        public TmcDriver(
            IOptions<TmcDriverOptions> opt,
            TmcUartProvider tmcUartProvider,
            GpioController _gpioController
            ) {
            options = opt.Value;
            uartProvider = tmcUartProvider;
            gpioController = _gpioController;
            tmcDriverIcs = options.TmcDriverIcs.Select((o, i) => {
                TmcUart uart = (o.UartUid != null ? uartProvider.TryToGet(o.UartUid) : null) ?? uartProvider[0];
                TmcDriverIc ic = new TmcDriverIc(o, uart, i, gpioController);
                TmcDriverWrapper.tmcUartMap[(Int16)i] = uart;
                return ic;
                }).ToArray();
            tmcDriverIcNameMap = tmcDriverIcs.ToDictionary(o => o.Options.Name);
            tmcDriverIcUidMap = tmcDriverIcs.ToDictionary(o => o.Options.Uid);
            tmcDriverIcGroupMap = tmcDriverIcs.GroupBy(o=>o.Options.GroupName).ToDictionary(o=>o.Key, o => o.AsEnumerable());
            TmcDriverWrapper.Init((Int16)tmcDriverIcs.Length, tmcDriverIcs.Select(o => o.Options.GetUartAddress()).ToArray());
        }

        public void RunFor(string uid, int milliseconds, double speed = 1.0)
        {
            TmcDriverIc ic = tmcDriverIcUidMap[uid] ?? throw new Exception();
            if(!ic.IsInit)
                ic.Init();
            ic.RunFor(new TimeSpan(0,0,0,0,milliseconds), speed);
        }

        public void Move(string uid, int steps, double speed = 1.0)
        {
            TmcDriverIc ic = tmcDriverIcUidMap[uid] ?? throw new Exception();
            if (!ic.IsInit)
                ic.Init();
            ic.Move(steps, speed);
        }

        public void GroupRunFor(string groupName, int milliseconds, double speed = 1.0)
        {
            IEnumerable<TmcDriverIc> _ics = tmcDriverIcGroupMap[groupName] ?? throw new Exception();
            TmcDriverIc[] ics = _ics.ToArray();
            foreach(var ic in _ics)
            {
                if(!ic.IsInit)
                    ic.Init();
            }
            //foreach (var ic in ics)
            //{
            //    ic.SetSpeedRps(0);
            //    ic.SetAddress(true);
            //    ic.EnableDriver();
            //}
            //Thread.Sleep(4);
            //ics.First().RunFor(new TimeSpan(0, 0, 0, 0, milliseconds), speed);
            //foreach (var ic in ics)
            //{
            //    ic.SetAddress(false);
            //    ic.DisableDriver();
            //}
            var t = new TimeSpan(0, 0, 0, 0, milliseconds);
            TimeSpan[] ticks = new TimeSpan[ics.Length];
            //Task.WhenAll(ics.Select(ic => Task.Run(() => ic.RunFor(t, speed))));
            foreach (var ic in ics)
            {
                ic.SetSpeedRps(speed);
            }
            stopwatch.Restart();
            for(int i=0,l=ics.Length; i<l; i++)
            {
                ics[i].EnableDriver();
                ticks[i] = stopwatch.Elapsed + t;
            }
            int disableCnt = 0;
            TimeSpan timeout = stopwatch.Elapsed + 2 * t;
            do
            {
                for (int i = 0, l = ics.Length; i < l; i++)
                {
                    if (stopwatch.Elapsed >= ticks[i])
                    {
                        if (ics[i].IsEnabled)
                        {
                            ics[i].DisableDriver();
                            disableCnt++;
                        }
                    }
                }
                if (stopwatch.Elapsed >= timeout)
                {
                    for (int i = 0, l = ics.Length; i < l; i++)
                        if (ics[i].IsEnabled)
                            ics[i].DisableDriver();
                    break;
                }
            } while (disableCnt < ics.Length);
            stopwatch.Stop();
        }

        public void GroupMove(string groupName, int steps, double speed = 1.0)
        {
            IEnumerable<TmcDriverIc> _ics = tmcDriverIcGroupMap[groupName] ?? throw new Exception();
            TmcDriverIc[] ics = _ics.ToArray();
            foreach (var ic in _ics)
            {
                if (!ic.IsInit)
                    ic.Init();
            }
            //foreach (var ic in ics)
            //{
            //    ic.SetSpeedRps(0);
            //    ic.SetAddress(true);
            //    ic.EnableDriver();
            //}
            //Thread.Sleep(4);
            //ics.First().Move(steps, speed);
            //foreach (var ic in ics)
            //{
            //    ic.SetAddress(false);
            //    ic.DisableDriver();
            //}
            //Task.WhenAll(ics.AsParallel().Select(ic => Task.Run(() => ic.Move(steps, speed))).ToArray());
            foreach (var ic in ics)
            {
                ic.SetSpeedRps(speed);
            }
            TimeSpan t = TimeSpan.FromTicks((long)ics.First().CalculateTicksFromStepsForSpeed(steps, speed));
            TimeSpan[] ticks = new TimeSpan[ics.Length];
            stopwatch.Restart();
            for (int i = 0, l = ics.Length; i < l; i++)
            {
                ics[i].EnableDriver();
                ticks[i] = stopwatch.Elapsed + t;
            }
            int disableCnt = 0;
            TimeSpan timeout = stopwatch.Elapsed + 2 * t;
            do
            {
                for (int i = 0, l = ics.Length; i < l; i++)
                {
                    if (stopwatch.Elapsed >= ticks[i])
                    {
                        if (ics[i].IsEnabled)
                        {
                            ics[i].DisableDriver();
                            disableCnt++;
                        }
                    }
                }
                if(stopwatch.Elapsed >= timeout)
                {
                    for(int i=0,l = ics.Length; i < l; i++)
                        if (ics[i].IsEnabled)
                            ics[i].DisableDriver();
                    break;
                }
            } while (disableCnt < ics.Length);
            stopwatch.Stop();
        }

        public void Init(string uid)
        {
            TmcDriverIc ic = tmcDriverIcUidMap[uid] ?? throw new Exception();
            ic.Init();
        }

        public void Dispose() {

        }
    }
}
