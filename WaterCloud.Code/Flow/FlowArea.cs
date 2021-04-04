using System;
using System.Collections.Generic;
using System.Text;

namespace WaterCloud.Code
{
    public class FlowArea
    {
        public string id { get; set; }

        public string name { get; set; }

        public string color { get; set; }

        public int left { get; set; }
        public int top { get; set; }

        public int width { get; set; }
        public int height { get; set; }
        public bool alt { get; set; }
    }
}
