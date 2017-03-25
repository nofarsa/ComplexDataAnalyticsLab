namespace DiscoStation
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.browse_btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.methodBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.class_box = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bins_box = new System.Windows.Forms.TextBox();
            this.run_btn = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.fname_lbl = new System.Windows.Forms.Label();
            this.vmap_btn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(187, 218);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // browse_btn
            // 
            this.browse_btn.Location = new System.Drawing.Point(12, 7);
            this.browse_btn.Name = "browse_btn";
            this.browse_btn.Size = new System.Drawing.Size(75, 23);
            this.browse_btn.TabIndex = 1;
            this.browse_btn.Text = "Browse File";
            this.browse_btn.UseVisualStyleBackColor = true;
            this.browse_btn.Click += new System.EventHandler(this.browse_btn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Abstraction method:";
            // 
            // methodBox
            // 
            this.methodBox.FormattingEnabled = true;
            this.methodBox.Items.AddRange(new object[] {
            "SAX",
            "EQW"});
            this.methodBox.Location = new System.Drawing.Point(120, 39);
            this.methodBox.Name = "methodBox";
            this.methodBox.Size = new System.Drawing.Size(121, 21);
            this.methodBox.TabIndex = 3;
            this.methodBox.SelectedIndexChanged += new System.EventHandler(this.methodBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Class separator:";
            // 
            // class_box
            // 
            this.class_box.Location = new System.Drawing.Point(120, 64);
            this.class_box.Name = "class_box";
            this.class_box.Size = new System.Drawing.Size(25, 20);
            this.class_box.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Bins number:";
            // 
            // bins_box
            // 
            this.bins_box.Location = new System.Drawing.Point(120, 86);
            this.bins_box.Name = "bins_box";
            this.bins_box.Size = new System.Drawing.Size(25, 20);
            this.bins_box.TabIndex = 7;
            // 
            // run_btn
            // 
            this.run_btn.Location = new System.Drawing.Point(12, 218);
            this.run_btn.Name = "run_btn";
            this.run_btn.Size = new System.Drawing.Size(75, 23);
            this.run_btn.TabIndex = 8;
            this.run_btn.Text = "Run";
            this.run_btn.UseVisualStyleBackColor = true;
            this.run_btn.Click += new System.EventHandler(this.run_btn_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(16, 123);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(135, 17);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Abstracted Time Series";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // fname_lbl
            // 
            this.fname_lbl.AutoSize = true;
            this.fname_lbl.Location = new System.Drawing.Point(120, 12);
            this.fname_lbl.Name = "fname_lbl";
            this.fname_lbl.Size = new System.Drawing.Size(80, 13);
            this.fname_lbl.TabIndex = 10;
            this.fname_lbl.Text = "No file selected";
            // 
            // vmap_btn
            // 
            this.vmap_btn.Location = new System.Drawing.Point(16, 147);
            this.vmap_btn.Name = "vmap_btn";
            this.vmap_btn.Size = new System.Drawing.Size(129, 23);
            this.vmap_btn.TabIndex = 11;
            this.vmap_btn.Text = "Browse Variable Map";
            this.vmap_btn.UseVisualStyleBackColor = true;
            this.vmap_btn.Click += new System.EventHandler(this.vmap_btn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 180);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Dataset Name:";
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(100, 177);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(100, 20);
            this.nameBox.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.vmap_btn);
            this.Controls.Add(this.fname_lbl);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.run_btn);
            this.Controls.Add(this.bins_box);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.class_box);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.methodBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.browse_btn);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button browse_btn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox methodBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox class_box;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox bins_box;
        private System.Windows.Forms.Button run_btn;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label fname_lbl;
        private System.Windows.Forms.Button vmap_btn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox nameBox;
    }
}

