using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGSSADPacker
{
  public class RGSSADWriter : Writer
  {
    public RGSSADWriter(BinaryWriter writer) : base(writer) { }

    public override void WritePackage(List<FileEntry> files)
    {
      OnPackingProcessChanged?.Invoke(0f, "Writing Header...");

      // 8 bytes header
      byte[] header = { (byte)'R', (byte)'G', (byte)'S', (byte)'S', (byte)'A', (byte)'D', (byte)0x00, (byte)0x01 };
      outWriter.Write(header);

      // struct:
      //   uint32 fileNameLength
      //   [N] fileName
      //   uint32 fileLength
      //   [N] fileData
      int currentFileIndex = 0;
      foreach (FileEntry file in files)
      {
        OnPackingProcessChanged?.Invoke(currentFileIndex / (float)files.Count, file.fileName);
        currentFileIndex++;

        UInt32 magicKey = 0xDEADCAFE;
        byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(file.fileName);

        // 1. 4bytes fileNameLength
        outWriter.Write((UInt32)(fileNameBytes.Length ^ magicKey));
        magicKey = magicKey * 7 + 3;

        // 2. [N] fileName
        foreach (byte b in fileNameBytes)
        {
          outWriter.Write((byte)(b ^ (byte)(magicKey & 0xFF)));
          magicKey = magicKey * 7 + 3;
        }

        using (FileStream fileStream = File.OpenRead(file.fullPath))
        {
          // 3. 4bytes fileLength
          outWriter.Write((UInt32)(fileStream.Length ^ magicKey));
          magicKey = magicKey * 7 + 3;

          // 4. [N] fileData
          UInt32 dataKey = magicKey;
          for (UInt32 i = 0; i < fileStream.Length; ++i)
          {
            byte data = (byte)fileStream.ReadByte();

            int subCount = (int)(i % 4);
            outWriter.Write((byte)(data ^ (byte)(dataKey >> 8 * subCount)));

            if (subCount == 3)
              dataKey = dataKey * 7 + 3;
          }
        }

        Debug.WriteLine(file.fileName);
      }
    }
  }
}
