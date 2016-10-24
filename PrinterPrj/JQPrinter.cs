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
        /// 缺省构造函数，需要配合bool Open(int port_num,int baudrate, int timeout)使用
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
        /// 构造函数，需要配合bool Open(int port_num,int baudrate, int timeout)使用
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
        /// 构造函数，1.需要设置打印机型号，2.需要传入已经存在SerialPort，3.配合bool Open()使用
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
        /// 打开(在构造函数JQPrinter时没有传入SeiralPort时使用)
        /// 1.打开端口
        /// 2.生成JPL对象
        /// </summary>
        /// <param name="port_num">串口号</param>
        /// <param name="timeout">打开串口的超时</param>
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
        /// 打开(在构造函数JQPrinter时已经传入SeiralPort时使用)
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
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (!isOpen)
                return;
            port.Close();
        }

        /// <summary>
        /// 获取通讯端口状态
        /// </summary>
        /// <returns></returns>
        public PORT_STATE getPortState()
        {
            return port.getState();
        }

        /// <summary>
        /// 获取打印机状态
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
                printerErrorString = "纸仓盖未关好";
            }
            else if ((ret[0] & 1) != 0)
            {
                printerErrorString = "打印机缺纸";
            }
            else
                printerErrorString = "打印机正常";
            return true;
        }

        /// <summary>
        /// 获取打印机状态信息
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