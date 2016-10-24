namespace Printer.JPL_Set
{
    public class JPL_Param
    {
        public Port port;
        public int pageWidth;
        public int pageHeight;
        public JPL_Param(Port port)
        {
            this.port = port;
            pageWidth = -1;
            pageHeight = -1;
        }
    }

    public class BaseJPL
    {
        protected Port port;
        protected JPL_Param param;

        /*
         * ���캯��
         */
        public BaseJPL(JPL_Param param)
        {
            if (param.port == null)
                return;
            this.param = param;
            this.port = param.port;
        }
    }

    public class JPL
    {
        public enum QRCODE_ECC
        {
            LEVEL_L,//�ɾ���7%
            LEVEL_M,//�ɾ���15%
            LEVEL_Q,//�ɾ���25%
            LEVEL_H,//�ɾ���30%	
        }
        // <summary>
        /// ��ת�Ƕ�
        /// </summary>
        public enum IMAGE_ROTATE
        {
            x0,
            x90,
            x180,
            x270
        }
        /// <summary>
        /// ��ӡ����Text,2D Barcode,��ת��ʽ
        /// </summary>
        public enum ROTATE
        {
            x0 = 0x00,
            x90 = 0x01,
            x180 = 0x10,
            x270 = 0x11,
        }
        public enum TEXT_ENLARGE
        {
            x1,
            x2,
            x3,
            x4,
        }

        public enum IMAGE_ENLARGE
        {
            x1,
            x2,
            x3,
            x4,
        }
        /// <summary>
        /// ҳ����ת�Ƕ�
        /// </summary>
        public enum PAGE_ROTATE
        {
            x0,
            x90,
        }
        // <summary>
        /// ��ӡ������ת�Ƕ�
        /// </summary>
        public enum BAR_ROTATE
        {
            ANGLE_0,
            ANGLE_90,
            ANGLE_180,
            ANGLE_270
        }
        /// <summary>
        /// ö�����ͣ����뵥Ԫ,JPL����
        /// </summary>
        public enum BAR_UNIT
        {
            x1 = 1,
            x2 = 2,
            x3 = 3,
            x4 = 4,
            x5 = 5,
            x6 = 6,
            x7 = 7,
        }
        public Page page;
        public Text text;
        public Barcode barcode;
        public Graphic graphic;
        public Image image;
        private Port port;

        private JPL_Param param;

        public JPL(Port port, PRINTER_TYPE printer_type)
        {
            if (port == null)
                return;
            switch (printer_type)
            {
                case PRINTER_TYPE.JLP351:
                    break;
                case PRINTER_TYPE.JLP351_IC:
                    break;
                default:
                    return;
            }
            this.port = port;
            this.param = new JPL_Param(port);
            page = new Page(param);
            text = new Text(param);
            barcode = new Barcode(param);
            
            graphic = new Graphic(param);
            image = new Image(param);
        }

        /// <summary>
        /// ��ֽ����һ�ű�ǩ��ʼ
        /// </summary>
        /// <returns></returns>
        public bool feedNextLabelBegin()
        {
            byte[] cmd = { 0x1A, 0x0C, 0x00 };
            return port.write(cmd);
        }

    }
}