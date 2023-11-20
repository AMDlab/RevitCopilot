using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RevitCopilot.Model
{
    public class CompileManager
    {
        public void Compile(string csMethod)
        {
            // コンパイラのオプションを設定する
            var options = new CompilerParameters
            {
                GenerateInMemory = true,
                WarningLevel = 4,
                TreatWarningsAsErrors = false,
            };
            options.ReferencedAssemblies.AddRange(GetDllFileNames());

            // コンパイルを実行する
            var providerOptions = new Dictionary<string, string>
            {
                {"CompilerVersion", "v4.0"}  // これは .NET Framework 4.5 以上をターゲットとする C# 6.0 と互換性があります
            };
            var provider = new CSharpCodeProvider(providerOptions);
            CompilerResults results = provider.CompileAssemblyFromSource(options, csMethod);

            if (results.Errors.HasErrors)
            {
                // コンパイルエラーがあれば例外とする
                string exMessage = string.Empty;
                foreach (CompilerError error in results.Errors)
                {
                    exMessage += error.ErrorText + Environment.NewLine;
                }
                throw new Exception(exMessage);
            }
            else
            {
                // コンパイルされたアセンブリから、クラスとメソッドを取得する
                Assembly assembly = results.CompiledAssembly;
                var classTypes = assembly.GetTypes();
                if (classTypes.Count() != 1)
                {
                    throw new Exception($"クラスが{classTypes.Count()}個存在している。");
                }
                var classType = classTypes[0];
                var instance = Activator.CreateInstance(classType, null);
                var methods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (methods.Count() != 1)
                {
                    throw new Exception($"メソッドが{methods.Count()}個存在している。");
                }
                // メソッドを呼び出す
                var method = methods.First();
                method.Invoke(instance, new object[] { RevitDocuments.Doc });
            }
        }
        private string[] GetDllFileNames()
        {
            string executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string directoryPath = Path.GetDirectoryName(executingAssemblyPath);
            string[] fileNames = Directory.GetFiles(directoryPath, "*.dll");
            return fileNames;
        }
    }
}