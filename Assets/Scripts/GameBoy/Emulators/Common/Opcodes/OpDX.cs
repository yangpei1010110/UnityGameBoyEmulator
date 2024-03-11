﻿namespace GameBoy.Emulators.Common.Opcodes
{
    public static class OpDX
    {
        public static void XD0_RET_NC(Cpu cpu)
        {
            if (!cpu.Reg.c)
            {
                cpu.ProgramCounter = Op.Pop16(cpu);
                cpu.ClockCounter += 20;
            }
            else
            {
                cpu.ProgramCounter += 1;
                cpu.ClockCounter += 8;
            }
        }

        public static void XD1_POP_DE(Cpu cpu)
        {
            cpu.Reg.DE = Op.Pop16(cpu);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 12;
        }

        public static void XD2_JP_NC_A16(Cpu cpu)
        {
            if (!cpu.Reg.c)
            {
                cpu.ProgramCounter = Op.Read16(cpu, cpu.ProgramCounter);
                cpu.ClockCounter += 16;
            }
            else
            {
                cpu.ProgramCounter += 3;
                cpu.ClockCounter += 12;
            }
        }

        public static void XD4_CALL_NC_A16(Cpu cpu)
        {
            ushort address = Op.Read16(cpu, cpu.ProgramCounter + 1);
            cpu.ProgramCounter += 3;
            cpu.ClockCounter += 12;
            if (!cpu.Reg.c)
            {
                Op.Push16(cpu, cpu.ProgramCounter);
                cpu.ProgramCounter = address;
                cpu.ClockCounter += 12;
            }
        }

        public static void XD5_PUSH_DE(Cpu cpu)
        {
            Op.Push16(cpu, cpu.Reg.DE);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 16;
        }

        public static void XD7_RST_10H(Cpu cpu)
        {
            Op.Push16(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter = 0x10;
            cpu.ClockCounter += 16;
        }

        public static void XD8_RET_C(Cpu cpu)
        {
            if (cpu.Reg.c)
            {
                cpu.ProgramCounter = Op.Pop16(cpu);
                cpu.ClockCounter += 20;
            }
            else
            {
                cpu.ProgramCounter += 1;
                cpu.ClockCounter += 8;
            }
        }

        public static void XD9_RETI(Cpu cpu)
        {
            cpu.EnableInterruptMaster();
            OpCX.XC9_RET(cpu);
        }

        public static void XDA_JP_C_A16(Cpu cpu)
        {
            if (cpu.Reg.c)
            {
                cpu.ProgramCounter = Op.Read16(cpu, cpu.ProgramCounter);
                cpu.ClockCounter += 16;
            }
            else
            {
                cpu.ProgramCounter += 3;
                cpu.ClockCounter += 12;
            }
        }

        public static void XDC_CALL_C_A16(Cpu cpu)
        {
            ushort address = Op.Read16(cpu, cpu.ProgramCounter + 1);
            cpu.ProgramCounter += 3;
            cpu.ClockCounter += 12;
            if (cpu.Reg.c)
            {
                Op.Push16(cpu, cpu.ProgramCounter);
                cpu.ProgramCounter = address;
                cpu.ClockCounter += 12;
            }
        }

        public static void XDF_RST_18H(Cpu cpu)
        {
            Op.Push16(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter = 0x18;
            cpu.ClockCounter += 16;
        }
    }
}