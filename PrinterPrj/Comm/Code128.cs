using System;

namespace Printer
{
    public class Code128 {
	    private const int CODE128_FORMAT_A = 1;
	    private const int CODE128_FORMAT_B = 2;
	    private const int CODE128_FORMAT_C = 3;

	    private const int V_CODE_A = 101;
	    private const int V_CODE_B = 100;
	    private const int V_CODE_C = 99;
	    private const int V_START_A = 103;
	    private const int V_START_B = 104;
        private const int V_START_C = 105; 
    	
	    public byte []encode_data = null; //�����ű������ַ����� 
	    public string decode_string = "";//�����Ž������ַ���
	    /*
	     * ���캯��
	     */
        public Code128(string text)
	    {
		    encode_data = encode(text);
	    }
    	
	    private bool charIsNumber(char data)
	    {
		    if(data <'0' || data>'9') 
			    return false;
		    return true;
	    }

        private bool charIsCodeB(char ch)
	    {
		    return (ch >= 32 && ch <= 127);
	    }

        private bool charIsNotCodeB(char ch) 
	    {
		    return (ch <= 31); 
	    }

        private bool charIsCodeA(char ch) 
	    {
		    return (ch <= 95);
	    }

        private bool charIsNotCodeA(char ch) 
	    {
		    return (ch >= 96 && ch <= 127);
	    }

        private bool isContinueNum(string str, int index, int max)
	    {
		    if (index + 4 > max) 
			    return false; //��ֹ���ݷ���Խ��
	        return ((charIsNumber(str[index+0]) 
	    		    && charIsNumber(str[index+1]) 
	    		    && charIsNumber(str[index+2]) 
	    		    && charIsNumber(str[index+3])
	    		    )) ;
	    }
    	
