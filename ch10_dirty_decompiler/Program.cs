using System;
using ICSharpCode.Decompiler.Ast;
using System.IO;
using Mono.Cecil;
using ICSharpCode.Decompiler;
using System.Collections.Generic;

namespace Decompiler {
	class MainClass {
		public static void Main (string[] args) {

			if (args.Length != 2) {
				Console.WriteLine("Dirty C# decompiler requires two arguments.");
				Console.WriteLine("decompiler.exe <path to assembly> <path to write-out directory>");
				return;
			}

			IEnumerable<AssemblyClass> klasses = GenerateAssemblyMethodSource (args[0]);
			foreach (AssemblyClass klass in klasses) {

				if (!Directory.Exists(args[1] + Path.DirectorySeparatorChar + klass.namespase))
					Directory.CreateDirectory(args[1] + Path.DirectorySeparatorChar + klass.namespase);

				string path = args [1] + Path.DirectorySeparatorChar + klass.namespase + Path.DirectorySeparatorChar + klass.name + ".cs";
				File.WriteAllText(path, klass.source);
			}
		}

		private static IEnumerable<AssemblyClass> GenerateAssemblyMethodSource (string assemblyPath) {
			AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly (assemblyPath, new ReaderParameters (ReadingMode.Deferred) { ReadSymbols = true });
			AstBuilder astBuilder = null;
			foreach (var defmod in assemblyDefinition.Modules) {
				foreach (var typeInAssembly in defmod.Types) {
					AssemblyClass klass = new AssemblyClass ();
					klass.name = typeInAssembly.Name;
					klass.namespase = typeInAssembly.Namespace;
					astBuilder = new AstBuilder (new DecompilerContext (assemblyDefinition.MainModule) { CurrentType = typeInAssembly });
					astBuilder.AddType (typeInAssembly);

					StringWriter output = new StringWriter ();
					astBuilder.GenerateCode (new PlainTextOutput (output));
					klass.source = output.ToString ();
					yield return klass;
				}
			}
		}
	}

	public class AssemblyClass {
		public string namespase;
		public string name;
		public string source;
	}
}