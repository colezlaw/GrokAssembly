using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace GrokAssembly
{
	class MainClass
	{
		public static int Main (string[] args)
		{
			int retval = 0;
			XmlWriterSettings settings = new XmlWriterSettings ();
			settings.Encoding = new System.Text.UTF8Encoding (false);
			XmlWriter writer = XmlWriter.Create(Console.OpenStandardOutput(), settings);
			writer.WriteStartDocument ();
			writer.WriteStartElement ("assembly");

			if (args.Length < 1) {
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
						// Getting custom attributes sometimes required dependent assemblies to be available
					} catch (TypeLoadException) {
						// This may happen if a parent assembly is not available
					} finally {
						Console.SetOut(temp);
					}

					writer.WriteStartElement ("product");
					writer.WriteString (assembly.GetName ().Name);
					writer.WriteEndElement ();

					writer.WriteStartElement ("version");
					writer.WriteString (assembly.GetName ().Version.ToString ());
					writer.WriteEndElement ();

					writer.WriteStartElement ("fullname");
					writer.WriteString(assembly.FullName);
					writer.WriteEndElement();

					writer.WriteStartElement ("types");
					foreach (Type type in assembly.GetTypes()) {
						writer.WriteStartElement ("type");
						writer.WriteString (type.FullName);
						writer.WriteEndElement ();
					}
					writer.WriteEndElement ();	
				} catch (BadImageFormatException) {
					writer.WriteStartElement ("error");
					writer.WriteString ("Bad assembly file");
					writer.WriteEndElement ();
					retval = 3;
				} catch (ReflectionTypeLoadException) {
					// Nothing much we can do - the type information isn't available - maybe
					// becaused referenced libraries aren't around
				} catch (Exception) {
					// At this point, really not much we can do
				}
			}

			writer.WriteEndElement ();
			writer.Close ();
			return retval;
		}
	}
}