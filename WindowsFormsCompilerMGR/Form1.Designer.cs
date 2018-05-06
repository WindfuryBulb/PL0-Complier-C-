namespace WindowsFormsCompilerMGR
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBoxForCoding = new System.Windows.Forms.RichTextBox();
            this.compileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBoxForCoding
            // 
            this.richTextBoxForCoding.Location = new System.Drawing.Point(28, 21);
            this.richTextBoxForCoding.Name = "richTextBoxForCoding";
            this.richTextBoxForCoding.Size = new System.Drawing.Size(397, 283);
            this.richTextBoxForCoding.TabIndex = 0;
            this.richTextBoxForCoding.Text = "";
            this.richTextBoxForCoding.TextChanged += new System.EventHandler(this.richTextBoxForCoding_TextChanged);
            // 
            // compileButton
            // 
            this.compileButton.Location = new System.Drawing.Point(300, 319);
            this.compileButton.Name = "compileButton";
            this.compileButton.Size = new System.Drawing.Size(124, 29);
            this.compileButton.TabIndex = 1;
            this.compileButton.Text = "编译";
            this.compileButton.UseVisualStyleBackColor = true;
            this.compileButton.Click += new System.EventHandler(this.compileButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 367);
            this.Controls.Add(this.compileButton);
            this.Controls.Add(this.richTextBoxForCoding);
            this.Name = "Form1";
            this.Text = "PL/0 Complier - MAGRY 20170111";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxForCoding;
        private System.Windows.Forms.Button compileButton;
    }
}

