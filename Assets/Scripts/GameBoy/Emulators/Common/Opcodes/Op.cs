using System;

namespace GameBoy.Emulators.Common.Opcodes
{
    public static class Op
    {
        #region Make

        public static ushort Make16(byte     high, byte low) => (ushort)((high << 8) | low);
        public static byte   MakeLow(ushort  value) => (byte)(value & 0x00FF);
        public static byte   MakeHigh(ushort value) => (byte)((value & 0xFF00) >> 8);

        #endregion

        #region OverFlowDetect

        public static bool DH_ADC(byte left, byte right, bool c) => (left & 0x0F) + (right & 0x0F) + (c ? 1 : 0) > 0x0F;

        public static bool D_ADC(byte left, byte right, bool c) => left + right + (c ? 1 : 0) > 0xFF;

        public static bool DH_ADC(ushort left, ushort right, bool c) =>
            (left & 0x00FF) + (right & 0x00FF) + (c ? 1 : 0) > 0x00FF;

        public static bool D_ADC(ushort left, ushort right, bool c) => left + right + (c ? 1 : 0) > 0xFFFF;

        public static bool DH_SBC(byte left, byte right, bool c) => (left & 0x0F) < (right & 0x0F) + (c ? 1 : 0);

        public static bool D_SBC(byte left, byte right, bool c) => left < right + (c ? 1 : 0);

        public static bool DH_SBC(ushort left, ushort right, bool c) =>
            (left & 0x00FF) < (right & 0x00FF) + (c ? 1 : 0);

        public static bool D_SBC(ushort left, ushort right, bool c) => left < right + (c ? 1 : 0);

        public static bool DH_ADD(byte left, byte right) => ((left ^ right ^ (left + right)) & 0b0001_0000) != 0;

        public static bool D_ADD(byte left, byte right) => ((left ^ right ^ (left + right)) & 0b1_0000_0000) != 0;

        public static bool DH_ADD(ushort left, ushort right) =>
            ((left ^ right ^ (left + right)) & 0b0000_0001_0000_0000) != 0;

        public static bool D_ADD(ushort left, ushort right) =>
            ((left ^ right ^ (left + right)) & 0b1_0000_0000_0000_0000) != 0;

        public static bool DH_SUB(byte left, byte right) => ((left ^ right ^ (left - right)) & 0b0001_0000) != 0;

        public static bool D_SUB(byte left, byte right) => ((left ^ right ^ (left - right)) & 0b1_0000_0000) != 0;

        public static bool DH_SUB(ushort left, ushort right) =>
            ((left ^ right ^ (left - right)) & 0b0000_0001_0000_0000) != 0;

        public static bool D_SUB(ushort left, ushort right) =>
            ((left ^ right ^ (left - right)) & 0b1_0000_0000_0000_0000) != 0;

        public static bool DetectHalfOverflowAdd(byte left, byte right, byte val = 0)
            => (left & 0x0F)
             + (right & 0x0F)
             + val
             > 0x0F;

        public static bool DetectOverflowAdd(byte left, byte right, byte val = 0)
            => left
             + right
             + val
             > 0xFF;

        public static bool DetectHalfOverflowAdd(ushort left, ushort right, ushort val = 0)
            => (left & 0x00FF)
             + (right & 0x00FF)
             + val
             > 0x00FF;

        public static bool DetectOverflowAdd(ushort left, ushort right, ushort val = 0)
            => left
             + right
             + val
             > 0xFFFF;

        #endregion

        #region ReadWrite

