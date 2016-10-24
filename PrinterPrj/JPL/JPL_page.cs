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
        /// ��ʼҳ��
        /// </summary>
        /// <param name="originX">ԭ��x����</param>
        /// <param name="originY">ԭ��y����</param>
        /// <param name="pageWidth">ҳ���ȣ���λdot,1dot =0.125mm</param>
        /// <param name="pageHeight">ҳ��߶�</param>
        /// <param name="rotate">ѡ��Ƕ�</param>
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
        /// ���ƴ�ӡҳ�����
        /// </summary>
        /// <returns></returns>
        public bool end()
        {
            byte[] cmd = new byte[] { 0x1A, 0x5D, 0x00 };
            return port.write(cmd);
        }

        /// <summary>
        /// ��ӡҳ������ ֮ǰ����ҳ�洦��ֻ�ǰѴ�ӡ���󻭵��ڴ��У�����Ҫͨ��������������ݴ�ӡ����
        /// </summary>
        /// <returns></returns>
        public bool print()
        {
            byte[] cmd = new byte[] { 0x1A, 0x4F, 0x00 };
            return port.write(cmd);
        }
    }
}