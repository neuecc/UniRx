using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using UniRxAnalyzer;

namespace UniRxAnalyzer.Test
{
    [TestClass]
    public class HandleObservableAnalyzerTest : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new UniRxAnalyzer.HandleObservableAnalyzer();
        }

        [TestMethod]
        public void UnHandle()
        {
            var source = @"
using System;
   
class Test
{
    IObservable<int> GetObservable() => null;

    void Hoge()
    {
        GetObservable();
    }
}";
            var expected = new DiagnosticResult
            {
                Id = UniRxAnalyzer.HandleObservableAnalyzer.DiagnosticId,
                Message = "This call does not handle IObservable<T>.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 9)
                }
            };

            this.VerifyCSharpDiagnostic(source, expected);
        }

        [TestMethod]
        public void HandleConditional()
        {
            var source = @"
using System;
   
class Test
{
    IObservable<int> GetObservable() => null;

    void Hoge()
    {
        var x = (true) ? GetObservable() : GetObservable();
    }
}";
            
            this.VerifyCSharpDiagnostic(source);
        }

        [TestMethod]
        public void OkayReturn()
        {
            var source = @"
using System;
   
class Test
{
    IObservable<int> GetObservable() => null;

    IObservable<int> Hoge()
    {
        return GetObservable();
    }
}";
            this.VerifyCSharpDiagnostic(source);
        }

        [TestMethod]
        public void OkayAssignLocal()
        {
            var source = @"
using System;
   
class Test
{
    IObservable<int> GetObservable() => null;

    void Hoge()
    {
        var x = GetObservable();
    }
}";
            this.VerifyCSharpDiagnostic(source);
        }

        [TestMethod]
        public void OkayAssignField()
        {
            var source = @"
using System;
   
class Test
{
    IObservable<int> GetObservable() => null;

    IObservable<int> x;

    void Hoge()
    {
        x = GetObservable();
    }
}";
            this.VerifyCSharpDiagnostic(source);
        }

        [TestMethod]
        public void OkayMethodChain()
        {
            var source = @"
using System;
   
class Test
{
    IObservable<int> GetObservable() => null;

    void Huga(IObservable<int> x) { }

    void Hoge()
    {
        Huga(GetObservable());
    }
}";
            this.VerifyCSharpDiagnostic(source);
        }

        [TestMethod]
        public void OkayLINQ()
        {
            var source = @"
using System;
   
class Test
{
    IObservable<int> GetObservable() => null;

    void Hoge()
    {
        var q = from x in GetObservable()
                select x;
    }
}";
            this.VerifyCSharpDiagnostic(source);
        }

        [TestMethod]
        public void NgAfterLINQ()
        {
            var source = @"
using System;
   
class Test
{
    IObservable<int> GetObservable() => null;

    void Hoge()
    {
        var q = from x in GetObservable()
                select x;

        GetObservable();
    }
}";

            var expected = new DiagnosticResult
            {
                Id = UniRxAnalyzer.HandleObservableAnalyzer.DiagnosticId,
                Message = "This call does not handle IObservable<T>.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
            {
                    new DiagnosticResultLocation("Test0.cs", 13, 9)
                }
            };

            this.VerifyCSharpDiagnostic(source, expected);
        }

        [TestMethod]
        public void CallMethod()
        {
            var source = @"
using System;
   
class Test
{
    IObservable<int> GetObservable() => null;

    void Huga(IObservable<int> source)
    {
    }

    void Hoge()
    {
        Huga(GetObservable());
    }
}";

            this.VerifyCSharpDiagnostic(source);
        }

        [TestMethod]
        public void Constructor()
        {
            var source = @"
using System;

class Test2
{
    readonly IObservable<int> source;

    public Test2(IObservable<int> source)
    {
        this.source = source;
    }
}
   
class Test
{
    IObservable<int> GetObservable() => null;

    void Hoge()
    {
        var _ = new Test2(GetObservable());
    }
}";

            this.VerifyCSharpDiagnostic(source);
        }


        [TestMethod]
        public void LambdaReturn()
        {
            var source = @"
using System;

class Test
{
    IObservable<int> GetObservable() => null;

    void Hoge()
    {
        Func<IObservable<int>> _ = () => GetObservable();
    }
}";

            this.VerifyCSharpDiagnostic(source);
        }

        [TestMethod]
        public void NgInlambda()
        {
            var source = @"
using System;
   
class Test
{
    IObservable<int> GetObservable() => null;

    void Hoge()
    {
        var lambda = new Func<int, int>(x =>
        {
            GetObservable();
            return 0;
        });
    }
}";
            var expected = new DiagnosticResult
            {
                Id = UniRxAnalyzer.HandleObservableAnalyzer.DiagnosticId,
                Message = "This call does not handle IObservable<T>.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 12, 13)
                }
            };

            this.VerifyCSharpDiagnostic(source, expected);
        }

        int Hoge() => 3;
    }
}