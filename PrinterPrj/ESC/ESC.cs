using System.Threading;

namespace Printer.ESC_Set
{
    public class BaseESC
    {
        protected Port port;
        protected PRINTER_TYPE printerType;
        protected int maxDots;//允许打印的大点数
        protected int canvasMaxHeight;//打印机画板最大高度，单位dots.	
        /*
         * 构造函数
         */
        public BaseESC(Port port, PRINTER_TYPE printer_type)
        {
            if (port == null)
                return;
            this.port = port;

            printerType = printer_type;
            switch (printerType)
            {
                case PRINTER_TYPE.VMP02:
                    maxDots = 384;
                    canvasMaxHeight = 100;
                    break;
                case PRINTER_TYPE.VMP02_P:
                    maxDots = 384;
                    canvasMaxHeight = 200;
                    break;
                case PRINTER_TYPE.ULT113x:
                    maxDots = 576;
                    canvasMaxHeight = 120;
                    break;
                case PRINTER_TYPE.JLP351:
                    maxDots = 576;
                    canvasMaxHeight = 250;
                    break;
                default:
                    maxDots = 576;
                    canvasMaxHeight = 100;
                    break;
            }
        }
        /*
         * 设置打印对象的x，y坐标
         */
        public bool setXY(int x, int y)
        {
            if (x < 0 || x >= maxDots || x > 0x1FF)
            {
                return false;
            }

            if (y < 0 || y >= canvasMaxHeight || y > 0x7F)
            {
                return false;
            }

            byte[] cmd = { 0x1B, 0x24, 0x00, 0x00 };
            int pos = ((x & 0x1FF) | ((y & 0x7F) << 9));
            cmd[2] = (byte)pos;
            cmd[3] = (byte)(pos >> 8);
            port.write(cmd);
            return true;
        }
        /*
         * 设置打印对象对齐方式
         * 支持打印对象:文本(text),条码(barcode)
         */
        public bool setAlign(ALIGN align)
        {
            byte[] cmd = { 0x1B, 0x61, 0x00 };
            cmd[2] = (byte)align;
            return port.write(cmd);
        }
        public bool setLineSpace(int dots)
        {
            byte[] cmd = { 0x1B, 0x33, 0x00 };
            cmd[2] = (byte)dots;
            return port.write(cmd);
        }

        /// <summary>
        /// 恢复打印机初始设置
        /// </summary>
        /// <returns></returns>
        public bool init()
        {
            byte[] cmd = { 0x1B, 0x40 };
            return port.write(cmd);
        }

        /// <summary>
        /// 换行回车
        /// </summary>
        /// <returns></returns>
        public bool enter()
        {
             byte[] cmd = { 0x0D, 0x0A };
             return port.write(cmd);
        }
    }

    public class ESC
    {
        public enum CARD_TYPE_MAIN
        {
            CDT_AT24Cxx = 0x01,
            CDT_SLE44xx = 0x11,
            CDT_CPU = 0x21,
        };

        public enum BAR_TEXT_SIZE
        {
            ASCII_12x24,
            ASCII_8x16,
        }

        public enum BAR_UNIT
	    {
		    x1 = 1,
		    x2 = 2,
		    x3 = 3,
		    x4 = 4
	    }

        public enum BAR_TEXT_POS
        {
            NONE,
            TOP,
            BOTTOM,
        }

        public class LINE_POINT
        {
            public int startPoint;
            public int endPoint;
            public LINE_POINT()
            {
            }
            public LINE_POINT(int start_point, int end_point)
            {
	            startPoint = (short)start_point;
	            endPoint = (short)end_point;
            }
        }
        /// <summary>
        /// 枚举类型：文本放大方式
        /// </summary>
        public enum TEXT_ENLARGE
        {
            NORMAL = 0x00,                        //正常字符 
            HEIGHT_DOUBLE = 0x01,                 //倍高字符
            WIDTH_DOUBLE = 0x10,                  //倍宽字符
            HEIGHT_WIDTH_DOUBLE = 0x11,          //倍高倍宽字符            
        }
        /// <summary>
        /// 枚举类型：字体高度
        /// </summary>
        public enum FONT_HEIGHT
        {
            x24,
            x16,
            x32,
            x48,
            x64,
        }
        /// <summary>
        /// 图像模式
        /// </summary>
        public enum IMAGE_MODE : byte
        {
            SINGLE_WIDTH_8_HEIGHT = 0x01,        //单倍宽8点高
            DOUBLE_WIDTH_8_HEIGHT = 0x00,        //倍宽8点高
            SINGLE_WIDTH_24_HEIGHT = 0x21,       //单倍宽24点高
            DOUBLE_WIDTH_24_HEIGHT = 0x20,       //倍宽24点高
        }

        public enum IMAGE_ENLARGE
        {
            NORMAL,//正常
            HEIGHT_DOUBLE,//倍高 
            WIDTH_DOUBLE,//倍宽
            HEIGHT_WIDTH_DOUBLE	//倍高倍宽	
        }

        private Port port;
        public Text text;
        public Image image;
        public Graphic graphic;
        public Barcode barcode;
        public CardReader card_reader;
        public ESC(Port port, PRINTER_TYPE printer_type)
        {
            if (port == null)
                return;
            this.port = port;
            text = new Text(port, printer_type);
            image = new Image(port, printer_type);
            graphic = new Graphic(port, printer_type);
            barcode = new Barcode(port, printer_type);
            card_reader = new CardReader(port, printer_type);
        }

   

        public bool wakeUp()
        {
            if (!port.writeNULL())
                return false;
           
            Thread.Sleep(50);
            return text.init();
        }

        public bool getState(byte[] ret, int timerout_read)
        {
            byte[] cmd = { 0x10, 0x04, 0x05 };
            if (!port.write(cmd))
                return false;
            if (!port.read(ret, 2, timerout_read))
                return false;
            return true;
        }
        /*
         * 换行回车
         */
        public bool feedEnter()
        {
            byte[] cmd = { 0x0D, 0x0A };
            return port.write(cmd);
        }
        /*
         * 走纸几行
         * 输入参数:
         * --int lines:几行
         */
        public bool feedLines(int lines)
        {
            byte[] cmd = { 0x1B, 0x64, 00 };
            cmd[2] = (byte)lines;
            return port.write(cmd);
        }
        /*
         * 走纸几点
         * 输入参数:
         * --int dots:多少个点
         */
        public bool feedDots(int dots)
        {
            byte[] cmd = { 0x1B, 0x4A, 00 };
            cmd[2] = (byte)dots;
            return port.write(cmd);
        }

    }
}
