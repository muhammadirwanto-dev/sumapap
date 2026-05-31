using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Sumapap.Mvvm.Maui.SourceGenerator.Tests
{
    public class ViewModelOwnerAttributeGeneratorTests
    {
        [Fact]
        public void Generator_WithViewModelOwnerAttribute_GeneratesExpectedCode()
        {
            // Arrange
            var source = @"
using Sumapap.Mvvm.Attributes;

namespace TestApp.Views
{
    [ViewModelOwner(typeof(TestViewModel))]
    public partial class TestPage
    {
    }
}

namespace TestApp.Views
{
    public class TestViewModel { }
}

namespace Sumapap.Mvvm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class ViewModelOwnerAttribute : System.Attribute
    {
        public ViewModelOwnerAttribute(System.Type viewModelType) { }
        public bool IsDefaultConstructor { get; init; }
    }
}

namespace Sumapap.Mvvm.Maui
{
    public static class MauiServiceProvider
    {
        public static System.IServiceProvider Current => null!;
    }
}

namespace Sumapap.Mvvm.Abstractions
{
    public interface IViewModelOwner<TViewModel> { }
}
";

            var expectedOutput = @"using Sumapap.Mvvm.Maui;
using Sumapap.Mvvm.Abstractions;

namespace TestApp.Views
{
    public partial class TestPage : IViewModelOwner<TestApp.Views.TestViewModel>
    {
        public TestApp.Views.TestViewModel ViewModel { get; private set; } = default!;

        private void PostInitializeComponent()
        {
            ViewModel = MauiServiceProvider.Current.GetRequiredService<TestApp.Views.TestViewModel>();
            BindingContext = ViewModel;
        }
    }
}";

            // Act
            var result = RunGenerator(source);

            // Assert
            Assert.NotNull(result.Results);
            Assert.NotEmpty(result.Results);
            Assert.Single(result.Results[0].GeneratedSources);

            var generatedSource = result.Results[0].GeneratedSources[0].SourceText.ToString();
            AssertCodeEquals(expectedOutput, generatedSource);
        }

        [Fact]
        public void Generator_WithIsDefaultConstructorTrue_GeneratesExpectedCode()
        {
            // Arrange
            var source = @"
using Sumapap.Mvvm.Attributes;

namespace TestApp.Views
{
    [ViewModelOwner(typeof(TestViewModel), IsDefaultConstructor = true)]
    public partial class TestPage
    {
    }
}

namespace TestApp.Views
{
    public class TestViewModel { }
}

namespace Sumapap.Mvvm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class ViewModelOwnerAttribute : System.Attribute
    {
        public ViewModelOwnerAttribute(System.Type viewModelType) { }
        public bool IsDefaultConstructor { get; init; }
    }
}

namespace Sumapap.Mvvm.Maui
{
    public static class MauiServiceProvider
    {
        public static System.IServiceProvider Current => null!;
    }
}

namespace Sumapap.Mvvm.Abstractions
{
    public interface IViewModelOwner<TViewModel> { }
}
";

            var expectedOutput = @"using Sumapap.Mvvm.Maui;
using Sumapap.Mvvm.Abstractions;

namespace TestApp.Views
{
    public partial class TestPage : IViewModelOwner<TestApp.Views.TestViewModel>
    {
        public TestApp.Views.TestViewModel ViewModel { get; }

        public TestPage()
        {
            InitializeComponent();

            ViewModel = MauiServiceProvider.Current.GetRequiredService<TestApp.Views.TestViewModel>();
            BindingContext = ViewModel;
        }
    }
}";

            // Act
            var result = RunGenerator(source);

            // Assert
            Assert.NotNull(result.Results);
            Assert.NotEmpty(result.Results);
            Assert.Single(result.Results[0].GeneratedSources);

            var generatedSource = result.Results[0].GeneratedSources[0].SourceText.ToString();
            AssertCodeEquals(expectedOutput, generatedSource);
        }

        [Fact]
        public void Generator_WithGenericAttribute_GeneratesExpectedCode()
        {
            // Arrange
            var source = @"
using Sumapap.Mvvm.Attributes;

namespace TestApp.Views
{
    [ViewModelOwner<TestViewModel>]
    public partial class TestPage
    {
    }
}

namespace TestApp.Views
{
    public class TestViewModel { }
}

namespace Sumapap.Mvvm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class ViewModelOwnerAttribute : System.Attribute
    {
        public ViewModelOwnerAttribute(System.Type viewModelType) { }
        public bool IsDefaultConstructor { get; init; }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class ViewModelOwnerAttribute<TViewModel> : ViewModelOwnerAttribute
    {
        public ViewModelOwnerAttribute() : base(typeof(TViewModel)) { }
    }
}

namespace Sumapap.Mvvm.Maui
{
    public static class MauiServiceProvider
    {
        public static System.IServiceProvider Current => null!;
    }
}

namespace Sumapap.Mvvm.Abstractions
{
    public interface IViewModelOwner<TViewModel> { }
}
";

            var expectedOutput = @"using Sumapap.Mvvm.Maui;
using Sumapap.Mvvm.Abstractions;

namespace TestApp.Views
{
    public partial class TestPage : IViewModelOwner<TestApp.Views.TestViewModel>
    {
        public TestApp.Views.TestViewModel ViewModel { get; private set; } = default!;

        private void PostInitializeComponent()
        {
            ViewModel = MauiServiceProvider.Current.GetRequiredService<TestApp.Views.TestViewModel>();
            BindingContext = ViewModel;
        }
    }
}";

            // Act
            var result = RunGenerator(source);

            // Assert
            Assert.NotNull(result.Results);
            Assert.NotEmpty(result.Results);
            Assert.Single(result.Results[0].GeneratedSources);

            var generatedSource = result.Results[0].GeneratedSources[0].SourceText.ToString();
            AssertCodeEquals(expectedOutput, generatedSource);
        }

        [Fact]
        public void Generator_WithoutAttribute_DoesNotGenerateCode()
        {
            // Arrange
            var source = @"
namespace TestApp.Views
{
    public partial class TestPage
    {
    }
}
";

            // Act
            var result = RunGenerator(source);

            // Assert
            Assert.Empty(result.Results[0].GeneratedSources);
        }

        [Fact]
        public void Generator_WithMultipleClasses_GeneratesCodeForEach()
        {
            // Arrange
            var source = @"
using Sumapap.Mvvm.Attributes;

namespace TestApp.Views
{
    [ViewModelOwner(typeof(FirstViewModel))]
    public partial class FirstPage
    {
    }

    [ViewModelOwner(typeof(SecondViewModel))]
    public partial class SecondPage
    {
    }

    public class FirstViewModel { }
    public class SecondViewModel { }
}

namespace Sumapap.Mvvm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class ViewModelOwnerAttribute : System.Attribute
    {
        public ViewModelOwnerAttribute(System.Type viewModelType) { }
        public bool IsDefaultConstructor { get; init; }
    }
}

