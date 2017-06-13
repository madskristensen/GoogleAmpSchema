using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.Validation.Def;
using Microsoft.Html.Editor.Validation.Errors;
using Microsoft.Html.Editor.Validation.Validators;
using Microsoft.VisualStudio.Utilities;

namespace GoogleAmpSchema
{
    [Export(typeof(IHtmlElementValidatorProvider))]
    [ContentType("htmlx")]
    class StyleValidatorProvider : BaseHtmlElementValidatorProvider<StyleValidator>
    { }

    class StyleValidator : BaseValidator
    {
        public override IList<IHtmlValidationError> ValidateElement(ElementNode element)
        {
            var results = new ValidationErrorCollection();

            if (string.IsNullOrEmpty(element?.Name) || !element.Name.Equals("style", StringComparison.OrdinalIgnoreCase))
                return results;

            if (element.GetDocMode() != DocMode.AMP)
                return results;

            var attr = element.GetAttribute("amp-boilerplate") ?? element.GetAttribute("amp-custom");

            if (attr == null)
            {
                results.Add(element, "<style> elements must have an \"amp-boilerplate\" or \"amp-custom\" attribute.", HtmlValidationErrorLocation.ElementName);
            }

            return results;
        }
    }
}