	    /*
	     * ��ԭʼ�ַ�������Ϊ��У��ͷ�ʽ��code128�ַ�����
	     */
	    private byte[] encode(string src)
	    {
		    byte[] dstc = new byte[80 * 4];
		    int src_len = src.Length;
		    int last_code_type = 0;
		    int len = 0;
		    int checkcodeID = 0;
		    int ch;

		    int CodeType = 0;// CODE128_FORMAT_A

		    if (src_len > 80) // ���80���ַ���
			    return null;
		    if (isContinueNum(src, 0, src_len)) {
			    CodeType = CODE128_FORMAT_C;
			    dstc[len++] = (byte)'{';
                dstc[len++] = (byte)'C';
			    checkcodeID = V_START_C;
		    } else if (charIsCodeB(src[0])) {
			    CodeType = CODE128_FORMAT_B;
                dstc[len++] = (byte)'{';
                dstc[len++] = (byte)'B';
			    checkcodeID = V_START_B;
		    } else if (charIsCodeA(src[0])) {
			    CodeType = CODE128_FORMAT_A;
                dstc[len++] = (byte)'{';
                dstc[len++] = (byte)'A';
			    checkcodeID = V_START_A;
		    } else
			    return null;

		    for (int i = 0; i < src_len; i++) {
			    if (src[i] > 127)
				    return null;// CODE128 ��Χ0~127
		    }
		    for (int i = 0, k = 1; i < src_len;) {
			    switch (CodeType) {
			    case CODE128_FORMAT_C:// CODE128_FORMAT_C:
				    last_code_type = CODE128_FORMAT_C;
				    for (; i < src_len; i += 2) {
					    if ((i + 1) < src_len) {
						    if (charIsNumber(src[i])
								    && charIsNumber(src[i + 1])) {
							    dstc[len++] = (byte) src[i];
							    dstc[len++] = (byte) src[i + 1];
							    checkcodeID += ((src[i] - 0x30) * 10 + (src[i + 1] - 0x30)) * (k++);
						    } else {
							    if ((charIsNumber(src[i]) && charIsCodeB(src[i + 1]))
									    || charIsCodeB(src[i])) {
								    CodeType = CODE128_FORMAT_B;
                                    dstc[len++] = (byte)'{';
                                    dstc[len++] = (byte)'B';
								    checkcodeID += V_CODE_B * (k++);
								    break;
							    } else if ((charIsNumber(src[i]) && charIsCodeA(src[i + 1]))
									    || charIsCodeA(src[i])) {
								    CodeType = CODE128_FORMAT_A;
                                    dstc[len++] = (byte)'{';
                                    dstc[len++] = (byte)'A';
								    checkcodeID += V_CODE_A * (k++);
								    break;
							    }
						    }
					    } else {
						    if (charIsCodeB(src[i])) {
							    CodeType = CODE128_FORMAT_B;
                                dstc[len++] = (byte)'{';
                                dstc[len++] = (byte)'B';
							    checkcodeID += V_CODE_B * (k++);
							    break;
						    } else if (charIsCodeA(src[i])) {
							    CodeType = CODE128_FORMAT_A;
                                dstc[len++] = (byte)'{';
                                dstc[len++] = (byte)'A';
							    checkcodeID += V_CODE_A * (k++);
							    break;
						    }
					    }
				    }
				    break;
			    case CODE128_FORMAT_B:// CODE128_FORMAT_B
				    last_code_type = CODE128_FORMAT_B;
				    for (; i < src_len; i++) {
					    if (isContinueNum(src, i, src_len)) {
						    CodeType = CODE128_FORMAT_C;
                            dstc[len++] = (byte)'{';
                            dstc[len++] = (byte)'C';
						    checkcodeID += V_CODE_C * (k++);
						    break;
					    } else if (charIsNotCodeB(src[i])) {
						    CodeType = CODE128_FORMAT_A;
                            dstc[len++] = (byte)'{';
                            dstc[len++] = (byte)'A';
						    checkcodeID += V_CODE_A * (k++);
						    break;
					    } else {
						    if (src[i] == '{') {
                                dstc[len++] = (byte)'{';
                                dstc[len++] = (byte)'{';
						    } else {
							    dstc[len++] = (byte) src[i];
						    }
						    checkcodeID += (k++) * (src[i] - 32);
					    }
				    }
				    break;
			    case CODE128_FORMAT_A:// CODE128_FORMAT_A:
				    last_code_type = CODE128_FORMAT_A;
				    for (; i < src_len; i++) {
					    if (isContinueNum(src, i, src_len)) {
						    CodeType = CODE128_FORMAT_C;
                            dstc[len++] = (byte)'{';
                            dstc[len++] = (byte)'C';
						    checkcodeID += V_CODE_C * (k++);
						    break;
					    } else if (charIsNotCodeA(src[i])) {
						    CodeType = CODE128_FORMAT_B;
                            dstc[len++] = (byte)'{';
                            dstc[len++] = (byte)'B';
						    checkcodeID += V_CODE_B * (k++);
						    break;
					    } else {
						    dstc[len++] = (byte) src[i];
						    ch = src[i];
						    if (ch >= 32 && ch <= 95)
							    checkcodeID += (k++) * (ch - 32);
						    else if (ch <= 31 && ch >= 0)
							    checkcodeID += (k++) * (ch + 64);
					    }
				    }
				    break;
			    default:
				    return null;
			    }
		    }
		    checkcodeID = checkcodeID % 103;
		    switch (last_code_type)// ����У���
		    {
		    case 1:// CODE128_FORMAT_A:
			    if (checkcodeID >= 0 && checkcodeID <= 63) // 0~63
				    dstc[len++] = (byte) (checkcodeID + 32);
			    else if (checkcodeID >= 64 && checkcodeID <= 95) // 64~95
				    dstc[len++] = (byte) (checkcodeID - 64);
			    else if (checkcodeID == 96) {
				    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'3';
			    } else if (checkcodeID == 97) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'2';
			    } else if (checkcodeID == 98) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'S';
			    } else if (checkcodeID == 99) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'C';
			    } else if (checkcodeID == 100) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'B';
			    } else if (checkcodeID == 101) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'4';
			    } else if (checkcodeID == 102) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'1';
			    }
			    break;
		    case 2:// CODE128_FORMAT_B:
			    if (checkcodeID >= 0 && checkcodeID <= 95) {
				    if (checkcodeID == 0x5B) // ת��'{'
				    {
                        dstc[len++] = (byte)'{';
                        dstc[len++] = (byte)'{';
				    } else
					    dstc[len++] = (byte) (checkcodeID + 32);
			    } else if (checkcodeID == 96) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'3';
			    } else if (checkcodeID == 97) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'2';
			    } else if (checkcodeID == 98) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'S';
			    } else if (checkcodeID == 99) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'C';
			    } else if (checkcodeID == 100) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'4';
			    } else if (checkcodeID == 101) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'A';
			    } else if (checkcodeID == 102) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'1';
			    }
			    break;
		    case 3:// CODE128_FORMAT_C:
			    if (checkcodeID >= 0 && checkcodeID <= 99) {
				    dstc[len++] = (byte) (checkcodeID / 10 + 48);
				    dstc[len++] = (byte) (checkcodeID % 10 + 48);
			    } else if (checkcodeID == 100) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'B';
			    } else if (checkcodeID == 101) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'A';
			    } else if (checkcodeID == 102) {
                    dstc[len++] = (byte)'{';
                    dstc[len++] = (byte)'1';
			    }
			    break;
		    default:
			    return null;
		    }
		    byte[] data = new byte[len + 1];
            Array.Copy(dstc, 0, data, 0, len);
		    data[len] = 0;
		    return data;
	    }

        private string[] Code128Rule = 
		    {
			    "11011001100",
			    "11001101100",
			    "11001100110",
			    "10010011000",
			    "10010001100",
			    "10001001100",
			    "10011001000",
			    "10011000100",
			    "10001100100",
			    "11001001000",
			    "11001000100",
			    "11000100100",
			    "10110011100",
			    "10011011100",
			    "10011001110",
			    "10111001100",
			    "10011101100",
			    "10011100110",
			    "11001110010",
			    "11001011100",
			    "11001001110",
			    "11011100100",
			    "11001110100",
			    "11101101110",
			    "11101001100",
			    "11100101100",
			    "11100100110",
			    "11101100100",
			    "11100110100",
			    "11100110010",
			    "11011011000",
			    "11011000110",
			    "11000110110",
			    "10100011000",
			    "10001011000",
			    "10001000110",
			    "10110001000",
			    "10001101000",
			    "10001100010",
			    "11010001000",
			    "11000101000",
			    "11000100010",
			    "10110111000",
			    "10110001110",
			    "10001101110",
			    "10111011000",
			    "10111000110",
			    "10001110110",
			    "11101110110",
			    "11010001110",
			    "11000101110",
			    "11011101000",
			    "11011100010",
			    "11011101110",
			    "11101011000",
			    "11101000110",
			    "11100010110",
			    "11101101000",
			    "11101100010",
			    "11100011010",
			    "11101111010",
			    "11001000010",
			    "11110001010",
			    "10100110000",
			    "10100001100",
			    "10010110000",
			    "10010000110",
			    "10000101100",
			    "10000100110",
			    "10110010000",
			    "10110000100",
			    "10011010000",
			    "10011000010",
			    "10000110100",
			    "10000110010",
			    "11000010010",
			    "11001010000",
			    "11110111010",
			    "11000010100",
			    "10001111010",
			    "10100111100",
			    "10010111100",
			    "10010011110",
			    "10111100100",
			    "10011110100",
			    "10011110010",
			    "11110100100",
			    "11110010100",
			    "11110010010",
			    "11011011110",
			    "11011110110",
			    "11110110110",
			    "10101111000",
			    "10100011110",
			    "10001011110",
			    "10111101000",
			    "10111100010",
			    "11110101000",
			    "11110100010",
			    "10111011110",
			    "10111101110",
			    "11101011110",
			    "11110101110",
			    "11010000100",//START A
			    "11010010000", //START B
			    "11010011100" //START C
		    };
    	
	    /*
	     * ��ȡ�ַ���������
	     */
	    private int  getCharIndex(int type,char c)
	    {
		    if(type==0) //type A
		    {
			    if(c>=0x20&&c<=0x5f) 
				    return (int)c-0x20;
			    else if(c<0x20) return (int)c+0x40;
			    else if(c==0x80) return 103;
			    else if(c==0x81) return  104;
			    else if(c==0x82) return  105;
			    else if(c==0x83) return  102;
			    else if(c==0x84) return  97;
			    else if(c==0x85) return  96;
			    else if(c==0x86) return  101;
			    else if(c==0x88) return  100;
			    else if(c==0x89) return  99;
			    else if(c==0x90) return  98;
			    else return 255;
		    }
		    else if(type==1) //type B
	        {
			    if(c>=0x20&&c<=0x7f) return (int)c-0x20;
			    else if(c==0x80) return 103;
			    else if(c==0x81) return  104;
			    else if(c==0x82) return  105;
			    else if(c==0x83) return  102;
			    else if(c==0x84) return  97;
			    else if(c==0x85) return  96;
			    else if(c==0x86) return  100;
			    else if(c==0x87) return  101;
			    else if(c==0x89) return  99;
			    else if(c==0x90) return  98;
			    else return 255;
	        }
		    else if(type==2) //type C
	        {
			    if(c<100) return c;
			    else if(c==0x80) return 103;
			    else if(c==0x81) return  104;
			    else if(c==0x82) return  105;
			    else if(c==0x83) return  102;
			    else if(c==0x87) return  101;
			    else if(c==0x88) return  100;
			    else return 255;
		    }
		    else return 255;
	    }
    	
	    /*
	     * ׷������Ļ�����Ԫ
	     */
        private bool addChar(int type, char ch)
	    {
		    int index = getCharIndex(type,ch);
	        if(index==255) 
	    	    return false;
	        decode_string += Code128Rule[index];
	        return true;
	    }
    	
	    /*
	     * �Ѵ�У���code128�ı�����Ϊcode128����
	     */
        public bool decode(byte[] str)
	    {
		    int i;
		    byte c;
		    int type=0;
		    byte []Cc = {0,0,0};
		    int code_type=0;
		    int str_len = str.Length-1;
		    for(i = 0; i < str_len;i++)
		    {
			    if(str[i]=='{')
			    {
				    if(str[i+1]=='C')
				    {	
					    code_type=1;
					    if (str[i+2]=='{' && str[i+3]=='1')
					    { 
						    i+=2;
					    }
				    }
				    else if (str[i+1] == '{')
				    {	
					    code_type=0;
				    }
				    else 
				    {	
					    code_type=0;
				    }			
				    i++;
			    }
			    else
			    {
				    if(code_type==1)
				    {	
					    i++; 
				    }
				    else
				    {	
				    }
			    }
		    }				
		    if(str[0]!='{') 
			    return false;
		    switch(str[1])
		    {
		        case (byte)'A':
				    type=0;
				    c=0x80;
				    break;
                case (byte)'B':
		            type=1;
		            c=0x81;
		            break;
                case (byte)'C':
			        type=2;
			        c=0x82;
			        break;
			    default: 
				    return false;
	        }

		    addChar(type,(char)c);
		    for(i=2;i<str_len;i++)
		    {
                if (str[i] == (byte)'{')
			    {
				    switch(str[i+1])
				    {
                        case (byte)'A':
						    if(type!=0)
						    {
							    c=0x87;
							    if(!addChar(type,(char)c)) 
								    return false;
						    }
						    type=0;
						    i++;
						    continue;
                        case (byte)'B':
						    if(type!=1)
						    {	
							    c=0x88;
							    if(!addChar(type,(char)c)) 
								    return false;
							    }
						    type=1;
						    i++;
						    continue;
                        case (byte)'C':
						    if(type!=2)
						    {	
							    c=0x89;	
							    if(!addChar(type,(char)c)) 
								    return false;
						    }
						    type=2;
						    i++;
						    continue;
                        case (byte)'1':
						    c=0x83;i++;
						    break;
                        case (byte)'2':
						    c=0x84;i++;
						    break;
                        case (byte)'3':
						    c=0x85;i++;break;
                        case (byte)'4':
						    c=0x86;i++;break;
                        case (byte)'S':
						    c=0x90;i++;break;
                        case (byte)'{':
                            c = (byte)'{'; i++; break;
					    default: 
						    return false;
				    }
				    if(!addChar(type,(char)c)) 
					    return false;
				    continue;
			    }
			    else if(type==2)
			    {
				    Cc[0]= str[i];
				    Cc[1]= str[i+1];
				    Cc[2]= 0;
				    if(Cc[0]<'0'||Cc[0]>'9'||Cc[1]<'0'||Cc[1]>'9') 
					    return false;
				    c =  (byte)((Cc[0]-'0')*10+(Cc[1]-'0'));
				    if(!addChar(type,(char)c)) 
					    return false;
				    i++;
				    continue;
			    }
			    else
			    {
				    c = str[i];
				    if(!addChar(type,(char)c)) 
					    return false;
			    }
		    }
		    decode_string +="1100011101011";
		    return true;
	    }
    }
}