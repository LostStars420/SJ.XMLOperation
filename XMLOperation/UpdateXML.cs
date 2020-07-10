using System;
using System.IO;
using System.Xml;

namespace XMLOperation
{
    public class UpdateXML
    {
        XmlDocument document;
        String filePath;
        string destfilePath = "E:\\project\\XMLOperation\\XMLOperation\\bin\\Debug\\stu2.icd";
        public UpdateXML(String filePath)
        {
            try
            {
                document = new XmlDocument();
                this.filePath = filePath;
                //  Stream inStream = new FileStream(filePath,FileMode.Append); 
                document.Load(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void UpdateXml()
        {
            bool isSuccess = false;
            try
            {
                Console.WriteLine("修改STU");
                Console.WriteLine("输入修改后的值：");
                String newSTUName = Console.ReadLine();
                String patternSTU = @"(STU[1-9]+)";
                while (!System.Text.RegularExpressions.Regex.IsMatch(newSTUName, patternSTU))
                {
                    Console.WriteLine("请输入正确的STU");
                    newSTUName = Console.ReadLine();
                }
                XmlNodeList connectedNodeList = document.GetElementsByTagName("ConnectedAP");
                for (int i = 0; i < connectedNodeList.Count; i++)
                {
                    XmlNode currNode = connectedNodeList.Item(i);
                    XmlAttributeCollection attr = currNode.Attributes;

                    XmlAttribute attribute = attr["iedName"];
                    attribute.Value = newSTUName;
                }

                XmlNodeList iedNodeList = document.GetElementsByTagName("IED");
                for (int i = 0; i < iedNodeList.Count; i++)
                {
                    XmlNode currNode = iedNodeList.Item(i);
                    XmlAttributeCollection attr = currNode.Attributes;
                    XmlAttribute attribute = attr["name"];
                    attribute.Value = newSTUName;
                }


                Console.WriteLine("修改APPID");
                Console.WriteLine("输入修改后的值：");
                String newAPPIDValue = Console.ReadLine();
                string pattern = @"([^A-Fa-f0-9]|\s+?)+";
                while (System.Text.RegularExpressions.Regex.IsMatch(newAPPIDValue, pattern))
                {
                    Console.WriteLine("请输入正确的16进制格式");
                    newAPPIDValue = Console.ReadLine();
                }
                XmlNodeList appidNodeList = document.GetElementsByTagName("P");
                for (int i = 0; i < appidNodeList.Count; i++)
                {
                    XmlNode currNode = appidNodeList.Item(i);
                    XmlAttributeCollection attr = currNode.Attributes;
                    for (int j = 0; j < attr.Count; j++)
                    {
                        XmlNode currAttr = attr[j];
                        if (currAttr.Value.Equals("APPID"))
                        {
                            if (currNode.ChildNodes.Count > 0)
                            {
                                XmlNodeList childNodeList = currNode.ChildNodes;
                                for (int k = 0; k < childNodeList.Count; k++)
                                {
                                    XmlNode childNode = childNodeList.Item(k);
                                    if (childNode.NodeType == XmlNodeType.Text)
                                    {
                                        childNode.InnerText = newAPPIDValue;
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("修改Val");
                Console.WriteLine("输入路径：");
                String path = null;
                while (!(path = Console.ReadLine()).Equals("#"))
                {
                    bool isFoundSTU = false;

                    String IntechDevice = path.Substring(0, path.IndexOf("/"));
                    String STU = IntechDevice.Substring(0, IntechDevice.Length - 3);
                    String LD0 = IntechDevice.Substring(IntechDevice.Length - 3);
                    String postStr = path.Substring(path.IndexOf("/") + 1);
                    String[] splitPath = postStr.Split('$');

                    XmlNodeList rootList = (XmlNodeList)document.GetElementsByTagName("IED");
                    for (int i = 0; i < rootList.Count; i++)
                    {
                        XmlNode currNode = rootList.Item(i);
                        if (currNode.NodeType == XmlNodeType.Element)
                        {
                            XmlAttributeCollection attr = currNode.Attributes;

                            for (int j = 0; j < attr.Count; j++)
                            {
                                if (attr[j].Value.Equals(STU))
                                {
                                    isFoundSTU = true;
                                    break;
                                }
                            }

                            if (isFoundSTU == true)
                            {
                                XmlNode node = SearchNodeByAttr(currNode, LD0);
                                if (node != null)
                                {
                                    XmlNode childNode = searchLNByAttrCollect(node, splitPath[0]);
                                    if (childNode != null)
                                    {
                                        int m = 1;
                                        while (m < splitPath.Length)
                                        {
                                            childNode = SearchNodeByAttr(childNode, splitPath[m]);
                                            m++;
                                        }
                                        if (childNode != null)
                                        {
                                            XmlNodeList grandNodeList = childNode.ChildNodes;
                                            for (int l = 0; l < grandNodeList.Count; l++)
                                            {
                                                XmlNode currgrandNode = grandNodeList.Item(l);

                                                if (currgrandNode.ChildNodes.Count > 0)
                                                {
                                                    XmlNodeList lastNodeList = currgrandNode.ChildNodes;
                                                    for (int k = 0; k < lastNodeList.Count; k++)
                                                    {
                                                        if (lastNodeList.Item(k).NodeType == XmlNodeType.Text)
                                                        {
                                                            Console.WriteLine("输入修改的值：");
                                                            String newValue = Console.ReadLine();
                                                            lastNodeList.Item(k).Value = newValue;
                                                            isSuccess = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                document.Save(destfilePath);
                if (isSuccess)
                {
                    Console.WriteLine("修改成功");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 根据属性值查找节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        private XmlNode SearchNodeByAttr(XmlNode node, String attrName)
        {
            XmlNodeList nodeList;
            XmlNode returnNode = null;
            Boolean isFound = false;
            nodeList = node.ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode currNode = nodeList.Item(i);
                if (currNode.NodeType == XmlNodeType.Element)
                {
                    XmlAttributeCollection attr = currNode.Attributes;

                    for (int j = 0; j < attr.Count; j++)
                    {
                        XmlNode attrNode = attr.Item(j);
                        if (attrNode.Value.Equals(attrName))
                        {
                            isFound = true;
                            break;
                        }
                    }

                    if (isFound == true)
                    {
                        returnNode = currNode;
                        break;
                    }
                    else
                    {
                        if (currNode.ChildNodes.Count > 0)
                        {
                            returnNode = SearchNodeByAttr(currNode, attrName);
                        }

                    }
                }
            }
            return returnNode;
        }


        /// <summary>
        /// 根据属性值查找LN
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        private XmlNode searchLNByAttrCollect(XmlNode node, String attrName)
        {
            XmlNodeList nodeList;
            XmlNode returnNode = null;
            Boolean isFound = false;
            nodeList = node.ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode currNode = nodeList.Item(i);
                if (currNode.NodeType == XmlNodeType.Element)
                {
                    XmlAttributeCollection attr = currNode.Attributes;

                    String LN = "";
                    String lnClass = "";
                    String inst = "";
                    String prefix = "";

                    if (attr["lnClass"] != null)
                    {
                        lnClass = attr["lnClass"].Value;
                    }

                    if (attr["inst"] != null)
                    {
                        inst = attr["inst"].Value.ToString();
                    }

                    if (attr["prefix"] != null)
                    {
                        prefix = attr["prefix"].Value;
                    }

                    LN = prefix + lnClass + inst;
                    if (LN.Equals(attrName))
                    {
                        isFound = true;
                    }
                    if (isFound == true)
                    {
                        returnNode = currNode;
                        break;
                    }
                    else
                    {
                        if (currNode.ChildNodes.Count > 0)
                        {
                            returnNode = searchLNByAttrCollect(currNode, attrName);
                        }
                    }
                }
            }
            return returnNode;
        }

        /// <summary>
        /// 判断十六进制字符串hex是否正确
        /// </summary>
        /// <param name="hex">十六进制字符串</param>
        /// <returns>true：不正确，false：正确</returns>
        public bool IsIllegalHexadecimal(string hex, string pattern)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(hex, pattern);
        }
    }
}
