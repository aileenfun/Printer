using System.IO.Ports;
using Printer.JPL_Set;
using Printer.ESC_Set;

using System.Net;
using System.Net.Sockets;

namespace Printer
{
    public class JQPrinter
    {
        private PRINTER_TYPE printerType;
        private Port port = null;
        private string printerErrorString;
        public JPL jpl = null;
        public ESC esc = null;
        public bool isOpen
        {
            get
            {
                if (port == null)
                    return false;
                return port.isOpen;
            }
        }

        /// <summary>
        /// ȱʡ���캯������Ҫ���bool Open(int port_num,int baudrate, int timeout)ʹ��
        /// </summary>
        public JQPrinter()
        {
            printerType = PRINTER_TYPE.JLP351;
            if (port == null)
            {
                port = new Port();
                jpl = new JPL(port, printerType);
                esc = new ESC(port, printerType);
            }
        }

        /// <summary>
        /// ���캯������Ҫ���bool Open(int port_num,int baudrate, int timeout)ʹ��
        /// </summary>
        /// <param name="printer_type"></param>
        public JQPrinter(PRINTER_TYPE printer_type)
        {
            printerType = printer_type;
            if (port == null)
            {
                port = new Port();
                jpl = new JPL(port, printerType);
                esc = new ESC(port, printerType);
            }
        }

        /// <summary>
        /// ���캯����1.��Ҫ���ô�ӡ���ͺţ�2.��Ҫ�����Ѿ�����SerialPort��3.���bool Open()ʹ��
        /// </summary>
        /// <param name="printer_type"></param>
        public JQPrinter(PRINTER_TYPE printer_type, SerialPort serial_port)
        {
            printerType = printer_type;
            this.port = new Port(serial_port);
            jpl = new JPL(port, printerType);
            esc = new ESC(port, printerType);
        }

        /// <summary>
        /// ��(�ڹ��캯��JQPrinterʱû�д���SeiralPortʱʹ��)
        /// 1.�򿪶˿�
        /// 2.����JPL����
        /// </summary>
        /// <param name="port_num">���ں�</param>
        /// <param name="timeout">�򿪴��ڵĳ�ʱ</param>
        /// <returns></returns>
        public bool Open(int port_num, int baudrate, int timeout)
        {
            if (isOpen)
            {
                return true;
            }
            if (!port.Open(port_num, baudrate, timeout))
                return false;

            return true;
        }

        /// <summary>
        /// ��(�ڹ��캯��JQPrinterʱ�Ѿ�����SeiralPortʱʹ��)
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (isOpen)
            {
                return true;
            }

            return port.Open(1000);
        }

        public bool Open(IPAddress ip,int portNum,int timeout)
        {
            if (isOpen)
            {
                return true;
            }

            return port.Open(ip, portNum, timeout);
        }

        /// <summary>
        /// �ر�
        /// </summary>
        public void Close()
        {
            if (!isOpen)
                return;
            port.Close();
        }

        /// <summary>
        /// ��ȡͨѶ�˿�״̬
        /// </summary>
        /// <returns></returns>
        public PORT_STATE getPortState()
        {
            return port.getState();
        }

        /// <summary>
        /// ��ȡ��ӡ��״̬
        /// </summary>
        /// <param name="timeout_read"></param>
        /// <returns></returns>
        public bool getPrinterState(int timeout_read)
        {
            if (!isOpen)
                return false;
            byte[] ret = { 0, 0 };
            printerErrorString = string.Empty;

            byte[] cmd = { 0x10, 0x04, 0x05 };
            if (!port.write(cmd))
                return false;

            if (!port.read(ret, 2, timeout_read))
                return false;

            if ((ret[0] & 16) != 0)
            {
                printerErrorString = "ֽ�ָ�δ�غ�";
            }
            else if ((ret[0] & 1) != 0)
            {
                printerErrorString = "��ӡ��ȱֽ";
            }
            else
                printerErrorString = "��ӡ������";
            return true;
        }

        /// <summary>
        /// ��ȡ��ӡ��״̬��Ϣ
        /// </summary>
        /// <returns></returns>
        public string getPrinterStateInfo()
        {
            return printerErrorString;
        }

        public bool wakeUp()
        {
            return esc.wakeUp();
        }
    }
}