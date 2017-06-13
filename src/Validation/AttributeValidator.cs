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
    class AttributeValidatorProvider : BaseHtmlElementValidatorProvider<AttributeValidator>
    { }

    class AttributeValidator : BaseValidator
    {
        static HtmlElement _global = HtmlCache.Elements.Single(e => e.Name == "*");

        public override IList<IHtmlValidationError> ValidateElement(ElementNode element)
        {
            var results = new ValidationErrorCollection();
            var attributes = _global.Attributes.ToList();

            var match = HtmlCache.Elements.SingleOrDefault(e => e.Name.Equals(element.Name, StringComparison.OrdinalIgnoreCase));

            if (match != null && match.Attributes != null)
                attributes.AddRange(match.Attributes);

            var html = HtmlCache.Elements.FirstOrDefault(e => e.Name.Equals(element.Name, StringComparison.OrdinalIgnoreCase));

            if (!IsRequiredAttributesValid(html, element, out string error))
            {
                results.AddAttributeError(element, error, HtmlValidationErrorLocation.ElementName, 0);
            }

            foreach (HtmlAttribute htmlAttr in attributes)
            {
                var attrNode = element.GetAttribute(htmlAttr.Name);

                if (attrNode == null)
                    continue;

                int index = element.GetAttributeIndex(attrNode.Name);

                if (!IsTypeValid(attrNode.Value, htmlAttr, out error))
                {
                    results.AddAttributeError(element, error, HtmlValidationErrorLocation.AttributeValue, index);
                }

                if (!IsRequireValid(htmlAttr, element, out error))
                {
                    results.AddAttributeError(element, error, HtmlValidationErrorLocation.AttributeName, index);
                }
            }

            return results;
        }

        private bool IsRequiredAttributesValid(HtmlElement html, ElementNode element, out string error)
        {
            error = string.Empty;

            if (html == null)
                return true;

            foreach (var attribute in html.Attributes)
            {
                if (attribute.Required && element.GetAttribute(attribute.Name) == null)
                {
                    error = $"\"{attribute.Name}\" is a required attribute on \"{element.Name}\" elements.";
                    return false;
                }
            }

            return true;
        }

        static bool IsRequireValid(HtmlAttribute attribute, ElementNode element, out string error)
        {
            error = string.Empty;

            if (!string.IsNullOrEmpty(attribute.Require) && element.GetAttribute(attribute.Require) == null)
            {
                error = $"When using \"{attribute.Name}\" you must also specify \"{attribute.Require}\".";
                return false;
            }

            return true;
        }

        static bool IsTypeValid(string value, HtmlAttribute attribute, out string error)
        {
            error = string.Empty;

            if (attribute.Type == "boolean")
            {
                error = $"The value \"{value}\" must be either \"true\" or \"false\".";
                return value == "true" || value == "false";
            }

            if (attribute.Type == "number")
            {
                error = $"The value \"{value}\" is not a valid number.";
                return double.TryParse(value, out double number);
            }

            if (attribute.Type == "enum")
            {
                error = $"The value \"{value}\" doesn't match any of the allowed enum values.";
                return attribute.Values.Contains(value);
            }

            return true;
        }
    }
}