        public static byte Read(Cpu cpu, int address)
        {
            address &= 0xFFFF;
            if (address <= Ram.MAP_ROM_BANK_1_END)
            {
                return cpu.Cart.Read(address);
            }
            else if (address <= Ram.MAP_VRAM_END)
            {
                return cpu.VRAM[address - Ram.MAP_VRAM];
            }
            else if (address <= Ram.MAP_EXTERNAL_RAM_END)
            {
                return cpu.Cart.Read(address);
            }
            else if (address <= Ram.MAP_WORK_RAM_END)
            {
                return cpu.WRAM[address - Ram.MAP_WORK_RAM];
            }
            else if (address <= Ram.MAP_ECHO_RAM_END)
            {
                return cpu.WRAM[address - Ram.MAP_ECHO_RAM];
            }
            else if (address <= Ram.MAP_OAM_END)
            {
                return cpu.OAM[address - Ram.MAP_OAM];
            }
            else if (address <= Ram.MAP_UNUSED_END)
            {
            }
            else if (address <= Ram.MAP_IO_REGISTERS_END)
            {
                if (address >= 0xFF10 && address <= 0xFF3F)
                {
                    return cpu.Apu.Read(address);
                }

                switch (address)
                {
                    case 0xFF00: return cpu.Joypad.p1;
                    case 0xFF01: return cpu.Serial.sb;
                    case 0xFF02: return cpu.Serial.sc;
                    case 0xFF03: return 0xFF;          // TODO
                    case 0xFF04: return Cpu.ReadDiv(cpu);
                    case 0xFF05: return cpu.tima;
                    case 0xFF06: return cpu.tma;
                    case 0xFF07: return cpu.tac;
                    case 0xFF0F: return (byte)(cpu._intFlags | 0xE0);
                    case 0xFF40: return cpu.Ppu.lcdc;
                    case 0xFF41: return cpu.Ppu.lcds;
                    case 0xFF42: return cpu.Ppu.scroll_y;
                    case 0xFF43: return cpu.Ppu.scroll_x;
                    case 0xFF44: return cpu.Ppu.ly;
                    case 0xFF45: return cpu.Ppu.lyc;
                    case 0xFF46: return cpu.Ppu.dma;
                    case 0xFF47: return cpu.Ppu.bgp;
                    case 0xFF48: return cpu.Ppu.obp0;
                    case 0xFF49: return cpu.Ppu.obp1;
                    case 0xFF4A: return cpu.Ppu.wy;
                    case 0xFF4B: return cpu.Ppu.wx;
                }

                return 0xFF;
            }
            else if (address <= Ram.MAP_HRAM_END)
            {
                return cpu.HRAM[address - Ram.MAP_HRAM];
            }
            else if (address <= Ram.MAP_INTERRUPT_ENABLE)
            {
                return (byte)(cpu._intEnableFlags | 0xE0);
            }

            return 0xFF;
        }

        public static ushort Read16(Cpu cpu, int address) =>
            (ushort)(Read(cpu, address) | (Read(cpu, address + 1) << 8));

