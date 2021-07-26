using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace Common
{
    /// <summary>  
    /// 提供序列化和反序列化对象的相关静态方法。  
    /// </summary>  
    public class SerializerHelper
    {
        /// <summary>  
        /// 将指定的对象序列化为XML文件或二进制文件并返回执行状态。  
        /// </summary>  
        /// <param name="o">要序列化的对象</param>  
        /// <param name="path">保存路径</param>  
        /// <param name="isBinaryFile">序列化后生成的文件类型是否为二进制文件，true为二进制文件，否则为xml文件或文本文件</param>  
        /// <returns>返回执行状态</returns>  
        public static bool Serialize(Object o, string path, bool isBinaryFile)
        {
            bool flag = false;
            try
            {
                if (isBinaryFile)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        formatter.Serialize(stream, o);
                        flag = true;
                    }
                }
                else
                {
                    XmlSerializer serializer = new XmlSerializer(o.GetType());
                    using (XmlTextWriter writer = new XmlTextWriter(path, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        XmlSerializerNamespaces n = new XmlSerializerNamespaces();
                        n.Add("", "");
                        serializer.Serialize(writer, o, n);
                        flag = true;
                    }
                }
            }
            catch { flag = false; }
            return flag;
        }
        /// <summary>  
        /// 将指定的对象序列化为XML格式的字符串并返回。  
        /// </summary>  
        /// <param name="o">待序列化的对象</param>  
        /// <returns>返回序列化后的字符串</returns>  
        public static string XMLSerialize(Object o)
        {
            string xml = "";
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                using (MemoryStream mem = new MemoryStream())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(mem, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        XmlSerializerNamespaces n = new XmlSerializerNamespaces();
                        n.Add("", "");
                        serializer.Serialize(writer, o, n);
                        mem.Seek(0, SeekOrigin.Begin);
                        using (StreamReader reader = new StreamReader(mem))
                        {
                            xml = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch { xml = ""; }
            return xml;
        }
        public static XmlElement XMLSerializey(Object o)
        {
            string xml = "";
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                using (MemoryStream mem = new MemoryStream())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(mem, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        XmlSerializerNamespaces n = new XmlSerializerNamespaces();
                        n.Add("", "");
                        serializer.Serialize(writer, o, n);
                        mem.Seek(0, SeekOrigin.Begin);
                        using (StreamReader reader = new StreamReader(mem))
                        {
                            xml = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch { xml = ""; }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement;
        }
        /// <summary>  
        /// 将指定的对象序列化为XML文件，并返回执行状态。  
        /// </summary>  
        /// <param name="o">要序列化的对象</param>  
        /// <param name="path">生成的文件名称</param>  
        /// <returns>返回执行状态</returns>  
        public static bool XMLSerialize(Object o, string path)
        {
            return SerializerHelper.Serialize(o, path, false);
        }
        /// <summary>  
        /// 将指定的对象序列化为二进制文件，并返回执行状态。  
        /// </summary>  
        /// <param name="o">要序列化的对象</param>  
        /// <param name="path">生成的文件名称</param>  
        /// <returns>返回执行状态</returns>  
        public static bool BinarySerialize(Object o, string path)
        {
            return SerializerHelper.Serialize(o, path, true);
        }
        public static void XMLSerializeToWriter(Object o, XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(o.GetType());
            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, string.Empty);
            keySerializer.Serialize(writer, o, xmlns);
        }

        /// <summary>  
        /// 将指定的xml格式的字符反序列化为对应的对象并返回。  
        /// </summary>  
        /// <param name="t">对象的类型</param>  
        /// <param name="xml">待反序列化的xml格式的字符的内容</param>  
        /// <returns>返回对应的对象</returns>  
        public static Object XMLDeserializeFromstr(Type t, string xml)
        {
            Object o = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(t);
                using (MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    o = serializer.Deserialize(mem);
                }
            }
            catch { o = null; }
            return o;
        }
        /// <summary>  
        /// 将指定XML文件，反序列化为对应的对象并返回。  
        /// </summary>  
        /// <param name="t">对象的类型</param>  
        /// <param name="path">XML文件路径</param>  
        /// <returns>返回对象</returns>  
        public static Object XMLDeserialize(Type t, string path)
        {
            return SerializerHelper.Deserialize(t, path, false);
        }
        /// <summary>  
        /// 将指定二进制文件，反序列化为对应的对象并返回。  
        /// </summary>  
        /// <param name="t">对象的类型</param>  
        /// <param name="path">XML文件路径</param>  
        /// <returns>返回对象</returns>  
        public static Object BinaryDeserialize(Type t, string path)
        {
            return SerializerHelper.Deserialize(t, path, true);
        }
        public static Object XMLDeserialize(Type t, XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(t);
            return keySerializer.Deserialize(reader);
        }
        /// <summary>  
        /// 从指定的文件中反序列化出对应的对象并返回。  
        /// </summary>  
        /// <param name="t">要反序列化的对象类型</param>  
        /// <param name="path">文件路径</param>  
        /// <param name="isBinaryFile">反序列化的文件类型是否为二进制文件，true为二进制文件，否则为xml文件或文本文件</param>  
        /// <returns>返回Object</returns>  
        public static object Deserialize(Type t, string path, bool isBinaryFile)
        {
            Object o = null;
            try
            {
                if (!isBinaryFile)
                {
                    XmlSerializer serializer = new XmlSerializer(t);
                    using (XmlTextReader reader = new XmlTextReader(path))
                    {
                        o = serializer.Deserialize(reader);
                    }
                }
                else
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        o = formatter.Deserialize(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
                o = null;
            }
            return o;
        }

    } 
}
