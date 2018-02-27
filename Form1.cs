//#define UCODE
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace editor
{
    public struct Record
    {
        public LC LC;
        public int key;
    }
    public enum Mode
    {
        None = 0,
        Edit = 1,
        Minibuf = 2
    };

    public partial class Form1 : Form
    {
        private int Line;

        private int Col;

        public List<string> sentences = new List<string>();

        public Stack<Record> UndoStack = new Stack<Record>();

        private Text ThisFile = new Text();

        private Minibuf minibuf = new Minibuf();

        public OpenFileDialog openFileDialog1 = new OpenFileDialog();
        public SaveFileDialog saveFileDialog1 = new SaveFileDialog();

        public Mode Mode = Mode.None;

        public bool isAlt = false;
        public bool isCtrl = false;
        public bool isShift = false;

        public Form1()
        {
            InitializeComponent();
            isSearchMode = false;
            Line = 0;
            Col = 0;
        }

        public void ShowText()
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LC lc = new LC
            {
                L = Line,
                C = Col
            };
            Record record = new Record
            {
                LC = lc,
                key = 11
            };
            UndoStack.Push(record);
            Mode = Mode.None;
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = Mode.None;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e) //open
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                sentences.Clear();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    sentences.Add(line);
                }
                ThisFile.SetText(sentences);
                sr.Close();
            }
            Mode = Mode.None;

            Line = 0; Col = 0;
            pictureBox1.Refresh();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);

                for (int i = 0; i < sentences.Count; i++)
                {
                    sw.WriteLine(sentences[i]);
                }
                sw.Close();
            }
            Mode = Mode.None;
            pictureBox1.Refresh();
        }

        private void redoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Mode = Mode.None;
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = Mode.None;
            Search search_window = new Search(this);
            search_window.Activate();
            search_window.Show();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C interpreter.exe test.uco > result.txt";
            process.StartInfo = startInfo;
            process.Start();
            Label:
            StreamReader sr;
            try{
                sr = new StreamReader("result.txt");
            }
            catch (FileNotFoundException)
            {
                goto Label;
            }
            catch (IOException)
            {
                goto Label; 
            }
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                sentences.Add(line);
            }
            ThisFile.SetText(sentences);
            sr.Close();
            Line = 0; Col = 0;
            pictureBox1.Refresh();
            Mode = Mode.None;
        }
        
        private void Undo()
        {
            if (UndoStack.Count == 0) return;
            Record tempRC = UndoStack.Pop();
            ThisFile.Erase(tempRC.LC.L, tempRC.LC.C);
            Line = tempRC.LC.L;
            Col = tempRC.LC.C - 1;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ForeColor = Color.Black;
            Mode = Mode.None;
        }
        public bool isUCO = true;

        private void interpreterToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            this.ForeColor = Color.Black;
            Mode = Mode.None;
        }

        private void interpreterToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            this.ForeColor = Color.White;
        }

        private void interpreterToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            this.ForeColor = Color.Black;
            Mode = Mode.None;
        }
        private void cursorShow(Graphics graphics)
        {
            Point point = new Point(Col * 10, (Line+1-startline) * 20); 
            Size sz = new Size(10, 2);
            Pen drawPen = new Pen(Color.FromArgb(240, 240, 240));
            SolidBrush sb = new SolidBrush(Color.FromArgb(240, 240, 240));
            Rectangle rectangle = new Rectangle(point, sz);
            graphics.DrawRectangle(drawPen, rectangle);
            graphics.FillRectangle(sb, rectangle);
        }
        public int startline;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font drawFont = new Font("monaco", 12);

            string drawString = "";
            sentences = ThisFile.GetText();
            int L = 0, C = 0;
            int count = 0;
            startline = (Line > 31) ? Line - 31 : 0;
            for (int i = startline; i < startline+(((sentences.Count-startline)<33)? (sentences.Count - startline):33); i++)
            {
                bool searchpoint = false;
                bool isInstruction = false;
                for (int j = 0; j < sentences[i].Length; j++)
                {
                    drawString = sentences[i][j].ToString();
                    PointF drawPoint = new PointF(C * 10, L * 20);
                    if (j == 11) isInstruction = true;
                    if (sentences[i][j] == ' ') isInstruction = searchpoint = false;
                    if (count == searchLength ) searchpoint = false;
                    SolidBrush drawBrush;

                    if (isUCO&&j < 10)
                    {
                        drawBrush = new SolidBrush(Color.FromArgb(221, 215, 95));
                    }
                    else if (isUCO && isInstruction)
                    {
                        drawBrush = new SolidBrush(Color.FromArgb(175, 255, 85));
                    }
                    else
                    {
                        drawBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
                    }

                    if (isSearchMode)
                    {
                        LC temp = new LC(L, C);

                        if (StartingPoint.Any(x => x.L == temp.L && x.C == temp.C))
                        {
                            count = 0;
                            searchpoint = true;
                        }
                        if(searchpoint )
                        {
                            count++;
                            Point point = new Point((int)drawPoint.X+2,(int)drawPoint.Y+1);
                            Size sz = new Size(10, 18);
                            Pen drawPen = new Pen(Color.FromArgb(215, 95, 0));
                            SolidBrush sb = new SolidBrush(Color.FromArgb(215, 95, 0));
                            Rectangle rectangle = new Rectangle(point, sz);
                            graphics.DrawRectangle(drawPen, rectangle);
                            graphics.FillRectangle(sb, rectangle);
                        }
                    }
                    
                    C++;
                    
                    graphics.DrawString(drawString, drawFont, drawBrush, drawPoint);
                }
                L++;
                C = 0;
                //drawString += (sentences[i] + "\n");
            }
            cursorShow(e.Graphics);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Mode = Mode.Edit;
        }

        public List<LC> StartingPoint;
        public bool isSearchMode;
        public int searchLength;
        public void Search(string SearchText)
        {
            searchLength = SearchText.Length;
            isSearchMode = true;
            StartingPoint = ThisFile.Search(SearchText);
            pictureBox1.Refresh();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {//평션키 체크
            if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Up)
            {
                
            }
            switch ((int)e.KeyCode)
            {
                case 17:
                    isCtrl = true;
                    isAlt = false;
                    isShift = false;
                    break;
                case 18:
                    isAlt = true;
                    isCtrl = false;
                    isShift = false;
                    break;
                case 37://<-
                    if (Col > 0)
                    {
                        Col--;
                    }
                    else
                    {
                        if (Line == 0) break;
                        sentences = ThisFile.GetText();
                        Line--;
                        Col = sentences[Line].Length;
                    }
                    break;
                case 39://->
                    sentences = ThisFile.GetText();
                    if (Col < sentences[Line].Length)
                    {
                        Col++;
                    }
                    else
                    {
                        if (Line == sentences.Count - 1) break;
                        Line++;
                        Col = 0;
                    }
                    break;
                case 38:
                    sentences = ThisFile.GetText();
                    if (Line != 0)
                    {
                        Line--;
                        Col = (sentences[Line].Length < Col) ? sentences[Line].Length : Col;
                    }
                    break;
                case 40:
                    sentences = ThisFile.GetText();
                    if (Line != sentences.Count - 1)
                    {
                        Line++;
                        Col = (sentences[Line].Length < Col) ? sentences[Line].Length : Col;
                    }
                    break;
                case 46://del
                    ThisFile.DErase(Line, Col);
                    break;
            }
              //MessageBox.Show(((int)e.KeyCode).ToString());
            pictureBox1.Refresh();
            pictureBox2.Refresh();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
           
            if (isAlt == true)
            {
                switch (e.KeyChar)
                {
                    case 'x':
                        Mode = Mode.Minibuf;
                        break;
                    default:

                        break;
                }
                isAlt = false;

                return;
            }
            if(isCtrl == true)
            {
                isSearchMode = false;
                switch (e.KeyChar)
                {
                    case 'v':
                    case 'y':
                        string text = Clipboard.GetText();
                        for(int i = 0; i < text.Length; i++)
                        {
                            if(text[i] != '\n') ThisFile.Insert(Line, Col++, text[i].ToString());
                            else
                            {
                                ThisFile.LineFeed(Line, Col);
                                Line++;
                                Col = 0;
                            }
                        }
                        Mode = Mode.Edit;
                        break;
                    case 'z':
                        Undo();
                        break;
                    default:
                        isCtrl = false;
                        goto outa;
                }
                isCtrl = false;
                pictureBox1.Refresh();
                pictureBox2.Refresh();
                return;
            }
            outa:
            if (Mode == Mode.Edit)
            {

                switch ((int)e.KeyChar)
                {
                    case 8://erase
                        if (Col == 0)
                        {
                            int lth = sentences[Line].Length;
                            if (Line == 0) return;
                            ThisFile.Erase(Line, Col);
                            Line--;
                            sentences = ThisFile.GetText();
                            Col = sentences[Line].Length-lth;
                        }
                        else
                        {
                            ThisFile.Erase(Line, Col--);
                        }

                        break;
                    case 9://tab
                        if (Col < 11)
                        {
                            while (Col < 11)
                            {
                                ThisFile.Insert(Line, Col++, " ");
                            }
                        }
                        else
                        {
                            ThisFile.Insert(Line, Col++, " "); ThisFile.Insert(Line, Col++, " "); ThisFile.Insert(Line, Col++, " "); ThisFile.Insert(Line, Col++, " ");
                        }
                        break;
                    case 13://enter
                        ThisFile.LineFeed(Line, Col);
                        Line++;
                        Col = 0;
                        break;
                    default:
                        LC tempLC = new LC(Line, Col+1);
                        Record temp = new Record
                        {
                            LC = tempLC,
                            key = e.KeyChar
                        };
                        UndoStack.Push(temp);                     
                        ThisFile.Insert(Line, Col++, e.KeyChar.ToString());
                        isSearchMode = false;
                        break;
                }

            }
            else if (Mode == Mode.Minibuf)
            {
                switch ((int)e.KeyChar)
                {
                    case 13:
                        minibuf.Run();
                        break;
                    default:
                        minibuf.Insert(e.KeyChar.ToString());
                        break;
                }
            }
            pictureBox1.Refresh();
            pictureBox2.Refresh();
        }

        private void minibuffer_Click(object sender, EventArgs e)
        {
            Mode = Mode.Minibuf;
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            string drawString = "";
            for (int i = 0; i < 35; i++)
            {
                drawString = drawString.Insert(i, " ");
            }
            drawString = drawString.Insert(drawString.Length - 1, "L : ");
            drawString = drawString.Insert(drawString.Length - 1, Line.ToString());
            drawString = drawString.Insert(drawString.Length - 1, "  ");
            drawString = drawString.Insert(drawString.Length - 1, "C : ");
            drawString = drawString.Insert(drawString.Length - 1, Col.ToString());
            for (int i = drawString.Length - 1; i < 49; i++)
            {
                drawString = drawString.Insert(drawString.Length - 1, " ");
            }
            if (isAlt)
            {
                drawString = drawString.Insert(drawString.Length - 1, "(Alt)----------------------------");
            }
            else if (isCtrl)
            {
                drawString = drawString.Insert(drawString.Length - 1, "(Ctrl)---------------------------");
            }
            else if (Mode == Mode.Edit)
            {
                drawString = drawString.Insert(drawString.Length - 1, "(Text)----------------------------");
            }
            else if (Mode == Mode.Minibuf)
            {
                drawString = drawString.Insert(drawString.Length - 1, "(MiniBuf)------------------------");
            }


            Font drawFont = new Font("monaco", 10);
            SolidBrush drawBrush = new SolidBrush(Color.FromArgb(32, 32, 32));
            PointF drawPoint = new PointF(0, 0);
            graphics.DrawString(drawString, drawFont, drawBrush, drawPoint);
        }

       
        private void normalToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            isUCO = false;
            pictureBox1.Refresh();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void ucodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isUCO = true;
            pictureBox1.Refresh();
        }
    }
}
