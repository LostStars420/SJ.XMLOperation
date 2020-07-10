using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLOperation
{
    public class Program
    {
        static void Main(string[] args)
        {
            //D:\\Users\\sojo_qyl\\Desktop\\
            string filePath = "E:\\project\\XMLOperation\\XMLOperation\\bin\\Debug\\stu1.icd";
            new UpdateXML(filePath).UpdateXml();
            Console.ReadKey();
        }
    }
}
