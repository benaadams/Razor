// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Razor.Parser.SyntaxTree;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Microsoft.AspNetCore.Razor.Parser.TagHelpers
{
    public class TagHelperAttributeNode
    {
        public TagHelperAttributeNode(string name, SyntaxTreeNode value)
            : this(name, value, HtmlAttributeStructure.DoubleQuotedValue)
        {
        }

        public TagHelperAttributeNode(string name, SyntaxTreeNode value, HtmlAttributeStructure structure)
        {
            Name = name;
            Value = value;
            Structure = structure;
        }

        public string Name { get; }

        public SyntaxTreeNode Value { get; }

        public HtmlAttributeStructure Structure { get; }
    }
}
