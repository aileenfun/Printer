using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Printer.ESC_Set
{
    public class CardReader : BaseESC
    {
        public enum CARD_ERROR
        {
            ERROR_OP_CMD = 0xF3,//不支持的操作命令
            ERROR_ACK_DATA_ZERO = 0xF4,
            ERROR_EXCUEE_FRAME = 0xF5,
            ERROR_VERIFY_PASSWORD = 0xF6,
            ERROR_OP_ERROR = 0xF7,
            ERROR_NO_CARD = 0xF8,
            ERROR_OP_READ = 0xF9,
            ERROR_OP_WRITE = 0xFA,
            ERROR_PARAM_LENGTH_MIN = 0xFB,
            ERROR_PARAM_LENGTH_MAX = 0xFC,
            ERROR_CHANNEL = 0xFD, //通道错误
            ERROR_CARD_POWER_OFF = 0xFE, //卡未上电
            ERROR_CARD_TYPE = 0xFF,//卡类型错误
        }

        public enum CARD_CMD
        {
            SET_CARD_TYPE   = 0xF3,
            RESET           = 0xF6,
            WRITE_READ      = 0xF7,
            CHECK_CARD_IN   = 0xF8,
        }

        public enum CARD_TYPE_SUB_IC 
        {
            CDT_CPU_T0 = 0x00,
            CDT_CPU_T1 = 0x01
        }

        private byte[] cmd = { 0,0,0,0,0,0};

        private byte[] req = new byte[256 + 16];
        private byte[] rsp = new byte[256 + 16];
        private int rsp_len;
            
         /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port"></param>
        /// <param name="printer_type"></param>
        public CardReader(Port port, PRINTER_TYPE printer_type) 
            :base(port, printer_type)
        {
		    
	    }

        /// <summary>
        /// 读卡模块上电
        /// 1)大卡上电不受此指令控制，大卡插入时自动上电，拔出时自动下电。
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool powerOn()
        {
            int timeout = 3000;
            cmd[0] = 0x1B;
            cmd[1] = 0x17;
            port.flushReadBuffer();
            if (!port.write(cmd, 0, 2))
                return false;
            if (!port.read(rsp, 2, timeout))
                return false;
            if (rsp[0] == 0xAA && rsp[1] == 0xFE)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 读卡模块下电
        ///  1)大卡上电不受此指令控制，大卡插入时自动上电，拔出时自动下电。
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool powerOff()
        {
            int timeout = 3000;
            cmd[0] = 0x1B;
            cmd[1] = 0x18;
            port.flushReadBuffer();
            if (!port.write(cmd, 0, 2))
                return false;
            if (!port.read(rsp, 2, timeout))
                return false;
            if (rsp[0] == 0x55 && rsp[1] == 0xFE)
            {
                return true;
            }

            return false;
        }

        private bool send_and_wait(CARD_CMD cmd_flag,byte []req,int req_len,byte []rsp, out int rsp_len)
        {
            rsp_len = 0;
            int len = 1 + req_len;
            cmd[0] = 0x1B;
            cmd[1] = 0x1A;
            cmd[2] = (byte)len;
            cmd[3] = (byte)(len >> 8);
            cmd[4] = (byte)cmd_flag;

            port.flushReadBuffer();
            
            if (!port.write(cmd, 0, 5))
                return false;
            if (req_len > 0)
            {
                if (!port.write(req, 0, req_len))
                    return false;
            }
            
            if (!port.read(rsp, 1, 3000)) //read cmd_flag
                return false;
            if ((CARD_CMD)rsp[0] != cmd_flag)
            {
                if (rsp[0] == 0xFF)
                {
                    MessageBox.Show("错误类型 " + rsp[0].ToString());
                }
                return false;
            }
            
            if (!port.read(rsp, 1, 3000)) //read success flag
                return false;
            if (rsp[0] == 0x00) //命令成功，无返回数据
                return true;
            else if (rsp[0] == 0x01) //命令成功，有返回数据
            {
                if (!port.read(rsp, 2, 3000)) //read data len
                    return false;
                rsp_len = rsp[0] | (rsp[1] << 8);
                if (rsp_len > rsp.Length)
                {
                    MessageBox.Show("rsp 缓冲区太小 <" + rsp_len.ToString());
                    return false;
                }
                return port.read(rsp, rsp_len, 3000); //read data
            }
            else if (rsp[0] == 0xFF)//命令失败
            {
                if (!port.read(rsp, 1, 3000)) //read success flag
                    return false;
                foreach (int i in Enum.GetValues(typeof(CARD_ERROR)))
                {
                    if (i == rsp[0])
                    {
                        MessageBox.Show(Enum.GetName(typeof(CARD_ERROR), i));
                        return false;
                    }
                }
                MessageBox.Show("未知错误类型 "+rsp[0].ToString());
                return false;
            }
            return false;
              
        }

        /// <summary>
        /// 检测大卡座是否有卡插入
        /// </summary>
        /// <returns></returns>
        public bool checkBigCardInsert(out bool state)
        {
            state = false;
            req[0] = 0;//暂时只支持channel 0
            if (!send_and_wait(CARD_CMD.CHECK_CARD_IN, req, 1, rsp, out rsp_len))
                return false;
            if (rsp_len != 1)
                return false;
            state = (rsp[0] == 1);
            return true;
        }

        /// <summary>
        /// 设置卡类型
        /// 1)channel 1,2只支持CPU卡
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool setCardType(int channel,ESC.CARD_TYPE_MAIN card_type_main,int card_type_sub)
        {
            req[0] = (byte)channel;
            req[1] = (byte)card_type_main;
            req[2] = (byte)card_type_sub;
            if (!send_and_wait(CARD_CMD.SET_CARD_TYPE, req, 3, rsp, out rsp_len))
                return false;
             return true;
        }

        /// <summary>
        /// CPU卡复位
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool cardCPU_Reset(int channel,byte []reset,out int rest_len)
        {
            rest_len = 0;
            req[0] = (byte)channel;
            if (!send_and_wait(CARD_CMD.RESET, req, 1, rsp, out rsp_len))
                return false;
            rest_len = rsp_len;
            if (reset.Length >= rsp_len)
            {
                Array.Copy(rsp, 0, reset, 0, rsp_len);
                return true;
            }
            else
               return false;
        }

        /// <summary>
        /// 从CPU卡读数据
        /// 1)请求的命令需要放在ADPU中
        /// 2)返回的结果在read中
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="ADPU"></param>
        /// <param name="adpu_len"></param>
        /// <param name="read"></param>
        /// <param name="read_len"></param>
        /// <returns></returns>
        public bool cardCPU_Read(int channel, byte[] ADPU, int adpu_len, byte[] read, out int read_len)
        {
            read_len = 0;
            req[0] = (byte)channel;
            req[1] = 0;//read
            Array.Copy(ADPU,0,req,2,adpu_len);
            if (!send_and_wait(CARD_CMD.WRITE_READ, req, adpu_len+2, rsp, out rsp_len))
                return false;
            read_len = rsp_len;
            if (rsp_len == 2)
            {
                if (rsp[0] ==0x99 && rsp[1] == 0x61)
                    return false;
            }
            if (read.Length >= rsp_len)
            {
                Array.Copy(rsp, 0, read, 0, rsp_len);
                return true;
            }
            else
            {
                MessageBox.Show("缓冲区太小");
                return false;
            }
        }

        /// <summary>
        /// 向CPU卡写数据
        /// 1)请求的命令及数据需要放在ADPU中
        /// 2)返回的结果在ret中
        /// 3)Select 等语句用此方法
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="ADPU"></param>
        /// <param name="adpu_len"></param>
        /// <param name="ret"></param>
        /// <param name="ret_len"></param>
        /// <returns></returns>
        public bool cardCPU_Write(int channel, byte[] ADPU, int adpu_len,byte[] ret,out int ret_len)
        {
            ret_len = 0;
            req[0] = (byte)channel;
            req[1] = 1;//write
            Array.Copy(ADPU, 0, req, 2, adpu_len);
            if (!send_and_wait(CARD_CMD.WRITE_READ, req, adpu_len + 2, rsp, out rsp_len))
                return false;
            ret_len = rsp_len;
            if (ret_len > 0)
            {
                if (ret.Length >= ret_len)
                    Array.Copy(rsp, 0, ret, 0, ret_len);
                else
                {
                    MessageBox.Show("缓冲区太小");
                    return false;
                }
            }
            else if (ret_len == 2)
            {
                if (rsp[0] == 0x99 && rsp[1] == 0x61)
                    return false;
            }
            return true;
        }
    }

     
}
