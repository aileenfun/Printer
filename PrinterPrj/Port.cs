using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Printer
{
    public class Port
    {
        private enum PortTpye
        {
            SerialPort,
            Socket,
        }
        private SerialPort m_serialPort = null;

        
        private Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private IPEndPoint m_socketEndPoint = null;
        private bool mSocketOpen = false;
        
        private PortTpye port_type = PortTpye.SerialPort;

        public bool isOpen
        {
            get
            {
                if (port_type == PortTpye.SerialPort)
                {
                    if (m_serialPort == null)
                        return false;
                    return m_serialPort.IsOpen;
                }
                else
                {
                    return mSocketOpen;
                }
            }
        }
        private PORT_STATE portState = PORT_STATE.PORT_CLOSED;
        //构造函数

        /// <summary>
        /// 基本构造函数
        /// 1..new serial port 
        /// </summary>
        public Port()
        {
            m_serialPort = new SerialPort();
        }

        /// <summary>
        /// 构造函数
        /// 1.传入serialport
        /// </summary>
        /// <param name="port"></param>
        public Port(SerialPort port)
        {
            m_serialPort = port;
        }

        public bool Open(int port_num, int baudrate, int timeout)
        {
            if (isOpen)
                return true;

            m_serialPort.BaudRate = baudrate;
            m_serialPort.PortName = "////.//COM" + port_num.ToString();

            return open(timeout);
        }

        public bool Open(int timeout)
        {
            return open(timeout);
        }

        public bool Open(IPAddress ip, int portNum, int timeout)
        {
            m_socketEndPoint = new IPEndPoint(ip, portNum);//服务器的IP和端口
            try
            {
                //因为客户端只是用来向特定的服务器发送信息，所以不需要绑定本机的IP和端口。不需要监听。
                clientSocket.Connect(m_socketEndPoint);
            }
            catch (SocketException e1)
            {
                MessageBox.Show("unable to connect to server");
                MessageBox.Show(e1.ToString());
                return false;
            }
            port_type = PortTpye.Socket;
            mSocketOpen = true;
            return true;
        }

        void Log(string str)
        {
            // MessageBox.Show(e.ToString());
        }

        private bool open(int timeout)
        {
            if (m_serialPort.IsOpen)
                return true;

            if (timeout < 1000)
                timeout = 1000;
            if (timeout > 30 * 1000)
                timeout = 30 * 1000;

            int loop = timeout / 100;
            while ((loop--) > 0)
            {
                try
                {
                    m_serialPort.Open();
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                    return false;
                }
                if (isOpen == true)
                    return true;
                Thread.Sleep(100);
            }
            return false;
        }

        public void Close()
        {
            if (port_type == PortTpye.Socket)
            {
                Thread.Sleep(1000);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                mSocketOpen = false;
            }
            else
            {
                m_serialPort.Close();
            }            
        }

        /// <summary>
        /// 获取端口状态
        /// </summary>
        /// <returns></returns>
        public PORT_STATE getState()
        {
            if (isOpen)
            {
                portState = PORT_STATE.PORT_OPEND;
            }
            else
            {
                portState = PORT_STATE.PORT_CLOSED;
            }
            return portState;
        }

        public void flushReadBuffer()
        {
            m_serialPort.DiscardInBuffer();
        }

        public bool write(byte[] buffer, int offset, int length)
        {
            if (!isOpen)
                return false;
            if (port_type == PortTpye.SerialPort)
            {
                m_serialPort.Write(buffer, offset, length);
            }
            else
            {
                try
                {
                    int r = clientSocket.Send(buffer, offset, length, SocketFlags.None);
                    if (r != length)
                    {
                        MessageBox.Show("error");
                    }
                }
                catch (SocketException e)
                {
                    MessageBox.Show(e.Message);
                }

            }
            return true;
        }

        public bool write(byte[] buffer)
        {
            return write(buffer, 0, buffer.Length);
        }

        public bool write(UInt16 s)
        {
            byte[] buffer = { 0, 0 };
            buffer[0] = (byte)s;
            buffer[1] = (byte)(s >> 8);
            return write(buffer, 0, 2);
        }

        public bool write(byte s)
        {
            byte[] buffer = { 0 };
            buffer[0] = (byte)s;
            return write(buffer, 0, 1);
        }

        public bool writeNULL()
        {
            byte[] ZERO = { 0x00 };
            return write(ZERO);
        }

        public bool write(string text)
        {
            byte[] GBK_bytes = Encoding.GetEncoding(936).GetBytes(text);//Encoding.Default.GetBytes(text);

            if (!write(GBK_bytes, 0, GBK_bytes.Length))
                return false;
            return writeNULL();//必须发送0x00作为字符串的结束符号
        }

        public bool read(byte[] buffer, int length, int timeout_read)
        {
            if (length > buffer.Length)
                return false;
            int interval = 20;
            m_serialPort.ReadTimeout = timeout_read;
            int readed = 0;
            int offset = 0;
            int need_read = length;
            int loop = timeout_read / interval;
            while(loop-- >= 0)
            {
                readed = m_serialPort.Read(buffer, offset, need_read);
                if (readed > 0)
                {
                    offset += readed;
                    need_read -= readed;
                    if (need_read == 0)
                        return true;
                }
                else
                {
                    Thread.Sleep(interval);
                }
            }
            return false;
        }

    }
}