namespace Sumapap.Mvvm.Maui
{
    public static class MauiServiceProvider
    {
        public static System.IServiceProvider Current => null!;
    }
}

namespace Sumapap.Mvvm.Abstractions
{
    public interface IViewModelOwner<TViewModel> { }
}
";

            var expectedFirstOutput = @"using Sumapap.Mvvm.Maui;
using Sumapap.Mvvm.Abstractions;

namespace TestApp.Views
{
    public partial class FirstPage : IViewModelOwner<TestApp.Views.FirstViewModel>
    {
        public TestApp.Views.FirstViewModel ViewModel { get; private set; } = default!;

        private void PostInitializeComponent()
        {
            ViewModel = MauiServiceProvider.Current.GetRequiredService<TestApp.Views.FirstViewModel>();
            BindingContext = ViewModel;
        }
    }
}";

            var expectedSecondOutput = @"using Sumapap.Mvvm.Maui;
using Sumapap.Mvvm.Abstractions;

namespace TestApp.Views
{
    public partial class SecondPage : IViewModelOwner<TestApp.Views.SecondViewModel>
    {
        public TestApp.Views.SecondViewModel ViewModel { get; private set; } = default!;

        private void PostInitializeComponent()
        {
            ViewModel = MauiServiceProvider.Current.GetRequiredService<TestApp.Views.SecondViewModel>();
            BindingContext = ViewModel;
        }
    }
}";

            // Act
            var result = RunGenerator(source);

            // Assert
            Assert.Equal(2, result.Results[0].GeneratedSources.Length);

            var firstSource = result.Results[0].GeneratedSources[0].SourceText.ToString();
            var secondSource = result.Results[0].GeneratedSources[1].SourceText.ToString();

            AssertCodeEquals(expectedFirstOutput, firstSource);
            AssertCodeEquals(expectedSecondOutput, secondSource);
        }

        [Fact]
        public void Generator_WithCustomNamespace_GeneratesExpectedCode()
        {
            // Arrange
            var source = @"
using Sumapap.Mvvm.Attributes;

namespace My.Custom.Namespace.Views
{
    [ViewModelOwner(typeof(TestViewModel))]
    public partial class TestPage
    {
    }

    public class TestViewModel { }
}

namespace Sumapap.Mvvm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class ViewModelOwnerAttribute : System.Attribute
    {
        public ViewModelOwnerAttribute(System.Type viewModelType) { }
        public bool IsDefaultConstructor { get; init; }
    }
}

