#r "EnvDTE"
#r "System"
#r "System.Core"
#r "System.Xml"
#r "System.Xml.Linq"

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

var solutionFilePath = ((EnvDTE.DTE)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0")).Solution.FullName;
var solutionDir = Directory.GetParent(solutionFilePath);

var dllPath = Path.Combine(solutionDir.FullName, @"Dlls\UniRx\bin\Debug\UniRx.dll");
var xmlPath = Path.Combine(solutionDir.FullName, @"Dlls\UniRx\bin\Debug\UniRx.xml");

var comments = ParseXmlComment(XDocument.Parse(File.ReadAllText(xmlPath)));
var commentsLookup = comments.ToLookup(x => x.ClassName);

var publicTypes = new[] { Assembly.LoadFrom(dllPath) }
    .SelectMany(x =>
    {
        try
        {
            return x.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null);
        }
        catch
        {
            return Type.EmptyTypes;
        }
    })
    .Where(x => x != null)
    .Where(x => x.IsPublic && !typeof(Delegate).IsAssignableFrom(x) && !x.GetCustomAttributes<ObsoleteAttribute>().Any())
    .Select(x => new
    {
        type = x,
        methods = x.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod)
            .Where(y => !y.IsSpecialName && !y.GetCustomAttributes<ObsoleteAttribute>().Any())
            .ToArray(),
        properties = x.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty | BindingFlags.SetProperty)
            .Where(y => !y.IsSpecialName && !y.GetCustomAttributes<ObsoleteAttribute>().Any())
            .ToArray(),
        fields = x.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetField | BindingFlags.SetField)
            .Where(y => !y.IsSpecialName && !y.GetCustomAttributes<ObsoleteAttribute>().Any())
            .ToArray(),
        staticMethods = x.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod)
            .Where(y => !y.IsSpecialName && !y.GetCustomAttributes<ObsoleteAttribute>().Any())
            .ToArray(),
        // events = x.GetEvents()
    })
    .ToArray();

var sb = new StringBuilder();

foreach (var item in publicTypes.OrderBy(x => x.type.FullName))
{
    sb.AppendLine("`" + BeautifyType(item.type, false) + "`");
    sb.AppendLine("---");
    var desc = commentsLookup[item.type.FullName].FirstOrDefault(x => x.MemberType == MemberType.Type)?.Summary ?? "";
    if (desc != "")
    {
        sb.AppendLine(desc);
    }

    sb.AppendLine("```csharp");
    var stat = (item.type.IsAbstract && item.type.IsSealed) ? "static " : "";
    var abst = (item.type.IsAbstract && !item.type.IsInterface && !item.type.IsSealed) ? "abstract ": "";
    var classOrStructOrEnumOrInterface = item.type.IsInterface ? "interface" : item.type.IsEnum ? "enum" : item.type.IsValueType ? "struct" : "class";

    sb.AppendLine($"public {stat}{abst}{classOrStructOrEnumOrInterface} {BeautifyType(item.type, true)}");
    var impl = string.Join(", ", new[] { item.type.BaseType }.Concat(item.type.GetInterfaces()).Where(x => x != null && x != typeof(object) && x != typeof(ValueType)).Select(x => BeautifyType(x)));
    if (impl != "")
    {
        sb.AppendLine("    : " + impl);
    }
    sb.AppendLine("```");
    sb.AppendLine();

    if (item.type.IsEnum)
    {
        var enums = Enum.GetNames(item.type)
            .Select(x => new { Name = x, Value = ((Int32)Enum.Parse(item.type, x)) })
            .OrderBy(x => x.Value)
            .ToArray();

        BuildTable(sb, "enum", enums, commentsLookup[item.type.FullName], x => x.Value.ToString(), x => x.Name, x => x.Name);
    }
    else
    {
        BuildTable(sb, "field", item.fields, commentsLookup[item.type.FullName], x => BeautifyType(x.FieldType), x => x.Name, x => x.Name);
        BuildTable(sb, "property", item.properties, commentsLookup[item.type.FullName], x => BeautifyType(x.PropertyType), x => x.Name, x => x.Name);
        BuildTable(sb, "method", item.methods, commentsLookup[item.type.FullName], x => BeautifyType(x.ReturnType), x => x.Name, x => BeautifyMethodInfo(x));
        BuildTable(sb, "static method", item.staticMethods, commentsLookup[item.type.FullName], x => BeautifyType(x.ReturnType), x => x.Name, x => BeautifyMethodInfo(x));
        // BuildTable(sb, "event", item.events, commentsLookup[item.type.FullName], x => Type.EmptyTypes.First(), x => x.Name);
    }
}

