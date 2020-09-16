using System;

/* 
IntervalTree implementation by Ido Ran 
https://archive.codeplex.com/?p=intervaltree
http://dotdotnet.blogspot.com/2011/06/interval-tree-c-implementation.html
BSD License
*/

namespace Tortilla {
    public class StepCounter : Register<byte> {
        public override void OnTick(ClockState state) {
            if (state == ClockState.TickOn) {
                ++Value;
            }
        }
    }
}
