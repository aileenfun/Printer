using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Printer.ESC_Set
{
    public class Graphic : BaseESC
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port"></param>
        /// <param name="printer_type"></param>
        public Graphic(Port port, PRINTER_TYPE printer_type) 
            :base(port, printer_type)
        {
		    
	    }

        public bool linedrawOut(ESC.LINE_POINT[] line_points)
        {
            byte line_count = (byte)line_points.Length;
            if (line_count > 8)
                return false;
            byte[] cmd = { 0x1D, 0x27, 0 };
            cmd[2] = line_count;
            if (!port.write(cmd))
                return false;
            byte[] data = { 0, 0, 0, 0 };
            for (int i = 0; i < line_count; i++)
            {
                if (line_points[i].startPoint < 0)
                    line_points[i].startPoint = 0;
                if (line_points[i].startPoint >= maxDots)
                    line_points[i].startPoint = maxDots - 1;
                data[0] = (byte)line_points[i].startPoint;
                data[1] = (byte)(line_points[i].startPoint >> 8);
                data[2] = (byte)line_points[i].endPoint;
                data[3] = (byte)(line_points[i].endPoint >> 8);
                if (!port.write(data))
                    return false;
            }
            return true;
        }
    }
}
