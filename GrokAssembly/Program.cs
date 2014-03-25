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
				Console.Error.WriteLine ("args.length was " + args.Length);
				retval = 1;
			} else if (!File.Exists (args [0])) {
				writer.WriteStartElement ("error");
				writer.WriteString ("File does not exist");
				writer.WriteEndElement ();
				Console.Error.WriteLine ("File " + args [0] + " does not exist");
				retval = 2;
			} else {
				try {
					Assembly assembly = Assembly.LoadFile (Path.GetFullPath(args [0]));

					TextWriter temp = Console.Out;
					Console.SetOut(Console.Error);
					try {
						AssemblyCompanyAttribute[] companies = (AssemblyCompanyAttribute[]) assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
						if (companies != null && companies.Length > 0) {
							writer.WriteStartElement("company");
							writer.WriteString(companies[0].Company);
							writer.WriteEndElement();
						}
					} catch (FileNotFoundException) {
						// Getting custom attributes sometimes required dependent assemblies to be available
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
					Console.Error.WriteLine ("Assembly " + args [0] + " bad assembly file");
					retval = 3;
				} catch (ReflectionTypeLoadException) {
					// Nothing much we can do - the type information isn't available - maybe
					// becaused referenced libraries aren't around
				}
			}

			writer.WriteEndElement ();
			writer.Close ();
			return retval;
		}
	}
}