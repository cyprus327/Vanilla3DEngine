using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanilla3DEngine.Classes {
    public class DeltaTime {
        public DeltaTime() {
            _t1 = DateTime.Now;
            _t2 = DateTime.Now;
        }

        private DateTime _t1;
        private DateTime _t2;

        public float Get() {
            _t2 = DateTime.Now;
            float deltaTime = (_t2.Ticks - _t1.Ticks) / 10000000f;
            _t1 = _t2;
            return deltaTime;
        }
    }
}
