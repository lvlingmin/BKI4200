using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Common
{
  public  class ReadTxtWarn
    {
        public static string ReaderFile(string path)
        {
            string fileData = string.Empty;
            try
            {   ///读取文件的内容      
                StreamReader reader = new StreamReader(path, Encoding.Default);
                fileData = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                 throw new Exception(ex.Message,ex);    
            }  ///抛出异常      
            return fileData;
        }

        public static void WriteFile(string path, string text)
        {
            try
            {   
                //创建一个文件流，用以写入或者创建一个StreamWriter
                StreamWriter m_streamWriter = new StreamWriter(path, false, Encoding.Default);
                m_streamWriter.Flush();
                //使用StreamWriter来往文件中写入内容
                m_streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                //把text写入文件
                m_streamWriter.Write(text);
                //关闭此文件
                m_streamWriter.Flush();
                m_streamWriter.Close();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }  ///抛出异常  
        }
    }
}
