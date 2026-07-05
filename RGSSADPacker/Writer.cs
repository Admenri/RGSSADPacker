using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGSSADPacker
{
  public struct FileEntry
  {
    public string fileName;
    public string fullPath;
  }

  public abstract class Writer
  {
    protected BinaryWriter outWriter;

    protected Writer(BinaryWriter writer)
    {
      outWriter = writer;
    }

    public abstract void WritePackage(List<FileEntry> files);

    public Action<float, string> OnPackingProcessChanged;
  }
}
