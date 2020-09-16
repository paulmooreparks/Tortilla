using System;
using Tortilla;

/* 
IntervalTree implementation by Ido Ran 
https://archive.codeplex.com/?p=intervaltree
http://dotdotnet.blogspot.com/2011/06/interval-tree-c-implementation.html
BSD License
*/

namespace Tortilla {
    public class Bus<DataType> : Tortilla.IDataBus<DataType> {
        public DataType Value { get; set; }
    }
}
