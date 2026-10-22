using GameBoy.Emulators.Common.Cpus;

namespace GameBoy.Emulators.Common.Opcodes
{
    /// <summary>
    ///     SM83 解释器. 每条指令按 M-cycle 推进定时器, 串口和 PPU.
    /// </summary>
    public static class Sm83
    {
        public static int Step(Cpu cpu)
        {
            if (cpu.Stopped)
            {
                if (cpu.Joypad.AnyPressed)
                {
                    cpu.Stopped = false;
                }
                else
                {
                    cpu.TickCycles(4);
                    FinishIme(cpu);
                    return 4;
                }
            }

            byte pending = (byte)(cpu._intFlags & cpu._intEnableFlags & 0x1F);
            if (cpu.Halted)
            {
                cpu.TickCycles(4);
                if (pending != 0)
                {
                    cpu.Halted = false;
                }

                if (cpu.Halted || !cpu._ime || pending == 0)
                {
                    FinishIme(cpu);
                    return 4;
                }

                Service(cpu);
                FinishIme(cpu);
                return 24;
            }

            if (cpu._ime && pending != 0)
            {
                Service(cpu);
                FinishIme(cpu);
                return 20;
            }

            int extra = Execute(cpu);
            FinishIme(cpu);
            return 4 + extra;
        }

        static void FinishIme(Cpu cpu)
        {
            if (cpu._imeCountdown == 0)
            {
                return;
            }

            cpu._imeCountdown--;
            if (cpu._imeCountdown == 0)
            {
                cpu._ime = true;
            }
        }

        static void Service(Cpu cpu)
        {
            cpu.Halted = false;
            cpu.Stopped = false;
            cpu._ime = false;
            cpu._imeCountdown = 0;
            byte flags = (byte)(cpu._intFlags & cpu._intEnableFlags & 0x1F);
            int bit;
            ushort vector;
            if ((flags & CpuOp.INT_VBLANK) != 0)
            {
                bit = CpuOp.INT_VBLANK;
                vector = 0x40;
            }
            else if ((flags & CpuOp.INT_LCD_STAT) != 0)
            {
                bit = CpuOp.INT_LCD_STAT;
                vector = 0x48;
            }
            else if ((flags & CpuOp.INT_TIMER) != 0)
            {
                bit = CpuOp.INT_TIMER;
                vector = 0x50;
            }
            else if ((flags & CpuOp.INT_SERIAL) != 0)
            {
                bit = CpuOp.INT_SERIAL;
                vector = 0x58;
            }
            else
            {
                bit = CpuOp.INT_JOYPAD;
                vector = 0x60;
            }

            cpu._intFlags &= (byte)~bit;
            cpu.TickCycles(8);
            Push(cpu, cpu.Reg.PC);
            cpu.Reg.PC = vector;
            cpu.TickCycles(4);
        }

        static int Execute(Cpu cpu)
        {
            byte op = FetchOpcode(cpu);
            cpu.Reg.IR = op;
            if (op == 0x76)
            {
                byte pending = (byte)(cpu._intFlags & cpu._intEnableFlags & 0x1F);
                if (!cpu._ime && pending != 0)
                {
                    cpu.HaltBug = true;
                }
                else
                {
                    cpu.Halted = true;
                }

                return 0;
            }

            if (op < 0x40)
            {
                return ExecuteControl(cpu, op);
            }

            if (op < 0x80)
            {
                return ExecuteLoad(cpu, op);
            }

            if (op < 0xC0)
            {
                return ExecuteAluReg(cpu, op);
            }

            return ExecuteHigh(cpu, op);
        }

