namespace TortillaUI {
   partial class MainWindow {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing) {
         if (disposing && (components != null)) {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent() {
            this.registers = new System.Windows.Forms.TextBox();
            this.debug = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breakToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runButton = new System.Windows.Forms.Button();
            this.breakButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.resetButton = new System.Windows.Forms.Button();
            this.traceCheckBox = new System.Windows.Forms.CheckBox();
            this.memoryOutput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.startAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.endAddress = new System.Windows.Forms.TextBox();
            this.addressRangeError = new System.Windows.Forms.Label();
            this.stepCheckBox = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // registers
            // 
            this.registers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.registers.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.registers.Location = new System.Drawing.Point(13, 57);
            this.registers.Multiline = true;
            this.registers.Name = "registers";
            this.registers.ReadOnly = true;
            this.registers.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.registers.Size = new System.Drawing.Size(719, 113);
            this.registers.TabIndex = 0;
            // 
            // debug
            // 
            this.debug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.debug.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.debug.Location = new System.Drawing.Point(13, 295);
            this.debug.Multiline = true;
            this.debug.Name = "debug";
            this.debug.ReadOnly = true;
            this.debug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.debug.Size = new System.Drawing.Size(719, 396);
            this.debug.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(744, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.breakToolStripMenuItem,
            this.stepToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.resetToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.toolsToolStripMenuItem.Text = "E&mulator";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.runToolStripMenuItem.Text = "&Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // breakToolStripMenuItem
            // 
            this.breakToolStripMenuItem.Name = "breakToolStripMenuItem";
            this.breakToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.breakToolStripMenuItem.Text = "&Break";
            // 
            // stepToolStripMenuItem
            // 
            this.stepToolStripMenuItem.Name = "stepToolStripMenuItem";
            this.stepToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.stepToolStripMenuItem.Text = "S&tep";
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.stopToolStripMenuItem.Text = "&Stop";
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.resetToolStripMenuItem.Text = "R&eset";
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(94, 28);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 23);
            this.runButton.TabIndex = 3;
            this.runButton.Text = "&Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // breakButton
            // 
            this.breakButton.Location = new System.Drawing.Point(175, 28);
            this.breakButton.Name = "breakButton";
            this.breakButton.Size = new System.Drawing.Size(75, 23);
            this.breakButton.TabIndex = 4;
            this.breakButton.Text = "&Break";
            this.breakButton.UseVisualStyleBackColor = true;
            this.breakButton.Click += new System.EventHandler(this.breakButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(256, 28);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 6;
            this.stopButton.Text = "&Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(13, 28);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 7;
            this.resetButton.Text = "Start/R&eset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // traceCheckBox
            // 
            this.traceCheckBox.AutoSize = true;
            this.traceCheckBox.Checked = true;
            this.traceCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.traceCheckBox.Location = new System.Drawing.Point(421, 32);
            this.traceCheckBox.Name = "traceCheckBox";
            this.traceCheckBox.Size = new System.Drawing.Size(103, 17);
            this.traceCheckBox.TabIndex = 8;
            this.traceCheckBox.Text = "Tr&ace execution";
            this.traceCheckBox.UseVisualStyleBackColor = true;
            this.traceCheckBox.CheckedChanged += new System.EventHandler(this.traceCheckBox_CheckedChanged);
            // 
            // memoryOutput
            // 
            this.memoryOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.memoryOutput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memoryOutput.Location = new System.Drawing.Point(13, 200);
            this.memoryOutput.Multiline = true;
            this.memoryOutput.Name = "memoryOutput";
            this.memoryOutput.ReadOnly = true;
            this.memoryOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.memoryOutput.Size = new System.Drawing.Size(719, 89);
            this.memoryOutput.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Start:";
            // 
            // startAddress
            // 
            this.startAddress.Location = new System.Drawing.Point(51, 174);
            this.startAddress.Name = "startAddress";
            this.startAddress.Size = new System.Drawing.Size(100, 20);
            this.startAddress.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(157, 177);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "End:";
            // 
            // endAddress
            // 
            this.endAddress.Location = new System.Drawing.Point(192, 174);
            this.endAddress.Name = "endAddress";
            this.endAddress.Size = new System.Drawing.Size(100, 20);
            this.endAddress.TabIndex = 13;
            // 
            // addressRangeError
            // 
            this.addressRangeError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addressRangeError.ForeColor = System.Drawing.Color.Red;
            this.addressRangeError.Location = new System.Drawing.Point(298, 177);
            this.addressRangeError.Name = "addressRangeError";
            this.addressRangeError.Size = new System.Drawing.Size(434, 17);
            this.addressRangeError.TabIndex = 14;
            this.addressRangeError.Text = "Error text here";
            this.addressRangeError.Visible = false;
            // 
            // stepCheckBox
            // 
            this.stepCheckBox.AutoSize = true;
            this.stepCheckBox.Location = new System.Drawing.Point(337, 32);
            this.stepCheckBox.Name = "stepCheckBox";
            this.stepCheckBox.Size = new System.Drawing.Size(78, 17);
            this.stepCheckBox.TabIndex = 15;
            this.stepCheckBox.Text = "S&ingle step";
            this.stepCheckBox.UseVisualStyleBackColor = true;
            this.stepCheckBox.CheckedChanged += new System.EventHandler(this.stepCheckBox_CheckedChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 703);
            this.Controls.Add(this.stepCheckBox);
            this.Controls.Add(this.addressRangeError);
            this.Controls.Add(this.endAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.startAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.memoryOutput);
            this.Controls.Add(this.traceCheckBox);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.breakButton);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.debug);
            this.Controls.Add(this.registers);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Tortilla CPU Emulator";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.SizeChanged += new System.EventHandler(this.MainWindow_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox registers;
      private System.Windows.Forms.TextBox debug;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem breakToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.Button breakButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.CheckBox traceCheckBox;
        private System.Windows.Forms.TextBox memoryOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox startAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox endAddress;
        private System.Windows.Forms.Label addressRangeError;
        private System.Windows.Forms.CheckBox stepCheckBox;
    }
}

