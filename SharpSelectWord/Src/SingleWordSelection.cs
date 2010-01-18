// SharpSelectWord is a addin to SharpDevelop that can select text.
// Copyright (C) 2007  Marcus Holmgren
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA


using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace MarcusHolmgren.SharpDevelop.SelectWord
{
    /// <summary>
    /// Purpose of the SingleWordSelection class is to select the first word positioned closest to the current cursor position.
    /// </summary>
    public sealed class SingleWordSelection : ISelection
    {
        private readonly IDocument itsDocument;
        private TextLocation itsStartPosition;
        private TextLocation itsEndPosition;
        private int itsOffset;
        private int itsEndOffset;
        private string itsSelectedText;

        /// <summary>
        /// Initialize a new instance of the SingleWordSelection class.
        /// </summary>
        /// <param name="document">IDocument object.</param>
        /// <param name="offset">Current position for the cursor.</param>
        /// <exception cref="ArgumentNullException">If parameter document is not provided an ArgumentNullException is throw.</exception>
        public SingleWordSelection(IDocument document, int offset)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            itsDocument = document;

            if (offset < 0) itsOffset = 0;
            else if (offset >= document.TextLength) itsOffset = document.TextLength - 1;
            else itsOffset = offset;

            itsEndOffset = itsOffset;

            FindSelection();
        }


        public bool ContainsOffset(int offset)
        {
            return itsOffset != offset;
        }

        public TextLocation StartPosition
        {
            get { return itsStartPosition; }
            set { itsStartPosition = value; }
        }

        public TextLocation EndPosition
        {
            get { return itsEndPosition; }
            set { itsEndPosition = value; }
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
            get { return  string.IsNullOrEmpty(itsSelectedText) ? true : false; }
        }

        public string SelectedText
        {
            get{ return itsSelectedText; }
        }


        public bool ContainsPosition(TextLocation position)
        {
            System.Diagnostics.Debug.WriteLine("ContainsPosition");
            //if (itsStartPosition.Line == position.Line)

            return false;
        }

        /// <summary>
        /// Navigates the current offset and locates the closest word.
        /// </summary>
        private void FindSelection()
        {
            if (itsDocument.TextLength > 0)
            {
                if (NotPositionedAtSingleCharacter())
                {
                    SetStartingLocation();
                    SetStartOffset();
                    SetEndOffset();
                }

                itsSelectedText = itsDocument.GetText(Offset, EndOffset - Offset);
            }
        }

        /// <summary>
        /// Check if the cursor is positioned inside a single character quote.
        /// </summary>
        /// <returns>True if cursor is positioned at a single character, otherwise false.</returns>
        private bool NotPositionedAtSingleCharacter()
        {
            const char SingleQuote = '\'';
            bool isSingleCharacter = false;
            if (itsDocument.GetCharAt(Offset) == SingleQuote && (Offset - 2) > 0)
            {
                if (itsDocument.GetCharAt(Offset - 2) == SingleQuote)
                {
                    itsOffset--;
                    isSingleCharacter = true;
                }
            }
            else if ((Offset - 1) > 0 && itsDocument.GetCharAt(Offset - 1) == SingleQuote)
            {
                if ((EndOffset + 1) < itsDocument.TextLength && itsDocument.GetCharAt(EndOffset + 1) == SingleQuote)
                {
                    itsEndOffset++;
                    isSingleCharacter = true;
                }
            }

            if (isSingleCharacter)
            {
                StartPosition = itsDocument.OffsetToPosition(Offset);
                EndPosition = itsDocument.OffsetToPosition(EndOffset);
            }

            return !isSingleCharacter;
        }

        /// <summary>
        /// Reposition starting offset if the cursor is positioned at the end of a word.
        /// </summary>
        private void SetStartingLocation()
        {
            char firstChar = itsDocument.GetCharAt(itsOffset);
            if (firstChar == ' ' || firstChar == '.' || firstChar == '(')
            {
                char secondChar = itsDocument.GetCharAt(itsOffset - 1);
                if (itsOffset > 0 && secondChar != ' ' || secondChar != '.' || secondChar != '(')
                    itsOffset--;
            }
        }

        /// <summary>
        /// Navigate the document from the current cursor position and locates the ending position of the word.
        /// </summary>
        private void SetEndOffset()
        {
            int temp = itsOffset;

            bool ok;
            do
            {
                char c = itsDocument.GetCharAt(temp);
                ok = ValidCharacter(c);

                if (ok && temp < itsDocument.TextLength - 1) 
                    temp++;
                else ok = false;
            }
            while (ok);

            itsEndOffset = temp;
            EndPosition = itsDocument.OffsetToPosition(temp);
        }

        /// <summary>
        /// Navigate the document from the current cursor position and locates the starting position of the word.
        /// </summary>
        private void SetStartOffset()
        {
            int temp = itsOffset;

            if (itsDocument.GetCharAt(temp) == '\"') temp--;

            bool ok;
            do
            {
                char c = itsDocument.GetCharAt(temp);
                ok = ValidCharacter(c);

                if (ok && temp > 0) temp--;
                else
                {
                    if (!ok && temp > -1  && temp < itsDocument.TextLength - 1) 
                        temp++;
                    ok = false;
                }
            }
            while (ok);

            itsOffset = temp;
            StartPosition = itsDocument.OffsetToPosition(temp);
        }

        /// <summary>
        /// Validates a character.
        /// </summary>
        /// <param name="input">Character to validate.</param>
        /// <returns>True if the input character is valid, otherwise false.</returns>
        private static bool ValidCharacter(char input)
        {
            bool result = true;
            char[] invalidChars = new char[] { ' ', '\n', '\r', '\t', '(', ')', '{', '}', '[', ']', '<', '>', '"', '.', ',', ';', ':' };

            foreach (char c in invalidChars)
            {
                if (input == c)
                    result = false;
            }
            return result;
        }
    }
}
