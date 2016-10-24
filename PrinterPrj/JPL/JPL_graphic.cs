using System;
using System.Drawing;

namespace Printer.JPL_Set
{
    public class Graphic : BaseJPL
    {
        private byte[] cmd = new byte[16];
        public Graphic(JPL_Param param)
            : base(param)
        {
        }
        /*
         * 在页面内绘制线段 
         */
        public bool line(Point start, Point end, int width, COLOR color)
        {
            byte[] cmd = { 0x1A, 0x5C, 0x01 };
            port.write(cmd);
            port.write((UInt16)start.X);
            port.write((UInt16)start.Y);
            port.write((UInt16)end.X);
            port.write((UInt16)end.Y);
            port.write((UInt16)width);
            return port.write((byte)color);
        }

        public bool line(int start_x, int start_y, int end_x, int end_y, int width)
        {
            cmd[0] = 0x1A; cmd[1] = 0x5C; cmd[2] = 0x01;
            cmd[3] = (byte)(start_x); cmd[4] = (byte)(start_x >> 8);
            cmd[5] = (byte)(start_y); cmd[6] = (byte)(start_y >> 8);
            cmd[7] = (byte)(end_x); cmd[8] = (byte)(end_x >> 8);
            cmd[9] = (byte)(end_y); cmd[10] = (byte)(end_y >> 8);
            cmd[11] = (byte)(width); cmd[12] = (byte)(width >> 8);
            cmd[13] = (byte)COLOR.Black;

            return port.write(cmd, 0, 14);
        }       

        public bool line(Point start, Point end, int width)
        {
            return line(start, end, width, COLOR.Black);
        }

        public bool line(Point start, Point end)
        {
            byte[] cmd = { 0x1A, 0x5C, 0x00 };
            port.write(cmd);
            port.write((UInt16)start.X);
            port.write((UInt16)start.Y);
            port.write((UInt16)end.X);
            return port.write((UInt16)end.Y);
        }

        public bool rect(int left, int top, int right, int bottom)
        {
            byte[] cmd = { 0x1A, 0x26, 0x00 };
            port.write(cmd);
            port.write((UInt16)left);
            port.write((UInt16)top);
            port.write((UInt16)right);
            return port.write((UInt16)bottom);
        }

        public bool rect(int left, int top, int right, int bottom, int width, COLOR color)
        {
            byte[] cmd = { 0x1A, 0x26, 0x01 };
            port.write(cmd);
            port.write((UInt16)left);
            port.write((UInt16)top);
            port.write((UInt16)right);
            port.write((UInt16)bottom);
            port.write((UInt16)width);
            return port.write((byte)color);
        }

        public bool rectFill(int left, int top, int right, int bottom, COLOR color)
        {
            byte[] cmd = new byte[] { 0x1A, 0x2A, 0x00 };
            port.write(cmd);
            port.write((UInt16)left);
            port.write((UInt16)top);
            port.write((UInt16)right);
            port.write((UInt16)bottom);
            return port.write((byte)color);
        }
    }
}