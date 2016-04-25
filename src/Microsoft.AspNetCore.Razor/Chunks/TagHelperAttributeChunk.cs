// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Microsoft.AspNetCore.Razor.Chunks
{
    public class TagHelperAttributeChunk : Chunk
    {
        public string Name { get; set; }

        public Chunk Value { get; set; }

        public HtmlAttributeStructure Structure { get; set; }
    }
}
