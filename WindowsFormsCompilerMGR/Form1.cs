using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsCompilerMGR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public String bufCode;
        private static String[] codes;

        private void compileButton_Click(object sender, EventArgs e)
        {
            bufCode = richTextBoxForCoding.Text;
            codes = bufCode.Split(new char[] { '\n' });
            //MessageBox.Show(bufCode);
            LexicalAnalysis lex = new LexicalAnalysis();
            bool lex_flag = lex.Analysing(bufCode);
            ArrayList LexResult=lex.Lexicals_Result;
            if (lex_flag == true)
            {
                SyntaxAnalysys syn = new SyntaxAnalysys(LexResult);
                syn.Analysis();
            }
        }
        //错误显示
        public static void ShowError(String error_msg, int line)
        {
            String line_msg = "in line " + Convert.ToString(line);
            if (line > 0)
            {
                String show_msg = error_msg + "\n" + line_msg + "\n";
                if (line - 1 > 0)
                    show_msg += " " + Convert.ToString(line - 1) + "\t" + codes[line - 2] + "\n";
                show_msg += ">" + Convert.ToString(line) + "\t" + codes[line - 1];
                if(line + 1 <= codes.Count())
                    show_msg += "\n " + Convert.ToString(line + 1) + "\t" + codes[line] + "\n";
                MessageBox.Show(show_msg);
            }
            else
                MessageBox.Show(error_msg);
        }

        private void richTextBoxForCoding_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
