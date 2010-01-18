using ICSharpCode.TextEditor.Document;
using MarcusHolmgren.SharpDevelop.SelectWord;
using NUnit.Framework;

namespace MarcusHolmgren.SharpDevelop.SelectWordTest
{
    [TestFixture]
    public class ExtendBlockSelectionTest
    {
        [Test, ExpectedException(typeof(System.ArgumentNullException))]
        public void NullDocumentConstructorParameter()
        {
            new ExtendBlockSelection(null, null);
        }

        [Test, ExpectedException(typeof(System.ArgumentNullException))]
        public void NullSelectionConstructorParameter()
        {
            IDocument document = TestHelper.MakeDocument(string.Empty);
            new ExtendBlockSelection(document, null);
        }

        [Test]
        public void ExtendSelection()
        {
            const string TextMessage = "using(workflowRuntime)\n\n\n\tstring name = \"|Darth Vader\"";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);
            
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("Darth", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\"Darth Vader\"", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        //[Test]
        public void ExtendSelection2()
        {
            const string TextMessage = "using(workflowRuntime)\n\n\n\tstring name = \"Darth Vader|\"";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("Vader", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\"Darth Vader\"", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\tstring name = \"Darth Vader\"", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\tstring name = \"Darth Vader\"", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        //[Test]
        public void ExtendSelection3()
        {
            const string TextMessage = "Console.WriteLine|(\"Is the bug fixed?\");";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("WriteLine", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("Console.WriteLine", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void ExtendSelectionStartOfDocument()
        {
            const string TextMessage = "|Console.WriteLine(\"Is the bug fixed?\");";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("Console", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("Console.WriteLine(\"", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void ExtendSelectionEndOfDocument()
        {
            const string TextMessage = "Console.WriteLine(\"Is the bug fixed?\");'|";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.IsEmpty(selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\");'", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }


        [Test]
        public void ExtendSelectionNegativeOffSet()
        {
            const string TextMessage = "Console.WriteLine(\"Is the bug fixed?\");";
            IDocument document = TestHelper.MakeDocument(TextMessage);
            int offset = -1;
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("Console", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("Console.WriteLine(\"", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void ExtendSelectionOffsetOutsideText()
        {
            const string TextMessage = "Console.WriteLine(\"Is the bug fixed?\");";
            IDocument document = TestHelper.MakeDocument(TextMessage);
            int offset = 249;
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            TestHelper.CallAllProperties(selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\");", selectionManager.SelectedText);
        }

        [Test]
        public void ExtendBlock()
        {
            const string TextMessage = "\n\n\tstring doobii = \"Aha a |unit test\";\t\n\n\n";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);

            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("unit", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            TestHelper.CallAllProperties(selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\"Aha a unit test\"", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\tstring doobii = \"Aha a unit test\";\t\n", selectionManager.SelectedText);
        }

        [Test]
        public void Whawhoooiii()
        {
            const string TextMessage =
                "\n{\nConsole.WriteLine();\n\tConsole.WriteLine(\"Get back| to work!\");\n\tConsole.WriteLine();\n}\n";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);

            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("back", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\"Get back to work!\"", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("(\"Get back to work!\")", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\tConsole.WriteLine(\"Get back to work!\");\n", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("Console.WriteLine();\n\tConsole.WriteLine(\"Get back to work!\");\n\tConsole.WriteLine();\n", selectionManager.SelectedText);
        }

        [Test]
        public void Whawhoooiii2342()
        {
            const string TextMessage =
                "\n{\nConsole.WriteLine();\n\tConsole.WriteLine(\"Get back to w|ork!\");\n\tConsole.WriteLine();\n}\n";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);

            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("work!", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\"Get back to work!\"", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("(\"Get back to work!\")", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\tConsole.WriteLine(\"Get back to work!\");\n", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("Console.WriteLine();\n\tConsole.WriteLine(\"Get back to work!\");\n\tConsole.WriteLine();\n", selectionManager.SelectedText);
        }

        [Test]
        public void ExtendBlockToCurrentLine()
        {
            const string TextMessage = "\n\n\tstring doobii = \"Aha a |unit test\";\t\n\n\n";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);

            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("unit", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            TestHelper.CallAllProperties(selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\"Aha a unit test\"", selectionManager.SelectedText);

            selection = new ExtendBlockSelection(document, selection);
            selectionManager.SetSelection(selection.StartPosition, selection.EndPosition);
            Assert.AreEqual("\tstring doobii = \"Aha a unit test\";\t\n", selectionManager.SelectedText);

        }
    }
}