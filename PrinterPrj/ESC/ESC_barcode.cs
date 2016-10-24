using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Printer.ESC_Set
{
    public class Barcode : BaseESC
    {
        public enum ESC_BAR_2D
        {				   	
	        PDF417 = 0,
	        DATAMATIX = 1,
	        QRCODE = 2,	
	        GRIDMATIX = 10, 
        };

        private byte[] cmd = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port"></param>
        /// <param name="printer_type"></param>
        public Barcode(Port port, PRINTER_TYPE printer_type) 
            :base(port, printer_type)
        {
		    
	    }
        /// <summary>
        /// 设置条码文字位置
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool setTextPosition(ESC.BAR_TEXT_POS pos)
        {
            byte[] cmd = { 0x1D, 0x48, 0x00 };
            cmd[2] = (byte)pos;
            return port.write(cmd);
        }
        /// <summary>
        /// 设置1维条码高度
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool set1DHeight(int height)
        {
            byte[] cmd = { 0x1D, 0x68, 0x00 };
            cmd[2] = (byte)height;
            return port.write(cmd);
        }
        /// <summary>
        /// 设置1维，2维条码基本单元大小
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool setUnit(ESC.BAR_UNIT unit)
        {
            byte[] cmd = { 0x1D, 0x77, 0x00 };
            cmd[2] = (byte)unit;
            return port.write(cmd);
        }
        /// <summary>
        /// 设置条码文字大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool setTextSize(ESC.BAR_TEXT_SIZE size)
        {
            byte[] cmd = { 0x1D, 0x66, 0x00 };
            cmd[2] = (byte)size;
            return port.write(cmd);
        }

        /// <summary>
        /// CODE128基本方式，此函数不会计算校验和。需要你自己在data中算好校验和
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool code128_base(byte[] data)
        {
            byte[] cmd = { 0x1D, 0x6B, 0x08 };
            port.write(cmd);
            return port.write(data);
        }
        /// <summary>
        /// Code128自动计算校验和
        /// 1)内容不输出
        /// </summary>
        /// <param name="align"></param>
        /// <param name="unit"></param>
        /// <param name="height"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool code128_auto_drawOut(ALIGN align, ESC.BAR_UNIT unit, int height, ESC.BAR_TEXT_POS pos, ESC.BAR_TEXT_SIZE size, string str)
        {
            Code128 code128 = new Code128(str);
            byte[] buf = code128.encode_data;
            if (buf == null)
                return false;
            if (!setAlign(align))
                return false;
            setUnit(unit);
            if (!set1DHeight(height))
                return false;
            setTextPosition(pos);
            setTextSize(size);
            return code128_base(buf);
        }

        public bool code128_auto_printOut(ALIGN align, ESC.BAR_UNIT unit, int height, ESC.BAR_TEXT_POS pos, ESC.BAR_TEXT_SIZE size, string str)
        {
            if (!code128_auto_drawOut(align,unit,height,pos,size,str))
                return false;
            enter();
            if (!setAlign(ALIGN.LEFT))
                return false;
            return true;
        }

        /// <summary>
        /// 选择2D条码类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool barcode2D_SetType(ESC_BAR_2D type)
        {
            cmd[0] = 0x1D;  cmd[1] = 0x5A;
            cmd[2] = (byte)type;
            return port.write(cmd,0, 3);
        }
        /// <summary>
        /// 绘制2D条码
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool barcode2D_DrawOut(byte m, byte n, byte k, String text)
        {
            int size = Encoding.GetEncoding(936).GetByteCount(text);//Encoding.Default.GetByteCount(text);
            if (size == 0)
            {
                return false;//data is empty
            }
            cmd[0] = 0x1B;  cmd[1] = 0x5A;
            cmd[2] = m; cmd[3] = n; cmd[4] = k;
            cmd[5] = (byte)size; cmd[6] = (byte)(size >> 8);
            port.write(cmd,0, 7);
            return port.write(text);
        }
        /// <summary>
        /// 绘制QRCode
        /// </summary>
        /// <param name="version">版本号,当version = 0，自动计算版本号，通过版本号可以可以QRCode尺寸大小</param>
        /// <param name="ecc">纠错级别，0~3,纠错级别越高越容易识别，但是可容纳内容减小</param>
        /// <param name="text">条码内容</param>
        /// <returns></returns>
        public bool barcode2D_QRCode(byte version, byte ecc, String text)
        {
            barcode2D_SetType(ESC_BAR_2D.QRCODE);
            return barcode2D_DrawOut(version, ecc, 0, text);
        }
        /// <summary>
        ///  绘制QRCode,根据参数
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="unit">条码基本单元宽度</param>
        /// <param name="version">版本号,当version = 0，自动计算版本号，通过版本号可以可以QRCode尺寸大小</param>
        /// <param name="ecc">纠错级别，0~3,纠错级别越高越容易识别，但是可容纳内容减小</param>
        /// <param name="text">条码内容</param>
        /// <returns></returns>
        public bool barcode2D_QRCode(int x, int y, ESC.BAR_UNIT unit,byte version, byte ecc, String text)
        {
            this.setXY(x, y);
            this.setUnit(unit);
            barcode2D_SetType(ESC_BAR_2D.QRCODE);
            return barcode2D_DrawOut(version, ecc, 0, text);
        }
        /// <summary>
        /// 绘制PDF417
        /// </summary>
        /// <param name="columnNumber">每列容纳字符数目</param>
        /// <param name="ecc">纠错能力,0~8,级别越高，纠正码字数越多，纠正能力越强，条码也越大</param>
        /// <param name="hwratio">长宽比列</param>
        /// <param name="text">条码内容</param>
        /// <returns></returns>
        public bool barcode2D_PDF417(byte columnNumber, byte ecc, byte hwratio, String text)
        {
            barcode2D_SetType(ESC_BAR_2D.PDF417);
            return barcode2D_DrawOut(columnNumber, ecc, hwratio, text);
        }
        /// <summary>
        /// 绘制DATAMatrix
        /// </summary>
        /// <param name="text">条码内容</param>
        /// <returns></returns>
        public bool barcode2D_DATAMatrix(String text)
        {
            barcode2D_SetType(ESC_BAR_2D.DATAMATIX);
            return barcode2D_DrawOut(0, 0, 0, text);
        }
        /// <summary>
        /// 绘制GRIDMatrix
        /// </summary>
        /// <param name="ecc">纠错级别</param>
        /// <param name="text">条码内容</param>
        /// <returns></returns>
        public bool barcode2D_GRIDMatrix(byte ecc, String text)
        {
            barcode2D_SetType(ESC_BAR_2D.GRIDMATIX);
            return barcode2D_DrawOut(ecc, 0, 0, text);
        }
    }
}
