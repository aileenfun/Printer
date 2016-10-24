using System;

namespace Printer.JPL_Set
{
    public class Page : BaseJPL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        public Page(JPL_Param param)
            : base(param)
        {
        }
        /// <summary>
        /// 开始页面
        /// </summary>
        /// <param name="originX">原点x坐标</param>
        /// <param name="originY">原点y坐标</param>
        /// <param name="pageWidth">页面宽度，单位dot,1dot =0.125mm</param>
        /// <param name="pageHeight">页面高度</param>
        /// <param name="rotate">选择角度</param>
        /// <returns></returns>
        public bool start(int originX, int originY, int pageWidth, int pageHeight, JPL.PAGE_ROTATE rotate)
        {
            if (originX < 0 || originX > 575)
                return false;
            if (originY < 0)
                return false;
            if (pageWidth < 0 || pageWidth > 576)
                return false;
            if (pageHeight < 0)
                return false;
            param.pageWidth = pageWidth;
            param.pageHeight = pageHeight;
            byte[] cmd = { 0x1A, 0x5B, 0x01 };
            if (!port.write(cmd))
                return false;
            if (!port.write((UInt16)originX))
                return false;
            if (!port.write((UInt16)originY))
                return false;
            if (!port.write((UInt16)pageWidth))
                return false;
            if (!port.write((UInt16)pageHeight))
                return false;
            return port.write((byte)rotate);
        }

        /// <summary>
        /// 绘制打印页面结束
        /// </summary>
        /// <returns></returns>
        public bool end()
        {
            byte[] cmd = new byte[] { 0x1A, 0x5D, 0x00 };
            return port.write(cmd);
        }

        /// <summary>
        /// 打印页面内容 之前做的页面处理，只是把打印对象画到内存中，必须要通过这个方法把内容打印出来
        /// </summary>
        /// <returns></returns>
        public bool print()
        {
            byte[] cmd = new byte[] { 0x1A, 0x4F, 0x00 };
            return port.write(cmd);
        }
    }
}