namespace Sumapap.Mvvm.Maui
{
    public static class MauiServiceProvider
    {
        public static System.IServiceProvider Current => null!;
    }
}

namespace Sumapap.Mvvm.Abstractions
{
    public interface IViewModelOwner<TViewModel> { }
}
";

            var expectedOutput = @"using Sumapap.Mvvm.Maui;
using Sumapap.Mvvm.Abstractions;

namespace My.Custom.Namespace.Views
{
    public partial class TestPage : IViewModelOwner<My.Custom.Namespace.Views.TestViewModel>
    {
        public My.Custom.Namespace.Views.TestViewModel ViewModel { get; private set; } = default!;

        private void PostInitializeComponent()
        {
            ViewModel = MauiServiceProvider.Current.GetRequiredService<My.Custom.Namespace.Views.TestViewModel>();
            BindingContext = ViewModel;
        }
    }
}";

            // Act
            var result = RunGenerator(source);

            // Assert
            Assert.NotNull(result.Results);
            Assert.NotEmpty(result.Results);
            Assert.Single(result.Results[0].GeneratedSources);

            var generatedSource = result.Results[0].GeneratedSources[0].SourceText.ToString();
            AssertCodeEquals(expectedOutput, generatedSource);
        }

        [Fact]
        public void Generator_WithViewModelFromDifferentNamespace_GeneratesExpectedCode()
        {
            // Arrange
            var source = @"
using Sumapap.Mvvm.Attributes;
using TestApp.ViewModels;

namespace TestApp.Views
{
    [ViewModelOwner(typeof(TestViewModel))]
    public partial class TestPage
    {
    }
}

namespace TestApp.ViewModels
{
    public class TestViewModel { }
}

namespace Sumapap.Mvvm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class ViewModelOwnerAttribute : System.Attribute
    {
        public ViewModelOwnerAttribute(System.Type viewModelType) { }
        public bool IsDefaultConstructor { get; init; }
    }
}

namespace Sumapap.Mvvm.Maui
{
    public static class MauiServiceProvider
    {
        public static System.IServiceProvider Current => null!;
    }
}

namespace Sumapap.Mvvm.Abstractions
{
    public interface IViewModelOwner<TViewModel> { }
}
";

            var expectedOutput = @"using Sumapap.Mvvm.Maui;
using Sumapap.Mvvm.Abstractions;

