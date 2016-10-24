using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Printer.JPL_Set
{
    public class Image : BaseJPL
    {
        private byte[] _cmd = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public Image(JPL_Param param)
            : base(param)
        {
        }

        /// <summary>
        /// 根据数组内数据绘制图像,无修饰效果
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool drawOut(int x, int y, int width, int height, char[] data)
        {
            if (width < 0 || height < 0)
                return false;
            byte[] cmd = { 0x1A, 0x21, 0x00 };
            int HeightWriteUnit = 10;
            int WidthByte = ((width - 1) / 8 + 1);
            int HeightWrited = 0;
            int HeightLeft = height;

            while (true)
            {
                if (HeightLeft <= HeightWriteUnit)
                {   
                    port.write(cmd);
                    port.write((UInt16)x);
                    port.write((UInt16)y);
                    port.write((UInt16)width);
                    port.write((UInt16)HeightLeft);
                    int index = HeightWrited * WidthByte;
                    for (int i = 0; i < HeightLeft * WidthByte; i++)
                    {
                        port.write((byte)data[index++]);
                    }
                    return true;
                }
                else
                {
                    port.write(cmd);
                    port.write((UInt16)x);
                    port.write((UInt16)y);
                    port.write((UInt16)width);
                    port.write((UInt16)HeightWriteUnit);
                    int index = HeightWrited * WidthByte;
                    for (int i = 0; i < HeightWriteUnit * WidthByte; i++)
                    {
                        port.write((byte)data[index++]);
                    }
                    y += HeightWriteUnit;
                    HeightWrited += HeightWriteUnit;
                    HeightLeft -= HeightWriteUnit;
                }
            }
        }

        /// <summary>
        /// 根据数组内数据绘制图像，可根据参数设置修饰效果
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="data"></param>
        /// <param name="Reverse"></param>
        /// <param name="Rotate"></param>
        /// <param name="EnlargeX"></param>
        /// <param name="EnlargeY"></param>
        /// <returns></returns>
        public bool drawOut(int x, int y, int width, int height, byte[] data, bool Reverse, JPL.IMAGE_ROTATE Rotate, JPL.IMAGE_ENLARGE EnlargeX, JPL.IMAGE_ENLARGE EnlargeY)
        {
            if (width < 0 || height < 0)
                return false;
            int WidthByte = ((width - 1) / 8 + 1);
            int dataSize = WidthByte * height;
            if (dataSize != data.Length)
                return false;
            
            UInt16 ShowType = 0;
            if (Reverse) ShowType |= 0x0001;
            ShowType |= ((UInt16)(((UInt16)Rotate << 1) & 0x0006));
            ShowType |= (UInt16)(((UInt16)EnlargeX << 8) & 0x0F00);
            ShowType |= (UInt16)(((UInt16)EnlargeY << 12) & 0xF000);

            byte[] cmd = { 0x1A, 0x21, 0x01 };
            int HeightWriteUnit = 10;
            int HeightWrited = 0;
            int HeightLeft = height;

            _cmd[0] = 0x1A;
            _cmd[1] = 0x21;
            _cmd[2] = 0x01;
            _cmd[7] = (byte)(width);
            _cmd[8] = (byte)(width >> 8);
            _cmd[11] = (byte)(ShowType);
            _cmd[12] = (byte)(ShowType >> 8);                

            while (true)
            {
                _cmd[3] = (byte)(x);
                _cmd[4] = (byte)(x >> 8);
                _cmd[5] = (byte)(y);
                _cmd[6] = (byte)(y >> 8);

                if (HeightLeft > HeightWriteUnit)
                {
                    _cmd[9] = (byte)(HeightWriteUnit);
                    _cmd[10] = (byte)(HeightWriteUnit >> 8);
                    port.write(_cmd, 0, 13);

                    port.write(data, HeightWrited * WidthByte, HeightWriteUnit * WidthByte);
                    switch (Rotate)
                    {
                        case JPL.IMAGE_ROTATE.x0:
                            y += HeightWriteUnit * ((int)EnlargeX + 1);
                            break;
                        case JPL.IMAGE_ROTATE.x90:
                            x -= HeightWriteUnit * ((int)EnlargeY + 1);
                            break;
                        case JPL.IMAGE_ROTATE.x180:
                            y -= HeightWriteUnit * ((int)EnlargeX + 1);
                            break;
                        case JPL.IMAGE_ROTATE.x270:
                            x += HeightWriteUnit * ((int)EnlargeY + 1);
                            break;
                    }
                    HeightWrited += HeightWriteUnit;
                    HeightLeft -= HeightWriteUnit;
                }
                else
                {
                    _cmd[9] = (byte)(HeightLeft);
                    _cmd[10] = (byte)(HeightLeft >> 8);
                    port.write(_cmd,0,13);
                    port.write(data, HeightWrited * WidthByte, HeightLeft * WidthByte);
                    return true;
                }                
            }
        }

        /// <summary>
        /// 根据bitmap画出图像
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bitmap"></param>
        /// <param name="rotate"></param>
        /// <returns></returns>
        public bool drawOut(int x, int y, Bitmap bitmap, JPL.IMAGE_ROTATE rotate,JPL.IMAGE_ENLARGE ex,JPL.IMAGE_ENLARGE ey)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            if (width > param.pageWidth || height > param.pageHeight)
                return false;

            ImageConver conver = new ImageConver();
            byte[] data = conver.CovertImageHorizontal(bitmap, 128);
            if (data == null)
                return false;
            return drawOut(x, y, conver.Width, conver.Height, data, false, rotate, ex, ey);
        }

        /// <summary>
        /// 根据文件路径画出图像
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="image_path">图像路径</param>
        /// <param name="rotate">旋转方式</param>
        /// <returns></returns>
        public bool drawOut(int x, int y, string image_path, JPL.IMAGE_ROTATE rotate, JPL.IMAGE_ENLARGE ex, JPL.IMAGE_ENLARGE ey)
        {
            ImageConver conver = new ImageConver();
            byte[] data = conver.CovertImageHorizontal(image_path, 128);

            if (data == null)
                return false;

            return drawOut(x, y, conver.Width, conver.Height, data, false, rotate, ex, ey);
        }
    }
}