using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        string filePath = "";
        bool needsSaving = false;
        
        public Form1()
        {
            InitializeComponent();
            saveFileDialog1.FileOk += (s, e) => WriteToFile(saveFileDialog1.FileName);
            selectAllToolStripMenuItem.Click += (s, e) => richTextBox1.SelectAll();
        }

        private void WriteToFile(string fileName)
        {
            if (Path.GetExtension(fileName) == ".rtf")
            {
                richTextBox1.SaveFile(fileName);
            }
            var output = new StreamWriter(File.OpenWrite(fileName));
            output.Write(richTextBox1.Text);
            output.Close();
            filePath = fileName;
            needsSaving = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                richTextBox1.Clear();
                if (Path.GetExtension(openFileDialog1.FileName) == ".rtf")
                {
                    richTextBox1.LoadFile(filePath);
                }
                else
                {
                    var input = File.OpenText(openFileDialog1.FileName);
                    richTextBox1.Text = input.ReadToEnd();
                    input.Close();
                }
                richTextBox1.Enabled = true;
                needsSaving = false;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (needsSaving)
            {
                saveFileDialog1.FileName = filePath;
                saveFileDialog1.ShowDialog();
            }
            else
                WriteToFile(filePath);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newFileDialog = new NewFileForm();
            if (newFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                richTextBox1.Enabled = true;
                filePath = newFileDialog.FileName;
                needsSaving = true;
                richTextBox1.Clear();
            }

        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
            {
                richTextBox1.Undo();
            }
        }

        private void redoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanRedo)
            {
                richTextBox1.Redo();
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionFont = fontDialog1.Font;
            }
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionColor = colorDialog1.Color;
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pDoc = new PrintDocument();
            pDoc.DocumentName = filePath;
            pDoc.PrintPage += PrinterHandler;
            printDialog1.Document = pDoc;

            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDialog1.Document.Print();
            }
        }

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pDoc = new PrintDocument();
            pDoc.DocumentName = filePath;
            pDoc.PrintPage += PrinterHandler;
            printPreviewDialog1.Document = pDoc;
            printPreviewDialog1.ShowDialog();
        }

        private void PrinterHandler(object sender, PrintPageEventArgs e)
        {
            int count = 0;
            Font printFont = fontDialog1.Font;
            Brush printBrush = new SolidBrush(colorDialog1.Color);
            foreach (var line in richTextBox1.Lines)
            {
                float yPos = e.MarginBounds.Top + (count * printFont.GetHeight(e.Graphics));
                e.Graphics.DrawString(line, printFont, printBrush, e.MarginBounds.Left, yPos, new StringFormat());

                count++;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
