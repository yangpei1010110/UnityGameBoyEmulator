﻿namespace GameBoy.Emulators.Common.Opcodes
{
    public static class OpBX
    {
        public static void XB0_OR_A_B(Cpu cpu)
        {
            cpu.Reg.A |= cpu.Reg.B;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XB1_OR_A_C(Cpu cpu)
        {
            cpu.Reg.A |= cpu.Reg.C;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XB2_OR_A_D(Cpu cpu)
        {
            cpu.Reg.A |= cpu.Reg.B;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XB3_OR_A_E(Cpu cpu)
        {
            cpu.Reg.A |= cpu.Reg.B;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XB4_OR_A_H(Cpu cpu)
        {
            cpu.Reg.A |= cpu.Reg.B;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XB5_OR_A_L(Cpu cpu)
        {
            cpu.Reg.A |= cpu.Reg.B;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XB6_OR_A_HL(Cpu cpu)
        {
            cpu.Reg.A |= Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XB7_OR_A_A(Cpu cpu)
        {
            cpu.Reg.A |= cpu.Reg.A;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XB8_CP_A_B(Cpu cpu)
        {
            byte v1 = cpu.Reg.A;
            byte v2 = cpu.Reg.B;
            sbyte r = unchecked((sbyte)((sbyte)v1 - (sbyte)v2));
            cpu.Reg.z = r == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (v1 & 0x0F) < (v2 & 0x0F);
            cpu.Reg.c = v1 < v2;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XB9_CP_A_C(Cpu cpu)
        {
            byte v1 = cpu.Reg.A;
            byte v2 = cpu.Reg.C;
            sbyte r = unchecked((sbyte)((sbyte)v1 - (sbyte)v2));
            cpu.Reg.z = r == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (v1 & 0x0F) < (v2 & 0x0F);
            cpu.Reg.c = v1 < v2;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XBA_CP_A_D(Cpu cpu)
        {
            byte v1 = cpu.Reg.A;
            byte v2 = cpu.Reg.D;
            sbyte r = unchecked((sbyte)((sbyte)v1 - (sbyte)v2));
            cpu.Reg.z = r == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (v1 & 0x0F) < (v2 & 0x0F);
            cpu.Reg.c = v1 < v2;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XBB_CP_A_E(Cpu cpu)
        {
            byte v1 = cpu.Reg.A;
            byte v2 = cpu.Reg.E;
            sbyte r = unchecked((sbyte)((sbyte)v1 - (sbyte)v2));
            cpu.Reg.z = r == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (v1 & 0x0F) < (v2 & 0x0F);
            cpu.Reg.c = v1 < v2;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XBC_CP_A_H(Cpu cpu)
        {
            byte v1 = cpu.Reg.A;
            byte v2 = cpu.Reg.H;
            sbyte r = unchecked((sbyte)((sbyte)v1 - (sbyte)v2));
            cpu.Reg.z = r == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (v1 & 0x0F) < (v2 & 0x0F);
            cpu.Reg.c = v1 < v2;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XBD_CP_A_L(Cpu cpu)
        {
            byte v1 = cpu.Reg.A;
            byte v2 = cpu.Reg.L;
            sbyte r = unchecked((sbyte)((sbyte)v1 - (sbyte)v2));
            cpu.Reg.z = r == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (v1 & 0x0F) < (v2 & 0x0F);
            cpu.Reg.c = v1 < v2;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void XBE_CP_A_HL(Cpu cpu)
        {
            byte v1 = cpu.Reg.A;
            byte v2 = Op.Read(cpu, cpu.Reg.HL);
            sbyte r = unchecked((sbyte)((sbyte)v1 - (sbyte)v2));
            cpu.Reg.z = r == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (v1 & 0x0F) < (v2 & 0x0F);
            cpu.Reg.c = v1 < v2;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 8;
        }

        public static void XBF_CP_A_A(Cpu cpu)
        {
            byte v1 = cpu.Reg.A;
            byte v2 = cpu.Reg.A;
            sbyte r = unchecked((sbyte)((sbyte)v1 - (sbyte)v2));
            cpu.Reg.z = r == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (v1 & 0x0F) < (v2 & 0x0F);
            cpu.Reg.c = v1 < v2;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }
    }
}