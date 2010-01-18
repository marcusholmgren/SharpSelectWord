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


using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace MarcusHolmgren.SharpDevelop.SelectWord
{
    public class DeselectWordCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            ITextEditorControlProvider tecp = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorControlProvider;
            if (tecp == null) return;
            
            TextArea textArea = tecp.TextEditorControl.ActiveTextAreaControl.TextArea;

            ISelection selector = MakeWordSelector(textArea);
            textArea.SelectionManager.SetSelection(selector);

            textArea.Refresh();
        }

        private static ISelection MakeWordSelector(TextArea textArea)
        {
            IDocument document = textArea.Document;

            if (!textArea.SelectionManager.HasSomethingSelected)
            {
                int currentOffset = textArea.Caret.Offset;
                return new SingleWordSelection(document, currentOffset);
            }
            else
            {
                SelectionManager selectionManager = textArea.SelectionManager;
                ISelection firstSelection = selectionManager.SelectionCollection[0];

                return new ExtendBlockSelection(document, firstSelection);
            }
        }
    }
}

