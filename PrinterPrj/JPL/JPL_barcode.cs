using System;

namespace Printer.JPL_Set
{
    public class Barcode : BaseJPL
    {
        /*
         * ö�����ͣ�JPL������������
         */
        public enum BAR_1D_TYPE
        {
            UPCA_AUTO = 0x41,
            UPCE_AUTO = 0x42,
            EAN8_AUTO = 0x43,
            EAN13_AUTO = 0x44,
            CODE39_AUTO = 0x45,
            ITF25_AUTO = 0x46,
            CODABAR_AUTO = 0x47,
            CODE93_AUTO = 0x48,
            CODE128_AUTO = 0x49,
        }
        /*
        * ���캯��
        */
        public Barcode(JPL_Param param)
            : base(param)
        {
        }

        /*
         *һά�������
         */
        private bool _1D_barcode(int x, int y, BAR_1D_TYPE type, int height, JPL.BAR_UNIT unit_width, JPL.BAR_ROTATE rotate, string text)
        {
            byte[] cmd = { 0x1A, 0x30, 0x00 };
            port.write(cmd);
            port.write((UInt16)x);
            port.write((UInt16)y);
            port.write((byte)type);
            port.write((UInt16)height);
            port.write((byte)unit_width);
            port.write((byte)rotate);
            return port.write(text);
        }
        /*
         * code128
         */
        public bool code128(int x, int y, int bar_height, JPL.BAR_UNIT unit_width, JPL.BAR_ROTATE rotate, string text)
        {
            return _1D_barcode(x, y, BAR_1D_TYPE.CODE128_AUTO, bar_height, unit_width, rotate, text);
        }
        /*
         * Code128
         */
        public bool code128(Printer.ALIGN align, int y, int bar_height, JPL.BAR_UNIT unit_width, JPL.BAR_ROTATE rotate, string text)
        {
            int x = 0;
            Code128 code128 = new Code128(text);
            if (code128.encode_data == null)
                return false;
            if (!code128.decode(code128.encode_data))
                return false;
            int bar_width = code128.decode_string.Length;
            if (align == ALIGN.CENTER)
                x = (param.pageWidth - bar_width * (int)unit_width) / 2;
            else if (align == ALIGN.RIGHT)
                x = param.pageWidth - bar_width * (int)unit_width;
            else
                x = 0;
            return _1D_barcode(x, y, BAR_1D_TYPE.CODE128_AUTO, bar_height, unit_width, rotate, text);
        }


        /*
         * QRCode
         * int version:�汾�ţ����Ϊ0�����Զ�����汾�š�
         *             ÿ���汾�����ɵ��ֽ���Ŀ��һ���ġ�������ݲ��㣬���Զ����հס�ͨ������һ����İ汾�����̶�QRCode��С��
         * int ecc������ʽ,ȡֵ0, 1��2��3��������Խ�ߣ���Ч�ַ�Խ�٣�ʶ����Խ�ߡ�ȱʡΪ2
         * int unit_width��������Ԫ��С
         */
        public bool QRCode(int x, int y, int version, JPL.QRCODE_ECC ecc, JPL.BAR_UNIT unit_width, JPL.ROTATE rotate, string text)
        {
            byte[] cmd = { 0x1A, 0x31, 0x00 };
            port.write(cmd);
            port.write((byte)version);
            port.write((byte)ecc);
            port.write((UInt16)x);
            port.write((UInt16)y);
            port.write((byte)unit_width);
            port.write((byte)rotate);
            return port.write(text);
        }
        /*
         * PDF417
         */
        public bool PDF417(int x, int y, int col_num, int ecc, int LW_ratio, JPL.BAR_UNIT unit_width, JPL.ROTATE rotate, string text)
        {
            byte[] cmd = { 0x1A, 0x31, 0x01 };
            port.write(cmd);
            port.write((byte)col_num);
            port.write((byte)ecc);
            port.write((byte)LW_ratio);
            port.write((UInt16)x);
            port.write((UInt16)y);
            port.write((byte)unit_width);
            port.write((byte)rotate);
            return port.write(text);
        }

        public bool DataMatrix(int x, int y, JPL.BAR_UNIT unit_width, JPL.ROTATE rotate, string text)
        {
            byte[] cmd = { 0x1A, 0x31, 0x02 };
            port.write(cmd);
            port.write((UInt16)x);
            port.write((UInt16)y);
            port.write((byte)unit_width);
            port.write((byte)rotate);
            return port.write(text);
        }

        public bool GridMatrix(int x, int y, byte ecc, JPL.BAR_UNIT unit_width, JPL.ROTATE rotate, string text)
        {
            byte[] cmd = { 0x1A, 0x31, 0x03 };
            port.write(cmd);
            port.write((byte)ecc);
            port.write((UInt16)x);
            port.write((UInt16)y);
            port.write((byte)unit_width);
            port.write((byte)rotate);
            return port.write(text);
        }
    }
}