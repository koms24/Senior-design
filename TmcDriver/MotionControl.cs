using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.TmcDriver
{
    public class MotionControl
    {
        public MotionControl() { }

        public Task<bool> MoveStraight(int distance_in_mm) {

            return Task.FromResult<bool>(true);
        }
    }
}
