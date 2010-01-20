/*
 * Created by SharpDevelop.
 * User: Marcus
 * Date: 2010-01-20
 * Time: 20:38
 * 
 */
using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace MarcusHolmgren.SharpDevelop.SelectWord
{
	/// <summary>
	/// Description of DecreaseSelection.
	/// </summary>
	public sealed class DecreaseSelection : ISelection
	{
		private readonly IDocument itsDocument;
        private int itsOffset;
        private int itsEndOffset;
        private string itsSelectedText;
		
		public DecreaseSelection(IDocument document, ISelection selection)
		{
			if (document == null) throw new ArgumentNullException("document");
			if (selection == null) throw new ArgumentNullException("selection");
			this.itsDocument = document;
			itsOffset = selection.Offset + 1;
			itsEndOffset = selection.EndOffset - 1;;
			
			
			itsSelectedText = document.TextContent.Substring(Offset, EndOffset-Offset);
		}
		
		
		public TextLocation StartPosition
		{
			get { return itsDocument.OffsetToPosition(Offset); }
			set { itsOffset = itsDocument.PositionToOffset(value); }
		}
		
		public TextLocation EndPosition
        {
			get { return itsDocument.OffsetToPosition(Offset); }
			set { itsEndOffset = itsDocument.PositionToOffset(value); }
        }
		
		public int Offset
        {
			get { return itsOffset; }
        }

        public int EndOffset
        {
        	get { return itsEndOffset; }
        }
		
		public int Length
        {
            get { return IsEmpty ? 0 : SelectedText.Length; }
        }

        public bool IsRectangularSelection
        {
            get { return false; }
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(SelectedText) ? true : false; }
        }
		
		public string SelectedText
		{
			get { return itsSelectedText; }
		}
		
		public bool ContainsOffset(int offset)
        {
            return itsOffset != offset;
        }
		
		public bool ContainsPosition(TextLocation position)
        {
            System.Diagnostics.Debug.WriteLine("ContainsPosition");
            return false;
        }
		
	}
}
