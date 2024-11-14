using System.IO;
using UnityEngine;

public static class SavWav
{
    public static void Save(string filePath, AudioClip clip)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // Write the header
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(0); // Placeholder for file size
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
                writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16); // Format chunk size
                writer.Write((short)1); // Format type (PCM)
                writer.Write((short)clip.channels); // Number of channels
                writer.Write(clip.frequency); // Sample rate
                writer.Write(clip.frequency * clip.channels * 2); // Byte rate
                writer.Write((short)(clip.channels * 2)); // Block align
                writer.Write((short)16); // Bits per sample
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                writer.Write(0); // Placeholder for data size

                // Write audio data
                float[] samples = new float[clip.samples * clip.channels];
                clip.GetData(samples, 0);
                byte[] byteData = new byte[samples.Length * 2]; // 16-bit audio (2 bytes per sample)
                int rescaleFactor = 32767; // Convert float samples to Int16

                for (int i = 0; i < samples.Length; i++)
                {
                    short sample = (short)(samples[i] * rescaleFactor);
                    byteData[i * 2] = (byte)(sample & 0xFF); // Low byte
                    byteData[i * 2 + 1] = (byte)((sample >> 8) & 0xFF); // High byte
                }

                writer.Write(byteData); // Write the byte array

                // Update file size placeholders
                writer.Seek(4, SeekOrigin.Begin);
                writer.Write((int)(fs.Length - 8)); // Size of file minus RIFF identifier
                writer.Seek(40, SeekOrigin.Begin);
                writer.Write((int)(fs.Length - 44)); // Size of data section
            }
        }
    }
}
