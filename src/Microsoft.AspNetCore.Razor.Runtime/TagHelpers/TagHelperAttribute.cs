// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Internal;

namespace Microsoft.AspNetCore.Razor.TagHelpers
{
    /// <summary>
    /// An HTML tag helper attribute.
    /// </summary>
    public class TagHelperAttribute : IHtmlContentContainer
    {
        /// <summary>
        /// Instantiates a new instance of <see cref="TagHelperAttribute"/> with the specified <paramref name="name"/>.
        /// <see cref="Structure"/> is set to <see cref="HtmlAttributeStructure.Minimized"/> and <see cref="Value"/> to
        /// <c>null</c>.
        /// </summary>
        /// <param name="name">The <see cref="Name"/> of the attribute.</param>
        public TagHelperAttribute(string name)
            : this(name, value: null, structure: HtmlAttributeStructure.Minimized)
        {
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="TagHelperAttribute"/> with the specified <paramref name="name"/>
        /// and <paramref name="value"/>. <see cref="Structure"/> is set to <see cref="HtmlAttributeStructure.DoubleQuotedValue"/>.
        /// </summary>
        /// <param name="name">The <see cref="Name"/> of the attribute.</param>
        /// <param name="value">The <see cref="Value"/> of the attribute.</param>
        public TagHelperAttribute(string name, object value)
            : this(name, value, structure: HtmlAttributeStructure.DoubleQuotedValue)
        {
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="TagHelperAttribute"/> with the specified <paramref name="name"/>,
        /// <paramref name="value"/> and <paramref name="structure"/>.
        /// </summary>
        /// <param name="name">The <see cref="Name"/> of the new instance.</param>
        /// <param name="value">The <see cref="Value"/> of the new instance.</param>
        /// <param name="structure">The <see cref="Structure"/> of the new instance.</param>
        /// <remarks>If <paramref name="structure"/> is <see cref="HtmlAttributeStructure.Minimized"/>,
        /// <paramref name="value"/> is ignored when this instance is rendered.</remarks>
        public TagHelperAttribute(string name, object value, HtmlAttributeStructure structure)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Value = value;
            Structure = structure;
        }

        /// <summary>
        /// Gets the name of the attribute.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the attribute.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// TODO
        /// </summary>
        public HtmlAttributeStructure Structure { get; }

        /// <inheritdoc />
        /// <remarks><see cref="Name"/> is compared case-insensitively.</remarks>
        public bool Equals(TagHelperAttribute other)
        {
            return
                other != null &&
                string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                Structure == other.Structure &&
                (Structure == HtmlAttributeStructure.Minimized || Equals(Value, other.Value));
        }

        /// <inheritdoc />
        public void WriteTo(TextWriter writer, HtmlEncoder encoder) => WriteTo(Name, Value, Structure, writer, encoder);

        /// <inheritdoc />
        public void CopyTo(IHtmlContentBuilder destination) => CopyTo(Name, Value, Structure, destination);

        /// <inheritdoc />
        public void MoveTo(IHtmlContentBuilder destination) => CopyTo(destination);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="structure"></param>
        /// <param name="destination"></param>
        public static void CopyTo(
            string name,
            object value,
            HtmlAttributeStructure structure,
            IHtmlContentBuilder destination)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            destination.AppendHtml(name);

            if (structure == HtmlAttributeStructure.Minimized)
            {
                return;
            }

            var valuePrefix = GetAttributeValuePrefix(structure);
            if (valuePrefix != null)
            {
                destination.AppendHtml(valuePrefix);
            }

            var htmlContent = value as IHtmlContent;
            if (htmlContent != null)
            {
                destination.AppendHtml(htmlContent);
            }
            else if (value != null)
            {
                destination.Append(value.ToString());
            }

            var valueSuffix = GetAttributeValueSuffix(structure);
            if (valueSuffix != null)
            {
                destination.AppendHtml(valueSuffix);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="structure"></param>
        /// <param name="writer"></param>
        /// <param name="encoder"></param>
        public static void WriteTo(
            string name,
            object value,
            HtmlAttributeStructure structure,
            TextWriter writer,
            HtmlEncoder encoder)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            writer.Write(name);

            if (structure == HtmlAttributeStructure.Minimized)
            {
                return;
            }

            var valuePrefix = GetAttributeValuePrefix(structure);
            if (valuePrefix != null)
            {
                writer.Write(valuePrefix);
            }

            var htmlContent = value as IHtmlContent;
            if (htmlContent != null)
            {
                htmlContent.WriteTo(writer, encoder);
            }
            else if (value != null)
            {
                encoder.Encode(writer, value.ToString());
            }

            var valueSuffix = GetAttributeValueSuffix(structure);
            if (valueSuffix != null)
            {
                writer.Write(valueSuffix);
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as TagHelperAttribute;

            return Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCodeCombiner = HashCodeCombiner.Start();
            hashCodeCombiner.Add(Name, StringComparer.Ordinal);
            hashCodeCombiner.Add(Value);
            hashCodeCombiner.Add(Structure);

            return hashCodeCombiner.CombinedHash;
        }

        private static string GetAttributeValuePrefix(HtmlAttributeStructure structure)
        {
            switch (structure)
            {
                case HtmlAttributeStructure.DoubleQuotedValue:
                    return "=\"";
                case HtmlAttributeStructure.SingleQuotedValue:
                    return "='";
                case HtmlAttributeStructure.UnquotedValue:
                    return "=";
            }

            return null;
        }

        private static string GetAttributeValueSuffix(HtmlAttributeStructure structure)
        {
            switch (structure)
            {
                case HtmlAttributeStructure.DoubleQuotedValue:
                    return "\"";
                case HtmlAttributeStructure.SingleQuotedValue:
                    return "'";
            }

            return null;
        }
    }
}