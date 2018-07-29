/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Copyright (c) 2018 The OWASP Foundation. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Reflection;

namespace GrokAssembly
{
    /// <summary>
    /// The main class for the GrokAssembly executable.
    /// Extracts extended properties from .NET assemblies and outputs it as
    /// XML for use by OWASP dependency-check.
    /// </summary>
    static class MainClass
    {
        /// <summary>
        /// The main method for the GrokAssembly executable.
        /// Accepts a single file path to a .NET assembly and extracts extended
        /// properties from .NET assemblies and outputs it as XML for use by
        /// OWASP dependency-check.
        /// </summary>
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
                writeNode(writer, "error", "Usage: GrokAssembly.exe <filename>");
                retval = 1;
            }
            else if (!File.Exists(args[0]))
            {
                writeNode(writer, "error", "File does not exist");
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


        /// <summary>
        /// Writes a complete xml node to the output stream. If the value is
        /// null or empty the node is not written.
        /// </summary>
        /// <param name="writer">A reference to an XMLWriter.</param>
        /// <param name="name">The name of the xml node.</param>
        /// <param name="value">The body text of the xml node..</param>
        private static void writeNode(XmlWriter writer, string name, string value)
        {
            if (value != null && value.Trim().Length > 0)
            {
                writer.WriteStartElement(name);
                writer.WriteString(xmlSanitize(value));
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Sanitizes the string for output in XML. Any non-XML characters are
        /// replaced by '?'.
        /// </summary>
        /// <returns>
        /// A string that can safetly be written into XML.
        /// </returns>
        /// <param name="input">The string to sanitize.</param>
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
