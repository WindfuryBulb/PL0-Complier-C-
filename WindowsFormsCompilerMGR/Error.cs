using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCompilerMGR
{
    class Error
    {
        private String error_msg;
        private int line;
        public Error(String error_msg, int line)
        {
            this.error_msg = error_msg;
            this.line = line;
        } 
        public void ErrorMsgSend()
        {           
            Form1.ShowError(error_msg,line);
        }
    }
}
