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
		private static string xmlSanitize(string input)
		{
			StringBuilder result = new StringBuilder();
			foreach (char c in input)
			{
				if (XmlConvert.IsXmlChar(c))
				{
					result.Append(c);
				}
				else {
					result.Append('?');
				}
			}

			return result.ToString();
		}

		public static int Main (string[] args)
		{
			int retval = 0;
			XmlWriterSettings settings = new XmlWriterSettings ();
			settings.Encoding = new System.Text.UTF8Encoding (false);
			XmlWriter writer = XmlWriter.Create(Console.OpenStandardOutput(), settings);
			writer.WriteStartDocument ();
			writer.WriteStartElement ("assembly");

			if (args.Length != 1) {
				writer.WriteStartElement ("error");
				writer.WriteString ("Usage: GrokAssembly.exe <filename>");
				writer.WriteEndElement ();
				retval = 1;
			} else if (!File.Exists (args [0])) {
				writer.WriteStartElement ("error");
				writer.WriteString ("File does not exist");
				writer.WriteEndElement ();
				retval = 2;
			} else {
				try {
					FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo (Path.GetFullPath(args [0]));

                    String companyName = fileInfo.CompanyName;

                    if (companyName != null) {
						writer.WriteStartElement("company");
						writer.WriteString(xmlSanitize(companyName));
						writer.WriteEndElement();
					} else {
                        writer.WriteStartElement("company");
						writer.WriteString("UNKNOWN");
						writer.WriteEndElement();
                    }

					writer.WriteStartElement ("product");
					writer.WriteString (xmlSanitize(fileInfo.ProductName));
					writer.WriteEndElement ();

					writer.WriteStartElement ("version");
					writer.WriteString (xmlSanitize(fileInfo.ProductVersion.ToString ()));
					writer.WriteEndElement ();

                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(Path.GetFullPath(args[0]));


                    writer.WriteStartElement ("fullname");
					writer.WriteString(xmlSanitize(assemblyName.FullName));
					writer.WriteEndElement();

					/*
					writer.WriteStartElement ("types");
					foreach (Type type in assembly.GetTypes()) {
						writer.WriteStartElement ("type");
						writer.WriteString (xmlSanitize(type.FullName));
						writer.WriteEndElement ();
					}
					writer.WriteEndElement ();
					*/
				} catch (BadImageFormatException) {
					writer.WriteStartElement ("error");
					writer.WriteString ("Bad assembly file");
					writer.WriteEndElement ();
					retval = 3;
				} catch (FileLoadException) {
					writer.WriteStartElement ("error");
					writer.WriteString ("Managed assembly cannot be loaded");
					writer.WriteEndElement ();
					retval = 6;
				} catch (Exception e) {
					writer.WriteStartElement ("error");
					writer.WriteString (e.Message);
					writer.WriteEndElement ();
					retval = 5;
				}
			}

			writer.WriteEndElement ();
			writer.Close ();
			return retval;
		}
	}
}
