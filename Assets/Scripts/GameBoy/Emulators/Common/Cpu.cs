using System;
using System.Runtime.InteropServices;
using GameBoy.Emulators.Common.Cpus;
using GameBoy.Emulators.Common.Opcodes;

namespace GameBoy.Emulators.Common
{
    public sealed class Cpu
    {
        public const  ulong  CLOCK_SPEED_UNSCALED = 4194304;
        public static double ClockSpeedScale      = 1.0;
        /// <summary>
        ///     Clock speed of the GameBoy
        ///     https://gbdev.io/pandocs/Specifications.html
        /// </summary>
        public static ulong CLOCK_SPEED => (ulong)Math.Ceiling(CLOCK_SPEED_UNSCALED * ClockSpeedScale);
        public readonly byte[] HRAM = new byte[Ram.MAP_HRAM_END - Ram.MAP_HRAM + 1];

        #region IO

        public readonly Joypad Joypad;

        #endregion

        public readonly Ppu Ppu;

        #region RAM

        public readonly Ram Ram;

        #endregion

        #region Serial

        public readonly Serial Serial;

        #endregion

        public readonly byte[] VRAM = new byte[Ram.MAP_VRAM_END - Ram.MAP_VRAM + 1];
        public readonly byte[] WRAM = new byte[Ram.MAP_WORK_RAM_END - Ram.MAP_WORK_RAM + 1];
        public readonly byte[] OAM  = new byte[Ram.MAP_OAM_END - Ram.MAP_OAM + 1];
        private         ulong  _clockCounter;
        public          bool   _ime;
        public          byte   _imeCountdown;
        public          byte   _intEnableFlags;

        public byte _intFlags;

        public bool Halted;
        public bool Stopped;
        public bool HaltBug;

        /// <summary>
        ///     CPU Registers
        /// </summary>
        public GameBoyEmulatorCpuRegister Reg = default(GameBoyEmulatorCpuRegister);
        /// <summary>
        ///     Clock counter
        /// </summary>
        public ulong ClockCounter
        {
            get => _clockCounter;
            set => _clockCounter = value;
        }

        public void TickCycles(int cycles)
        {
            for (int i = 0; i < cycles; i++)
            {
                _clockCounter++;
                TimerTick(this);
                if ((_clockCounter & 511) == 0)
                {
                    Serial.Tick(this);
                }

                Ppu.PpuTick(this);
                Apu.Tick(this);
            }
        }
        public bool IME
        {
            get => throw new Exception("Interrupt Master Enable cannot be read");
            set => _ime = value;
        }

        /// <summary>
        ///     GameBoy Emulator CPU Registers Program Counter
        /// </summary>
        public ushort ProgramCounter
        {
            get => Reg.PC;
            set => Reg.PC = value;
        }

        /// <summary>
        ///     GameBoy Emulator CPU Registers
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct GameBoyEmulatorCpuRegister
        {
            /// <summary>
            ///     Interrupt Register
            /// </summary>
            [FieldOffset(0)] public byte _ir;
            public byte IR
            {
                get => _ir;
                set => _ir = value;
            }
            /// <summary>
            ///     Interrupt Enable
            /// </summary>
            [FieldOffset(1)] public byte IE;
            /// <summary>
            ///     Accumulator & Flags
            /// </summary>
            public ushort AF
            {
                get => _af;
                set => _af = (ushort)(value & 0xFFF0);
            }
            [FieldOffset(2)] private ushort _af;
            /// <summary>
            ///     Accumulator
            /// </summary>
            [FieldOffset(3)] public byte A;
            /// <summary>
            ///     Flags
            /// </summary>
            public byte F
            {
                get => (byte)(_f & 0b11110000);
                set => _f = (byte)(value & 0b11110000);
            }
            [FieldOffset(2)] private byte   _f;
            [FieldOffset(4)] public  ushort BC;
            [FieldOffset(5)] public  byte   B;
            [FieldOffset(4)] public  byte   C;
            [FieldOffset(6)] public  ushort DE;
            [FieldOffset(7)] public  byte   D;
            [FieldOffset(6)] public  byte   E;
            [FieldOffset(8)] public  ushort HL;
            [FieldOffset(9)] public  byte   H;
            [FieldOffset(8)] public  byte   L;
            /// <summary>
            ///     Program Counter
            /// </summary>
            [FieldOffset(10)] public ushort PC;
            /// <summary>
            ///     Stack Pointer
            /// </summary>
            [FieldOffset(12)] public ushort SP;


            /// <summary>
            ///     Zero Flag
            /// </summary>
            public bool z
            {
                get => (_f & 0b10000000) != 0;
                set => _f = (byte)(value ? _f | 0b10000000 : _f & 0b01110000);
            }

            /// <summary>
            ///     Subtraction flag (BCD)
            /// </summary>
            public bool n
            {
                get => (_f & 0b01000000) != 0;
                set => _f = (byte)(value ? _f | 0b01000000 : _f & 0b10110000);
            }

            /// <summary>
            ///     Half Carry flag (BCD)
            /// </summary>
            public bool h
            {
                get => (_f & 0b00100000) != 0;
                set => _f = (byte)(value ? _f | 0b00100000 : _f & 0b11010000);
            }

            /// <summary>
            ///     Carry flag
            /// </summary>
            public bool c
            {
                get => (_f & 0b00010000) != 0;
                set => _f = (byte)(value ? _f | 0b00010000 : _f & 0b11100000);
            }
        }

