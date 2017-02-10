using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

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
					Assembly assembly = Assembly.LoadFile (Path.GetFullPath(args [0]));

					TextWriter temp = Console.Out;
					try {
						AssemblyCompanyAttribute[] companies = (AssemblyCompanyAttribute[]) assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
						if (companies != null && companies.Length > 0) {
							writer.WriteStartElement("company");
							writer.WriteString(companies[0].Company);
							writer.WriteEndElement();
						}
					} catch (FileNotFoundException) {
						writer.WriteStartElement("company");
						writer.WriteString("UNKNOWN");
						writer.WriteEndElement();
					} catch (TypeLoadException) {
						writer.WriteStartElement("company");
						writer.WriteString("UNKNOWN");
						writer.WriteEndElement();
					} finally {
						Console.SetOut(temp);
					}

					writer.WriteStartElement ("product");
					writer.WriteString (xmlSanitize(assembly.GetName ().Name));
					writer.WriteEndElement ();

					writer.WriteStartElement ("version");
					writer.WriteString (xmlSanitize(assembly.GetName ().Version.ToString ()));
					writer.WriteEndElement ();

					writer.WriteStartElement ("fullname");
					writer.WriteString(xmlSanitize(assembly.FullName));
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
				} catch (ReflectionTypeLoadException) {
					writer.WriteStartElement ("error");
					writer.WriteString ("Unable to get type information");
					writer.WriteEndElement ();
					retval = 4;
				} catch (FileLoadException) {
					writer.WriteStartElement ("error");
					writer.WriteString ("Managed assembly cannot be loaded");
					writer.WriteEndElement ();
					retval = 6;
				} catch (Exception) {
					writer.WriteStartElement ("error");
					writer.WriteString ("An unknown error has occurred");
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