        public static void Write(Cpu cpu, int address, byte value)
        {
            address &= 0xFFFF;
            try
            {
                if (address <= Ram.MAP_ROM_BANK_1_END)
                {
                    cpu.Cart.Write(address, value);
                }
                else if (address <= Ram.MAP_VRAM_END)
                {
                    cpu.VRAM[address - Ram.MAP_VRAM] = value;
                }
                else if (address <= Ram.MAP_EXTERNAL_RAM_END)
                {
                    cpu.Cart.Write(address, value);
                }
                else if (address <= Ram.MAP_WORK_RAM_END)
                {
                    cpu.WRAM[address - Ram.MAP_WORK_RAM] = value;
                }
                else if (address <= Ram.MAP_ECHO_RAM_END)
                {
                    cpu.WRAM[address - Ram.MAP_ECHO_RAM] = value;
                }
                else if (address <= Ram.MAP_OAM_END)
                {
                    cpu.OAM[address - Ram.MAP_OAM] = value;
                }
                else if (address <= Ram.MAP_UNUSED_END)
                {
                }
                else if (address <= Ram.MAP_IO_REGISTERS_END)
                {
                    if (address >= 0xFF10 && address <= 0xFF3F)
                    {
                        cpu.Apu.Write(address, value);
                        return;
                    }

                    switch (address)
                    {
                        case 0xFF00:
                            cpu.Joypad.Write(value);
                            return;
                        case 0xFF01:
                            cpu.Serial.sb = value;
                            return;
                        case 0xFF02:
                            cpu.Serial.sc = (byte)(0x7C | (value & 0x83));
                            return;
                        case 0xFF03: return;
                        case 0xFF04:
                            Cpu.ResetDiv(cpu);
                            return;
                        case 0xFF05:
                            cpu.tima = value;
                            return;
                        case 0xFF06:
                            cpu.tma = value;
                            return;
                        case 0xFF07:
                            cpu.tac = (byte)(0xF8 | (value & 0x07));
                            return;
                        case 0xFF0F:
                            cpu._intFlags = (byte)(value & 0x1F);
                            return;
                        case 0xFF40:
                            bool wasOn = cpu.Ppu.Enabled;
                            bool nowOn = (value & (1 << 7)) != 0;
                            cpu.Ppu.lcdc = value;
                            if (wasOn && !nowOn)
                            {
                                cpu.Ppu.PowerOff();
                            }
                            else if (!wasOn && nowOn)
                            {
                                cpu.Ppu.PowerOn();
                            }

                            return;
                        case 0xFF41:
                            cpu.Ppu.lcds = (byte)((cpu.Ppu.lcds & 0x07) | (byte)(value & 0xF8));
                            return;
                        case 0xFF42:
                            cpu.Ppu.scroll_y = value;
                            return;
                        case 0xFF43:
                            cpu.Ppu.scroll_x = value;
                            return;
                        case 0xFF44: return; // read only
                        case 0xFF45:
                            cpu.Ppu.lyc = value;
                            return;
                        case 0xFF46:
                            cpu.Ppu.dma = value;
                            int source = value << 8;
                            for (int i = 0; i < 0xA0; i++)
                            {
                                cpu.OAM[i] = Read(cpu, source + i);
                            }

                            return;
                        case 0xFF47:
                            cpu.Ppu.bgp = value;
                            return;
                        case 0xFF48:
                            cpu.Ppu.obp0 = value;
                            return;
                        case 0xFF49:
                            cpu.Ppu.obp1 = value;
                            return;
                        case 0xFF4A:
                            cpu.Ppu.wy = value;
                            return;
                        case 0xFF4B:
                            cpu.Ppu.wx = value;
                            return;
                    }

                    // ignore
                    // throw new Exception($"address write:{address:X4}");
                }
                else if (address <= Ram.MAP_HRAM_END)
                {
                    cpu.HRAM[address - Ram.MAP_HRAM] = value;
                }
                else if (address <= Ram.MAP_INTERRUPT_ENABLE)
                {
                    cpu._intEnableFlags = value;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new Exception($"Invalid address:{address:X4}", e);
            }
        }

        public static void Write16(Cpu cpu, int address, ushort value)
        {
            Write(cpu, address, (byte)(value & 0x00FF));
            Write(cpu, address + 1, (byte)((value & 0xFF00) >> 8));
        }

        public static void Push16(Cpu cpu, ushort value)
        {
            cpu.Reg.SP -= 2;
            Write16(cpu, cpu.Reg.SP, value);
        }

        public static ushort Pop16(Cpu cpu)
        {
            ushort value = Read16(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 2;
            return value;
        }

        #endregion

        #region CB Instruction Set

        public static void CbOperation(Cpu cpu, byte opBit, ref byte opVal)
        {
            switch (opBit)
            {
                case 0x00: // 00000
                    RLC(cpu, ref opVal);
                    return;
                case 0x01: // 00001
                    RRC(cpu, ref opVal);
                    return;
                case 0x02: // 00010
                    RL(cpu, ref opVal);
                    return;
                case 0x03: // 00011
                    RR(cpu, ref opVal);
                    return;
                case 0x04: // 00100
                    SLA(cpu, ref opVal);
                    return;
                case 0x05: // 00101
                    SRA(cpu, ref opVal);
                    return;
                case 0x06: // 00110
                    SWAP(cpu, ref opVal);
                    return;
                case 0x07: // 00111
                    SRL(cpu, ref opVal);
                    return;
                default:
                    if (opBit <= 0x0F)
                    {
                        BIT(cpu, opVal, (byte)(opBit - 0x08));
                        return;
                    }
                    else if (opBit <= 0x17)
                    {
                        RES(cpu, ref opVal, (byte)(opBit - 0x10));
                        return;
                    }
                    else if (opBit <= 0x1F)
                    {
                        SET(cpu, ref opVal, (byte)(opBit - 0x18));
                        return;
                    }
                    else
                    {
                        throw new Exception($" cb op {Convert.ToString(opBit, 2)} not implemented");
                    }
            }
        }

        public static void GetRegister(Cpu cpu, byte opReg, ref byte value)
        {
            switch (opReg)
            {
                case 0x00:
                    value = ref cpu.Reg.B;
                    return;
                case 0x01:
                    value = ref cpu.Reg.C;
                    return;
                case 0x02:
                    value = ref cpu.Reg.D;
                    return;
                case 0x03:
                    value = ref cpu.Reg.E;
                    return;
                case 0x04:
                    value = ref cpu.Reg.H;
                    return;
                case 0x05:
                    value = ref cpu.Reg.L;
                    return;
                case 0x06:
                    value = ref cpu.RomData[cpu.Reg.HL];
                    cpu.ClockCounter += 4;
                    return;
                case 0x07:
                    value = ref cpu.Reg.A;
                    return;
                default: throw new Exception($"Register code {opReg:X2} not implemented");
            }
        }

        public static void RLC(Cpu cpu, ref byte value)
        {
            value = (byte)((value << 1) | (value >> 7));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = (value & 0x01) != 0;
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }

        public static void RRC(Cpu cpu, ref byte value)
        {
            value = (byte)((value >> 1) | (value << 7));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = (value & 0x80) != 0;
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }

        public static void RL(Cpu cpu, ref byte value)
        {
            byte c = (byte)(value >> 7);
            value = (byte)((value << 1) | (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = c != 0;
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }

        public static void RR(Cpu cpu, ref byte value)
        {
            byte c = (byte)(value & 0x01);
            value = (byte)((value >> 1) | (cpu.Reg.c ? 0x80 : 0));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = c != 0;
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }

        public static void SLA(Cpu cpu, ref byte value)
        {
            cpu.Reg.c = (value & 0x80) != 0;
            value = (byte)(value << 1);
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }

        public static void SRA(Cpu cpu, ref byte value)
        {
            cpu.Reg.c = (value & 0x01) != 0;
            value = (byte)((value & 0x80) | (value >> 1));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }

        public static void SRL(Cpu cpu, ref byte value)
        {
            cpu.Reg.c = (value & 0x01) != 0;
            value = (byte)(value >> 1);
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }

        public static void SWAP(Cpu cpu, ref byte value)
        {
            value = (byte)((value << 4) | (value >> 4));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }

        public static void BIT(Cpu cpu, byte value, byte bit)
        {
            cpu.Reg.z = (value & (1 << bit)) != 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }

        public static void RES(Cpu cpu, ref byte value, byte bit)
        {
            value &= (byte)~(1 << bit);
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }

        public static void SET(Cpu cpu, ref byte value, byte bit)
        {
            value |= (byte)(1 << bit);
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }


        public static byte CB_RLC(Cpu cpu, byte value)
        {
            value = (byte)((value << 1) | (value >> 7));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = (value & 0x01) != 0;
            return value;
        }

        public static byte CB_RRC(Cpu cpu, byte value)
        {
            value = (byte)((value >> 1) | (value << 7));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = (value & 0x80) != 0;
            return value;
        }

        public static byte CB_RL(Cpu cpu, byte value)
        {
            byte c = (byte)(value >> 7);
            value = (byte)((value << 1) | (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = c != 0;
            return value;
        }

        public static byte CB_RR(Cpu cpu, byte value)
        {
            byte c = (byte)(value & 0x01);
            value = (byte)((value >> 1) | (cpu.Reg.c ? 0x80 : 0));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = c != 0;
            return value;
        }

        public static byte CB_SLA(Cpu cpu, byte value)
        {
            cpu.Reg.c = (value & 0x80) != 0;
            value = (byte)(value << 1);
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            return value;
        }

        public static byte CB_SRA(Cpu cpu, byte value)
        {
            cpu.Reg.c = (value & 0x01) != 0;
            value = (byte)((value & 0x80) | (value >> 1));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            return value;
        }

        public static byte CB_SRL(Cpu cpu, byte value)
        {
            cpu.Reg.c = (value & 0x01) != 0;
            value = (byte)(value >> 1);
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            return value;
        }

        public static byte CB_SWAP(Cpu cpu, byte value)
        {
            value = (byte)((value << 4) | (value >> 4));
            cpu.Reg.z = value == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
            return value;
        }

        public static byte CB_BIT(Cpu cpu, byte value, byte bit)
        {
            cpu.Reg.z = (value & (1 << bit)) != 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
            return value;
        }

        public static byte CB_RES(Cpu cpu, byte value, byte bit) => value &= (byte)~(1 << bit);

        public static byte CB_SET(Cpu cpu, byte value, byte bit) => value |= (byte)(1 << bit);

        #endregion
    }
}