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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace App.HttpApi
{
    /// <summary>
    /// 序列化方法类
    /// </summary>
    internal class SerializeHelper
    {
        //----------------------------------------------------
        // 序列化转换
        //----------------------------------------------------
        // 转化为字符串
        public static string ToText(object obj)
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
                var cfg = HttpApiConfig.Instance;
                var settings = new JsonSerializerSettings();
                settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                // 小驼峰命名法
                if (cfg.FormatLowCamel)
                    settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                // 递进格式
                settings.Formatting = cfg.FormatIndented;

                // 时间格式
                IsoDateTimeConverter datetimeConverter = new IsoDateTimeConverter();
                datetimeConverter.DateTimeFormat = cfg.FormatDateTime;
                settings.Converters.Add(datetimeConverter);

                // 枚举格式
                StringEnumConverter enumConverter = new StringEnumConverter();
                if (cfg.FormatEnum == EnumFomatting.Text)
                    settings.Converters.Add(enumConverter);

                //
                return JsonConvert.SerializeObject(obj, settings);
            }
        }

        // 转化为xml（对于未知类型会转化出错，考虑用三方类库）
        public static string ToXml(object obj)
        {
            if (obj == null)
                return "";
            else
            {
                // 用自己写的xml序列化类（未完善）
                var cfg = HttpApiConfig.Instance;
                var txt = new XmlSerializer(
                    cfg.FormatLowCamel, 
                    cfg.FormatEnum, 
                    cfg.FormatDateTime, 
                    cfg.FormatIndented==Formatting.Indented
                    ).ToXml(obj);
                return txt;

                /*
                // 用 Json 转为 xml，优点是统一
                // Bug: 数组无法正确序列化（类别信息会丢失）
                //var str = ToJson(obj);
                var type = obj.GetType();
                if (type.IsNullable())
                    type = type.GetNullableDataType();
                var name = type.Name;
                return JsonConvert.DeserializeXmlNode(str, name, false).OuterXml;
                */

                /*
                // 用微软官方的序列化类: 要求写一堆的[XmlInclude][XmlIgnore]等标签，对于未知的类是无能无力的，没法玩
                MemoryStream stream = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    var xs = new System.Xml.Serialization.XmlSerializer(obj.GetType());
                    xs.Serialize(writer, obj);
                    writer.Close();
                }
                return UnicodeEncoding.UTF8.GetString(stream.GetBuffer());
                */
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
