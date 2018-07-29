using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Reflection;

namespace GrokAssembly
{
    static class MainClass
    {
        public static int Main(string[] args)
        {
            int retval = 0;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new System.Text.UTF8Encoding(false);
            XmlWriter writer = XmlWriter.Create(Console.OpenStandardOutput(), settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("assembly");

            if (args.Length != 1)
            {
                writer.WriteStartElement("error");
                writer.WriteString("Usage: GrokAssembly.exe <filename>");
                writer.WriteEndElement();
                retval = 1;
            }
            else if (!File.Exists(args[0]))
            {
                writer.WriteStartElement("error");
                writer.WriteString("File does not exist");
                writer.WriteEndElement();
                retval = 2;
            }
            else
            {
                try
                {
                    FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(Path.GetFullPath(args[0]));

                    writeNode(writer, "CompanyName", fileInfo.CompanyName);
                    writeNode(writer, "ProductName", fileInfo.ProductName);
                    writeNode(writer, "ProductVersion", fileInfo.ProductVersion);
                    writeNode(writer, "Comments", fileInfo.Comments);
                    writeNode(writer, "FileDescription", fileInfo.FileDescription);
                    writeNode(writer, "FileName", fileInfo.FileName);
                    writeNode(writer, "FileVersion", fileInfo.FileVersion);
                    writeNode(writer, "InternalName", fileInfo.InternalName);
                    writeNode(writer, "LegalCopyright", fileInfo.LegalCopyright);
                    writeNode(writer, "LegalTrademarks", fileInfo.LegalTrademarks);
                    writeNode(writer, "OriginalFilename", fileInfo.OriginalFilename);

                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(Path.GetFullPath(args[0]));
                    writeNode(writer, "fullname", assemblyName.FullName);

                }
                catch (BadImageFormatException)
                {
                    writeNode(writer, "error", "Bad assembly file");
                    retval = 3;
                }
                catch (FileLoadException)
                {
                    writeNode(writer, "error", "Managed assembly cannot be loaded");
                    retval = 6;
                }
                catch (Exception e)
                {
                    writeNode(writer, "error", e.Message);
                    retval = 5;
                }
            }

            writer.WriteEndElement();
            writer.Close();
            return retval;
        }
        private static void writeNode(XmlWriter writer, string name, string value)
        {
            if (value != null && value.Trim().Length > 0)
            {
                writer.WriteStartElement(name);
                writer.WriteString(xmlSanitize(value));
                writer.WriteEndElement();
            }
        }
        private static string xmlSanitize(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                if (XmlConvert.IsXmlChar(c))
                {
                    result.Append(c);
                }
                else
                {
                    result.Append('?');
                }
            }
            return result.ToString();
        }

    }
}
