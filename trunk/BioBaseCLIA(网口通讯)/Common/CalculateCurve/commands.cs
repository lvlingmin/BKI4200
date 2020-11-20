using System;
using System.Text.RegularExpressions;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;
//using CLibrary;


namespace BioBaseCLIA.CalculateCurve
{
    /// <summary>
    /// 计算部分功能类方法
    /// </summary>
    public  class commands
    {
        public  void dataTranslate(int postion340, int jiange, string strData, ref int[,] cupdata, ref string head, ref int[] last)
        {
            int[] reactionData = new int[968];
            for (int i = 0; i < head.Length; i++)
            {
                reactionData[i] = 0;
            }
            for (int i = 0; i < 8; i++)
            {
                last[i] = 0;
            }
            for (int i = 0; i < 120; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    cupdata[i, j] = 0;
                }
            }
            string formatStr = strData.Replace(" ", "");
            //得到前8位数
            head = formatStr.Substring(0, 64);
            //List<string> list = new List<string>();
            //MatchCollection mc = Regex.Matches(temp, @"\w{2}");
            int t = 0;
            //foreach (Match m in mc)
            //{
            //    head[t++] = Convert.ToInt32(m.Value.ToString(), 16);
            //}
            if (formatStr.Length == 16)
                return;
            //填充二维数组
            string temp = formatStr.Substring(64);
            MatchCollection mc = Regex.Matches(temp, @"\w{4}");
            t = 0;
            foreach (Match m in mc)
            {
                reactionData[t++] = Convert.ToInt32(m.Value.ToString(), 16);
            }
            int cupTemp = 0, curCupNo = 0;
            curCupNo = Convert.ToInt32(head.Substring(4, 2), 16);
            if (strData.Length <= 3904)
                for (int i = 0; i < reactionData.Length; i += 8)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        cupdata[((postion340 + cupTemp - j * jiange + curCupNo) % 120), j] = reactionData[i + j];
                    }
                    cupTemp++;
                }
            else
            {
                for (int i = 0; i < reactionData.Length - 8; i += 8)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        cupdata[((postion340 + cupTemp - j * jiange + curCupNo) % 120), j] = reactionData[i + j];
                    }
                    cupTemp++;
                }
                for (int i = 0; i < 8; i++)
                {
                    last[i] = reactionData[960 + i];
                }
            }

        }
        //线性回归
        public  double LineBack(DataTable dt)
        {
            int cout = dt.Rows.Count;
            double heX = he(dt, 0);
            double heY = he(dt, 1);
            //double[] x = dx;//new double[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };//ast23--alp1//2.334472, 2.339348, 2.329251, 2.328136, 2.319636, 2.318534, 2.311948, 2.306136, 2.305412, 2.297848
            //double[] y = dy;//new double[] { 0.7565227, 0.779719, 0.8067556, 0.8337144, 0.8618082, 0.8894391, 0.9208201, 0.9503067, 0.9777844, 1.008213 };
            double x_ = heX / cout; //2.204112, 2.203469, 2.19642, 2.192281, 2.187531, 2.184063, 2.183434, 2.174988, 2.17095, 2.16201
            double y_ = heY / cout;
            double xx1 = 0;
            for (int i = 0; i < cout; i++)
            {
                xx1 += Math.Pow(double.Parse(dt.Rows[i][0].ToString()), 2);
            }
            double xx2 = Math.Pow(heX, 2) / cout;
            double xx = xx1 - xx2;

            double xy1 = 0;
            for (int i = 0; i < cout; i++)
            {
                xy1 += double.Parse(dt.Rows[i][0].ToString()) * double.Parse(dt.Rows[i][1].ToString());
            }
            double xy2 = heX * heY / cout;
            double xy = xy1 - xy2;

            double yy1 = 0;
            for (int i = 0; i < cout; i++)
            {
                yy1 += Math.Pow(double.Parse(dt.Rows[i][1].ToString()), 2);
            }
            double yy2 = Math.Pow(heY, 2) / cout;
            //double yy = yy1 - yy2;
            double b = xy / xx;
            //double a = y_ - b * x_;
            return double.IsNaN(b) ? 0 : b;
        }
        public  double lineBackA(DataTable dt)
        {
            int cout = dt.Rows.Count;
            double heX = he(dt, 0);
            double heY = he(dt, 1);
            //double[] x = dx;//new double[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };//ast23--alp1//2.334472, 2.339348, 2.329251, 2.328136, 2.319636, 2.318534, 2.311948, 2.306136, 2.305412, 2.297848
            //double[] y = dy;//new double[] { 0.7565227, 0.779719, 0.8067556, 0.8337144, 0.8618082, 0.8894391, 0.9208201, 0.9503067, 0.9777844, 1.008213 };
            double x_ = heX / cout; //2.204112, 2.203469, 2.19642, 2.192281, 2.187531, 2.184063, 2.183434, 2.174988, 2.17095, 2.16201
            double y_ = heY / cout;
            double xx1 = 0;
            for (int i = 0; i < cout; i++)
            {
                xx1 += Math.Pow(double.Parse(dt.Rows[i][0].ToString()), 2);
            }
            double xx2 = Math.Pow(heX, 2) / cout;
            double xx = xx1 - xx2;

            double xy1 = 0;
            for (int i = 0; i < cout; i++)
            {
                xy1 += double.Parse(dt.Rows[i][0].ToString()) * double.Parse(dt.Rows[i][1].ToString());
            }
            double xy2 = heX * heY / cout;
            double xy = xy1 - xy2;

            double yy1 = 0;
            for (int i = 0; i < cout; i++)
            {
                yy1 += Math.Pow(double.Parse(dt.Rows[i][1].ToString()), 2);
            }
            double yy2 = Math.Pow(heY, 2) / cout;
            double yy = yy1 - yy2;
            double b = xy / xx;
            double a = y_ - b * x_;
            return double.IsNaN(a) ? 0 : a;
        }
        //判断在哪个区域内
        public  int rigthValue(DataTable dt, double dataValue)
        {
            double[] dtDouble=new double[dt.Rows.Count];
            for (int j = 0; j < dt.Rows.Count; j++)
			{
                dtDouble[j]=double.Parse(dt.Rows[j][0].ToString());
			}
            //if (double.Parse(dt.Rows[0][0].ToString())>0)
                return searchI(dtDouble, 0, dataValue);
            //else
            //    return searchII(dtDouble, 0, dataValue);
        }
        public  int isExit(DataTable dt, double dataValue)
        {
            double[] dtDouble = new double[dt.Rows.Count];
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                if ( dataValue== double.Parse(dt.Rows[j][0].ToString()))
                    return j;
            }
            return -1;
        }
        private  int searchII(double[] values, int i, double dataValue)
        {
            if (i == values.Length) return i;
            if (dataValue < values[i])
                return searchI(values, i + 1, dataValue);
            else
                return i;
        }
        private  int searchI(double[] values, int i, double dataValue)
        {
            if (i == values.Length) return i;
            if (dataValue > values[i])
                return  searchI(values, i + 1, dataValue);
            else
                return i;
        }
         double he(DataTable dt,int colNum)
        {
            double temp = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                temp += double.Parse(dt.Rows[i][colNum].ToString());
            }
            return temp;
        }
        //以下为质控部分
        public  double CV(DataTable dt)
        {
            return STDEV(dt) / AVERAGE(dt);
        }
        public  double STDEV(DataTable dt)
        {
            double x_ = he(dt, 1) / dt.Rows.Count;
            double temp = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                temp += Math.Pow(double.Parse(dt.Rows[i][1].ToString()) - x_, 2);
            }
            return Math.Sqrt(temp / (dt.Rows.Count - 1));
        }
        public  double AVERAGE(DataTable dt)
        {
            return he(dt, 1) / dt.Rows.Count;
        }
        //返用哪个提示标志
        public  string getChaoBiaoSign(int i)
        {
            //if (iniVar.chaobiaosign == "1")
            //{
            //    if (i == 1)
            //        return "↑";
            //    else
            //        return "↓";
            //}
            //else
            //{
            //    if (i == 1)
            //        return "H";
            //    else
            //        return "L";
            //}
            return "L";
        }
        /// <summary>
        /// 格式化成十六进制
        /// </summary>
        /// <param name="num">源数据</param>
        /// <param name="len">最后的总长度</param>
        /// <returns></returns>
        public string intTo16(string num, int len)
        {
            return Convert.ToString(int.Parse(num), 16).PadLeft(len, '0');
            //return string.Format("{0:D" + len.ToString()+ "}", int.Parse(Convert.ToString(num, 16)));
        }
        //暂时没用
        public string StringToHex(string str)
        {
            str = str.Trim();
            byte[] ByteFoo = System.Text.Encoding.Default.GetBytes(str);
            string TempStr = "";
            foreach (byte b in ByteFoo)
            {
                TempStr += b.ToString("X"); //X表示十六进制显示 
            }
            return TempStr;
        }
        public string Hex(string Word)
        {
            int i = Word.Length;
            string temp;
            string end = "";
            byte[] array = new byte[2];
            int i1, i2;
            for (int j = 0; j < i; j++)
            {
                temp = Word.Substring(j, 1);
                array = System.Text.Encoding.Default.GetBytes(temp);

                if (array.Length.ToString() == "1")
                {

                    i1 = Convert.ToInt32(array[0]);
                    end += Convert.ToString(i1, 16);
                }
                else     //汉字字符   
                {
                    i1 = Convert.ToInt32(array[0]);
                    i2 = Convert.ToInt32(array[1]);
                    end += Convert.ToString(i1, 16);
                    end += Convert.ToString(i2, 16);
                }
            }

            return end.ToUpper();
        }
        //十六进制转换字节数组
        public byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
        //字节数组转换十六进制
        public string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }
    }
    public class txValue
    {
        public static readonly txValue Instance = new txValue();
        int i = -1;
        object obj = new object();
        public string getOne()
        {
            lock (obj)
            {
                i = (i + 1) % 26;
                return ((char)(i + 65)).ToString();
            }
        }
    }
    
   
    public class EnErity
    {
        private int[,] S = new int[2, 10] { { 6, 8, 4, 9, 7, 2, 1, 0, 5, 3 }, { 7, 6, 5, 9, 2, 8, 0, 4, 1, 3 } };
        private int[,] M = new int[4, 4] { { 5, 3, 2, 1 }, { 1, 5, 3, 2 }, { 2, 1, 5, 3 }, { 3, 2, 1, 5 } };
        private int[,] M1 = new int[4, 4] { { 5, 9, 4, 3 }, { 3, 5, 9, 4 }, { 4, 3, 5, 9 }, { 9, 4, 3, 5 } };
        private int[,] _data = new int[4, 3];
        private int[,] _data2 = new int[4, 3];
        private int[] Key = new int[] { 0, 1, 3 };
        public List<int> Encoder(List<int> data)
        {
            int p = 0;
            for (int i = 0; i < data.Count; i++)
            {
                data[i] = (data[i] + Key[p % Key.Length]) % 10;
                p++;
            }
            for (int i = 0; i < data.Count; i++)
            {
                _data[i / 3, i % 3] = data[i];
            }
            for (int k = 0; k < 5; k++)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        _data[j, i] = S[0, _data[j, i]];
                        //_data[j, i]=
                    }
                }
                int temp = _data[1, 2];
                _data[1, 2] = _data[1, 1];
                _data[1, 1] = _data[1, 0];
                _data[1, 0] = temp;

                temp = _data[2, 2];
                _data[2, 2] = _data[2, 1];
                _data[2, 1] = _data[2, 0];
                _data[2, 0] = temp;
                temp = _data[2, 2];
                _data[2, 2] = _data[2, 1];
                _data[2, 1] = _data[2, 0];
                _data[2, 0] = temp;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        _data2[j, i] = (M[j, 0] * _data[0, i] + M[j, 1] * _data[1, i] + M[j, 2] * _data[2, i] + M[j, 3] * _data[3, i]) % 10;
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        _data[j, i] = _data2[j, i];
                    }
                }
            }
            List<int> rt = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    rt.Add(_data[i, j]);
                }
            }
            return rt;
        }
        public List<int> Decoder(List<int> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                _data[i / 3, i % 3] = data[i];
            }
            for (int k = 0; k < 5; k++)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        _data2[j, i] = (M1[j, 0] * _data[0, i] + M1[j, 1] * _data[1, i] + M1[j, 2] * _data[2, i] + M1[j, 3] * _data[3, i]) % 10;
                    }
                }
                int temp = _data2[1, 0];
                _data2[1, 0] = _data2[1, 1];
                _data2[1, 1] = _data2[1, 2];
                _data2[1, 2] = temp;

                temp = _data2[2, 0];
                _data2[2, 0] = _data2[2, 1];
                _data2[2, 1] = _data2[2, 2];
                _data2[2, 2] = temp;
                temp = _data2[2, 0];
                _data2[2, 0] = _data2[2, 1];
                _data2[2, 1] = _data2[2, 2];
                _data2[2, 2] = temp;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        _data[j, i] = S[1, _data2[j, i]];
                    }
                }
            }
            List<int> rt = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    rt.Add(_data[i, j]);
                }
            }
            int p = 0;
            for (int i = 0; i < rt.Count; i++)
            {
                rt[i] = (rt[i] + (10 - Key[p % Key.Length]) % 10) % 10;
                p++;
            }
            return rt;
        }
        public string Encoder(string str)
        {
            string st = string.Empty;
            List<int> strs = new List<int>();
            foreach (char a in str)
            {
                strs.Add(int.Parse(a.ToString()));
            }
            foreach (int lt in Encoder(strs))
            {
                st += lt.ToString();
            }
            return st;
        }
        public string Decoder(string str)
        {
            string st = string.Empty;
            List<int> strs = new List<int>();
            foreach (char a in str)
            {
                strs.Add(int.Parse(a.ToString()));
            }
            foreach (int lt in Decoder(strs))
            {
                st += lt.ToString();
            }
            return st;
        }
        public void test()
        {
            List<int> tt = Encoder(new List<int> { 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 7 });
            tt = Decoder(tt);
        }
    }

    
}
