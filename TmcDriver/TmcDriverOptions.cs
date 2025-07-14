using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.TmcDriver
{
    public class TmcDriverOptions {
        public static string SectionName = "TmcDriver"; 
        public TmcUartOptions[] TmcUarts { get; set; } = [];
        public TmcDriverIcOptions[] TmcDriverIcs { get; set; } = [];
    }
}