        static int ExecuteControl(Cpu cpu, byte op)
        {
            switch (op)
            {
                case 0x00:
                    return 0;
                case 0x01:
                    cpu.Reg.BC = Imm16(cpu);
                    return 8;
                case 0x02:
                    Write(cpu, cpu.Reg.BC, cpu.Reg.A);
                    return 4;
                case 0x03:
                    cpu.TickCycles(4);
                    cpu.Reg.BC++;
                    return 4;
                case 0x04:
                    cpu.Reg.B = Inc8(cpu, cpu.Reg.B);
                    return 0;
                case 0x05:
                    cpu.Reg.B = Dec8(cpu, cpu.Reg.B);
                    return 0;
                case 0x06:
                    cpu.Reg.B = Imm8(cpu);
                    return 4;
                case 0x07:
                    Rlca(cpu);
                    return 0;
                case 0x08:
                {
                    ushort addr = Imm16(cpu);
                    Write(cpu, addr, (byte)cpu.Reg.SP);
                    Write(cpu, (ushort)(addr + 1), (byte)(cpu.Reg.SP >> 8));
                    return 16;
                }
                case 0x09:
                    AddHl(cpu, cpu.Reg.BC);
                    return 4;
                case 0x0A:
                    cpu.Reg.A = Read(cpu, cpu.Reg.BC);
                    return 4;
                case 0x0B:
                    cpu.TickCycles(4);
                    cpu.Reg.BC--;
                    return 4;
                case 0x0C:
                    cpu.Reg.C = Inc8(cpu, cpu.Reg.C);
                    return 0;
                case 0x0D:
                    cpu.Reg.C = Dec8(cpu, cpu.Reg.C);
                    return 0;
                case 0x0E:
                    cpu.Reg.C = Imm8(cpu);
                    return 4;
                case 0x0F:
                    Rrca(cpu);
                    return 0;
                case 0x10:
                    cpu.Reg.PC++;
                    cpu.Stopped = true;
                    return 0;
                case 0x11:
                    cpu.Reg.DE = Imm16(cpu);
                    return 8;
                case 0x12:
                    Write(cpu, cpu.Reg.DE, cpu.Reg.A);
                    return 4;
                case 0x13:
                    cpu.TickCycles(4);
                    cpu.Reg.DE++;
                    return 4;
                case 0x14:
                    cpu.Reg.D = Inc8(cpu, cpu.Reg.D);
                    return 0;
                case 0x15:
                    cpu.Reg.D = Dec8(cpu, cpu.Reg.D);
                    return 0;
                case 0x16:
                    cpu.Reg.D = Imm8(cpu);
                    return 4;
                case 0x17:
                    Rla(cpu);
                    return 0;
                case 0x18:
                    Jr(cpu, true);
                    return 8;
                case 0x19:
                    AddHl(cpu, cpu.Reg.DE);
                    return 4;
                case 0x1A:
                    cpu.Reg.A = Read(cpu, cpu.Reg.DE);
                    return 4;
                case 0x1B:
                    cpu.TickCycles(4);
                    cpu.Reg.DE--;
                    return 4;
                case 0x1C:
                    cpu.Reg.E = Inc8(cpu, cpu.Reg.E);
                    return 0;
                case 0x1D:
                    cpu.Reg.E = Dec8(cpu, cpu.Reg.E);
                    return 0;
                case 0x1E:
                    cpu.Reg.E = Imm8(cpu);
                    return 4;
                case 0x1F:
                    Rra(cpu);
                    return 0;
                case 0x20:
                    return Jr(cpu, !cpu.Reg.z);
                case 0x21:
                    cpu.Reg.HL = Imm16(cpu);
                    return 8;
                case 0x22:
                    Write(cpu, cpu.Reg.HL++, cpu.Reg.A);
                    return 4;
                case 0x23:
                    cpu.TickCycles(4);
                    cpu.Reg.HL++;
                    return 4;
                case 0x24:
                    cpu.Reg.H = Inc8(cpu, cpu.Reg.H);
                    return 0;
                case 0x25:
                    cpu.Reg.H = Dec8(cpu, cpu.Reg.H);
                    return 0;
                case 0x26:
                    cpu.Reg.H = Imm8(cpu);
                    return 4;
                case 0x27:
                    Daa(cpu);
                    return 0;
                case 0x28:
                    return Jr(cpu, cpu.Reg.z);
                case 0x29:
                    AddHl(cpu, cpu.Reg.HL);
                    return 4;
                case 0x2A:
                    cpu.Reg.A = Read(cpu, cpu.Reg.HL++);
                    return 4;
                case 0x2B:
                    cpu.TickCycles(4);
                    cpu.Reg.HL--;
                    return 4;
                case 0x2C:
                    cpu.Reg.L = Inc8(cpu, cpu.Reg.L);
                    return 0;
                case 0x2D:
                    cpu.Reg.L = Dec8(cpu, cpu.Reg.L);
                    return 0;
                case 0x2E:
                    cpu.Reg.L = Imm8(cpu);
                    return 4;
                case 0x2F:
                    cpu.Reg.A = (byte)~cpu.Reg.A;
                    cpu.Reg.n = true;
                    cpu.Reg.h = true;
                    return 0;
                case 0x30:
                    return Jr(cpu, !cpu.Reg.c);
                case 0x31:
                    cpu.Reg.SP = Imm16(cpu);
                    return 8;
                case 0x32:
                    Write(cpu, cpu.Reg.HL--, cpu.Reg.A);
                    return 4;
                case 0x33:
                    cpu.TickCycles(4);
                    cpu.Reg.SP++;
                    return 4;
                case 0x34:
                {
                    byte value = Read(cpu, cpu.Reg.HL);
                    value = Inc8(cpu, value);
                    Write(cpu, cpu.Reg.HL, value);
                    return 8;
                }
                case 0x35:
                {
                    byte value = Read(cpu, cpu.Reg.HL);
                    value = Dec8(cpu, value);
                    Write(cpu, cpu.Reg.HL, value);
                    return 8;
                }
                case 0x36:
                {
                    byte value = Imm8(cpu);
                    Write(cpu, cpu.Reg.HL, value);
                    return 8;
                }
                case 0x37:
                    cpu.Reg.n = false;
                    cpu.Reg.h = false;
                    cpu.Reg.c = true;
                    return 0;
                case 0x38:
                    return Jr(cpu, cpu.Reg.c);
                case 0x39:
                    AddHl(cpu, cpu.Reg.SP);
                    return 4;
                case 0x3A:
                    cpu.Reg.A = Read(cpu, cpu.Reg.HL--);
                    return 4;
                case 0x3B:
                    cpu.TickCycles(4);
                    cpu.Reg.SP--;
                    return 4;
                case 0x3C:
                    cpu.Reg.A = Inc8(cpu, cpu.Reg.A);
                    return 0;
                case 0x3D:
                    cpu.Reg.A = Dec8(cpu, cpu.Reg.A);
                    return 0;
                case 0x3E:
                    cpu.Reg.A = Imm8(cpu);
                    return 4;
                case 0x3F:
                    cpu.Reg.n = false;
                    cpu.Reg.h = false;
                    cpu.Reg.c = !cpu.Reg.c;
                    return 0;
                default:
                    return 0;
            }
        }

