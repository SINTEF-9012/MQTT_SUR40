using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_SUR40
{
    public class MQTT_SUR40_Message
    {
        public int Id;
        public string Type;

        public double X;
        public double Y;
        public double Orientation;

        public double Width;
        public double Height;

        public string TagSchema;
        public string TagSeries;
        public string TagExtendedValue;
        public string TagValue;
    }
}
