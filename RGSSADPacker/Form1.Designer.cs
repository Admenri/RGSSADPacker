namespace RGSSADPacker
{
    partial class MainForm
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
      treeView_files = new TreeView();
      button_pack = new Button();
      comboBox_type = new ComboBox();
      button_addDir = new Button();
      SuspendLayout();
      // 
      // treeView_files
      // 
      treeView_files.Location = new Point(12, 12);
      treeView_files.Name = "treeView_files";
      treeView_files.Size = new Size(920, 562);
      treeView_files.TabIndex = 0;
      treeView_files.MouseUp += treeView_files_MouseUp;
      // 
      // button_pack
      // 
      button_pack.Location = new Point(781, 589);
      button_pack.Name = "button_pack";
      button_pack.Size = new Size(151, 46);
      button_pack.TabIndex = 1;
      button_pack.Text = "Pack";
      button_pack.UseVisualStyleBackColor = true;
      button_pack.Click += button_pack_Click;
      // 
      // comboBox_type
      // 
      comboBox_type.FormattingEnabled = true;
      comboBox_type.ItemHeight = 24;
      comboBox_type.Items.AddRange(new object[] { "RGSSAD (XP/VX)", "RGSS3A (VXA)" });
      comboBox_type.Location = new Point(553, 597);
      comboBox_type.Name = "comboBox_type";
      comboBox_type.Size = new Size(207, 32);
      comboBox_type.TabIndex = 2;
      // 
      // button_addDir
      // 
      button_addDir.Location = new Point(12, 589);
      button_addDir.Name = "button_addDir";
      button_addDir.Size = new Size(158, 46);
      button_addDir.TabIndex = 3;
      button_addDir.Text = "Add";
      button_addDir.UseVisualStyleBackColor = true;
      button_addDir.Click += button_addDir_Click;
      // 
      // MainForm
      // 
      AutoScaleDimensions = new SizeF(11F, 24F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(944, 647);
      Controls.Add(button_addDir);
      Controls.Add(comboBox_type);
      Controls.Add(button_pack);
      Controls.Add(treeView_files);
      FormBorderStyle = FormBorderStyle.FixedDialog;
      MaximizeBox = false;
      Name = "MainForm";
      Text = "RGSSAD Packer - AEP260705";
      ResumeLayout(false);
    }

    #endregion

    private TreeView treeView_files;
    private Button button_pack;
    private ComboBox comboBox_type;
    private Button button_addDir;
  }
}
