namespace Printer
{
    /// <summary>
    /// 枚举类型：打印机型号
    /// </summary>
    public enum PRINTER_TYPE
    {
        VMP02,
        VMP02_P,
        JLP351,
        JLP351_IC,
        ULT113x,
        ULT1131_IC,
        EXP341
    }

    /// <summary>
    /// 通讯端口状态
    /// </summary>
    public enum PORT_STATE
    {
        PORT_OPEND,
        PORT_CLOSED,
        //BT_ADAPTER_NULL,//btAdapter对象为null
        //BT_REMOTE_DEVIECE_NULL,
        //BT_ADAPTER_ERROR,
        //BT_CREAT_RFCOMM_SERVICE_ERROR,
        //BT_CONNECT_ERROR,
        //BT_SOCKET_CLOSE_ERROR,
        //BT_GET_OUT_STREAM_ERROR,
        //BT_GET_IN_STREAM_ERROR,
    }

    /// <summary>
    /// 打印对象对齐方式
    /// </summary>
	public enum ALIGN
	{
		LEFT,
		CENTER,
		RIGHT,
	}

    public enum COLOR
    {
        White,
        Black,
    }
}