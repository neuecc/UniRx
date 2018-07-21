using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UniRx
{
    public static class TriggerExtensionGenerator
    {
        /*
                Generate liket following text...

                /// <summary>Get for OnAnimatorIKAsync | OnAnimatorMoveAsync.</summary>
                public static AsyncAnimatorTrigger GetAsyncAnimatorTrigger(this GameObject gameObject)
                {
                    return GetOrAddComponent<AsyncAnimatorTrigger>(gameObject);
                }

                /// <summary>Get for OnAnimatorIKAsync | OnAnimatorMoveAsync.</summary>
                public static AsyncAnimatorTrigger GetAsyncAnimatorTrigger(this Component component)
                {
                    return component.gameObject.GetAsyncAnimatorTrigger();
                }

        */
        class TypeInfo
        {
            public string TypeName { get; set; }
            public string[] Methods { get; set; }

            public override string ToString()
            {
                return TypeName + ", " + string.Join(" | ", Methods);
            }
        }

        public static string GenerateAsyncTriggerExtension(string rootDir)
        {
            var typeInfos = new List<TypeInfo>();

            // parse code

            foreach (var item in Directory.EnumerateFiles(rootDir))
            {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("Trigger"))
                {
                    var file = File.ReadAllText(item);

                    var tree = CSharpSyntaxTree.ParseText(file, new CSharpParseOptions(preprocessorSymbols: new[]
                    {
                        "CSHARP_7_OR_LATER"
                    }));

                    var type = tree.GetCompilationUnitRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();

                    var methods = type.DescendantNodes()
                        .OfType<MethodDeclarationSyntax>()
                        .Where(x => x.Modifiers.Any(y => y.Value.ToString() == "public"))
                        .Select(x => x.Identifier.Text)
                        .ToArray();

                    typeInfos.Add(new TypeInfo { TypeName = type.Identifier.Text, Methods = methods });
                }
            }

            // generate text

            var sb = new StringBuilder();
            foreach (var item in typeInfos)
            {
                if (item.TypeName == "AsyncStateMachineTrigger") continue;

                var methodText = string.Join(" | ", item.Methods);

                var template = $@"
/// <summary>Get for {methodText}.</summary>
public static {item.TypeName} Get{item.TypeName}(this GameObject gameObject)
{{
    return GetOrAddComponent<{item.TypeName}>(gameObject);
}}

/// <summary>Get for {methodText}.</summary>
public static {item.TypeName} Get{item.TypeName}(this Component component)
{{
    return component.gameObject.Get{item.TypeName}();
}}";

                if (item.TypeName == "AsyncMouseTrigger")
                {
                    sb.AppendLine();
                    sb.AppendLine("#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)");
                }

                sb.AppendLine(template);

                if (item.TypeName == "AsyncMouseTrigger")
                {
                    sb.AppendLine();
                    sb.AppendLine("#endif");
                }
            }

            return sb.ToString();
        }
    }
}