Console.WriteLine(sb.ToString());

void BuildTable<T>(StringBuilder sb, string label, T[] array, IEnumerable<XmlDocumentComment> docs, Func<T, string> type, Func<T, string> name, Func<T, string> finalName)
{
    if (array.Any())
    {
        sb.AppendLine("List of " + label);
        sb.AppendLine();

        IEnumerable<T> seq = array;
        if (label == "enum") // :)
        {
            sb.AppendLine("|Value|Name|Summary|");
        }
        else
        {
            sb.AppendLine("|Type|Name|Summary|");
            seq = array.OrderBy(x => name(x));
        }
        sb.AppendLine("|---|---|---|");

        foreach (var item2 in seq)
        {
            var summary = docs.FirstOrDefault(x => x.MemberName == name(item2))?.Summary ?? "";
            sb.AppendLine($"|`{type(item2)}`|{finalName(item2)}|{summary}|");
        }
        sb.AppendLine();
    }
}

XmlDocumentComment[] ParseXmlComment(XDocument xDoc)
{
    return xDoc.Descendants("member")
        .Select(x =>
        {
            var match = Regex.Match(x.Attribute("name").Value, @"(.):(.+)\.(\w+)?(\(.+\)|$)");
            if (!match.Groups[1].Success) return null;

            var memberType = (MemberType)match.Groups[1].Value[0];
            if (memberType == MemberType.None) return null;

            var summary = ((string)x.Element("summary")) ?? "";
            if (summary != "")
            {
                summary = string.Join("  ", summary.Split(new[] { "\r","\n", "\t" }, StringSplitOptions.RemoveEmptyEntries).Select(y => y.Trim()));
            }

            var returns = ((string)x.Element("returns")) ?? "";
            var remarks = ((string)x.Element("remarks")) ?? "";
            var parameters = x.Elements("param")
                .Select(e => Tuple.Create(e.Attribute("name").Value, e))
                .Distinct(new Item1EqualityCompaerer<string, XElement>())
                .ToDictionary(e => e.Item1, e => e.Item2.Value);

            var className = (memberType == MemberType.Type)
                ? match.Groups[2].Value + "." + match.Groups[3].Value
                : match.Groups[2].Value;

            return new XmlDocumentComment
            {
                MemberType = memberType,
                ClassName = className,
                MemberName = match.Groups[3].Value,
                Summary = summary.Trim(),
                Remarks = remarks.Trim(),
                Parameters = parameters,
                Returns = returns.Trim()
            };
        })
        .Where(x => x != null)
        .ToArray();
}

class XmlDocumentComment
{
    public MemberType MemberType { get; set; }
    public string ClassName { get; set; }
    public string MemberName { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
    public string Returns { get; set; }

    public override string ToString()
    {
        return MemberType + ":" + ClassName + "." + MemberName;
    }
}

class Item1EqualityCompaerer<T1, T2> : EqualityComparer<Tuple<T1, T2>>
{
    public override bool Equals(Tuple<T1, T2> x, Tuple<T1, T2> y)
    {
        return x.Item1.Equals(y.Item1);
    }

    public override int GetHashCode(Tuple<T1, T2> obj)
    {
        return obj.Item1.GetHashCode();
    }
}

string BeautifyType(Type t, bool isFull = false)
{
    if (t == null) return "";
    if (t == typeof(void)) return "void";
    if (!t.IsGenericType) return (isFull) ? t.FullName : t.Name;

    var innerFormat = string.Join(", ", t.GetGenericArguments().Select(x => BeautifyType(x)));
    return Regex.Replace(isFull ? t.GetGenericTypeDefinition().FullName : t.GetGenericTypeDefinition().Name, @"`.+$", "") + "<" + innerFormat + ">";
}

string BeautifyMethodInfo(MethodInfo methodInfo)
{
    var isExtension = methodInfo.GetCustomAttributes<System.Runtime.CompilerServices.ExtensionAttribute>(false).Any();

    var seq = methodInfo.GetParameters().Select(x =>
    {
        var suffix = x.HasDefaultValue ? (" = " + (x.DefaultValue ?? $"null")) : "";
        return "`" + BeautifyType(x.ParameterType) + "` " + x.Name + suffix;
    });

    return methodInfo.Name + "(" + (isExtension ? "this " : "") + string.Join(", ", seq) + ")";
}

enum MemberType
{
    Field = 'F',
    Property = 'P',
    Type = 'T',
    Event = 'E',
    Method = 'M',
    None = 0
}