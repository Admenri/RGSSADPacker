namespace RGSSADPacker
{
  partial class ProcessForm
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
      progressBar1 = new ProgressBar();
      textBox1 = new TextBox();
      SuspendLayout();
      // 
      // progressBar1
      // 
      progressBar1.Location = new Point(12, 42);
      progressBar1.Name = "progressBar1";
      progressBar1.Size = new Size(474, 36);
      progressBar1.TabIndex = 0;
      // 
      // textBox1
      // 
      textBox1.BackColor = SystemColors.Menu;
      textBox1.BorderStyle = BorderStyle.None;
      textBox1.Enabled = false;
      textBox1.Location = new Point(12, 94);
      textBox1.Name = "textBox1";
      textBox1.Size = new Size(474, 23);
      textBox1.TabIndex = 2;
      textBox1.Text = "C:/to/file";
      textBox1.TextAlign = HorizontalAlignment.Center;
      // 
      // ProcessForm
      // 
      AutoScaleDimensions = new SizeF(11F, 24F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(498, 169);
      Controls.Add(textBox1);
      Controls.Add(progressBar1);
      MaximizeBox = false;
      MinimizeBox = false;
      Name = "ProcessForm";
      Text = "Processing...";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private ProgressBar progressBar1;
    private TextBox textBox1;
  }
}