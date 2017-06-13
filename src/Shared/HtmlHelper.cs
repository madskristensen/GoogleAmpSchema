using Microsoft.Html.Core.Tree.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAmpSchema
{
    public static class HtmlHelper
    {
        public static DocMode GetDocMode(this ElementNode element)
        {
            var parent = element;

            while (parent != null)
            {
                if (parent.Name == "html")
                {
                    if (parent.GetAttribute("amp") != null || parent.GetAttribute("⚡") != null)
                    {
                        return DocMode.AMP;
                    }

                    return DocMode.HTML;
                }

                parent = parent.Parent;
            }

            return DocMode.Unknown;
        }
    }

    public enum DocMode
    {
        AMP,
        HTML,
        Unknown
    }
}
