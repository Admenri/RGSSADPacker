using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RGSSADPacker
{
  public partial class ProcessForm : Form
  {
    public ProcessForm()
    {
      InitializeComponent();
    }

    public ProgressBar ProgressBarControl
    {
      get { return progressBar1; }
    }

    public TextBox TextBoxControl
    {
      get { return textBox1; }
    }
  }
}