namespace TestApp.Views
{
    public partial class TestPage : IViewModelOwner<TestApp.ViewModels.TestViewModel>
    {
        public TestApp.ViewModels.TestViewModel ViewModel { get; private set; } = default!;

        private void PostInitializeComponent()
        {
            ViewModel = MauiServiceProvider.Current.GetRequiredService<TestApp.ViewModels.TestViewModel>();
            BindingContext = ViewModel;
        }
    }
}";

            // Act
            var result = RunGenerator(source);

            // Assert
            Assert.NotNull(result.Results);
            Assert.NotEmpty(result.Results);
            Assert.Single(result.Results[0].GeneratedSources);

            var generatedSource = result.Results[0].GeneratedSources[0].SourceText.ToString();
            AssertCodeEquals(expectedOutput, generatedSource);
        }

        [Fact]
        public void Generator_WithGenericAttributeAndDefaultConstructor_GeneratesExpectedCode()
        {
            // Arrange
            var source = @"
using Sumapap.Mvvm.Attributes;

namespace TestApp.Views
{
    [ViewModelOwner<TestViewModel>(IsDefaultConstructor = true)]
    public partial class TestPage
    {
    }
}

namespace TestApp.Views
{
    public class TestViewModel { }
}

namespace Sumapap.Mvvm.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class ViewModelOwnerAttribute : System.Attribute
    {
        public ViewModelOwnerAttribute(System.Type viewModelType) { }
        public bool IsDefaultConstructor { get; init; }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class ViewModelOwnerAttribute<TViewModel> : ViewModelOwnerAttribute
    {
        public ViewModelOwnerAttribute() : base(typeof(TViewModel)) { }
    }
}

namespace Sumapap.Mvvm.Maui
{
    public static class MauiServiceProvider
    {
        public static System.IServiceProvider Current => null!;
    }
}

namespace Sumapap.Mvvm.Abstractions
{
    public interface IViewModelOwner<TViewModel> { }
}
";

            var expectedOutput = @"using Sumapap.Mvvm.Maui;
using Sumapap.Mvvm.Abstractions;

namespace TestApp.Views
{
    public partial class TestPage : IViewModelOwner<TestApp.Views.TestViewModel>
    {
        public TestApp.Views.TestViewModel ViewModel { get; }

        public TestPage()
        {
            InitializeComponent();

            ViewModel = MauiServiceProvider.Current.GetRequiredService<TestApp.Views.TestViewModel>();
            BindingContext = ViewModel;
        }
    }
}";

            // Act
            var result = RunGenerator(source);

            // Assert
            Assert.NotNull(result.Results);
            Assert.NotEmpty(result.Results);
            Assert.Single(result.Results[0].GeneratedSources);

            var generatedSource = result.Results[0].GeneratedSources[0].SourceText.ToString();
            AssertCodeEquals(expectedOutput, generatedSource);
        }

        private static GeneratorDriverRunResult RunGenerator(string source)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                .Cast<MetadataReference>();

            var compilation = CSharpCompilation.Create(
                "TestAssembly",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new ViewModelOwnerAttributeGenerator();

            var driver = CSharpGeneratorDriver.Create(generator);
            driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
                compilation,
                out var outputCompilation,
                out var diagnostics);

            return driver.GetRunResult();
        }

        private static void AssertCodeEquals(string expected, string actual)
        {
            var normalizedExpected = NormalizeCode(expected);
            var normalizedActual = NormalizeCode(actual);

            if (normalizedExpected != normalizedActual)
            {
                var message = $@"
Generated code does not match expected output.

=== EXPECTED ===
{expected}

=== ACTUAL ===
{actual}

=== DIFF (Normalized) ===
Expected Length: {normalizedExpected.Length}
Actual Length: {normalizedActual.Length}
";
                throw new Xunit.Sdk.XunitException(message);
            }
        }

        private static string NormalizeCode(string code)
        {
            return code
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Trim();
        }
    }
}
