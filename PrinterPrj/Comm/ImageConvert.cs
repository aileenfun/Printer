using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Printer.ESC_Set;

namespace Printer
{
    /// <summary>
    /// ͼ��ת����
    /// </summary>
    class ImageConver
    {
        private int width;
        /// <summary>
        /// ͼ����
        /// </summary>
        public int Width
        {
            get { return width; }
        }
        private int height;
        /// <summary>
        /// ͼ��߶�
        /// </summary>
        public int Height
        {
            get { return height; }
        }
        /// <summary>
        /// ���캯��
        /// </summary>
        public ImageConver()
        {
            width = 0;
            height = 0;
        }

        /// <summary>
        /// ������ֵ�ж��Ƿ�Ϊ�ڵ㡣
        /// 1)ֵ��RGB�������
        /// </summary>
        /// <param name="dot_color">�����ɫ</param>
        /// <param name="gray_threshold">��ֵ</param>
        /// <returns>true:Ϊ�ڵ㣻false:Ϊ�ڵ�</returns>
        private bool PixelIsBlack(Color dot_color, int gray_threshold)
        {
            byte gray = (byte)(dot_color.R * 0.299 + dot_color.G * 0.587 + dot_color.B * 0.114);  //�Ҷȹ�ʽ��
            if (gray < gray_threshold)                  //С����ֵΪ��ɫ
            {
                return true;
            }
            else                                       //������ֵΪ��ɫ
            {
                return false;
            }
        }

        /// <summary>
        /// ��ֱ��ʽת��ͼ��Ϊ����
        /// </summary>
        /// <param name="bitmap">ͼ��</param>
        /// <param name="gray_threshold">��ֵ</param>
        /// <returns>null��ʾʧ�ܣ����򷵻���������</returns>
        public byte[] CovertImageVertical(Bitmap bitmap, int gray_threshold, int column_dots)
        {
            width = bitmap.Width;
            height = bitmap.Height;

            int count = (height - 1) / column_dots + 1;//�ֳɶ��ٸ�СͼƬ
            int column_bytes = column_dots / 8;
            byte[] data = new byte[width * column_bytes * count];

            int index = 0;
            //����λͼ����ĻҶ�ֵ��ȷ����ӡλͼ��Ӧ�ĵ�ĺڰ�ɫ
            int sx = 0, sy = 0;                                                                          //λͼ��x��y����ֵ��
            for (int i = 0; i < count; i++)                                                     //8�������й�����С�У�һ��λͼ��ҪLengthColumn��С�У�
            {
                for (int j = 0; j < width; j++)                                     //һС�е�λͼ������BmpWidth�У���ҪBmpWidth���ֽڴ�ţ�
                {
                    sx = j;                                                                             //λͼ��ǰ���ص��x����Ϊ����x��
                    for (int k = 0; k < 8; k++)                                                        //k��С����8�������еĵ�ǰ�У�
                    {
                        sy = (i << 3) + k;                                                              //λͼ��ǰ���ص��y����Ϊ(С������8)+k;
                        if (sy >= height)                                            //���λͼ��ǰ���ص��y�������ʵ��λͼ�߶�(λͼʵ�ʸ߶ȿ��ܲ�Ϊ8��������)�����Ըõ���ɫ�����жϣ�
                        {
                            continue;
                        }
                        else
                        {
                            if (PixelIsBlack(bitmap.GetPixel(sx, sy), gray_threshold))                    //�жϵ�ǰ���Ƿ�Ϊ��ɫ��
                            {
                                data[index] |= (byte)(0x01 << (7 - k));      //���Ϊ��ɫ����ǰ������Ӧ�ֽڵĶ�ӦΪ��ֵΪ1��
                            }
                        }
                    }
                    index++;                                                      //һС�е�һ�����ص��ж���Ϻ�λͼ����ʵ�ʳ��ȼ�1��
                }
            }
            return data;
        }

        /// <summary>
        /// ��ֱ��ʽת��ͼ��Ϊ����
        /// </summary>
        /// <param name="image_path">ͼ��·��</param>
        /// <param name="gray_threshold">��ֵ</param>
        /// <returns>null��ʾʧ�ܣ����򷵻���������</returns>
        public byte[] CovertImageVertical(string image_path, int gray_threshold)
        {
            if (!File.Exists(image_path))
            {
                MessageBox.Show("�ļ�·������:" + image_path);
                return null;
            }

            Bitmap bitmap = new Bitmap(image_path);
            byte[] data = CovertImageVertical(bitmap, gray_threshold,8);
            bitmap.Dispose();
            return data;
        }

        /// <summary>
        /// ˮƽ��ʽת��ͼ��Ϊ����
        /// </summary>
        /// <param name="bitmap">ͼ��</param>
        /// <param name="gray_threshold">��ֵ</param>
        /// <returns>null��ʾʧ�ܣ����򷵻���������</returns>
        public byte[] CovertImageHorizontal(Bitmap bitmap, int gray_threshold)
        {
            width = bitmap.Width;
            height = bitmap.Height;
            int BytesPerLine = (width - 1) / 8 + 1;

            byte[] data = new byte[BytesPerLine * height];

            int index = 0;

            //����λͼ����ĻҶ�ֵ��ȷ����ӡλͼ��Ӧ�ĵ�ĺڰ�ɫ
            int x = 0, y = 0;                                                                          //λͼ��x��y����ֵ��
            for (int i = 0; i < height; i++)                                        //ÿ���ж�һ�����У���Ҫ�ж�BmpHeight��
            {
                for (int j = 0; j < BytesPerLine; j++)                                                    //ÿ����ҪLengthRow�ֽڴ�����ݣ�
                {
                    for (int k = 0; k < 8; k++)                                                        //ÿ���ֽڴ��8���㣬��1��1λ��
                    {
                        x = (j << 3) + k;                                                              //x����Ϊ�Ѽ����ֽڡ�8+��ǰ�ֽڵ�λk��
                        y = i;                                                                         //��ǰ�У�
                        if (x >= width)                                             //���λͼ��ǰ���ص��y�������ʵ��λͼ���(λͼʵ�ʿ�ȿ��ܲ�Ϊ8��������)�����Ըõ���ɫ�����жϣ�
                        {
                            continue;
                        }
                        else
                        {
                            if (PixelIsBlack(bitmap.GetPixel(x, y), gray_threshold))
                            {
                                data[index] |= (byte)(0x01 << k);
                            }
                        }
                    }
                    index++;                                                       //һ��������8���ж���Ϻ�λͼ����ʵ�ʳ��ȼ�1��
                }
                x = 0;
            }

            return data;
        }

        /// <summary>
        /// ˮƽ��ʽת��ͼ��Ϊ����
        /// </summary>
        /// <param name="image_path">ͼ��·��</param>
        /// <param name="gray_threshold">��ֵ</param>
        /// <returns>null��ʾʧ�ܣ����򷵻���������</returns>
        public byte[] CovertImageHorizontal(string image_path, int gray_threshold)
        {
            if (!File.Exists(image_path))
            {
                MessageBox.Show("�ļ�·������:" + image_path);
                return null;
            }

            Bitmap bitmap = new Bitmap(image_path);
            byte[] data = CovertImageHorizontal(bitmap, gray_threshold);
            bitmap.Dispose();
            return data;
        }
    }
}