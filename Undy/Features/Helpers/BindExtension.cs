using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Undy.Features.Helpers
{
    /// <summary>
    /// Clean binding markup extension for GridViewColumns.
    /// Usage:
    /// DisplayMemberBinding="{helpers:Bind ProductName}"
    /// DisplayMemberBinding="{helpers:Bind TotalPrice:C}"
    /// DisplayMemberBinding="{helpers:Bind SalesDate:dd-MM-yyyy}"
    /// </summary>
    [MarkupExtensionReturnType(typeof(Binding))]
    public class BindExtension : MarkupExtension
    {
        public string Path { get; set; }
        public string Format { get; set; }

        public BindExtension(string expression)
        {
            // Example expression inputs:
            // "ProductName"
            // "TotalPrice:C"
            // "SalesDate:dd-MM-yyyy"

            var parts = expression.Split(':');

            Path = parts[0];

            if (parts.Length > 1)
                Format = parts[1];
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding(Path);

            if (!string.IsNullOrEmpty(Format))
                binding.StringFormat = $"{{0:{Format}}}";

            return binding;
        }
    }
}
