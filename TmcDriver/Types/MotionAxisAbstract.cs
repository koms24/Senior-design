using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.TmcDriver.Types
{
    public enum MotionAxisDisplacementUnits { 
        mm,
        rad
    }

    public abstract class MotionAxisAbstract {

        protected DriverInstance[] drivers = [];

        public abstract MotionAxisDisplacementUnits Units { get; }


    }
}
