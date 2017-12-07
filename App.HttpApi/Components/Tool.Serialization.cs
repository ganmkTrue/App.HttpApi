﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Web;

namespace App.HttpApi
{
    internal partial class Tool
    {
        //----------------------------------------------------
        // 序列化转换
        //----------------------------------------------------
        // 转化为字符串
        public static string ToString(object obj)
        {
            return (obj == null) ? "" : obj.ToString();
        }

        // 转化为json字符串（用 Newtonsoft.Json 序列化）
        public static string ToJson(object obj)
        {
            if (obj == null)
                return "{}";
            else
            {
                Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
                settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
                settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                IsoDateTimeConverter datetimeConverter = new IsoDateTimeConverter();
                datetimeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                settings.Converters.Add(datetimeConverter);
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj, settings);

                /*
                // 用 JavaScriptSerializer 序列化（结构会复杂很多）
                StringBuilder sb = new StringBuilder();
                JavaScriptSerializer jsonSer = new JavaScriptSerializer();
                jsonSer.Serialize(obj, sb);
                return sb.ToString();
                */
            }
        }

        // 转化为xml
        public static string ToXml(object obj)
        {
            if (obj == null)
                return "";
            else
            {
                MemoryStream stream = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(obj.GetType());
                    xs.Serialize(writer, obj);
                    writer.Close();
                }
                return UnicodeEncoding.UTF8.GetString(stream.GetBuffer());
            }
        }

        // 转化为base64编码的图像字符串
        public static string ToImageBase64(object obj)
        {
            Bitmap img = obj as Bitmap;
            if (img == null)
                return "";
            else
            {
                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Png);
                byte[] bytes = ms.GetBuffer();
                string str = "data:image/png;base64," + Convert.ToBase64String(bytes);
                return str;
            }
        }

        // 转化为二进制字节数组
        public static byte[] ToBinary(object obj)
        {
            if (obj == null)
                return null;
            else
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter ser = new BinaryFormatter();
                ser.Serialize(ms, obj);
                byte[] bytes = ms.ToArray();
                ms.Close();
                return bytes;
            }
        }

        // 转化为二进制图像字节数组
        public static byte[] ToImageBytes(object obj)
        {
            Bitmap img = obj as Bitmap;
            if (img == null)
                return null;
            else
            {
                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Png);
                byte[] bytes = ms.ToArray();
                ms.Close();
                return bytes;
            }
        }

    }
}