        static int ExecuteLoad(Cpu cpu, byte op)
        {
            int dest = (op >> 3) & 7;
            int src = op & 7;
            byte value = Get(cpu, src);
            Set(cpu, dest, value);
            int cycles = 0;
            if (src == 6)
            {
                cycles += 4;
            }

            if (dest == 6)
            {
                cycles += 4;
            }

            return cycles;
        }

        static int ExecuteAluReg(Cpu cpu, byte op)
        {
            int kind = (op >> 3) & 7;
            int src = op & 7;
            byte value = Get(cpu, src);
            Alu(cpu, kind, value);
            return src == 6 ? 4 : 0;
        }

        static int ExecuteHigh(Cpu cpu, byte op)
        {
            switch (op)
            {
                case 0xC0: return RetCc(cpu, !cpu.Reg.z);
                case 0xC1: cpu.Reg.BC = Pop(cpu); return 8;
                case 0xC2: return JpCc(cpu, !cpu.Reg.z);
                case 0xC3: return JpCc(cpu, true);
                case 0xC4: return CallCc(cpu, !cpu.Reg.z);
                case 0xC5: cpu.TickCycles(4); Push(cpu, cpu.Reg.BC); return 12;
                case 0xC6: Alu(cpu, 0, Imm8(cpu)); return 4;
                case 0xC7: return Rst(cpu, 0x00);
                case 0xC8: return RetCc(cpu, cpu.Reg.z);
                case 0xC9: cpu.Reg.PC = Pop(cpu); cpu.TickCycles(4); return 12;
                case 0xCA: return JpCc(cpu, cpu.Reg.z);
                case 0xCB: return Cb(cpu);
                case 0xCC: return CallCc(cpu, cpu.Reg.z);
                case 0xCD: return CallCc(cpu, true);
                case 0xCE: Alu(cpu, 1, Imm8(cpu)); return 4;
                case 0xCF: return Rst(cpu, 0x08);
                case 0xD0: return RetCc(cpu, !cpu.Reg.c);
                case 0xD1: cpu.Reg.DE = Pop(cpu); return 8;
                case 0xD2: return JpCc(cpu, !cpu.Reg.c);
                case 0xD4: return CallCc(cpu, !cpu.Reg.c);
                case 0xD5: cpu.TickCycles(4); Push(cpu, cpu.Reg.DE); return 12;
                case 0xD6: Alu(cpu, 2, Imm8(cpu)); return 4;
                case 0xD7: return Rst(cpu, 0x10);
                case 0xD8: return RetCc(cpu, cpu.Reg.c);
                case 0xD9:
                    cpu.Reg.PC = Pop(cpu);
                    cpu.TickCycles(4);
                    cpu._ime = true;
                    cpu._imeCountdown = 0;
                    return 12;
                case 0xDA: return JpCc(cpu, cpu.Reg.c);
                case 0xDC: return CallCc(cpu, cpu.Reg.c);
                case 0xDE: Alu(cpu, 3, Imm8(cpu)); return 4;
                case 0xDF: return Rst(cpu, 0x18);
                case 0xE0:
                {
                    byte port = Imm8(cpu);
                    Write(cpu, (ushort)(0xFF00 + port), cpu.Reg.A);
                    return 8;
                }
                case 0xE1: cpu.Reg.HL = Pop(cpu); return 8;
                case 0xE2:
                    Write(cpu, (ushort)(0xFF00 + cpu.Reg.C), cpu.Reg.A);
                    return 4;
                case 0xE5: cpu.TickCycles(4); Push(cpu, cpu.Reg.HL); return 12;
                case 0xE6: Alu(cpu, 4, Imm8(cpu)); return 4;
                case 0xE7: return Rst(cpu, 0x20);
                case 0xE8: return AddSp(cpu);
                case 0xE9:
                    cpu.Reg.PC = cpu.Reg.HL;
                    return 0;
                case 0xEA:
                {
                    ushort addr = Imm16(cpu);
                    Write(cpu, addr, cpu.Reg.A);
                    return 12;
                }
                case 0xEE: Alu(cpu, 5, Imm8(cpu)); return 4;
                case 0xEF: return Rst(cpu, 0x28);
                case 0xF0:
                {
                    byte port = Imm8(cpu);
                    cpu.Reg.A = Read(cpu, (ushort)(0xFF00 + port));
                    return 8;
                }
                case 0xF1:
                    cpu.Reg.AF = Pop(cpu);
                    return 8;
                case 0xF2:
                    cpu.Reg.A = Read(cpu, (ushort)(0xFF00 + cpu.Reg.C));
                    return 4;
                case 0xF3:
                    cpu.DisableInterruptMaster();
                    return 0;
                case 0xF5: cpu.TickCycles(4); Push(cpu, cpu.Reg.AF); return 12;
                case 0xF6: Alu(cpu, 6, Imm8(cpu)); return 4;
                case 0xF7: return Rst(cpu, 0x30);
                case 0xF8: return LdHlSp(cpu);
                case 0xF9:
                    cpu.TickCycles(4);
                    cpu.Reg.SP = cpu.Reg.HL;
                    return 4;
                case 0xFA:
                {
                    ushort addr = Imm16(cpu);
                    cpu.Reg.A = Read(cpu, addr);
                    return 12;
                }
                case 0xFB:
                    cpu.EnableInterruptMaster();
                    return 0;
                case 0xFE: Alu(cpu, 7, Imm8(cpu)); return 4;
                case 0xFF: return Rst(cpu, 0x38);
                default:
                    return 0;
            }
        }

