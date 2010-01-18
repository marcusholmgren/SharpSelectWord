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
using System.Collections.Generic;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace MarcusHolmgren.SharpDevelop.SelectWord
{
    public sealed class ExtendBlockSelection : ISelection
    {
        private readonly IDocument itsDocument;
        private TextLocation itsStartPosition;
        private TextLocation itsEndPosition;
        private int itsOffset;
        private int itsEndOffset;
        private string itsSelectedText;

        /// <summary>
        /// Initialize a new instance of the ExtendBlockSelection class.
        /// </summary>
        /// <param name="document">Object that implements the IDocument interface.</param>
        /// <param name="selection">The current selection.</param>
        public ExtendBlockSelection(IDocument document, ISelection selection)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (selection == null)
                throw new ArgumentNullException("selection");

            itsDocument = document;
            itsOffset = selection.Offset;
            itsEndOffset = selection.EndOffset;

            ExtendCurrentSelection();
        }


        public void ExtendCurrentSelection()
        {
            if (CurrentSelectionIsCompleteLine())
                ExtendLine();
            else
                ExtendBlock();

            AdjustEndOffset();

            StartPosition = itsDocument.OffsetToPosition(Offset);
            EndPosition = itsDocument.OffsetToPosition(EndOffset);

            itsSelectedText = itsDocument.GetText(Offset, EndOffset - Offset);
        }


        private ExtensionDirection GetExtensionDirection()
        {
            bool searching = true;
            ExtensionDirection direction = ExtensionDirection.None;

            if (Offset != 0) itsOffset++;
            itsEndOffset--;

            do
            {
                if (MoveLeft() && IsLeftBlockChar(itsDocument.GetCharAt(Offset)))
                {
                    direction = ExtensionDirection.Right;
                    searching = false;
                }
                else if (MoveRight() && IsRightBlockChar(itsDocument.GetCharAt(EndOffset)))
                {
                    if (EndOffset == (itsDocument.TextLength - 1))
                        direction = ExtensionDirection.None;
                    else
                        direction = ExtensionDirection.Left;
                    searching = false;
                }

                if (searching)
                {
                    if (itsDocument.GetCharAt(Offset) == '\n')
                        direction = ExtensionDirection.Right;

                    if (itsDocument.GetCharAt(EndOffset) == '\n')
                        direction = ExtensionDirection.Left;

                    if (direction != ExtensionDirection.None)
                        searching = false;
                }
            } while (searching);

            return direction;
        }

        private void AdjustEndOffset()
        {
            MoveRight();

            if (itsEndOffset == (itsDocument.TextLength - 1))
                itsEndOffset = itsDocument.TextLength;
        }

        private void ExtendBlock()
        {
            ExtensionDirection direction = GetExtensionDirection();

            if (direction == ExtensionDirection.Right)
                ExtendSelectionRight();


            if (direction == ExtensionDirection.Left)
                ExtendSelectionLeft();


            /*
                //TODO direction search
                if (direction == ExtensionDirection.None)
                    System.Diagnostics.Debug.WriteLine("None direction");
                */
        }

        private void ExtendLine()
        {
            while (MoveLeft())
            {
                if (itsDocument.GetCharAt(Offset) == '\n')
                {
                    itsOffset++;
                    break;
                }
            }

            while (MoveRight())
            {
                if (itsDocument.GetCharAt(EndOffset) == '\n')
                    break;
            }
        }

        private bool CurrentSelectionIsCompleteLine()
        {
            return MoveLeft() && itsDocument.GetCharAt(Offset) == '\n' && itsDocument.GetCharAt(EndOffset - 1) == '\n';
        }

        private void ExtendSelectionLeft()
        {
            char blockChar;
            char tempChar = itsDocument.GetCharAt(EndOffset);
            switch (tempChar)
            {
                case ')':
                    blockChar = '(';
                    break;
                case '}':
                    blockChar = '{';
                    break;
                case ']':
                    blockChar = '[';
                    break;
                case '>':
                    blockChar = '<';
                    break;
                default:
                    blockChar = tempChar;
                    break;
            }
            char[] validChars = new char[] {blockChar, '\n'};
            do
            {
                if (IsValidChar(itsDocument.GetCharAt(Offset), validChars))
                    break;
            } while (MoveLeft());

            if (itsDocument.GetCharAt(Offset) == '\n')
            {
                while (itsDocument.GetCharAt(EndOffset) != '\n')
                    MoveRight();

                itsOffset++;
            }
        }

        private void ExtendSelectionRight()
        {
            char blockChar;
            char tempChar = itsDocument.GetCharAt(Offset);
            switch (tempChar)
            {
                case '(':
                    blockChar = ')';
                    break;
                case '{':
                    blockChar = '}';
                    break;
                case '[':
                    blockChar = ']';
                    break;
                case '<':
                    blockChar = '>';
                    break;
                default:
                    blockChar = tempChar;
                    break;
            }
            char[] validChars = new char[] {blockChar, '\n'};
            do
            {
                if (IsValidChar(itsDocument.GetCharAt(EndOffset), validChars))
                    break;
            } while (MoveRight());

            if (itsDocument.GetCharAt(EndOffset) == '\n')
            {
                while (itsDocument.GetCharAt(Offset) != '\n')
                    MoveLeft();

                itsOffset++;
            }
        }

        /// <summary>
        /// Moves offset one position to the left if possible.
        /// </summary>
        /// <returns>True if offset decreased with one position, otherwise false.</returns>
        private bool MoveLeft()
        {
            bool result = false;
            if (itsOffset > 0)
            {
                itsOffset--;
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Validates if the character is a opening block character.
        /// </summary>
        /// <param name="c">Character to test for validity.</param>
        /// <returns>True if it is a opening block char, otherwise false.</returns>
        private static bool IsLeftBlockChar(char c)
        {
            char[] validChars = new char[] {'"', '(', '<', '{', '['};
            return IsValidChar(c, validChars);
        }

        /// <summary>
        /// Moves endoffset one position to the right if possible.
        /// </summary>
        /// <returns>True if endoffset increased with one position, otherwise false.</returns>
        private bool MoveRight()
        {
            bool result = false;
            if (itsEndOffset < itsDocument.TextLength - 1)
            {
                itsEndOffset++;
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Validates if the character is a closing block character.
        /// </summary>
        /// <param name="c">Character to test for validity.</param>
        /// <returns>True if it is a closing block char, otherwise false.</returns>
        private static bool IsRightBlockChar(char c)
        {
            char[] validChars = new char[] {'"', ')', '>', '}', ']'};
            return IsValidChar(c, validChars);
        }

        private static bool IsValidChar(char c, IEnumerable<char> validChars)
        {
            bool result = false;
            foreach (char validChar in validChars)
                if (c == validChar) result = true;
            return result;
        }

        private enum ExtensionDirection
        {
            None = 0,
            Left = 1,
            Right = 2
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
            get { return string.IsNullOrEmpty(SelectedText) ? true : false; }
        }

        public string SelectedText
        {
            get { return itsSelectedText; }
        }


        public bool ContainsPosition(TextLocation position)
        {
            System.Diagnostics.Debug.WriteLine("ContainsPosition");
            return false;
        }
    }
}
