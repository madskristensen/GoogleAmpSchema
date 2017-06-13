﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;

namespace GoogleAmpSchema
{
    [HtmlCompletionProvider(CompletionTypes.Children, "*")]
    [ContentType("htmlx")]
    class ListCompletion : CompletionBase
    {
        public override string CompletionType
        {
            get { return CompletionTypes.Children; }
        }

        public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            return AddEntries(context, HtmlCache.Elements.Where(elem => elem.Type != "existing"));
        }
    }
}
