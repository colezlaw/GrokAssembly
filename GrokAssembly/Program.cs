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
 * Copyright (c) 2019 The OWASP Foundation. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

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

                    writeNode(writer, "companyName", fileInfo.CompanyName);
                    writeNode(writer, "productName", fileInfo.ProductName);
                    writeNode(writer, "productVersion", fileInfo.ProductVersion);
                    writeNode(writer, "comments", fileInfo.Comments);
                    writeNode(writer, "fileDescription", fileInfo.FileDescription);
                    writeNode(writer, "fileName", fileInfo.FileName);
                    writeNode(writer, "fileVersion", fileInfo.FileVersion);
                    writeNode(writer, "internalName", fileInfo.InternalName);
                    writeNode(writer, "legalCopyright", fileInfo.LegalCopyright);
                    writeNode(writer, "legalTrademarks", fileInfo.LegalTrademarks);
                    writeNode(writer, "originalFilename", fileInfo.OriginalFilename);

                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(Path.GetFullPath(args[0]));
                    writeNode(writer, "fullName", assemblyName.FullName);

                    writer.WriteStartElement("namespaces");
                    try
                    {
                        using (var stream = File.OpenRead(Path.GetFullPath(args[0])))
                        using (var peFile = new PEReader(stream))
                        {
                            var reader = peFile.GetMetadataReader();
                            HashSet<string> nspaces = new HashSet<string>();

                            foreach (var handle in reader.TypeDefinitions)
                            {
                                var entry = reader.GetTypeDefinition(handle);
                                string ns = reader.GetString(entry.Namespace);
                                if (!nspaces.Contains(ns))
                                {
                                    writeNode(writer, "namespace", ns);
                                    nspaces.Add(ns);
                                }
                            }
                            writer.WriteEndElement();
                        }
                    }
                    catch (Exception ex)
                    {
                        writer.WriteEndElement();
                        writeNode(writer, "warning", ex.Message);
                    }
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
