namespace ComputerVision
{
    partial class MainForm
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
            this.panelSourceA = new System.Windows.Forms.Panel();
            this.panelSourceB = new System.Windows.Forms.Panel();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.openFileDialogA = new System.Windows.Forms.OpenFileDialog();
            this.buttonLoadB = new System.Windows.Forms.Button();
            this.blockmatchingBttn = new System.Windows.Forms.Button();
            this.openFileDialogB = new System.Windows.Forms.OpenFileDialog();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.outputPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelSourceA
            // 
            this.panelSourceA.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelSourceA.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelSourceA.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panelSourceA.Location = new System.Drawing.Point(12, 12);
            this.panelSourceA.Name = "panelSourceA";
            this.panelSourceA.Size = new System.Drawing.Size(320, 240);
            this.panelSourceA.TabIndex = 0;
            // 
            // panelSourceB
            // 
            this.panelSourceB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelSourceB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelSourceB.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panelSourceB.Location = new System.Drawing.Point(348, 12);
            this.panelSourceB.Name = "panelSourceB";
            this.panelSourceB.Size = new System.Drawing.Size(320, 240);
            this.panelSourceB.TabIndex = 1;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(12, 492);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 2;
            this.buttonLoad.Text = "Load A";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonLoadB
            // 
            this.buttonLoadB.Location = new System.Drawing.Point(102, 492);
            this.buttonLoadB.Name = "buttonLoadB";
            this.buttonLoadB.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadB.TabIndex = 3;
            this.buttonLoadB.Text = "Load B";
            this.buttonLoadB.UseVisualStyleBackColor = true;
            this.buttonLoadB.Click += new System.EventHandler(this.buttonLoadB_Click);
            // 
            // blockmatchingBttn
            // 
            this.blockmatchingBttn.Location = new System.Drawing.Point(213, 492);
            this.blockmatchingBttn.Name = "blockmatchingBttn";
            this.blockmatchingBttn.Size = new System.Drawing.Size(135, 23);
            this.blockmatchingBttn.TabIndex = 4;
            this.blockmatchingBttn.Text = "Block Match compute";
            this.blockmatchingBttn.UseVisualStyleBackColor = true;
            this.blockmatchingBttn.Click += new System.EventHandler(this.button1_Click);
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(719, 12);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(656, 494);
            this.richTextBox.TabIndex = 5;
            this.richTextBox.Text = "";
            // 
            // outputPanel
            // 
            this.outputPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.outputPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.outputPanel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.outputPanel.Location = new System.Drawing.Point(1408, 12);
            this.outputPanel.Name = "outputPanel";
            this.outputPanel.Size = new System.Drawing.Size(320, 240);
            this.outputPanel.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1846, 527);
            this.Controls.Add(this.outputPanel);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.blockmatchingBttn);
            this.Controls.Add(this.buttonLoadB);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.panelSourceB);
            this.Controls.Add(this.panelSourceA);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSourceA;
        private System.Windows.Forms.Panel panelSourceB;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialogA;
        private System.Windows.Forms.Button buttonLoadB;
        private System.Windows.Forms.Button blockmatchingBttn;
        private System.Windows.Forms.OpenFileDialog openFileDialogB;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Panel outputPanel;
    }
}

