using ICSharpCode.TextEditor.Document;
using MarcusHolmgren.SharpDevelop.SelectWord;
using NUnit.Framework;

namespace MarcusHolmgren.SharpDevelop.SelectWordTest
{
    [TestFixture]
    public class SingleWordSelectionTest
    {
        [Test, ExpectedException(typeof(System.ArgumentNullException))]
        public void NullDocumentConstructorParameter()
        {
            new SingleWordSelection(null, 0);
        }


        [Test]
        public void CursorAtStartOfWord()
        {
            const string TextMessage = "using(WorkflowRuntime |workflowRuntime = new WorkflowRuntime())";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);
            
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);

            Assert.IsTrue(selection.ContainsOffset(18));
            Assert.AreEqual("workflowRuntime", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void CursorAtEndOfWord()
        {
            const string TextMessage = "using(WorkflowRuntime| workflowRuntime = new WorkflowRuntime())";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);
            
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);

            Assert.AreEqual("WorkflowRuntime", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void CursorAtEndOfWord23423()
        {
            const string TextMessage = "\"|Hello World\"";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);

            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);

            Assert.AreEqual("Hello", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void CursorBeforeOpeningParentisis()
        {
            const string TextMessage = "using(WorkflowRuntime workflowRuntime = new WorkflowRuntime|())";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);
            
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);

            Assert.AreEqual("WorkflowRuntime", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void CursorBeforeQuotationMark()
        {
            const string TextMessage = "\"hello there|\"";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);

            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);

            Assert.AreEqual("there", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void CursorPositionBeforeText()
        {
            const string TextMessage = "\rusing(WorkflowRuntime workflowRuntime = new WorkflowRuntime())";
            IDocument document = TestHelper.MakeDocument(TextMessage);
            int offset = -1;
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);

            Assert.AreEqual("using", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void NoSelectionOffsetEndOfDocument()
        {
            const string TextMessage = "using(WorkflowRuntime workflowRuntime = new WorkflowRuntime())\n\n\n";
            IDocument document = TestHelper.MakeDocument(TextMessage);
            int offset = 240;
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);

            Assert.IsTrue(selection.IsEmpty);
            Assert.IsEmpty(selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void SelectNerestWord()
        {
            const string TextMessage = "\tIs |the bug fixed?";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);
            
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);
            Assert.AreEqual("the", selectionManager.SelectedText);

            offset--;
            selection = new SingleWordSelection(document, offset);
            selectionManager.SetSelection(selection);
            Assert.AreEqual("Is", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void WordCloseToDot()
        {
            const string TextMessage = "char answer = Console|.ReadKey().KeyChar;";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);
            
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);
            Assert.AreEqual("Console", selectionManager.SelectedText);

            offset += 1;
            selection = new SingleWordSelection(document, offset);
            selectionManager.SetSelection(selection);
            Assert.AreEqual("ReadKey", selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void CharacterSelectStart()
        {
            const string TextMessage = "char[] validChars = new char[] {'\"', '|(', '<', '{', '['};";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);

            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);
            Assert.AreEqual("(", selectionManager.SelectedText);
        }

        [Test]
        public void CharacterSelectEnd()
        {
            const string TextMessage = "char[] validChars = new char[] {'\"', '(|', '<', '{', '['};";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);

            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);
            Assert.AreEqual("(", selectionManager.SelectedText);
        }

        [Test]
        public void CharacterSelectEndOfDocument()
        {
            const string TextMessage = "char[] validChars = new char[] {'\"', '(', '<', '{', '['};\n'|";
            int offset;
            IDocument document = TestHelper.MakeDocument(TextMessage, out offset);

            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);
            Assert.IsEmpty(selectionManager.SelectedText);
        }

        [Test]
        public void EmptyDocument()
        {
            const string TextMessage = "";
            IDocument document = TestHelper.MakeDocument(TextMessage);
            int offset = 21;
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);
            Assert.IsEmpty(selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);

            offset = -2;
            selection = new SingleWordSelection(document, offset);
            selectionManager.SetSelection(selection);
            Assert.IsEmpty(selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        [Test]
        public void NullDocument()
        {
            const string TextMessage = null;
            IDocument document = TestHelper.MakeDocument(TextMessage);
            int offset = 21;
            ISelection selection = new SingleWordSelection(document, offset);
            SelectionManager selectionManager = new SelectionManager(document);
            selectionManager.SetSelection(selection);
            Assert.IsEmpty(selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);

            offset = -2;
            selection = new SingleWordSelection(document, offset);
            selectionManager.SetSelection(selection);
            Assert.IsEmpty(selectionManager.SelectedText);
            TestHelper.CallAllProperties(selection);
        }

        

 


        

    }
}