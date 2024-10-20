using System;
using System.IO;
using System.Security;
using System.Collections.Generic;

namespace Almond.Util
{
    public class XMLParser
    {
        public Dictionary<string, Dictionary<string, string>> LoadMap(string fileName, out string key)
        {
            key = Path.GetFileNameWithoutExtension(fileName);
            var xml = Load(fileName);
            return LoadMap(xml);
        }

        public bool LoadMap(string fileName, out Dictionary<string, Dictionary<string, string>> map)
        {
            try
            {
                var xml = Load(fileName);
                map = LoadMap(xml);
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
                map = null;
                return false;
            }
        }

        public static bool LoadIntMap(string fileName, bool isForceOutterRecoure, out Dictionary<int, Dictionary<string, string>> map)
        {
            try
            {
                SecurityElement xml;
                xml = Load(fileName);
                if (xml == null)
                {
                    LoggerHelper.Error("File not exist: " + fileName);
                    map = null;
                    return false;
                }
                else
                {
                    map = LoadIntMap(xml, fileName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("Load Int Map Error: " + fileName + "  " + ex.Message);
                map = null;
                return false;
            }
        }

        public static Dictionary<int, Dictionary<string, string>> LoadIntMap(SecurityElement xml, string source)
        {
            var result = new Dictionary<int, Dictionary<string, string>>();

            int count = xml.Children.Count;
            for (int index = 0; index < count; index++ )
            {
                SecurityElement subMap = xml.Children[index] as SecurityElement;
                if (subMap.Children == null || subMap.Children.Count == 0)
                {
                    LoggerHelper.Warning("empty row in row NO." + index + " of " + source);
                    continue;
                }
                int key = int.Parse((subMap.Children[0] as SecurityElement).Text);
                if (result.ContainsKey(key))
                {
                    LoggerHelper.Warning(string.Format("Key {0} already exist, in {1}.", key, source));
                    continue;
                }

                var children = new Dictionary<string, string>();
                result.Add(key, children);
                for (int i = 1; i < subMap.Children.Count; i++)
                {
                    var node = subMap.Children[i] as SecurityElement;
                    string tag;
                    if (node.Tag.Length < 3)
                    {
                        tag = node.Tag;
                    }
                    else
                    {
                        var tagTial = node.Tag.Substring(node.Tag.Length - 2, 2);
                        if (tagTial == "_i" || tagTial == "_s" || tagTial == "_f" || tagTial == "_l" || tagTial == "_k" || tagTial == "_m")
                            tag = node.Tag.Substring(0, node.Tag.Length - 2);
                        else
                            tag = node.Tag;
                    }

                    if (node != null && !children.ContainsKey(tag))
                    {
                        if (string.IsNullOrEmpty(node.Text))
                            children.Add(tag, "");
                        else
                            children.Add(tag, node.Text.Trim());
                    }
                    else
                        LoggerHelper.Warning(string.Format("Key {0} already exist, index {1} of {2}.", node.Tag, i, node.ToString()));
                }
            }
            return result;
        }

        public static Dictionary<string, Dictionary<string, string>> LoadMap(SecurityElement xml)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();

            int count = xml.Children.Count;
            for (int index = 0; index < count; index++ )
            {
                SecurityElement subMap = xml.Children[index] as SecurityElement;
                string key = (subMap.Children[0] as SecurityElement).Text.Trim();
                if (result.ContainsKey(key))
                {
                    LoggerHelper.Warning(string.Format("Key {0} already exist, in {1}.", key, xml.ToString()));
                    continue;
                }

                var children = new Dictionary<string, string>();
                result.Add(key, children);
                for (int i = 1; i < subMap.Children.Count; i++)
                {
                    var node = subMap.Children[i] as SecurityElement;
                    if (node != null && !children.ContainsKey(node.Tag))
                    {
                        if (string.IsNullOrEmpty(node.Text))
                            children.Add(node.Tag, "");
                        else
                            children.Add(node.Tag, node.Text.Trim());
                    }
                    else
                        LoggerHelper.Warning(string.Format("Key {0} already exist, index {1} of {2}.", node.Tag, i, node.ToString()));
                }
            }
            return result;
        }


        public static SecurityElement Load(string fileName)
        {
            string xmlText = "";// LoadText(fileName);
            if (string.IsNullOrEmpty(xmlText))
                return null;
            else
                return LoadXML(xmlText);
        }
        public static SecurityElement LoadXML(string xml)
        {
            try
            {
                SecurityParser securityParser = new SecurityParser();
                securityParser.LoadXml(xml);
                return securityParser.ToXml();
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
                return null;
            }
        }

        public static void SaveText(string fileName, string text)
        {
            if (!Directory.Exists(Utils.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Utils.GetDirectoryName(fileName));
            }
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine($"<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    sw.Write(text);
                    sw.Flush();
                    sw.Close();
                }
                fs.Close();
            }
        }
    }
}