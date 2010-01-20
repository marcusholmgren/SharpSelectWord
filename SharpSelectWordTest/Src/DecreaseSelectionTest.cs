/*
 * Created by SharpDevelop.
 * User: Marcus
 * Date: 2010-01-20
 * Time: 20:33
 * 
 */
using System;
using ICSharpCode.TextEditor.Document;
using MarcusHolmgren.SharpDevelop.SelectWord;
using NUnit.Framework;

namespace MarcusHolmgren.SharpDevelop.SelectWordTest
{
	[TestFixture]
	public class DecreaseSelectionTest
	{
		[Test]
		public void Decrease_text_select_with_qoutes_to_only_word()
		{
			// Arrange
			const string textMessage = "\"|Hello World!\"";
			int offset;
			IDocument document = TestHelper.MakeDocument(textMessage, out offset);
			ISelection selection = new SingleWordSelection(document, offset);
			SelectionManager selectionManager = new SelectionManager(document);
			selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);

			selection = new ExtendBlockSelection(document, selection);
			selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
			Assert.AreEqual("\"Hello World!\"", selectionManager.SelectedText);
						
			// Act
			selection = new DecreaseSelection(document, selection);
			selectionManager.SetSelection(selection);
			
			// Arrange
			Assert.AreEqual("Hello World!", selectionManager.SelectedText);
		}
	}
}
