using System.Windows.Forms;

namespace Printer.ESC_Set
{
    public class Text : BaseESC
    {
        /*
         * 枚举类型：字体ID
         */
        public enum FONT_ID
        {
	        ASCII_12x24 = 0x00,                     
            ASCII_8x16 = 0x01,
            ASCII_16x32 = 0x03,
            ASCII_24x48 = 0x04,
            ASCII_32x64 = 0x05,
            GBK_24x24 = 0x10,
            GBK_16x16 = 0x11,
            GBK_32x32 = 0x13,
            GB2312_48x48 = 0x14,
        }
        /*
         * 构造函数
         */
        public Text(Port port,PRINTER_TYPE printerType) 
	        :base(port, printerType)
        {
        }

        /*
         * 设置文字的放大方式
         */
        public bool setFontEnlarge(ESC.TEXT_ENLARGE enlarge)
        {
	        byte []cmd = { 0x1D, 0x21, 0x00};
	        cmd[2] = (byte)enlarge;
	        return port.write(cmd);
        }	
        /*
         * 设置文本字体ID
         */
        public bool setFontID(FONT_ID id)
        {		
	        switch(id)
	        {
                case FONT_ID.ASCII_16x32:
                case FONT_ID.ASCII_24x48:
                case FONT_ID.ASCII_32x64:
                case FONT_ID.GBK_32x32:
                case FONT_ID.GB2312_48x48:
			        if(printerType == PRINTER_TYPE.VMP02 ||printerType == PRINTER_TYPE.ULT113x)
			        {
				        MessageBox.Show("JQ", "not support FONT_ID:"+id);
				        return true;
			        }
			        break;
	 	        default:
	 		        break;		
	        }
	        byte []cmd = { 0x1B, 0x4D, 0x00};
	        cmd[2] = (byte)id;
	        return port.write(cmd);
        }
        /*
         * 设置文本字体高度
         */
        public bool setFontHeight(ESC.FONT_HEIGHT height)
        {
	        switch(height)
	        {
                case ESC.FONT_HEIGHT.x24:
			        setFontID(FONT_ID.ASCII_12x24);
			        setFontID(FONT_ID.GBK_24x24);
			        break;
                case ESC.FONT_HEIGHT.x16:
			        setFontID(FONT_ID.ASCII_8x16);
			        setFontID(FONT_ID.GBK_16x16);
			        break;
                case ESC.FONT_HEIGHT.x32:
			        setFontID(FONT_ID.ASCII_16x32);
			        setFontID(FONT_ID.GBK_32x32);
			        break;
                case ESC.FONT_HEIGHT.x48:
			        setFontID(FONT_ID.ASCII_24x48);
			        setFontID(FONT_ID.GB2312_48x48);
			        break;
                case ESC.FONT_HEIGHT.x64:
                    setFontID(FONT_ID.ASCII_32x64);
			        break;
		        default:
			        return false;
	        }	
	        return true;
        }
        /*
         * 设置文本加粗方式
         */
        public bool setBold(bool bold)
        {
	        byte []cmd = { 0x1B, 0x45, 0x00};
	        cmd[2] = (byte)(bold?1:0);
	        return port.write(cmd);
        }	
        /*
         * 打印输出文本
         */
        public bool drawOut(string text) {
	        return port.write(text);
        }

        public bool printOut(string text)
        {
            if (!port.write(text))
                return false;
            return enter();            
        }
        /*
         * 打印输出文本
         */
        public bool drawOut(int x, int y, string text)
        {
	        if (!setXY(x,y))
		        return false;
	        return port.write(text);
        }	
        /*
         * 打印输出文本
         */
        public bool drawOut(int x, int y, ESC.TEXT_ENLARGE enlarge, string text)
        {
	        if (!setXY(x, y))
		        return false;
	        if (!setFontEnlarge(enlarge))
		        return false;
	        return port.write(text);
        }	
        /*
         * 打印输出文本
         */
        public bool drawOut(ESC.FONT_HEIGHT height, string text)
        {
	        if (!setFontHeight(height))
		        return false;
	        return port.write(text);
        }

        /*
         * 打印输出文本
         */
        public bool drawOut(int x, int y, ESC.FONT_HEIGHT height, ESC.TEXT_ENLARGE enlarge, string text)
        {
	        if (!setXY(x, y))
		        return false;
	        if (!setFontHeight(height))
		        return false;
	        if (!setFontEnlarge(enlarge))
		        return false;
	        return port.write(text);
        }	
        /*
         * 打印输出文本
         */
        public bool drawOut(int x, int y, ESC.FONT_HEIGHT height, bool bold, ESC.TEXT_ENLARGE enlarge, string text)
        {
	        if (!setXY(x, y))
		        return false;
	        if (!setFontHeight(height))
		        return false;
	        if (!setFontEnlarge(enlarge))
		        return false;
	        if (!setBold(bold))
		        return false;
	        return port.write(text);
        }
        /*
         * 打印输出文本
         */
        public bool drawOut(int x, int y, ESC.FONT_HEIGHT height, bool bold, string text)
        {
	        if (!setXY(x, y))
		        return false;
	        if (!setFontHeight(height))
		        return false;
	        if (!setBold(bold))
		        return false;
	        return port.write(text);
        }
        /*
         * 打印输出文本
         */
        public bool drawOut(ALIGN align, ESC.FONT_HEIGHT height, bool bold, ESC.TEXT_ENLARGE enlarge, string text)
        {
	        if (!setAlign(align))
		        return false;
	        if (!setFontHeight(height))
		        return false;
	        if (!setBold(bold))
		        return false;
	        if (!setFontEnlarge(enlarge))
		        return false;
	        return port.write(text);
        }
        /*
         * 打印输出文本
         */
        public bool drawOut(ALIGN align, bool bold, string text)
        {
	        if (!setAlign(align))
		        return false;
	        if (!setBold(bold))
		        return false;
	        return port.write(text);
        }

        /// <summary>
        /// 打印输出文本
        /// 1)文件立即输出
        /// 2)会恢复字体效果到缺省值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="height"></param>
        /// <param name="enlarge"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool printOut(int x, int y, ESC.FONT_HEIGHT height,bool bold, ESC.TEXT_ENLARGE enlarge, string text)
        {
            if (!setXY(x, y))   return false;
            if (bold) if (!setBold(true)) return false;
            if (!setFontHeight(height)) return false;
            if (!setFontEnlarge(enlarge)) return false;
            if (!port.write(text)) return false;            
            enter();
            //恢复字体效果
            if (bold) if (!setBold(false)) return false;
            if (!setFontHeight(ESC.FONT_HEIGHT.x24)) return false;
            if (!setFontEnlarge(ESC.TEXT_ENLARGE.NORMAL)) return false;

            return true;
        }

        public bool printOut(ALIGN align, ESC.FONT_HEIGHT height, bool bold, ESC.TEXT_ENLARGE enlarge, string text)
        {
            if (!setAlign(align))   return false;
            if (!setFontHeight(height)) return false;
            if (bold) if (!setBold(true)) return false;
            if (!setFontEnlarge(enlarge))   return false;
            if (!port.write(text))  return false;
            enter();
            //恢复        
            if (!setAlign(ALIGN.LEFT)) return false;
            if (!setFontHeight(ESC.FONT_HEIGHT.x24)) return false;
            if (bold) if (!setBold(false)) return false;            
            if (!setFontEnlarge(ESC.TEXT_ENLARGE.NORMAL)) return false;
            return true;
        }
    }
}
