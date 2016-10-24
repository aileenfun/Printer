using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Printer.ESC_Set
{
    public class Image: BaseESC
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port"></param>
        /// <param name="printer_type"></param>
	    public Image(Port port,PRINTER_TYPE printer_type) 
            :base(port, printer_type)
        {
		    
	    }

        /// <summary>
        /// 图像数据下载到打印机内存
        /// 1)图像数据扫描方式是从左到右，从上到下
        /// 2)数据总大小:x_bytes * y_bytes *8
        /// 3)x方向点数 x_bytes * 8
        /// 4)y方向点数 y_bytes * 8
        /// </summary>
        /// <param name="x_bytes"></param>
        /// <param name="y_bytes"></param>
        /// <param name="data"></param>
        /// <returns></returns>
	    private bool userImageDownloadIntoRAM(int x_bytes, int y_bytes, byte[] data) 
        {
		    byte[] cmd = { 0x1D, 0x2A, 0, 0 };
		    if (x_bytes <= 0)
			    return false;
		    if (y_bytes <= 0 || y_bytes > 127)
			    return false;
		    int all_data_size = x_bytes * y_bytes * 8;

		    if (all_data_size > 1024)
			    return false;
		    if (all_data_size != data.Length)
			    return false;

		    cmd[2] = (byte) x_bytes;
		    cmd[3] = (byte) y_bytes;
		    if (!port.write(cmd))
			    return false;
		    return port.write(data);
	    }

        /// <summary>
        /// 绘制RAM中预存储图像到打印画板
        /// 1)打印机不打印输出图像
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
	    private bool userImageDrawout(ESC.IMAGE_ENLARGE mode)
	    {	
		    byte[] cmd = { 0x1D, 0x2F, 0x00};
		    cmd[2] = (byte) mode;
		    return port.write(cmd);
	    }
    	
        /// <summary>
        /// 绘制图像到打印机绘图区域
        /// 1)并不立刻打印输出,一些情况会导致打印内容输出
        ///   A. 换行回车(\r \n 或者\r\n) 
        ///   B  打印对象导致x坐标超过画板宽度
        /// 2)图像高度超过打印画板高度高度会导致上部分图像丢失
        /// </summary>
        /// <param name="image_width_dots"></param>
        /// <param name="image_height_dots"></param>
        /// <param name="mode"></param>
        /// <param name="image_data"></param>
        /// <returns></returns>
	    private bool _drawOut(int image_width_dots, int image_height_dots,ESC.IMAGE_ENLARGE mode,byte []image_data)
	    {
		     //MessageBox.Show("drawOutVertical w:"+image_width_dots.ToString() +" h:"+image_height_dots.ToString());
		     int Y_Byte = (image_height_dots - 1) / 8 + 1;            //位图Y轴方向象素点的字节素；
             int X_Byte = (image_width_dots - 1) / 8 + 1;                      //位图X轴方向象素点的字节素，表示需要X_Byte幅8 x BmpHeight的位图拼成目标位图；
             byte[] DotsBuf = new byte[Y_Byte * 8];                      //存放8 x BmpHeight位图的点阵数据；
             for (int i = 0; i < DotsBuf.Length; i++)
                 DotsBuf[i] = 0;
             int DotsBufIndex = 0;                                        //8xBmpHeight数据索引
             int DotsByteIndex = 0;                                       //原始位图数据索引
             for (int i = 0; i < X_Byte; i++)
             {
                 for (int j = 0; j < 8; j++)
                 {
                     for (int k = 0; k < Y_Byte; k++)
                     {
                         DotsByteIndex = k * image_width_dots + i * 8 + j;
                         if ((i << 3)+j < image_width_dots)                       //当宽度大于位图实际宽度是，点阵数据为0，因为定义位图宽度为8的整数倍，而实际宽度可能不是整数倍
                             DotsBuf[DotsBufIndex++] = (byte) image_data[DotsByteIndex];
                         else
                             DotsBuf[DotsBufIndex++] = 0x00;
                     }
                 }
                 DotsBufIndex = 0;
                 userImageDownloadIntoRAM(1,Y_Byte, DotsBuf);                //定义位图
                 userImageDrawout(mode);         //打印定义位图
             }
		    return true;
	    }	   
    	
        /// <summary>
        /// 根据数组绘制图像到打印机画板
        ///  1)图像高度不能大于对打印机画板高度
        ///  2)由于图像并没有立即输出，还可以继续在相应的x,y坐标绘制打印对象
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="image_width_dots"></param>
        /// <param name="image_height_dots"></param>
        /// <param name="mode"></param>
        /// <param name="image_data"></param>
        /// <returns></returns>
	    public bool drawOut(int x,int y, int image_width_dots, int image_height_dots,ESC.IMAGE_ENLARGE mode,byte []image_data)
	    {
		    if(!setXY(x,y)) 
			    return false;
		    return _drawOut(image_width_dots, image_height_dots,mode,image_data);
	    }
    	
	    /// <summary>
        /// 根据bitmap对象，使用自定义位图方式绘制图像到打印画板
        /// 1)图像高度不能大于对打印机画板高度
        /// 2)由于图像并没有立即输出，还可以继续在相应的x,y坐标绘制打印对象
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
	    public bool drawOut(int x, int y, Bitmap bitmap) 
        {
		    int width = bitmap.Width;
		    int height = bitmap.Height;
		    if (width > this.maxDots)// || height > this.canvasMaxHeight)
		    {
                MessageBox.Show("w:" + width + " > " + maxDots.ToString(),"JQ");
			    return false;
		    }
		    if ( height > this.canvasMaxHeight)
		    {
                MessageBox.Show( "h:" + height + " > " + canvasMaxHeight.ToString(),"JQ");
			    return false;
		    }

            ImageConver conver = new ImageConver();
            byte[] data = conver.CovertImageVertical(bitmap, 128,8);

		    if (data == null)
			    return false;
		    if (!setXY(x, y))
			    return false;
		    return _drawOut(width, height, ESC.IMAGE_ENLARGE.NORMAL, data);
	    }

        /// <summary>
        /// 根据图片路径，使用自定义位图方式绘制图像到打印画板
        /// 1)图像高度不能大于对打印机画板高度
        /// 2)由于图像并没有立即输出，还可以继续在相应的x,y坐标绘制打印对象
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="image_path"></param>
        /// <returns></returns>
        public bool drawOut(int x, int y, string image_path)
        {
            if (!File.Exists(image_path))
            {
                MessageBox.Show("文件路径错误:" + image_path);
                return false;
            }

            Bitmap bitmap = new Bitmap(image_path);
            return drawOut(x, y, bitmap);
        }

        /// <summary>
        /// 根据bitmap对象打印图片
        /// 1)适用于所有厂家及所有型号的POS打印机
        /// 2)图像立即输出,不能在图像上绘制文字
        /// </summary>
        /// <param name="x"></param>
        /// <param name="bitmap"></param>
        /// <param name="sleep_time"></param>
        /// <returns></returns>
        public bool printOut(int x, Bitmap bitmap, int sleep_time) 
        {
		    int width = bitmap.Width;
		    int height = bitmap.Height;
		    if (width > this.maxDots)
			    return false;
            ImageConver conver = new ImageConver();
            byte[] data = conver.CovertImageVertical(bitmap, 128,8);

		    if (data == null)
			    return false;
            return _printOut(x, width, height, ESC.IMAGE_MODE.SINGLE_WIDTH_8_HEIGHT, data, sleep_time);
	    }

        public bool printOut(int x, Bitmap bitmap,bool enlargeX, int sleep_time)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            if (width > this.maxDots)   return false;
            ImageConver conver = new ImageConver();
            byte[] data = conver.CovertImageVertical(bitmap, 128, 8);

            if (data == null)   return false;

            ESC.IMAGE_MODE mode;
            mode = enlargeX ? ESC.IMAGE_MODE.DOUBLE_WIDTH_8_HEIGHT: ESC.IMAGE_MODE.SINGLE_WIDTH_8_HEIGHT;
            return _printOut(x, width, height, mode, data, sleep_time);
        }

        /// <summary>
        /// 根据文件路径打印图片
        /// 1)适用于所有厂家及所有型号的POS打印机
        /// 2)图像立即输出,不能在图像上绘制文字
        /// </summary>
        /// <param name="x"></param>
        /// <param name="image_path"></param>
        /// <param name="sleep_time">休眠时间，当波特率过高时，需要使用休眠时间</param>
        /// <returns></returns>
        public bool printOut(int x,string image_path,int sleep_time)
        {
             if (!File.Exists(image_path))
            {
                MessageBox.Show("文件路径错误:" + image_path);
                return false;
            }

            Bitmap bitmap = new Bitmap(image_path);
            return printOut(x, bitmap, sleep_time);
        }

        /// <summary>
        /// 根据文件路径打印图片,图像可设置倍宽
        /// 1)适用于所有厂家及所有型号的POS打印机
        /// 2)图像立即输出,不能在图像上绘制文字
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="image_path">文件路径</param>
        /// <param name="enlargeX">倍宽</param>
        /// <param name="sleep_time">传输单副图像的时间，单位ms，缺省设置为0，如果上位机通讯传输速度太快，需要设置此值10~100</param>
        /// <returns></returns>
        public bool printOut(int x, string image_path,bool enlargeX, int sleep_time)
        {
            if (!File.Exists(image_path))
            {
                MessageBox.Show("文件路径错误:" + image_path);
                return false;
            }

            Bitmap bitmap = new Bitmap(image_path);
            return printOut(x, bitmap,enlargeX,sleep_time);
        }

        /// <summary>
        /// 从上到下把一副大图片分割成n=((height-1)/8+1)个小图片,每个小图像宽width，高8个点。
        /// 1)适用于所有厂家及型号的POS打印机
        /// </summary>
        /// <param name="x"></param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="mode">小图像放大模式，暂时只支持8点高度</param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool _printOut(int x, int width, int height, ESC.IMAGE_MODE mode, byte[] data, int sleep_time)
        {
            switch (mode)
            {
                case ESC.IMAGE_MODE.SINGLE_WIDTH_8_HEIGHT:
                    if (width > maxDots)
                    {
                        MessageBox.Show("图像宽度超出打印区域,w:" + width.ToString() + ">" + maxDots.ToString());
                        return false;
                    }
                    break;
                case ESC.IMAGE_MODE.DOUBLE_WIDTH_8_HEIGHT:
                    if (width*2 > maxDots)
                    {
                        MessageBox.Show("图像宽度2倍后超出打印区域,w:" + (width*2).ToString() + ">" + maxDots.ToString());
                        return false;
                    }
                    break;
                default:
                    MessageBox.Show("暂时不支持24高度图像");
                    return false;
            }
            int imageCount = (height - 1) / 8 + 1;//分割成多少副图片
            if (data.Length != width * imageCount)
            {
                MessageBox.Show("数据长度和 IMAGE_MODE参数不匹配");
                return false;
            }
            byte[] cmd = { 0x1B, 0x2A };//发送命令头
            this.setLineSpace(0);//设置行间距为0
            for (int i = 0; i < imageCount; i++)
            {
                this.setXY(x, 0);
                port.write(cmd);
                port.write((byte)mode);
                port.write((UInt16)width);
                port.write(data, i * width, width);
                port.write("\r\n");
                Thread.Sleep(sleep_time);
            }
            setLineSpace(8);//恢复原始值 行间距为8    

            port.write("\r\n");
            return true;
        }
        
        /// <summary>
        /// 根据文件路径打印快速打印图片
        /// 1)此为济强电子独有指令，不兼容别的品牌的打印机
        /// 2)此方法是把一副大图片，分割成n多副小图片(高度为base_image_height)来打印。
        /// 3)根据上位机的数据传输速度，可以调整base_image_height的大小来获得不同的图像打印速度。
        /// 4)使用此方法，不可以在其余区域绘制别的东西。如果需要在图片上绘制别的打印对象请使用drawOut相关函数
        /// 5)需要配合最新的打印机固件使用
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="image_path">图像路径</param>
        /// <param name="sleep_time">单张图片见间距时间</param>
        /// <param name="base_image_height">单张图片的高度 ，最小值 4</param>
        /// <returns></returns>
        public bool printOutFast(int x, string image_path, int sleep_time, int base_image_height)
        {
            if (!File.Exists(image_path))
            {
                MessageBox.Show("文件路径错误:" + image_path);
                return false;
            }

            Bitmap bitmap = new Bitmap(image_path);
            return printOutFast(x, bitmap, sleep_time, base_image_height);
        }
      
        /// <summary>
        /// 通过Bitmap对象快速打印图像
        /// 1)需要配合最新的打印机固件使用
        /// </summary>
        /// <param name="x"></param>
        /// <param name="bitmap"></param>
        /// <param name="sleep_time"></param>
        /// <param name="base_image_height"></param>
        /// <returns></returns>
        public bool printOutFast(int x, Bitmap bitmap, int sleep_time,int base_image_height)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            if (width > this.maxDots)
                return false;
            ImageConver conver = new ImageConver();
            byte[] data = conver.CovertImageHorizontal(bitmap, 128);

            if (data == null)
                return false;
            return _printOutFast(x, width, height, 1, 1, data, sleep_time, base_image_height);
        }

        /// <summary>
        /// 快速打印图片基本函数
        /// 1)需要配合最新的打印机固件使用
        /// </summary>
        /// <param name="x"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="enlargeX"></param>
        /// <param name="enlargeY"></param>
        /// <param name="data"></param>
        /// <param name="sleep_time"></param>
        /// <param name="base_image_height"></param>
        /// <returns></returns>
        private bool _printOutFast(int x, int width, int height, int enlargeX, int enlargeY, byte[] data, int sleep_time, int base_image_height)
        {       
            if (width > this.maxDots )
            {
                return false;
            }

            if (enlargeX > 2 || enlargeY > 2 || enlargeX < 0 || enlargeY  < 0)
            {
                MessageBox.Show("图像放大错误");
                return false;
            }

            int width_byte = (width-1)/8+1;
            if (width_byte * height != data.Length)
            {
                MessageBox.Show("data size error");
                return false;
            }      

            byte[] cmd = { 0x1B, 0x2B };
            if (base_image_height < 4)
            {
                base_image_height = 4; 
            }
            int HeightWriteUnit = base_image_height; //每张图片的高度 4 =< u < 56

            
            if (width_byte * HeightWriteUnit > 2000)
            {
                MessageBox.Show("单张图片数据太多，请减小图像高度");
                return false;
            }

            int HeightWrited = 0;

            int HeightLeft = height; //剩下的高度
            this.setLineSpace(0);//设置行间距为0
            for (; HeightLeft > 0; )
            {
                if (HeightLeft <= HeightWriteUnit)
                {
                    HeightWriteUnit = HeightLeft;
                }
                this.setXY(x, 0);
                port.write(cmd);
                port.write((UInt16)width); //图片宽度
                port.write((UInt16)HeightWriteUnit); //图片高度
                port.write((byte)enlargeX);
                port.write((byte)enlargeY);
                port.write(data, HeightWrited * width_byte, HeightWriteUnit * width_byte);
                HeightWrited += HeightWriteUnit;
                HeightLeft -= HeightWriteUnit;
                if (sleep_time > 0 )
                    Thread.Sleep(sleep_time);
            }
            this.setLineSpace(8);//设置行间距为0
            return true;
        }
    }
}
