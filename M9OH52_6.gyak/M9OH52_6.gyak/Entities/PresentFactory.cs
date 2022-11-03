using M9OH52_6.gyak.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M9OH52_6.gyak.Entities
{
    public class PresentFactory: IToyFactory
    {
        public Color RibbonColor { get; set; }
        public Color BoxColor { get; set; }
        public Toy CreateNew()
        {
            return new Present(RibbonColor,BoxColor);
        }
    }
}