        #region Timer

        public ushort div;  // 0xFF04 Divider Register
        public byte   tima; // 0xFF05 Timer Counter
        public byte   tma;  // 0xFF06 Timer Modulo
        public byte   tac;  // 0xFF07 Timer Control

        public static void InitTimer(Cpu cpu)
        {
            cpu.div = 0xAC00;
            cpu.tima = 0x00;
            cpu.tma = 0x00;
            cpu.tac = 0xF8;
        }

        public static void TimerTick(Cpu cpu)
        {
            ushort prevDiv = cpu.div;
            cpu.div += 1;
            TimerEdge(cpu, prevDiv, cpu.div);
        }

        public static void ResetDiv(Cpu cpu)
        {
            ushort prevDiv = cpu.div;
            cpu.div = 0;
            TimerEdge(cpu, prevDiv, 0);
        }

        static void TimerEdge(Cpu cpu, ushort prevDiv, ushort now)
        {
            if (!TimaEnabled(cpu))
            {
                return;
            }

            int bit;
            switch (ClockSelect(cpu))
            {
                case 0: bit = 9; break;
                case 1: bit = 3; break;
                case 2: bit = 5; break;
                default: bit = 7; break;
            }

            bool wasHigh = (prevDiv & (1 << bit)) != 0;
            bool nowHigh = (now & (1 << bit)) != 0;
            if (!wasHigh || nowHigh)
            {
                return;
            }

            if (cpu.tima == 0xFF)
            {
                cpu._intFlags |= CpuOp.INT_TIMER;
                cpu.tima = cpu.tma;
            }
            else
            {
                cpu.tima += 1;
            }
        }

        public static byte ReadDiv(Cpu cpu) => (byte)(cpu.div >> 8);

        public static byte ClockSelect(Cpu cpu) => (byte)(cpu.tac & 0x03);

        public static bool TimaEnabled(Cpu cpu) => (cpu.tac & 0x04) != 0;

        #endregion

        #region CPU Method

        public readonly Apu Apu;
        public Cartridge Cart;

        public Cpu()
        {
            Ram = new Ram();
            Ppu = new Ppu();
            Joypad = new Joypad();
            Serial = new Serial();
            Apu = new Apu();
            Cart = new Cartridge(Array.Empty<byte>());
            Init();
        }

        public void Init()
        {
            Array.Clear(VRAM, 0, VRAM.Length);
            Array.Clear(WRAM, 0, WRAM.Length);
            Array.Clear(OAM, 0, OAM.Length);
            Array.Clear(HRAM, 0, HRAM.Length);
            Reg.AF = 0x01B0;
            Reg.BC = 0x0013;
            Reg.DE = 0x00D8;
            Reg.HL = 0x014D;
            ProgramCounter = 0x0100;
            Reg.SP = 0xFFFE;
            Halted = false;
            Stopped = false;
            HaltBug = false;
            _ime = false;
            _imeCountdown = 0;
            _intFlags = 0;
            _intEnableFlags = 0;
            InitTimer(this);
        }

        // public void Tick(byte ticks)
        // {
        //     ProgramCounter += ticks;
        //     ClockCounter += ticks * 4UL;
        // }

        public void EnableInterruptMaster()
        {
            _imeCountdown = 2;
        }

        public void DisableInterruptMaster()
        {
            _ime = false;
            _imeCountdown = 0;
        }

        #endregion

        #region ROM

        private byte[] _romData = Array.Empty<byte>();

        public byte[] RomData
        {
            get => _romData;
            set => Mount(value, null);
        }

        public void Mount(byte[] data, string path)
        {
            Cart?.Save();
            _romData = data ?? Array.Empty<byte>();
            Cart = new Cartridge(_romData, path);
            if (_romData.Length >= Ram.MAP_ROM_BANK_0_END + 1)
            {
                Ram.LoadRom(_romData);
            }
        }

        #endregion
    }
}