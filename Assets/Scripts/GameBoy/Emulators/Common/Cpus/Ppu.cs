using System;
using GameBoy.Emulators.Common.Opcodes;

namespace GameBoy.Emulators.Common.Cpus
{
    public class Ppu
    {
        public enum Mode : byte
        {
            HBLANK  = 0,
            VBLANK  = 1,
            OAM     = 2,
            DRAWING = 3,
        }

        public const ushort LINES_PER_FRAME = 154;
        public const ushort CYCLES_PER_LINE = 456;
        public const ushort SCREEN_WIDTH    = 160;
        public const ushort SCREEN_HEIGHT   = 144;
        const int OamCycles = 80;
        const int DrawCycles = 172;

        static readonly byte[] Shade = { 255, 170, 85, 0 };

        public byte bgp;
        public byte dma;
        public byte lcdc;
        public byte lcds;
        public uint lineCycles;
        public byte ly;
        public byte lyc;
        public byte obp0;
        public byte obp1;
        public byte scroll_x;
        public byte scroll_y;
        public byte wx;
        public byte wy;
        public int FrameCount;
        public int WindowLine;
        public readonly byte[] Framebuffer = new byte[SCREEN_WIDTH * SCREEN_HEIGHT * 4];

        public Ppu()
        {
            lcdc = 0x91;
            lcds = 0x00;
            scroll_y = 0x00;
            scroll_x = 0x00;
            ly = 0x00;
            lyc = 0x00;
            dma = 0x00;
            bgp = 0xFC;
            obp0 = 0xFF;
            obp1 = 0xFF;
            wy = 0x00;
            wx = 0x00;
            CurrentMode = Mode.OAM;
            lineCycles = 0;
            SetLycFlag();
            Clear(255);
        }

        public bool Enabled => (lcdc & (1 << 7)) != 0;
        public Mode CurrentMode
        {
            get => (Mode)(lcds & 0x03);
            set
            {
                lcds &= 0xFC;
                lcds |= (byte)value;
            }
        }

        public void SetLycFlag()     => lcds |= 1 << 2;
        public void ResetLycFlag()   => lcds = (byte)(lcds & ~(1 << 2));
        public bool HBlankIntEnabled => (lcds & (1 << 3)) != 0;
        public bool VBlankIntEnabled => (lcds & (1 << 4)) != 0;
        public bool OamIntEnabled    => (lcds & (1 << 5)) != 0;
        public bool LycIntEnabled    => (lcds & (1 << 6)) != 0;
        public bool BgEnabled        => (lcdc & 0x01) != 0;
        public bool ObjEnabled       => (lcdc & 0x02) != 0;
        public bool ObjTall          => (lcdc & 0x04) != 0;
        public bool WindowEnabled    => (lcdc & 0x20) != 0;

        public static void PpuTick(Cpu cpu)
        {
            Ppu ppu = cpu.Ppu;
            if (!ppu.Enabled)
            {
                return;
            }

            ppu.lineCycles++;
            if (ppu.ly < SCREEN_HEIGHT)
            {
                if (ppu.lineCycles == OamCycles)
                {
                    ppu.CurrentMode = Mode.DRAWING;
                }
                else if (ppu.lineCycles == OamCycles + DrawCycles)
                {
                    ppu.RenderScanline(cpu);
                    ppu.CurrentMode = Mode.HBLANK;
                    if (ppu.HBlankIntEnabled)
                    {
                        cpu._intFlags |= CpuOp.INT_LCD_STAT;
                    }
                }
            }

            if (ppu.lineCycles < CYCLES_PER_LINE)
            {
                return;
            }

            ppu.NextLine(cpu);
        }

        public void PowerOff()
        {
            ly = 0;
            lineCycles = 0;
            CurrentMode = Mode.HBLANK;
            Clear(255);
        }

        public void PowerOn()
        {
            ly = 0;
            lineCycles = 0;
            WindowLine = 0;
            CurrentMode = Mode.OAM;
            UpdateLyc(null);
        }

        void NextLine(Cpu cpu)
        {
            lineCycles = 0;
            ly++;
            if (ly == SCREEN_HEIGHT)
            {
                CurrentMode = Mode.VBLANK;
                cpu._intFlags |= CpuOp.INT_VBLANK;
                if (VBlankIntEnabled)
                {
                    cpu._intFlags |= CpuOp.INT_LCD_STAT;
                }
            }
            else if (ly >= LINES_PER_FRAME)
            {
                ly = 0;
                WindowLine = 0;
                FrameCount++;
                CurrentMode = Mode.OAM;
                if (OamIntEnabled)
                {
                    cpu._intFlags |= CpuOp.INT_LCD_STAT;
                }
            }
            else if (ly < SCREEN_HEIGHT)
            {
                CurrentMode = Mode.OAM;
                if (OamIntEnabled)
                {
                    cpu._intFlags |= CpuOp.INT_LCD_STAT;
                }
            }

            UpdateLyc(cpu);
        }

        void UpdateLyc(Cpu cpu)
        {
            if (ly == lyc)
            {
                SetLycFlag();
                if (cpu != null && LycIntEnabled)
                {
                    cpu._intFlags |= CpuOp.INT_LCD_STAT;
                }
            }
            else
            {
                ResetLycFlag();
            }
        }

