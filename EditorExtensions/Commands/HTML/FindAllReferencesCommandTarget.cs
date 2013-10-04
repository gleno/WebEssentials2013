﻿using Microsoft.CSS.Core;
using Microsoft.Html.Core;
using Microsoft.Html.Editor;
using Microsoft.Less.Core;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.Web.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;

namespace MadsKristensen.EditorExtensions
{
    internal class HtmlFindAllReferences : CommandTargetBase
    {
        private HtmlEditorTree _tree;
        private string _path, _className;

        public HtmlFindAllReferences(IVsTextView adapter, IWpfTextView textView)
            : base(adapter, textView, typeof(Microsoft.VisualStudio.VSConstants.VSStd97CmdID).GUID, (uint)Microsoft.VisualStudio.VSConstants.VSStd97CmdID.FindReferences)
        {
            _tree = HtmlEditorDocument.FromTextView(textView).HtmlEditorTree;
        }

        protected override bool Execute(uint commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (!string.IsNullOrEmpty(_className))
            {
                FileHelpers.SearchFiles("." + _className, "*.css;*.less;*.scss;*.sass");
            }

            return true;
        }
        
        private bool TryGetClassName(out string className)
        {
            int position = TextView.Caret.Position.BufferPosition.Position;
            className = null;

            ElementNode element = null;
            AttributeNode attr = null;

            _tree.GetPositionElement(position, out element, out attr);

            if (attr == null || attr.Name != "class")
                return false;

            string value = attr.Value;
            int beginning = position - attr.ValueRangeUnquoted.Start;
            int start = attr.Value.LastIndexOf(' ', beginning) + 1;
            int length = attr.Value.IndexOf(' ', start) - start;

            if (length < 0)
                length = attr.ValueRangeUnquoted.Length - start;

            className = attr.Value.Substring(start, length);

            return true;
        }

        protected override bool IsEnabled()
        {
            return TryGetClassName(out _className);
        }
    }
}