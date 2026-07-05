namespace RGSSADPacker
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private void button_addDir_Click(object sender, EventArgs e)
    {
      using (FolderBrowserDialog dialog = new FolderBrowserDialog())
      {
        dialog.ShowNewFolderButton = true;

        if (dialog.ShowDialog() == DialogResult.OK)
        {
          string selectedPath = dialog.SelectedPath;
          AssembleDirectoryIntoTreeView(selectedPath);
        }
      }
    }

    private void button_pack_Click(object sender, EventArgs e)
    {
      using (SaveFileDialog dialog = new SaveFileDialog())
      {
        dialog.FileName = comboBox_type.SelectedIndex == 1 ? "Game.rgss3a" : "Game.rgssad";
        dialog.Filter = "All Files (*.*)|*.*";

        if (dialog.ShowDialog() == DialogResult.OK)
        {
          ExecuteRGSSADPack(comboBox_type.SelectedIndex == 1, dialog.FileName);
        }
      }
    }

    private void AssembleDirectoryIntoTreeView(string targetDir)
    {
      DirectoryInfo rootDir = new DirectoryInfo(targetDir);

      treeView_files.Nodes.Clear();

      TreeNode rootNode = new TreeNode(rootDir.Name)
      {
        Tag = rootDir.FullName,
        ImageKey = "folder",
        SelectedImageKey = "folder"
      };
      treeView_files.Nodes.Add(rootNode);

      Action<DirectoryInfo, TreeNode>? autoAdder = null;
      autoAdder = (DirectoryInfo dir, TreeNode parentNode) =>
      {
        foreach (var subDir in dir.GetDirectories())
        {
          try
          {
            TreeNode dirNode = new TreeNode(subDir.Name)
            {
              Tag = subDir.FullName,
              ImageKey = "folder",
              SelectedImageKey = "folder"
            };
            parentNode.Nodes.Add(dirNode);

            autoAdder?.Invoke(subDir, dirNode);
          }
          catch (UnauthorizedAccessException)
          {
            TreeNode errorNode = new TreeNode($"{subDir.Name} (No Auth)")
            {
              Tag = subDir.FullName,
              ForeColor = System.Drawing.Color.Gray
            };
            parentNode.Nodes.Add(errorNode);
          }
        }

        foreach (var file in dir.GetFiles())
        {
          try
          {
            TreeNode fileNode = new TreeNode(file.Name)
            {
              Tag = file.FullName,
              ImageKey = "file",
              SelectedImageKey = "file"
            };
            parentNode.Nodes.Add(fileNode);
          }
          catch (UnauthorizedAccessException)
          {
            TreeNode errorNode = new TreeNode($"{file.Name} (No Auth)")
            {
              Tag = file.FullName,
              ForeColor = System.Drawing.Color.Gray
            };
            parentNode.Nodes.Add(errorNode);
          }
        }
      };

      autoAdder?.Invoke(rootDir, rootNode);
      rootNode.Expand();
    }

    private void treeView_files_MouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        TreeNode clickedNode = treeView_files.GetNodeAt(e.X, e.Y);

        if (clickedNode != null)
        {
          treeView_files.SelectedNode = clickedNode;

          ContextMenuStrip contextMenu = new ContextMenuStrip();

          ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("Remove");
          deleteMenuItem.Image = System.Drawing.SystemIcons.Warning.ToBitmap();
          deleteMenuItem.Click += DeleteMenuItem_Click;
          contextMenu.Items.Add(deleteMenuItem);

          ToolStripMenuItem expandAllMenuItem = new ToolStripMenuItem("Expand All");
          expandAllMenuItem.Click += (s, args) => clickedNode.ExpandAll();
          contextMenu.Items.Add(expandAllMenuItem);

          ToolStripMenuItem collapseAllMenuItem = new ToolStripMenuItem("Collapse All");
          collapseAllMenuItem.Click += (s, args) => clickedNode.Collapse();
          contextMenu.Items.Add(collapseAllMenuItem);

          contextMenu.Show(treeView_files, e.Location);
        }
      }
    }

    private void DeleteMenuItem_Click(object? sender, EventArgs e)
    {
      if (treeView_files.SelectedNode != null)
      {
        TreeNode nodeToDelete = treeView_files.SelectedNode;

        RemoveTreeNode(nodeToDelete);
      }
    }

    private void RemoveTreeNode(TreeNode node)
    {
      if (node != null)
      {
        TreeNode parent = node.Parent;

        if (parent != null)
        {
          parent.Nodes.Remove(node);
        }
        else
        {
          treeView_files.Nodes.Remove(node);
        }
      }
    }

    private List<FileEntry> ConvertTreeViewToFileEntries()
    {
      List<FileEntry> fileEntries = new List<FileEntry>();

      if (treeView_files.Nodes.Count > 0)
      {
        TreeNode rootNode = treeView_files.Nodes[0];
        string rootPath = rootNode.Tag?.ToString() ?? string.Empty;

        CollectFileEntries(rootNode, rootPath, string.Empty, fileEntries);
      }

      return fileEntries;
    }

    private void CollectFileEntries(TreeNode node, string rootPath, string relativePath, List<FileEntry> fileEntries)
    {
      foreach (TreeNode childNode in node.Nodes)
      {
        if (childNode.Tag != null)
        {
          string fullPath = childNode.Tag.ToString();

          if (IsFileNode(childNode))
          {
            string relativeFilePath = GetRelativePath(rootPath, fullPath);

            FileEntry entry = new FileEntry
            {
              fileName = relativeFilePath,
              fullPath = fullPath
            };

            fileEntries.Add(entry);
          }
          else
          {
            string currentRelativePath = GetRelativePath(rootPath, fullPath);
            CollectFileEntries(childNode, rootPath, currentRelativePath, fileEntries);
          }
        }
      }
    }

    private bool IsFileNode(TreeNode node)
    {
      if (node.Tag != null)
      {
        string path = node.Tag.ToString();
        return File.Exists(path);
      }

      return node.Nodes.Count == 0;
    }

    private string GetRelativePath(string rootPath, string fullPath)
    {
      if (string.IsNullOrEmpty(rootPath) || string.IsNullOrEmpty(fullPath))
        return fullPath;

      if (!rootPath.EndsWith("\\"))
        rootPath += "\\";

      if (fullPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
      {
        string relativePath = fullPath.Substring(rootPath.Length);
        return relativePath;
      }

      return Path.GetFileName(fullPath);
    }

    private void ExecuteRGSSADPack(bool rgss3a, string savePath)
    {
      button_pack.Enabled = false;

      List<FileEntry> fileEntries = ConvertTreeViewToFileEntries();

      ProcessForm processForm = new ProcessForm();
      processForm.Show(this);

      Thread packThread = new Thread(() =>
      {
        try
        {
          using (FileStream outStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
          using (BinaryWriter writer = new BinaryWriter(outStream))
          {
            Writer packer = rgss3a ? new RGSS3AWriter(writer) : new RGSSADWriter(writer);

            packer.OnPackingProcessChanged = (progress, message) =>
            {
              if (processForm.InvokeRequired)
              {
                processForm.Invoke(new Action(() =>
                {
                  processForm.ProgressBarControl.Value = (int)(progress * 100);
                  processForm.TextBoxControl.Text = message;
                }));
              }
              else
              {
                processForm.ProgressBarControl.Value = (int)(progress * 100);
                processForm.TextBoxControl.Text = message;
              }
            };

            packer.WritePackage(fileEntries);
          }
        }
        catch (Exception ex)
        {
          processForm.Invoke(new Action(() =>
          {
            MessageBox.Show(processForm, $"Error: {ex.Message}", "Failure",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
          }));
        }
        finally
        {
          processForm.Invoke(new Action(() =>
          {
            processForm.Close();
            button_pack.Enabled = true;
          }));
        }
      });

      packThread.IsBackground = true;
      packThread.Start();
    }
  }
}
