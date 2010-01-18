using System.Drawing;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace MarcusHolmgren.SharpDevelop.SelectWordTest
{
    class TestHelper
    {
        internal static void CallAllProperties(ISelection selection)
        {
            selection.ContainsOffset(0);
            selection.ContainsPosition(TextLocation.Empty);
            int offset = selection.EndOffset;
            TextLocation position = selection.EndPosition;
            bool empty = selection.IsEmpty;
            bool rectangularSelection = selection.IsRectangularSelection;
            int length = selection.Length;
            int offset2 = selection.Offset;
            string text = selection.SelectedText;
            TextLocation startPosition = selection.StartPosition;
        }

        internal static IDocument MakeDocument(string message)
        {
            IDocument document = new DocumentFactory().CreateDocument();
            document.TextContent = message;

            return document;
        }

        internal static IDocument MakeDocument(string message, out int offset)
        {
            IDocument document = new DocumentFactory().CreateDocument();
            offset = message.IndexOf('|');
            document.TextContent = message.Remove(offset, 1);

            return document;
        }
    }
}