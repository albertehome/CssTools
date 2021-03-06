﻿using System.Collections.Generic;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace CssTools
{
    internal class SelectorQuickInfoController : IIntellisenseController
    {
        private ITextView m_textView;
        private IList<ITextBuffer> m_subjectBuffers;
        private SelectorQuickInfoControllerProvider m_provider;

        internal SelectorQuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, SelectorQuickInfoControllerProvider provider)
        {
            m_textView = textView;
            m_subjectBuffers = subjectBuffers;
            m_provider = provider;

            m_textView.MouseHover += this.OnTextViewMouseHover;
        }

        private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
        {
            //find the mouse position by mapping down to the subject buffer
            SnapshotPoint? point = m_textView.BufferGraph
                                             .MapDownToFirstMatch(
                                                 new SnapshotPoint(m_textView.TextSnapshot, e.Position),
                                                 PointTrackingMode.Positive,
                                                 snapshot => m_subjectBuffers.Contains(snapshot.TextBuffer),
                                                 PositionAffinity.Predecessor);

            if (point == null)
                return;

            ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position,
            PointTrackingMode.Positive);

            if (m_provider.QuickInfoBroker.IsQuickInfoActive(m_textView))
                return;

            m_provider.QuickInfoBroker.TriggerQuickInfo(m_textView, triggerPoint, true);
        }

        public void Detach(ITextView textView)
        {
            if (m_textView != textView)
                return;

            m_textView.MouseHover -= this.OnTextViewMouseHover;
            m_textView = null;
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer) { }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer) { }
    }
}
