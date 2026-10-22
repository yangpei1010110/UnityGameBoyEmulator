using System;
using System.IO;
using System.Text;
using GameBoy.Emulators.Common;
using GameBoy.Emulators.Common.Opcodes;

namespace GbHeadless
{
    static class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("用法: GbHeadless <rom> [秒数] [ppm输出]");
                return 2;
            }

            string path = args[0];
            double seconds = args.Length > 1 ? double.Parse(args[1]) : 20;
            string ppm = args.Length > 2 ? args[2] : null;
            byte[] rom = File.ReadAllBytes(path);
            var cpu = new Cpu();
            cpu.Mount(rom, path);
            long budget = (long)(Cpu.CLOCK_SPEED_UNSCALED * seconds);
            long ran = 0;
            int serialCount = 0;
            var text = new StringBuilder();
            string result = "timeout";
            while (ran < budget)
            {
                ran += Sm83.Step(cpu);
                while (serialCount < cpu.Serial.output_buffer.Count)
                {
                    byte value = At(cpu, serialCount++);
                    text.Append((char)value);
                }

                string sofar = text.ToString();
                if (sofar.Contains("Passed") || sofar.Contains("Failed"))
                {
                    result = sofar.Contains("Failed") ? "failed" : "passed";
                    break;
                }
            }

            Console.WriteLine($"result={result}");
            Console.WriteLine($"cycles={ran} pc={cpu.ProgramCounter:X4} frames={cpu.Ppu.FrameCount}");
            Console.WriteLine(text.ToString());
            if (!string.IsNullOrEmpty(ppm))
            {
                WritePpm(ppm, cpu.Ppu.Framebuffer, 160, 144);
                Console.WriteLine($"ppm={ppm}");
            }

            return result == "passed" ? 0 : 1;
        }

        static byte At(Cpu cpu, int index)
        {
            int i = 0;
            foreach (byte value in cpu.Serial.output_buffer)
            {
                if (i++ == index)
                {
                    return value;
                }
            }

            return 0;
        }

        static void WritePpm(string path, byte[] frame, int width, int height)
        {
            using var stream = File.Create(path);
            using var writer = new StreamWriter(stream);
            writer.Write($"P6\n{width} {height}\n255\n");
            writer.Flush();
            for (int i = 0; i < width * height; i++)
            {
                stream.WriteByte(frame[i * 4]);
                stream.WriteByte(frame[i * 4 + 1]);
                stream.WriteByte(frame[i * 4 + 2]);
            }
        }
    }
}
