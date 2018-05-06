using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCompilerMGR
{
    class PCode
    {
        public const int LIT = 0;
        public const int OPR = 1;
        public const int LOD = 2;
        public const int STO = 3;
        public const int CAL = 4;
        public const int INT = 5;
        public const int JMP = 6;
        public const int JPC = 7;
        public const int RED = 8;
        public const int WRT = 9;

        private int addr;
        private int f;
        private int x;
        private int y;

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

        public int F
        {
            get
            {
                return f;
            }

            set
            {
                f = value;
            }
        }

        public int X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public PCode(int addr, int f, int x, int y)
        {
            this.f = f;
            this.x = x;
            this.y = y;
            this.addr = addr;
        }

        private String [] name = { "LIT", "OPR", "LOD", "STO", "CAL", "INT", "JMP", "JPC", "RED", "WRT" };
        public String PrintPCode()
        {
            Console.WriteLine("{0}\t{1}\t{2}\t{3}", addr, name[f], x, y);
            return addr + "\t" + name[f] + "\t" + x + "\t" + y;
        }
    }
}
