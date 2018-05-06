using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCompilerMGR
{
    class LexicalAnalysisResult
    {
        private int type;
        private String content;
        private int line;

        public int Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public String Content
        {
            get
            {
                return content;
            }

            set
            {
                content = value;
            }
        }

        public int Line
        {
            get
            {
                return line;
            }

            set
            {
                line = value;
            }
        }
    }
    class LexicalAnalysis
    {
        private int line;
        public LexicalAnalysis()
        {
            p = 0;
            flag = true;
            Lexicals = new ArrayList();
            line = 1;
        }
        /*
         * 词法分析程序输出形式规定
         * 输出的是记忆符对应的数字
         */
        public const int BEGINSY = 1;
        public const int ENDSY = 2;
        public const int IFSY = 3;
        public const int THENSY = 4;
        public const int ELSE = 5;

        public const int CONSTSY = 6;
        public const int VARSY = 7;
        public const int PROCEDURE = 8;
        public const int ODDSY = 9;
        public const int WHILESY = 10;
        public const int DOSY = 11;
        public const int CALLSY = 12;
        public const int WRITESY = 13;

        public const int READSY = 14;
        public const int REPEAT = 15;
        public const int UNTILSY = 16;
        //12-23新增
        public const int LESSSY = 17;
        public const int MORESY = 18;
        public const int POINTSY = 19;

        public const int IDSY = 20;
        public const int INTSY = 21;
        public const int PLUSSY = 22;
        public const int MINUSSY = 23;
        public const int STARSY = 24;
        public const int DIVISY = 25;
        public const int LPARSY = 26;
        public const int RPARSY = 27;
        public const int COMMASY = 28;
        public const int SEMISY = 29;
        public const int COLONSY = 30;
        public const int ASSIGNSY = 31;
        public const int EQUSY = 32;
        //12-23新增
        public const int NOTEQUALSY = 33;
        public const int NOTMORE = 34;
        public const int NOTLESS = 35;

        /*
         * 读取代码文件需要的变量 
         */
        int p;
        bool flag;
        String token;
        String buffer;
        Int64 num;
        char ch;
        int symbol;
        int length_buffer;
        ArrayList Lexicals;

        public ArrayList Lexicals_Result
        {
            get
            {
                return Lexicals;
            }
        }

        void clearToken()
        {
            token = "";
        }
        void readBuffer()
        {
            ch = buffer[p];
            p++;
        }
        bool isSpace()
        {
            if (ch == ' ') return true;
            else return false;
        }
        bool isNewline()
        {
            if (ch == '\n')
            {
                line++;
                return true;
            }
            else return false;
        }
        bool isTab()
        {
            if (ch == '\t') return true;
            else return false;
        }
        bool isLetter()
        {
            if (ch >= 65 && ch <= 90) return true;
            else if (ch >= 97 && ch <= 122) return true;
            else return false;
        }
        bool isDigit()
        {
            if (ch >= '0' && ch <= '9') return true;
            else return false;
        }
        bool isColon()
        {
            if (ch == ':') return true;
            else return false;
        }
        bool isComma()
        {
            if (ch == ',') return true;
            else return false;
        }
        bool isSemi()
        {
            if (ch == ';') return true;
            else return false;
        }
        bool isEqu()
        {
            if (ch == '=') return true;
            else return false;
        }
        bool isPlus()
        {
            if (ch == '+') return true;
            else return false;
        }
        bool isMinus()
        {
            if (ch == '-') return true;
            else return false;
        }
        bool isDivi()
        {
            if (ch == '/') return true;
            else return false;
        }
        bool isStar()
        {
            if (ch == '*') return true;
            else return false;
        }
        bool isLpar()
        {
            if (ch == '(') return true;
            else return false;
        }
        bool isRpar()
        {
            if (ch == ')') return true;
            else return false;
        }
        bool isLess()
        {
            if (ch == '<') return true;
            else return false;
        }
        bool isMore()
        {
            if (ch == '>') return true;
            else return false;
        }
        bool isPoint()
        {
            if (ch == '.') return true;
            else return false;
        }
        void retract()
        {
            p--;
        }
        void catToken()
        {
            token = token + ch;
            //cout << token << endl;
        }
        int reserver()
        {
            String temp = token;
            String [] letter_tokens = { "", "begin", "end", "if", "then", "else", "const", "var", "procedure", "odd", "while", "do", "call", "write", "read", "repeat", "until" };
            int length_array = 17;
            for (int i = 1; i < length_array; i++)
            {
                if (temp.Equals(letter_tokens[i]))
                    return i;
            }
            return 0;
        }
        Int64 transNum()
        {
            Int64 num = 0;
            int length = token.Length;
            for (int i = 0; i < length; i++)
            {
                num *= 10;
                num += (token[i] - '0');
            }
            return num;
        }
        void error()
        {
            flag = false;
            //cerr << "Error: Invalid Input." << endl;
            Form1.ShowError("错误：输入字符非法。", line);
        }

        int getsym()
        {
            clearToken();
            readBuffer();
            while ((isSpace() || isNewline() || isTab()) && p < length_buffer)
            {
                readBuffer();
            }
            if (isLetter())
            {
                while (isLetter() || isDigit())
                {
                    catToken();
                    readBuffer();
                }
                retract();
                int resultValue = reserver();
                if (resultValue == 0) symbol = IDSY;
                else symbol = resultValue;
            }
            else if (isDigit())
            {
                while (isDigit())
                {
                    catToken();
                    readBuffer();
                }
                //数字后面不能直接带字母
                if (isLetter())
                {
                    error();
                    return 0;
                }
                retract();
                num = transNum();
                symbol = INTSY;
            }
            else if (isColon())
            {
                readBuffer();
                if (isEqu()) symbol = ASSIGNSY;
                else
                {
                    retract();
                    symbol = COLONSY;
                }
            }
            else if (isPlus()) symbol = PLUSSY;
            else if (isMinus()) symbol = MINUSSY;
            else if (isStar()) symbol = STARSY;
            else if (isLpar()) symbol = LPARSY;
            else if (isRpar()) symbol = RPARSY;
            else if (isComma()) symbol = COMMASY;
            else if (isSemi()) symbol = SEMISY;
            else if (isDivi())
            {
                readBuffer();
                //只处理/**/型注释
                if (isStar())
                {
                    do
                    {
                        do
                        {
                            readBuffer();
                        } while (!isStar());
                        do
                        {
                            readBuffer();
                            if (isDivi()) return 0;
                        } while (isStar());
                    } while (!isStar());
                }
                else
                {
                    retract();
                    symbol = DIVISY;
                }
            }
            else if (isEqu()) symbol = EQUSY;
            else if (isLess())
            {
                readBuffer();
                if (isMore()) symbol = NOTEQUALSY;
                else if (isEqu()) symbol = NOTMORE;
                else
                {
                    retract();
                    symbol = LESSSY;
                }
            }
            else if (isMore())
            {
                readBuffer();
                if (isEqu()) symbol = NOTLESS;
                else
                {
                    retract();
                    symbol = MORESY;
                }
            }
            else if (isPoint()) symbol = POINTSY;
            else if (p >= length_buffer) return 0;
            else error();
            if (flag == true)
                print();
            return 0;
        }
        void print()
        {
            String[] MNEMONIC = { "ERROR","BEGINSY","ENDSY","IFSY","THENSY","ELSE","CONSTSY","VARSY","PROCEDURE","ODDSY","WHILESY","DOSY","CALLSY",
        "WRITESY","READSY","REPEAT","UNTILSY","LESSSY","MORESY","POINTSY","IDSY","INTSY","PLUSSY","MINUSSY","STARSY","DIVISY","LPARSY",
        "RPARSY","COMMASY","SEMISY","COLONSY","ASSIGNSY","EQUSY","NOTEQUALSY","NOTMORE","NOTLESS","LMBRACKET","RMBRACKET" };
            if (symbol > 35) Console.WriteLine("ERROR,-");
            else
            {
                Console.Write("{0},", MNEMONIC[symbol]);// MNEMONIC[symbol] << ',';
                if (symbol == INTSY) Console.WriteLine("{0}", num);//cout << num << endl;
                else if (symbol == IDSY) Console.WriteLine(token);//cout << token << endl;
                else Console.WriteLine("-");//cout << "-" << endl;
                
                if(MNEMONIC[symbol].Equals("ERROR"))
                {
                    Form1.ShowError("错误：输入非法。",0);
                    Lexicals.Clear();
                }
                else
                {
                    LexicalAnalysisResult R = new LexicalAnalysisResult();
                    R.Type = symbol;
                    R.Line = line;
                    if (symbol == INTSY) R.Content = num.ToString();
                    else if (symbol == IDSY) R.Content = token;
                    else R.Content = "";
                    Lexicals.Add(R);
                }
            }
        }
        public bool Analysing(String code)
        {
            p = 0;
            flag = true;
            buffer = code + " ";
            length_buffer = buffer.Length - 1;
            while (p < length_buffer && flag == true)
            {
                getsym();
            }
            return flag;
        }
    }
}
