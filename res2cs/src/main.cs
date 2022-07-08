using System;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Resources.Tools;
using Mono.Options;

public class EntryPoint
{
	public static int Main (string[] args)
	{
		Console.WriteLine("res2cs.exe");

		if (args.Length != 4)
		{
			Console.WriteLine("Usage: res2cs.exe <input-filename.resx> <output-filename.cs> <ClassName> <Application.NamespaceName>");
			return 1;
		}
		else
		{
			for (int i = 0; i < 4; ++i)
			{
				Console.WriteLine($"args[{i}] = {args[i]}");
			}
		}

		string inputFilename = args[0];
		string outputFilename = args[1];


		string baseName = args[2] ; // Имя создаваемого класса.
		string generatedCodeNamespace=args[3]; // Пространство имен создаваемого класса.


		using (StreamWriter sw = new StreamWriter(outputFilename))
		{
			string[] errors = null;
			CSharpCodeProvider provider = new CSharpCodeProvider();
			CodeCompileUnit code = StronglyTypedResourceBuilder.Create(inputFilename, baseName, 
			                                                         generatedCodeNamespace, provider, 
			                                                         false, out errors);    
			if (errors.Length > 0)
			{
				foreach (var error in errors)
				{
					Console.WriteLine(error); 
				}
			}
			
			provider.GenerateCodeFromCompileUnit(code, sw, new CodeGeneratorOptions());                                         
			sw.Close();
		}
		return 0;
	}
}