        void Clear(byte shade)
        {
            for (int i = 0; i < Framebuffer.Length; i += 4)
            {
                Framebuffer[i] = shade;
                Framebuffer[i + 1] = shade;
                Framebuffer[i + 2] = shade;
                Framebuffer[i + 3] = 255;
            }
        }

        void RenderScanline(Cpu cpu)
        {
            int line = ly;
            if (line >= SCREEN_HEIGHT)
            {
                return;
            }

            Span<byte> bg = stackalloc byte[SCREEN_WIDTH];
            bool windowDrawn = false;
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                byte colorId = 0;
                if (BgEnabled)
                {
                    colorId = BackgroundColor(cpu, x, line);
                }

                if (WindowEnabled && line >= wy && x + 7 >= wx)
                {
                    colorId = WindowColor(cpu, x);
                    windowDrawn = true;
                }

                bg[x] = colorId;
                WritePixel(line, x, PaletteColor(bgp, colorId));
            }

            if (windowDrawn)
            {
                WindowLine++;
            }

            if (ObjEnabled)
            {
                DrawSprites(cpu, line, bg);
            }
        }

        byte BackgroundColor(Cpu cpu, int x, int line)
        {
            int map = (lcdc & 0x08) != 0 ? 0x9C00 : 0x9800;
            int pixelY = (line + scroll_y) & 0xFF;
            int pixelX = (x + scroll_x) & 0xFF;
            return TileColor(cpu, map, pixelX, pixelY);
        }

        byte WindowColor(Cpu cpu, int x)
        {
            int map = (lcdc & 0x40) != 0 ? 0x9C00 : 0x9800;
            int pixelX = x - (wx - 7);
            if (pixelX < 0)
            {
                return 0;
            }

            return TileColor(cpu, map, pixelX, WindowLine);
        }

        byte TileColor(Cpu cpu, int map, int pixelX, int pixelY)
        {
            int tileX = (pixelX >> 3) & 31;
            int tileY = (pixelY >> 3) & 31;
            int tileId = Vram(cpu, map + tileY * 32 + tileX);
            int row = pixelY & 7;
            int tileAddr = (lcdc & 0x10) != 0
                ? 0x8000 + tileId * 16
                : 0x9000 + (sbyte)tileId * 16;
            return Decode(cpu, tileAddr, row, 7 - (pixelX & 7), false);
        }

        void DrawSprites(Cpu cpu, int line, Span<byte> bg)
        {
            int height = ObjTall ? 16 : 8;
            int drawn = 0;
            Span<int> indexes = stackalloc int[10];
            for (int i = 0; i < 40 && drawn < 10; i++)
            {
                int spriteY = cpu.OAM[i * 4] - 16;
                if (line < spriteY || line >= spriteY + height)
                {
                    continue;
                }

                indexes[drawn++] = i;
            }

            for (int n = drawn - 1; n >= 0; n--)
            {
                int i = indexes[n];
                int oam = i * 4;
                int spriteY = cpu.OAM[oam] - 16;
                int spriteX = cpu.OAM[oam + 1] - 8;
                int tile = cpu.OAM[oam + 2];
                byte attr = cpu.OAM[oam + 3];
                bool flipY = (attr & 0x40) != 0;
                bool flipX = (attr & 0x20) != 0;
                bool behind = (attr & 0x80) != 0;
                byte palette = (attr & 0x10) != 0 ? obp1 : obp0;
                int row = line - spriteY;
                if (flipY)
                {
                    row = height - 1 - row;
                }

                int tileAddr;
                if (height == 16)
                {
                    tile &= 0xFE;
                    tileAddr = 0x8000 + (tile + (row >= 8 ? 1 : 0)) * 16;
                    row &= 7;
                }
                else
                {
                    tileAddr = 0x8000 + tile * 16;
                }

                for (int px = 0; px < 8; px++)
                {
                    int x = spriteX + px;
                    if (x < 0 || x >= SCREEN_WIDTH)
                    {
                        continue;
                    }

                    int bit = flipX ? px : 7 - px;
                    byte colorId = Decode(cpu, tileAddr, row, bit, false);
                    if (colorId == 0)
                    {
                        continue;
                    }

                    if (behind && bg[x] != 0)
                    {
                        continue;
                    }

                    WritePixel(line, x, PaletteColor(palette, colorId));
                }
            }
        }

        static byte Decode(Cpu cpu, int tileAddr, int row, int bit, bool ignored)
        {
            int addr = tileAddr + row * 2;
            byte low = Vram(cpu, addr);
            byte high = Vram(cpu, addr + 1);
            int mask = 1 << bit;
            int color = 0;
            if ((low & mask) != 0)
            {
                color |= 1;
            }

            if ((high & mask) != 0)
            {
                color |= 2;
            }

            return (byte)color;
        }

        static byte Vram(Cpu cpu, int address)
        {
            int index = (address & 0x1FFF);
            if ((uint)index >= (uint)cpu.VRAM.Length)
            {
                return 0xFF;
            }

            return cpu.VRAM[index];
        }

        static byte PaletteColor(byte palette, int colorId)
        {
            int shade = (palette >> (colorId * 2)) & 0x03;
            return Shade[shade];
        }

        void WritePixel(int line, int x, byte shade)
        {
            int index = (line * SCREEN_WIDTH + x) * 4;
            Framebuffer[index] = shade;
            Framebuffer[index + 1] = shade;
            Framebuffer[index + 2] = shade;
            Framebuffer[index + 3] = 255;
        }
    }
}