        static int Cb(Cpu cpu)
        {
            byte cb = Imm8(cpu);
            int reg = cb & 7;
            int kind = cb >> 3;
            if (reg == 6)
            {
                byte value = Read(cpu, cpu.Reg.HL);
                if (kind < 8)
                {
                    value = Rotate(cpu, kind, value);
                    Write(cpu, cpu.Reg.HL, value);
                    return 12;
                }

                if (kind < 16)
                {
                    Bit(cpu, kind - 8, value);
                    return 8;
                }

                value = kind < 24
                    ? (byte)(value & ~(1 << (kind - 16)))
                    : (byte)(value | (1 << (kind - 24)));
                Write(cpu, cpu.Reg.HL, value);
                return 12;
            }

            byte current = Get(cpu, reg);
            if (kind < 8)
            {
                Set(cpu, reg, Rotate(cpu, kind, current));
            }
            else if (kind < 16)
            {
                Bit(cpu, kind - 8, current);
            }
            else if (kind < 24)
            {
                Set(cpu, reg, (byte)(current & ~(1 << (kind - 16))));
            }
            else
            {
                Set(cpu, reg, (byte)(current | (1 << (kind - 24))));
            }

            return 4;
        }

        static byte Rotate(Cpu cpu, int kind, byte value)
        {
            bool carry = cpu.Reg.c;
            byte result;
            bool next;
            switch (kind)
            {
                case 0:
                    next = (value & 0x80) != 0;
                    result = (byte)((value << 1) | (value >> 7));
                    break;
                case 1:
                    next = (value & 0x01) != 0;
                    result = (byte)((value >> 1) | (value << 7));
                    break;
                case 2:
                    next = (value & 0x80) != 0;
                    result = (byte)((value << 1) | (carry ? 1 : 0));
                    break;
                case 3:
                    next = (value & 0x01) != 0;
                    result = (byte)((value >> 1) | (carry ? 0x80 : 0));
                    break;
                case 4:
                    next = (value & 0x80) != 0;
                    result = (byte)(value << 1);
                    break;
                case 5:
                    next = (value & 0x01) != 0;
                    result = (byte)((value >> 1) | (value & 0x80));
                    break;
                case 6:
                    result = (byte)((value << 4) | (value >> 4));
                    next = false;
                    break;
                default:
                    next = (value & 0x01) != 0;
                    result = (byte)(value >> 1);
                    break;
            }

            cpu.Reg.z = result == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = kind == 6 ? false : next;
            return result;
        }

