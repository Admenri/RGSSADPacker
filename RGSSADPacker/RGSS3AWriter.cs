using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGSSADPacker
{
  public class RGSS3AWriter : Writer
  {
    private Random random;

    public RGSS3AWriter(BinaryWriter writer) : base(writer)
    {
      random = new Random();
    }

    private UInt32 GenerateMagicKey()
    {
      return (UInt32)random.Next();
    }

    public override void WritePackage(List<FileEntry> files)
    {
      OnPackingProcessChanged?.Invoke(0f, "Writing Header...");

      // 8 bytes header
      byte[] header = { (byte)'R', (byte)'G', (byte)'S', (byte)'S', (byte)'A', (byte)'D', (byte)0x00, (byte)0x03 };
      outWriter.Write(header);

      // 4 bytes basic key
      UInt32 basicKey = GenerateMagicKey(), magicKey = basicKey * 9 + 3;
      outWriter.Write(basicKey);

      // Calculate header total bytes
      int headerTotalBytes = 0;
      foreach (FileEntry file in files)
      {
        byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(file.fileName);
        headerTotalBytes += 4 * 4;
        headerTotalBytes += fileNameBytes.Length;
      }

      // Write header and file data
      int headerChunkOffset = 8 + 4;
      int dataChunkOffset = 8 + 4 + headerTotalBytes + 4 * 4;

      int currentFileIndex = 0;
      foreach (FileEntry file in files)
      {
        OnPackingProcessChanged?.Invoke(currentFileIndex / (float)files.Count, file.fileName);
        currentFileIndex++;

        UInt32 dataMagic = GenerateMagicKey();
        using (FileStream fileStream = File.OpenRead(file.fullPath))
        {
          byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(file.fileName);

          outWriter.Seek(headerChunkOffset, SeekOrigin.Begin);
          headerChunkOffset += 4 * 4 + fileNameBytes.Length;

          // 1. offset
          outWriter.Write((Int32)(dataChunkOffset ^ magicKey));
          // 2. dataSize
          outWriter.Write((Int32)(fileStream.Length ^ magicKey));
          // 3. dataMagic
          outWriter.Write((Int32)(dataMagic ^ magicKey));
          // 4. fileNameSize
          outWriter.Write((Int32)(fileNameBytes.Length ^ magicKey));
          // 5. fileName
          for (int i = 0; i < fileNameBytes.Length; ++i)
            outWriter.Write((byte)(fileNameBytes[i] ^ (byte)(0xFF & (magicKey >> 8 * (i % 4)))));

          // 6. dataChunk
          outWriter.Seek(dataChunkOffset, SeekOrigin.Begin);
          dataChunkOffset += (int)fileStream.Length;

          UInt32 dataKey = dataMagic;
          for (UInt32 i = 0; i < fileStream.Length; ++i)
          {
            byte data = (byte)fileStream.ReadByte();

            int subCount = (int)(i % 4);
            outWriter.Write((byte)(data ^ (byte)(dataKey >> 8 * subCount)));

            if (subCount == 3)
              dataKey = dataKey * 7 + 3;
          }
        }
      }

      // Header End Marker
      outWriter.Seek(8 + 4 + headerTotalBytes, SeekOrigin.Begin);
      outWriter.Write(magicKey);
      outWriter.Write(magicKey);
      outWriter.Write(magicKey);
      outWriter.Write(magicKey);
    }
  }
}
