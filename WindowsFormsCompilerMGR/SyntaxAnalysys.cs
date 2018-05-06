using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCompilerMGR
{
    class Symbols
    {
        private String name;
        public enum SymTypes
        {
            CONST = 0,
            VAR = 1,
            PROC = 2
        };
        private int type;
        private int value;      //PROC中代表pcode地址，常量有此值，变量没有此值
        private int level;
        private int addr;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public int Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public int Level
        {
            get
            {
                return level;
            }

            set
            {
                level = value;
            }
        }

        public int Addr
        {
            get
            {
                return addr;
            }

            set
            {
                addr = value;
            }
        }

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
    }
    
    class SyntaxAnalysys
    {
        private ArrayList Lexresult;
        private ArrayList symbols;
        private ArrayList errors;
        private bool flag;
        private int i;
        private int level;
        private int[] addr;
        //目标代码存储区
        private ArrayList PCodes;
        private int codeAddr;

        public SyntaxAnalysys(ArrayList lexresult)
        {
            Lexresult = lexresult;
            flag = true;
            i = 0;
            level = 0;
            addr = new int[10];
            symbols = new ArrayList();
            PCodes = new ArrayList();
            errors = new ArrayList();
            for (int i = 0; i < 10; i++)
                addr[i] = 3;
            codeAddr = 0;
        }

        public void Analysis()
        {
            MainProgram();
            if (flag == true)
            {
                string file = "pcode.txt";
                StreamWriter wr = new StreamWriter(new FileStream(file, FileMode.Create, FileAccess.Write));
                for (int i = 0; i < PCodes.Count; i++)
                {
                    PCode pc = (PCode)PCodes[i];
                    wr.WriteLine(pc.PrintPCode());
                    wr.Flush();
                }
                wr.Close();
                String name = System.IO.Directory.GetCurrentDirectory();
                Console.WriteLine(name);
                Form1.ShowError("生成的P-Code文件已保存到\n" + name + "\\pcode.txt",0);
            }
            else
            {
                Error err = (Error)errors[0];
                err.ErrorMsgSend();
            }
        }
        /*
         * 符号表项目
         * name
         * kind
         * value
         * level
         * addr
         */
        //程序分析
        private void MainProgram()
        {
            //<程序> := <分程序>.
            //先分程序后结束符
            //结束符
            
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            Program();
            if (flag == true)
            {
                //i++;
                if (i >= Lexresult.Count)
                {
                    flag = false;
                    Console.WriteLine("错误：无结束符号");
                    errors.Add(new Error("错误：无结束符号", 0));
                    return;
                }
                else
                {
                    lex = (LexicalAnalysisResult)Lexresult[i];
                    if (lex.Type == LexicalAnalysis.POINTSY && i == Lexresult.Count - 1)
                    {
                        PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 0));
                        codeAddr++;
                        Console.WriteLine("分析结束");
                        return;
                    }
                    else
                    {
                        flag = false;
                        Console.WriteLine("错误：结束符号.之后还有字符");
                        errors.Add(new Error("错误：结束符号.之后还有字符", lex.Line));
                        return;
                        //Program();//这里还需要改
                    }
                }
            }
        }
        //分程序分析
        private void Program()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            //先判断FIRST集
            switch (lex.Type)
            {
                case LexicalAnalysis.CONSTSY:
                case LexicalAnalysis.VARSY:
                case LexicalAnalysis.PROCEDURE:
                case LexicalAnalysis.IDSY:
                case LexicalAnalysis.IFSY:
                case LexicalAnalysis.CALLSY:
                case LexicalAnalysis.BEGINSY:
                case LexicalAnalysis.WHILESY:
                case LexicalAnalysis.READSY:
                case LexicalAnalysis.WRITESY:
                    break;
                default:
                    flag = false;
                    errors.Add(new Error("错误：程序开始的字符非法。", lex.Line));
                    return;
                    //break;
            }
            //超出范围tryandcatch
            //常量说明部分
            if (lex.Type == LexicalAnalysis.CONSTSY && i < Lexresult.Count)
            {
                i++;
                //进入常量说明部分分析
                ConstIndicate();
            }
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type == LexicalAnalysis.VARSY && i < Lexresult.Count)
            {
                i++;
                //进入变量说明部分分析
                VarIndicate();
            }
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type == LexicalAnalysis.PROCEDURE && i < Lexresult.Count)
            {
                i++;
                //过程说明部分子程序
                ProcIndicate();
            }
            //语句部分
            Code();
            return;
        }
        //常量说明部分
        private void ConstIndicate()
        {
            if (i >= Lexresult.Count)
            {
                flag = false;
                Console.WriteLine("错误：程序没有语句！");
                errors.Add(new Error("错误：程序没有语句。", 0));
                return;
            }
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            //常量定义
            if (lex.Type != LexicalAnalysis.IDSY)
            {
                flag = false;
                Console.WriteLine("错误：常量const后无标识符");
                errors.Add(new Error("错误：常量const后无标识符", lex.Line));
                return;
            }
            else
            {
                Symbols sym = new Symbols();
                sym.Type = (int)Symbols.SymTypes.CONST;
                sym.Level = 0;
                sym.Addr = 0;
                sym.Name = lex.Content;
                //addr[level]++;
                i++;
                lex = (LexicalAnalysisResult)Lexresult[i];
                if (lex.Type != LexicalAnalysis.EQUSY)
                {
                    flag = false;
                    Console.WriteLine("错误：const标识符后无=符号");
                    errors.Add(new Error("错误：const标识符后无=符号", lex.Line));
                    return;
                }
                i++;
                lex = (LexicalAnalysisResult)Lexresult[i];
                if (lex.Type != LexicalAnalysis.INTSY)
                {
                    flag = false;
                    Console.WriteLine("错误：常量标识符赋值应为无符号整数");
                    errors.Add(new Error("错误：常量标识符赋值应为无符号整数", lex.Line));
                    return;
                }
                if (Convert.ToInt64(lex.Content) > Convert.ToInt64(Int32.MaxValue))
                {
                    flag = false;
                    Console.WriteLine("错误：整数太大");
                    errors.Add(new Error("错误：整数太大", lex.Line));
                    return;
                }
                sym.Value = Convert.ToInt32(lex.Content);
                for(int i= symbols.Count - 1; i >= 0; i--)
                {
                    Symbols tempsym = (Symbols)symbols[i];
                    if (tempsym.Name.Equals(sym.Name) && level == tempsym.Level)
                    {
                        flag = false;
                        Console.WriteLine("错误：符号表已存在标识符"+sym.Name);
                        errors.Add(new Error("错误：符号表已存在标识符" + sym.Name, lex.Line));
                        return;
                    }
                }
                symbols.Add(sym);
                //递归子程序或结束
                i++;
                lex = (LexicalAnalysisResult)Lexresult[i];
                if (lex.Type == LexicalAnalysis.SEMISY)
                {
                    i++;
                    return;
                }
                else if (lex.Type == LexicalAnalysis.COMMASY)
                {
                    i++;
                    ConstIndicate();
                }
                else
                {
                    flag = false;
                    Console.WriteLine("错误：符号应为 , 或 ;");
                    errors.Add(new Error("错误：符号应为 , 或 ;", lex.Line));
                    return;
                }
            }
        }
        //变量说明部分
        private void VarIndicate()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.IDSY)
            {
                flag = false;
                Console.WriteLine("错误：var后面应为标识符");
                errors.Add(new Error("错误：var后面应为标识符", lex.Line));
                return;
            }
            Symbols sym = new Symbols();
            sym.Type = (int)Symbols.SymTypes.VAR;
            sym.Name = lex.Content;
            sym.Level = level;
            sym.Addr = addr[level];
            addr[level]++;
            i++;
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type == LexicalAnalysis.COMMASY)
            {
                for (int i = symbols.Count - 1; i >= 0; i--)
                {
                    Symbols tempsym = (Symbols)symbols[i];
                    if (tempsym.Name.Equals(sym.Name) && level == tempsym.Level)
                    {
                        flag = false;
                        Console.WriteLine("错误：符号表已存在标识符" + sym.Name);
                        errors.Add(new Error("错误：符号表已存在标识符" + sym.Name, lex.Line));
                        return;
                    }
                }
                symbols.Add(sym);
                i++;
                VarIndicate();
            }
            else if (lex.Type == LexicalAnalysis.SEMISY)
            {
                for (int i = symbols.Count - 1; i >= 0; i--)
                {
                    Symbols tempsym = (Symbols)symbols[i];
                    if (tempsym.Name.Equals(sym.Name) && level == tempsym.Level)
                    {
                        flag = false;
                        Console.WriteLine("错误：符号表已存在标识符" + sym.Name);
                        errors.Add(new Error("错误：符号表已存在标识符" + sym.Name, lex.Line));
                        return;
                    }
                }
                symbols.Add(sym);
                i++;
                return;
            }
            else
            {
                flag = false;
                Console.WriteLine("错误：符号应为 , 或 ;");
                errors.Add(new Error("错误：符号应为 , 或 ;", lex.Line));
                return;
            }
        }
        //过程说明部分
        private void ProcIndicate()
        {
            //JMP 0 主程序入口
            PCodes.Add(new PCode(codeAddr, PCode.JMP, 0, 0));
            int bufcodeaddr = codeAddr;
            //JMP此处的0表示待定
            codeAddr++;
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            //标识符
            if (lex.Type != LexicalAnalysis.IDSY)
            {
                flag = false;
                Console.WriteLine("错误：procedure后应为标识符");
                errors.Add(new Error("错误：procedure后应为标识符", lex.Line));
                return;
            }
            Symbols sym = new Symbols();
            sym.Name = lex.Content;
            sym.Type = (int)Symbols.SymTypes.PROC;
            sym.Level = level;
            sym.Value = codeAddr + 1;
            sym.Addr = addr[level];
            addr[level]++;
            //;
            PCodes.Add(new PCode(codeAddr, PCode.JMP, 0, codeAddr + 1));
            codeAddr++;

            i++;
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.SEMISY)
            {
                flag = false;
                Console.WriteLine("缺少 ;");
                errors.Add(new Error("错误：缺少 ;", lex.Line));
                return;
            }
            for (int i = symbols.Count - 1; i >= 0; i--)
            {
                Symbols tempsym = (Symbols)symbols[i];
                if (tempsym.Name.Equals(sym.Name) && level == tempsym.Level)
                {
                    flag = false;
                    Console.WriteLine("错误：符号表已存在标识符" + sym.Name);
                    errors.Add(new Error("错误：符号表已存在标识符" + sym.Name, lex.Line));
                    return;
                }
            }
            symbols.Add(sym);
            i++;
            level++;
            if (level > 10)
            {
                flag = false;
                errors.Add(new Error("错误：分程序层次过多", lex.Line));
                return;
            }
            Program();
            //符号表删除
            int cnt = symbols.Count - 1;
            for(int i = cnt; i >= 0; i--)
            {
                Symbols sym_prerem = (Symbols)symbols[i];
                if (sym_prerem.Level == level)
                    symbols.RemoveAt(i);
            }
            level--;
            //i++;
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.SEMISY)
            {
                flag = false;
                Console.WriteLine("缺少 ;");
                errors.Add(new Error("错误：缺少 ;", lex.Line));
                return;
            }
            i++;

            PCode pc = (PCode)PCodes[bufcodeaddr];
            pc.Y = codeAddr;

            PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 0));
            codeAddr++;

            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type == LexicalAnalysis.PROCEDURE)
            {
                i++;
                ProcIndicate();
            }
            else
            {
                //i++;
                return;
            }
        }
        //语句部分
        private void Code()
        {
            //等会儿再写这个233333
            if (i >= Lexresult.Count)
                return;
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type == LexicalAnalysis.IDSY)
                AssignCode();
            else if (lex.Type == LexicalAnalysis.IFSY)
                ConditionCode();
            else if (lex.Type == LexicalAnalysis.WHILESY)
                WhileDoLoopCode();
            else if (lex.Type == LexicalAnalysis.CALLSY)
                CallProcedure();
            else if (lex.Type == LexicalAnalysis.READSY)
                ReadCode();
            else if (lex.Type == LexicalAnalysis.WRITESY)
                WriteCode();
            else if (lex.Type == LexicalAnalysis.BEGINSY)
                ComplexCode();
            else if (lex.Type == LexicalAnalysis.REPEAT)
                RepeatCode();
            else
            {
                //empty
                //Console.WriteLine("Empty Code");
                return;
            }

            if (flag == true)
            {
                //i++;
                return;
            }
            else
            {
                //错误处理
                return;
            }
        }
        //赋值语句
        private void AssignCode()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.IDSY)
            {
                flag = false;
                Console.WriteLine("错误：赋值语句无标识符");
                errors.Add(new Error("错误：其他错误", lex.Line));
                return;
            }
            //符号表检查
            int preloc = -1;
            //之前的同一层符号表还没有检查
            for (int ind = symbols.Count - 1; ind >= 0; ind--)
            {
                Symbols tempsym = (Symbols)symbols[ind];
                if (lex.Content.Equals(tempsym.Name) && level >= tempsym.Level)
                {
                    preloc = ind;
                    break;
                }
            }
            if(preloc==-1)
            {
                flag = false;
                Console.WriteLine("错误：不存在符号{0}",lex.Content);
                errors.Add(new Error("错误：不存在符号"+lex.Content, lex.Line));
                return;
            }
            //PCODE 标识符相关
            PCodes.Add(new PCode(codeAddr, PCode.INT, 0, 1));
            codeAddr++;
            Symbols sym_t = (Symbols)symbols[preloc];
            int sym_lev = sym_t.Level;
            PCodes.Add(new PCode(codeAddr, PCode.LOD, level - sym_lev, sym_t.Addr));
            codeAddr++;
            //表达式检查
            i++;
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.ASSIGNSY)
            {
                flag = false;
                Console.WriteLine("错误：此处应为赋值符号");
                errors.Add(new Error("错误：应为赋值符号 :=", lex.Line));
                return;
            }
            i++;
            //进入表达式判断
            ExpressionCode();
            if (flag == true)
            {
                //PCODE
                PCodes.Add(new PCode(codeAddr, PCode.STO, level - sym_lev, sym_t.Addr));
                codeAddr++;
                return;
            }
            else
            {
                Console.WriteLine("Error!");
                errors.Add(new Error("错误：其他错误。", 0));
                return;
            }
        }
        //表达式
        private void ExpressionCode()
        {
            //[+|-]
            bool minusflag = false;
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type == LexicalAnalysis.PLUSSY)
            {
                //可以写PCODE
                /*PCodes.Add(new PCode(codeAddr, PCode.INT, 0, 1));
                codeAddr++;
                PCodes.Add(new PCode(codeAddr, PCode.LIT, 0, 0));
                codeAddr++;*/
                i++;
            }
            else if (lex.Type == LexicalAnalysis.MINUSSY)
            {
                //可以写PCODE
                minusflag = true;
                /*PCodes.Add(new PCode(codeAddr, PCode.INT, 0, 1));
                codeAddr++;
                PCodes.Add(new PCode(codeAddr, PCode.LIT, 0, 0));
                codeAddr++;*/
                i++;
            }
            //项
            Nape();
            if (minusflag == true)
            {
                PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 1));//转换为负数
                codeAddr++;
            }
            lex = (LexicalAnalysisResult)Lexresult[i];
            while(lex.Type==LexicalAnalysis.PLUSSY || lex.Type == LexicalAnalysis.MINUSSY)
            {
                i++;
                Nape();
                //PCODE
                if(lex.Type == LexicalAnalysis.PLUSSY)
                {
                    PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 2));//ADD
                    codeAddr++;
                }
                else
                {
                    PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 3));//SUB
                    codeAddr++;
                }
                lex = (LexicalAnalysisResult)Lexresult[i];
            }
            return;
        }
        //项
        private void Nape()
        {
            //因子
            Factor();
            if (flag == false) return;
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            while(lex.Type==LexicalAnalysis.STARSY || lex.Type == LexicalAnalysis.DIVISY)
            {
                i++;
                Factor();
                //pcode
                if (lex.Type == LexicalAnalysis.STARSY)
                {
                    PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 4));//MUL
                    codeAddr++;
                }
                else
                {
                    PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 5));//DIV
                    codeAddr++;
                }
                lex = (LexicalAnalysisResult)Lexresult[i];
            }
            return;
        }
        //因子
        private void Factor()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type == LexicalAnalysis.IDSY)
            {
                //检查标识符存不存在
                //没检查同层但是不同过程的相关声明，需要在符号表栈中标记
                int preloc = -1;
                for (int ind = symbols.Count - 1; ind >= 0; ind--)
                {
                    Symbols tempsym = (Symbols)symbols[ind];
                    if (lex.Content.Equals(tempsym.Name) && level >= tempsym.Level)
                    {
                        preloc = ind;
                        break;
                    }
                }
                if(preloc==-1)
                {
                    flag = false;
                    Console.WriteLine("错误：不存在符号{0}", lex.Content);
                    String msg = "错误：不存在符号" + lex.Content;
                    errors.Add(new Error(msg, lex.Line));
                    return;
                }
                //PCODE 标识符相关
                PCodes.Add(new PCode(codeAddr, PCode.INT, 0, 1));
                codeAddr++;
                Symbols sym_t = (Symbols)symbols[preloc];
                int sym_lev = sym_t.Level;
                PCodes.Add(new PCode(codeAddr, PCode.LOD, level - sym_lev, sym_t.Addr));
                codeAddr++;
                i++;
                return;
            }
            else if (lex.Type == LexicalAnalysis.INTSY)
            {
                int Num = Convert.ToInt32(lex.Content);
                //PCODE 数字相关
                PCodes.Add(new PCode(codeAddr, PCode.INT, 0, 1));
                codeAddr++;
                PCodes.Add(new PCode(codeAddr, PCode.LIT, 0, Num));
                codeAddr++;
                i++;
                return;
            }
            else if (lex.Type == LexicalAnalysis.LPARSY)
            {
                i++;
                ExpressionCode();
                //i++
                lex = (LexicalAnalysisResult)Lexresult[i];
                if (lex.Type != LexicalAnalysis.RPARSY)
                {
                    flag = false;
                    Console.WriteLine("错误：此处应为 ( ");
                    errors.Add(new Error("错误：此处应为 ( ", lex.Line));
                    return;
                }
                else
                {
                    //P-CODE
                    i++;
                    return;
                }
            }
            else
            {
                flag = false;
                Console.WriteLine("错误：缺少标识符、数字或(");
                errors.Add(new Error("错误：缺少标识符、数字或(", lex.Line));
                return;
            }
        }
        //条件
        private void Condition()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type == LexicalAnalysis.ODDSY)
            {
                i++;
                ExpressionCode();
                //PCODE
                //odd<表达式>
                //表达式如何odd的问题
                PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 6));//ODD
                return;
            }
            else
            {
                ExpressionCode();
                lex = (LexicalAnalysisResult)Lexresult[i];
                //关系运算符
                bool flag_type = false;
                flag_type |= (lex.Type != LexicalAnalysis.LESSSY);
                flag_type |= (lex.Type != LexicalAnalysis.MORESY);
                flag_type |= (lex.Type != LexicalAnalysis.NOTMORE);
                flag_type |= (lex.Type != LexicalAnalysis.NOTLESS);
                flag_type |= (lex.Type != LexicalAnalysis.NOTEQUALSY);
                flag_type |= (lex.Type != LexicalAnalysis.EQUSY);
                if (flag == false || flag_type == false)
                {
                    flag = false;
                    Console.WriteLine("Error!");
                    errors.Add(new Error("错误：此处应为关系运算符。", lex.Line));
                    return;
                }
                
                i++;
                ExpressionCode();
                //P-CODE 写什么条件
                if (lex.Type == LexicalAnalysis.LESSSY)
                {
                    PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 10));//LSS
                    codeAddr++;
                }
                else if (lex.Type == LexicalAnalysis.MORESY)
                {
                    PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 12));//GTR
                    codeAddr++;
                }
                else if (lex.Type == LexicalAnalysis.NOTMORE)
                {
                    PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 13));//LEQ
                    codeAddr++;
                }
                else if (lex.Type == LexicalAnalysis.NOTLESS)
                {
                    PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 11));//GEQ
                    codeAddr++;
                }
                else if (lex.Type == LexicalAnalysis.NOTEQUALSY)
                {
                    PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 9));//NEQ
                    codeAddr++;
                }
                else if (lex.Type == LexicalAnalysis.EQUSY)
                {
                    PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 8));//EQL
                    codeAddr++;
                }
                return;
            }
        }
        //条件语句
        private void ConditionCode()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.IFSY)
            {
                //flag = false;
                //Console.WriteLine("错误：此处应为if");
                return;
            }
            i++;
            Condition();
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.THENSY)
            {
                flag = false;
                Console.WriteLine("错误：缺少then");
                errors.Add(new Error("错误：缺少then", lex.Line));
                return;
            }
            i++;
            int bufjpc = 0, bufcodeaddr=codeAddr;
            PCodes.Add(new PCode(codeAddr, PCode.JPC, 0, 0));//y值还需要改成后面的bufjpc
            codeAddr++;
            Code();
            //i++
            lex = (LexicalAnalysisResult)Lexresult[i];
            bufjpc = codeAddr;
            PCode pc = (PCode)PCodes[bufcodeaddr];
            pc.Y = bufjpc;//??
            PCodes[bufcodeaddr] = pc;
            if (lex.Type != LexicalAnalysis.ELSE)
                return;
            else
            {
                i++;
                Code();
                return;
            }
        }
        //当型循环语句
        private void WhileDoLoopCode()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.WHILESY)
            {
                return;
            }
            i++;
            //条件
            int bufjpc = 0, bufcodeaddr = codeAddr;
            PCodes.Add(new PCode(codeAddr, PCode.JPC, 0, 0));//y值还需要改成后面的bufjpc
            codeAddr++;
            Condition();
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.DOSY)
            {
                flag = false;
                Console.WriteLine("错误：此处应为do");
                errors.Add(new Error("错误：应为do", lex.Line));
                return;
            }
            i++;
            //语句
            Code();
            PCodes.Add(new PCode(codeAddr, PCode.JMP, 0, bufcodeaddr));//y值为JPC所在位置
            codeAddr++;
            bufjpc = codeAddr;
            PCode pc = (PCode)PCodes[bufcodeaddr];
            pc.Y = bufjpc;//??
            PCodes[bufcodeaddr] = pc;
            return;
        }
        //过程调用语句
        //call+标识符
        private void CallProcedure()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if(lex.Type != LexicalAnalysis.CALLSY )
            {
                flag = false;
                return;
            }
            i++;
            lex = (LexicalAnalysisResult)Lexresult[i];
            //标识符
            if (lex.Type != LexicalAnalysis.IDSY)
            {
                flag = false;
                Console.WriteLine("错误：call后应为标识符");
                errors.Add(new Error("错误：call后应为标识符", lex.Line));
                return;
            }
            else
            {
                //符号表查找
                //符号表检查
                int preloc = -1;
                //之前的同一层符号表还没有检查
                for (int ind = symbols.Count - 1; ind >= 0; ind--)
                {
                    Symbols tempsym = (Symbols)symbols[ind];
                    if (lex.Content.Equals(tempsym.Name) && level >= tempsym.Level && tempsym.Type == (int)Symbols.SymTypes.PROC)
                    {
                        preloc = ind;
                        break;
                    }
                }
                if(preloc==-1)
                {
                    flag = false;
                    Console.WriteLine("错误：不存在符号{0}", lex.Content);
                    errors.Add(new Error("错误：不存在符号"+lex.Content, lex.Line));
                    return;
                }
                //PCODE
                Symbols sym = (Symbols)symbols[preloc];
                
                PCodes.Add(new PCode(codeAddr, PCode.CAL, level - sym.Level, sym.Value));
                codeAddr++;

                i++;
                return;
            }
        }
        //复合语句
        private void ComplexCode()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.BEGINSY)
            {
                flag = false;
                return;
            }
            i++;
            Code();
            lex = (LexicalAnalysisResult)Lexresult[i];
            while (lex.Type == LexicalAnalysis.SEMISY)
            {
                i++;
                Code();
                lex = (LexicalAnalysisResult)Lexresult[i];
            }
            if (lex.Type == LexicalAnalysis.ENDSY)
            {
                //PCODE 不知道该不该加
                //PCodes.Add(new PCode(codeAddr, PCode.OPR, 0, 0));
                //codeAddr++;

                i++;
                return;
            }
            else
            {
                flag = false;
                Console.WriteLine("错误：此处应为end或;");
                errors.Add(new Error("错误：此处应为end或;", lex.Line));
                return;
            }
        }
        //重复语句
        private void RepeatCode()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.REPEAT)
            {
                flag = false;
                Console.WriteLine("Error!");
                errors.Add(new Error("错误：其他错误。", 0));
                return;
            }
            i++;
            int bufcodeaddr = codeAddr;
            Code();
            lex = (LexicalAnalysisResult)Lexresult[i];
            while (lex.Type == LexicalAnalysis.SEMISY)
            {
                i++;
                Code();
                lex = (LexicalAnalysisResult)Lexresult[i];
            }
            if (lex.Type != LexicalAnalysis.UNTILSY)
            {
                flag = false;
                Console.WriteLine("错误：此处应为until或;");
                errors.Add(new Error("错误：此处应为until或;", lex.Line));
                return;
            }
            i++;
            Condition();
            PCodes.Add(new PCode(codeAddr, PCode.JPC, 0, codeAddr + 2));
            codeAddr++;
            PCodes.Add(new PCode(codeAddr, PCode.JMP, 0, bufcodeaddr));
            codeAddr++;
            return;
        }
        //读语句
        private void ReadCode()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.READSY)
            {
                flag = false;
                Console.WriteLine("Error!");
                errors.Add(new Error("错误：其他错误。", 0));
                return;
            }
            i++;
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.LPARSY)
            {
                flag = false;
                Console.WriteLine("错误：此处应为(");
                errors.Add(new Error("错误：此处应补充(", lex.Line));
                return;
            }
            i++;
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type == LexicalAnalysis.IDSY)
            {
                //PCODE?
                //检查符号表
                //检查标识符存不存在
                //没检查同层但是不同过程的相关声明，需要在符号表栈中标记
                int preloc = -1;
                for (int ind = symbols.Count - 1; ind >= 0; ind--)
                {
                    Symbols tempsym = (Symbols)symbols[ind];
                    if (lex.Content.Equals(tempsym.Name) && level >= tempsym.Level)
                    {
                        preloc = ind;
                        break;
                    }
                }
                if(preloc==-1)
                {
                    flag = false;
                    Console.WriteLine("错误：不存在符号{0}", lex.Content);
                    errors.Add(new Error("错误：不存在符号"+lex.Content, lex.Line));
                    return;
                }
                Symbols sym = (Symbols)symbols[preloc];
                PCodes.Add(new PCode(codeAddr, PCode.RED, level - sym.Level, sym.Addr));
                codeAddr++;
                i++;
            }
            else
            {
                flag = false;
                Console.WriteLine("无标识符");
                errors.Add(new Error("错误：无标识符", lex.Line));
                return;
            }
            lex = (LexicalAnalysisResult)Lexresult[i];
            while (lex.Type == LexicalAnalysis.COMMASY)
            {
                i++;
                lex = (LexicalAnalysisResult)Lexresult[i];
                if (lex.Type == LexicalAnalysis.IDSY)
                {
                    //PCODE?
                    //检查符号表
                    //检查标识符存不存在
                    //没检查同层但是不同过程的相关声明，需要在符号表栈中标记
                    int preloc = -1;
                    for (int ind = symbols.Count - 1; ind >= 0; ind--)
                    {
                        Symbols tempsym = (Symbols)symbols[ind];
                        if (lex.Content.Equals(tempsym.Name) && level >= tempsym.Level)
                        {
                            preloc = ind;
                            break;
                        }
                    }
                    if (preloc == -1)
                    {
                        flag = false;
                        Console.WriteLine("错误：不存在符号{0}", lex.Content);
                        errors.Add(new Error("错误：不存在符号"+lex.Content, lex.Line));
                        return;
                    }
                    Symbols sym = (Symbols)symbols[preloc];
                    PCodes.Add(new PCode(codeAddr, PCode.RED, level - sym.Level, sym.Addr));
                    codeAddr++;
                    //i++;
                }
                else
                {
                    flag = false;
                    Console.WriteLine("无标识符");
                    errors.Add(new Error("错误：无标识符", lex.Line));
                    return;
                }
                i++;
                lex = (LexicalAnalysisResult)Lexresult[i];
            }
            if (lex.Type != LexicalAnalysis.RPARSY)
            {
                flag = false;
                Console.WriteLine("错误：此处应为)");
                errors.Add(new Error("错误：此处应补充)", lex.Line));
                return;
            }
            else
            {
                i++;
                return;
            }
        }
        //写语句
        private void WriteCode()
        {
            LexicalAnalysisResult lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.WRITESY)
            {
                flag = false;
                Console.WriteLine("Error!");
                errors.Add(new Error("错误：其他错误。", 0));
                return;
            }
            i++;
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type != LexicalAnalysis.LPARSY)
            {
                flag = false;
                Console.WriteLine("错误：此处应为(");
                errors.Add(new Error("错误：此处应补充(", lex.Line));
                return;
            }
            i++;
            lex = (LexicalAnalysisResult)Lexresult[i];
            if (lex.Type == LexicalAnalysis.IDSY)
            {
                //PCODE?
                //检查符号表
                int preloc = -1;
                for (int ind = symbols.Count - 1; ind >= 0; ind--)
                {
                    Symbols tempsym = (Symbols)symbols[ind];
                    if (lex.Content.Equals(tempsym.Name) && level >= tempsym.Level)
                    {
                        preloc = ind;
                        break;
                    }
                }
                if (preloc == -1)
                {
                    flag = false;
                    Console.WriteLine("错误：不存在符号{0}", lex.Content);
                    errors.Add(new Error("错误：不存在符号" + lex.Content, lex.Line));
                    return;
                }
                Symbols sym = (Symbols)symbols[preloc];
                PCodes.Add(new PCode(codeAddr, PCode.INT, 0, 1));
                codeAddr++;
                PCodes.Add(new PCode(codeAddr, PCode.LOD, level - sym.Level, sym.Addr));
                codeAddr++;
                PCodes.Add(new PCode(codeAddr, PCode.WRT, 0, 0));
                codeAddr++;
                i++;
            }
            else if (lex.Type == LexicalAnalysis.INTSY)
            {
                PCodes.Add(new PCode(codeAddr, PCode.INT, 0, 1));
                codeAddr++;
                PCodes.Add(new PCode(codeAddr, PCode.LIT, 0, Convert.ToInt32(lex.Content)));
                codeAddr++;
                PCodes.Add(new PCode(codeAddr, PCode.WRT, 0, 0));
                codeAddr++;
                i++;
            }
            else
            {
                flag = false;
                Console.WriteLine("无标识符");
                errors.Add(new Error("错误：无标识符", lex.Line));
                return;
            }
            lex = (LexicalAnalysisResult)Lexresult[i];
            while (lex.Type == LexicalAnalysis.COMMASY)
            {
                i++;
                lex = (LexicalAnalysisResult)Lexresult[i];
                if (lex.Type == LexicalAnalysis.IDSY)
                {
                    //PCODE?
                    //检查符号表
                    int preloc = -1;
                    for (int ind = symbols.Count - 1; ind >= 0; ind--)
                    {
                        Symbols tempsym = (Symbols)symbols[ind];
                        if (lex.Content.Equals(tempsym.Name) && level >= tempsym.Level)
                        {
                            preloc = ind;
                            break;
                        }
                    }
                    if (preloc == -1)
                    {
                        flag = false;
                        Console.WriteLine("错误：不存在符号{0}", lex.Content);
                        errors.Add(new Error("错误：不存在符号" + lex.Content, lex.Line));
                        return;
                    }
                    Symbols sym = (Symbols)symbols[preloc];
                    PCodes.Add(new PCode(codeAddr, PCode.INT, 0, 1));
                    codeAddr++;
                    PCodes.Add(new PCode(codeAddr, PCode.LOD, level - sym.Level, sym.Addr));
                    codeAddr++;
                    PCodes.Add(new PCode(codeAddr, PCode.WRT, 0, 0));
                    codeAddr++;
                    //i++;
                }
                else if(lex.Type == LexicalAnalysis.INTSY)
                {
                    PCodes.Add(new PCode(codeAddr, PCode.INT, 0, 1));
                    codeAddr++;
                    PCodes.Add(new PCode(codeAddr, PCode.LIT, 0, Convert.ToInt32(lex.Content)));
                    codeAddr++;
                    PCodes.Add(new PCode(codeAddr, PCode.WRT, 0, 0));
                    codeAddr++;
                }
                else
                {
                    flag = false;
                    Console.WriteLine("无标识符");
                    errors.Add(new Error("错误：无标识符", lex.Line));
                    return;
                }
                i++;
                lex = (LexicalAnalysisResult)Lexresult[i];
            }
            if (lex.Type != LexicalAnalysis.RPARSY)
            {
                flag = false;
                Console.WriteLine("错误：此处应为)");
                errors.Add(new Error("错误：此处应补充)", lex.Line));
                return;
            }
            else
            {
                i++;
                return;
            }
        }
    }
    
}