        static void Bit(Cpu cpu, int bit, byte value)
        {
            cpu.Reg.z = (value & (1 << bit)) == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
        }

        static int Jr(Cpu cpu, bool taken)
        {
            sbyte offset = (sbyte)Imm8(cpu);
            if (!taken)
            {
                return 4;
            }

            cpu.TickCycles(4);
            cpu.Reg.PC = (ushort)(cpu.Reg.PC + offset);
            return 8;
        }

        static int JpCc(Cpu cpu, bool taken)
        {
            ushort addr = Imm16(cpu);
            if (!taken)
            {
                return 8;
            }

            cpu.TickCycles(4);
            cpu.Reg.PC = addr;
            return 12;
        }

        static int CallCc(Cpu cpu, bool taken)
        {
            ushort addr = Imm16(cpu);
            if (!taken)
            {
                return 8;
            }

            cpu.TickCycles(4);
            Push(cpu, cpu.Reg.PC);
            cpu.Reg.PC = addr;
            return 20;
        }

        static int RetCc(Cpu cpu, bool taken)
        {
            cpu.TickCycles(4);
            if (!taken)
            {
                return 4;
            }

            cpu.Reg.PC = Pop(cpu);
            cpu.TickCycles(4);
            return 16;
        }

        static int Rst(Cpu cpu, ushort vector)
        {
            cpu.TickCycles(4);
            Push(cpu, cpu.Reg.PC);
            cpu.Reg.PC = vector;
            return 12;
        }

        static int AddSp(Cpu cpu)
        {
            byte offset = Imm8(cpu);
            ApplyOffsetFlags(cpu, cpu.Reg.SP, offset);
            cpu.TickCycles(8);
            cpu.Reg.SP = (ushort)(cpu.Reg.SP + (sbyte)offset);
            return 12;
        }

