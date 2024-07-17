namespace OCRFiles
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            dataGridView1 = new DataGridView();
            FileName = new DataGridViewTextBoxColumn();
            NewFileName = new DataGridViewTextBoxColumn();
            FilePath = new DataGridViewTextBoxColumn();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(9, 10);
            button1.Margin = new Padding(2, 3, 2, 3);
            button1.Name = "button1";
            button1.Size = new Size(88, 32);
            button1.TabIndex = 0;
            button1.Text = "选择文件夹";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { FileName, NewFileName, FilePath });
            dataGridView1.Location = new Point(9, 48);
            dataGridView1.Margin = new Padding(2, 3, 2, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(604, 325);
            dataGridView1.TabIndex = 1;
            // 
            // FileName
            // 
            FileName.HeaderText = "文件名";
            FileName.MinimumWidth = 6;
            FileName.Name = "FileName";
            FileName.ReadOnly = true;
            FileName.Width = 150;
            // 
            // NewFileName
            // 
            NewFileName.HeaderText = "预测名称";
            NewFileName.MinimumWidth = 6;
            NewFileName.Name = "NewFileName";
            NewFileName.ReadOnly = true;
            NewFileName.Width = 150;
            // 
            // FilePath
            // 
            FilePath.HeaderText = "文件路径";
            FilePath.MinimumWidth = 6;
            FilePath.Name = "FilePath";
            FilePath.ReadOnly = true;
            FilePath.Width = 380;
            // 
            // button2
            // 
            button2.Location = new Point(102, 10);
            button2.Margin = new Padding(2, 3, 2, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 32);
            button2.TabIndex = 2;
            button2.Text = "配置";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(194, 10);
            button3.Margin = new Padding(2, 3, 2, 3);
            button3.Name = "button3";
            button3.Size = new Size(88, 32);
            button3.TabIndex = 3;
            button3.Text = "识别";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(287, 10);
            button4.Margin = new Padding(2, 3, 2, 3);
            button4.Name = "button4";
            button4.Size = new Size(88, 32);
            button4.TabIndex = 4;
            button4.Text = "重命名";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.Red;
            label1.Location = new Point(396, 18);
            label1.Name = "label1";
            label1.Size = new Size(167, 17);
            label1.TabIndex = 5;
            label1.Text = "作者：zgcwkj，免费禁止售卖";
            //label1.Visible = false;
            label1.Click += label1_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(622, 382);
            Controls.Add(label1);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(dataGridView1);
            Controls.Add(button1);
            Margin = new Padding(2, 3, 2, 3);
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "OCR 批量重命名（作者：zgcwkj）";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn FileName;
        private DataGridViewTextBoxColumn NewFileName;
        private DataGridViewTextBoxColumn FilePath;
        private Button button2;
        private Button button3;
        private Button button4;
        private Label label1;
    }
}
