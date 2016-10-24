using System;

namespace Printer.JPL_Set
{
    public class Text : BaseJPL
    {
        /*
         * ¹¹Ôìº¯Êý
         */
        public Text(JPL_Param param)
            : base(param)
        {

        }

        public bool drawOut(int x, int y, string text)
        {
            byte[] cmd = { 0x1A, 0x54, 0x00 };
            port.write(cmd);
            port.write((UInt16)x);
            port.write((UInt16)y);
            port.write(text);
            return port.writeNULL();
        }

        public bool drawOut(int x, int y, string text,
                int fontHeight, bool bold, bool reverse, bool underLine, bool deleteLine, JPL.TEXT_ENLARGE enlargeX, JPL.TEXT_ENLARGE enlargeY, JPL.ROTATE rotateAngle)
        {
            if (x < 0 || y < 0)
                return false;
            if (x >= param.pageWidth || y < 0)
                return false;

            byte[] cmd = new byte[] { 0x1A, 0x54, 0x01 };
            int font_type = 0;
            if (bold)
                font_type |= 0x0001;
            if (underLine)
                font_type |= 0x0002;
            if (reverse)
                font_type |= 0x0004;
            if (deleteLine)
                font_type |= 0x0008;
            switch (rotateAngle)
            {
                case JPL.ROTATE.x90:
                    font_type |= 0x0010;
                    break;
                case JPL.ROTATE.x180:
                    font_type |= 0x0020;
                    break;
                case JPL.ROTATE.x270:
                    font_type |= 0x0030;
                    break;
                default:
                    break;
            }
            int ex = (int)enlargeX;
            int ey = (int)enlargeY;
            ex &= 0x000F;
            ey &= 0x000F;
            font_type |= (ex << 8);
            font_type |= (ey << 12);

            port.write(cmd);
            port.write((UInt16)x);
            port.write((UInt16)y);
            port.write((UInt16)fontHeight);
            port.write((UInt16)font_type);
            port.write(text);
            return port.writeNULL();
        }

        private int calcFontWidth(int font_height)
        {
            if (font_height < 20)
            {
                return 16;
            }
            else if (font_height < 28)
            {
                return 24;
            }
            else if (font_height < 40)
            {
                return 32;
            }
            else if (font_height < 56)
            {
                return 48;
            }
            else
            {
                return 64;
            }
        }

        private bool isChinese(char c)
        {
            return (c >= 0x4e00 && c <= 0x9fbb);
        }

        private int calcTextWidth(int font_width, string text)
        {
            int hz_count = 0;
            int ascii_count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (isChinese(text[i]))
                {
                    hz_count++;
                }
                else
                {
                    ascii_count++;
                }
            }
            return (hz_count * font_width + ascii_count * font_width / 2);
        }

        private int calcTextStartPosition(ALIGN align, int font_height, int enlargeX, string text)
        {
            if (align == ALIGN.LEFT)
                return 0;

            int x = 0;
            int font_width = calcFontWidth(font_height);
            enlargeX++;
            int font_total_width = calcTextWidth(font_width, text) * enlargeX;
            switch (align)
            {
                case ALIGN.CENTER:
                    x = (param.pageWidth - font_total_width) / 2;
                    break;
                case ALIGN.RIGHT:
                    x = param.pageWidth - font_total_width;
                    break;
                default:
                    break;
            }
            return x;
        }

        public bool drawOut(ALIGN align, int y, string text, int fontHeight, bool bold, bool reverse, bool underLine, bool deleteLine, JPL.TEXT_ENLARGE enlargeX, JPL.TEXT_ENLARGE enlargeY, JPL.ROTATE rotateAngle)
        {
            int x = calcTextStartPosition(align, fontHeight, (int)enlargeX, text);
            return drawOut(x, y, text, fontHeight, bold, reverse, underLine, deleteLine, enlargeX, enlargeY, rotateAngle);
        }
    }
}