        static int LdHlSp(Cpu cpu)
        {
            byte offset = Imm8(cpu);
            ApplyOffsetFlags(cpu, cpu.Reg.SP, offset);
            cpu.TickCycles(4);
            cpu.Reg.HL = (ushort)(cpu.Reg.SP + (sbyte)offset);
            return 8;
        }

        static void ApplyOffsetFlags(Cpu cpu, ushort sp, byte offset)
        {
            cpu.Reg.z = false;
            cpu.Reg.n = false;
            cpu.Reg.h = ((sp & 0x0F) + (offset & 0x0F)) > 0x0F;
            cpu.Reg.c = ((sp & 0xFF) + offset) > 0xFF;
        }

        static void AddHl(Cpu cpu, ushort value)
        {
            cpu.TickCycles(4);
            int hl = cpu.Reg.HL;
            int sum = hl + value;
            cpu.Reg.n = false;
            cpu.Reg.h = (hl & 0x0FFF) + (value & 0x0FFF) > 0x0FFF;
            cpu.Reg.c = sum > 0xFFFF;
            cpu.Reg.HL = (ushort)sum;
        }

        static void Alu(Cpu cpu, int kind, byte value)
        {
            byte a = cpu.Reg.A;
            int carry = cpu.Reg.c ? 1 : 0;
            switch (kind)
            {
                case 0:
                {
                    int sum = a + value;
                    cpu.Reg.z = (byte)sum == 0;
                    cpu.Reg.n = false;
                    cpu.Reg.h = (a & 0x0F) + (value & 0x0F) > 0x0F;
                    cpu.Reg.c = sum > 0xFF;
                    cpu.Reg.A = (byte)sum;
                    break;
                }
                case 1:
                {
                    int sum = a + value + carry;
                    cpu.Reg.z = (byte)sum == 0;
                    cpu.Reg.n = false;
                    cpu.Reg.h = (a & 0x0F) + (value & 0x0F) + carry > 0x0F;
                    cpu.Reg.c = sum > 0xFF;
                    cpu.Reg.A = (byte)sum;
                    break;
                }
                case 2:
                {
                    int diff = a - value;
                    cpu.Reg.z = (byte)diff == 0;
                    cpu.Reg.n = true;
                    cpu.Reg.h = (a & 0x0F) < (value & 0x0F);
                    cpu.Reg.c = a < value;
                    cpu.Reg.A = (byte)diff;
                    break;
                }
                case 3:
                {
                    int diff = a - value - carry;
                    cpu.Reg.z = (byte)diff == 0;
                    cpu.Reg.n = true;
                    cpu.Reg.h = (a & 0x0F) < (value & 0x0F) + carry;
                    cpu.Reg.c = a < value + carry;
                    cpu.Reg.A = (byte)diff;
                    break;
                }
                case 4:
                    cpu.Reg.A = (byte)(a & value);
                    cpu.Reg.z = cpu.Reg.A == 0;
                    cpu.Reg.n = false;
                    cpu.Reg.h = true;
                    cpu.Reg.c = false;
                    break;
                case 5:
                    cpu.Reg.A = (byte)(a ^ value);
                    cpu.Reg.z = cpu.Reg.A == 0;
                    cpu.Reg.n = false;
                    cpu.Reg.h = false;
                    cpu.Reg.c = false;
                    break;
                case 6:
                    cpu.Reg.A = (byte)(a | value);
                    cpu.Reg.z = cpu.Reg.A == 0;
                    cpu.Reg.n = false;
                    cpu.Reg.h = false;
                    cpu.Reg.c = false;
                    break;
                default:
                {
                    int diff = a - value;
                    cpu.Reg.z = (byte)diff == 0;
                    cpu.Reg.n = true;
                    cpu.Reg.h = (a & 0x0F) < (value & 0x0F);
                    cpu.Reg.c = a < value;
                    break;
                }
            }
        }

        static byte Inc8(Cpu cpu, byte value)
        {
            byte result = (byte)(value + 1);
            cpu.Reg.z = result == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = (value & 0x0F) == 0x0F;
            return result;
        }

        static byte Dec8(Cpu cpu, byte value)
        {
            byte result = (byte)(value - 1);
            cpu.Reg.z = result == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (value & 0x0F) == 0;
            return result;
        }

        static void Daa(Cpu cpu)
        {
            int a = cpu.Reg.A;
            if (!cpu.Reg.n)
            {
                if (cpu.Reg.h || (a & 0x0F) > 9)
                {
                    a += 0x06;
                }

                if (cpu.Reg.c || a > 0x9F)
                {
                    a += 0x60;
                    cpu.Reg.c = true;
                }
            }
            else
            {
                if (cpu.Reg.h)
                {
                    a -= 0x06;
                }

                if (cpu.Reg.c)
                {
                    a -= 0x60;
                }
            }

            cpu.Reg.A = (byte)a;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.h = false;
        }

        static void Rlca(Cpu cpu)
        {
            byte a = cpu.Reg.A;
            bool carry = (a & 0x80) != 0;
            cpu.Reg.A = (byte)((a << 1) | (a >> 7));
            cpu.Reg.z = false;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = carry;
        }

        static void Rrca(Cpu cpu)
        {
            byte a = cpu.Reg.A;
            bool carry = (a & 0x01) != 0;
            cpu.Reg.A = (byte)((a >> 1) | (a << 7));
            cpu.Reg.z = false;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = carry;
        }

        static void Rla(Cpu cpu)
        {
            byte a = cpu.Reg.A;
            bool carry = (a & 0x80) != 0;
            cpu.Reg.A = (byte)((a << 1) | (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = false;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = carry;
        }

        static void Rra(Cpu cpu)
        {
            byte a = cpu.Reg.A;
            bool carry = (a & 0x01) != 0;
            cpu.Reg.A = (byte)((a >> 1) | (cpu.Reg.c ? 0x80 : 0));
            cpu.Reg.z = false;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = carry;
        }

        static byte Get(Cpu cpu, int index)
        {
            switch (index)
            {
                case 0: return cpu.Reg.B;
                case 1: return cpu.Reg.C;
                case 2: return cpu.Reg.D;
                case 3: return cpu.Reg.E;
                case 4: return cpu.Reg.H;
                case 5: return cpu.Reg.L;
                case 6: return Read(cpu, cpu.Reg.HL);
                default: return cpu.Reg.A;
            }
        }

        static void Set(Cpu cpu, int index, byte value)
        {
            switch (index)
            {
                case 0: cpu.Reg.B = value; break;
                case 1: cpu.Reg.C = value; break;
                case 2: cpu.Reg.D = value; break;
                case 3: cpu.Reg.E = value; break;
                case 4: cpu.Reg.H = value; break;
                case 5: cpu.Reg.L = value; break;
                case 6: Write(cpu, cpu.Reg.HL, value); break;
                default: cpu.Reg.A = value; break;
            }
        }

        static byte FetchOpcode(Cpu cpu)
        {
            byte value = Op.Read(cpu, cpu.Reg.PC);
            cpu.TickCycles(4);
            if (cpu.HaltBug)
            {
                cpu.HaltBug = false;
            }
            else
            {
                cpu.Reg.PC++;
            }

            return value;
        }

        static byte Imm8(Cpu cpu)
        {
            byte value = Op.Read(cpu, cpu.Reg.PC);
            cpu.TickCycles(4);
            cpu.Reg.PC++;
            return value;
        }

        static ushort Imm16(Cpu cpu)
        {
            byte low = Imm8(cpu);
            byte high = Imm8(cpu);
            return (ushort)(low | (high << 8));
        }

        static byte Read(Cpu cpu, ushort address)
        {
            cpu.TickCycles(4);
            return Op.Read(cpu, address);
        }

        static void Write(Cpu cpu, ushort address, byte value)
        {
            cpu.TickCycles(4);
            Op.Write(cpu, address, value);
        }

        static void Push(Cpu cpu, ushort value)
        {
            cpu.Reg.SP--;
            Write(cpu, cpu.Reg.SP, (byte)(value >> 8));
            cpu.Reg.SP--;
            Write(cpu, cpu.Reg.SP, (byte)value);
        }

        static ushort Pop(Cpu cpu)
        {
            byte low = Read(cpu, cpu.Reg.SP++);
            byte high = Read(cpu, cpu.Reg.SP++);
            return (ushort)(low | (high << 8));
        }
    }
}
