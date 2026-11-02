using System;
using GameBoy.Emulators.Common.Cpus.Structs;
using GameBoy.Emulators.Common.Opcodes;

namespace GameBoy.Emulators.Common.Cpus.Executors
{
    public static class ExecutorSet
    {
        #region Executor

        public static Executor GetExecutor(byte opcode) => default(Executor);

        // public static void Execute(Cpu cpu)
        // {
        //     executor.Execute(cpu);
        // }

        #endregion

        #region 00-0F

        public static void _00(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _01(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.BC = Op.Make16(w, z);
        }

        public static void _02(Cpu cpu)
        {
            Op.Write(cpu, cpu.Reg.BC, cpu.Reg.A);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _03(Cpu cpu)
        {
            cpu.Reg.BC += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _04(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.B += 1;
            cpu.Reg.z = cpu.Reg.B == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = (cpu.Reg.B & 0x0F) == 0;
        }

        public static void _05(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.B -= 1;
            cpu.Reg.z = cpu.Reg.B == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (cpu.Reg.B & 0x0F) == 0x0F;
        }

        public static void _06(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.B = z;
        }

        public static void _07(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.c = (cpu.Reg.A & 0x80) == 0x80;
            cpu.Reg.A = (byte)((cpu.Reg.A << 1) | (cpu.Reg.A >> 7));
            cpu.Reg.z = false;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
        }

        public static void _08(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            ushort wz = Op.Make16(w, z);
            Op.Write(cpu, wz, Op.MakeLow(cpu.Reg.SP));
            wz += 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, wz, Op.MakeHigh(cpu.Reg.SP));
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _09(Cpu cpu)
        {
            ushort v1 = cpu.Reg.HL;
            ushort v2 = cpu.Reg.BC;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(v1, v2);
            cpu.Reg.c = Op.D_ADD(v1, v2);
            cpu.Reg.HL += v2;
        }

        public static void _0A(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.BC);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.A = z;
        }

        public static void _0B(Cpu cpu)
        {
            cpu.Reg.BC -= 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _0C(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.C += 1;
            cpu.Reg.z = cpu.Reg.C == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = (cpu.Reg.C & 0x0F) == 0;
        }

        public static void _0D(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.C -= 1;
            cpu.Reg.z = cpu.Reg.C == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (cpu.Reg.C & 0x0F) == 0x0F;
        }

        public static void _0E(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.C = z;
        }

        public static void _0F(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.c = (cpu.Reg.A & 0x01) == 0x01;
            cpu.Reg.A = (byte)((cpu.Reg.A >> 1) | (cpu.Reg.A << 7));
            cpu.Reg.z = false;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
        }

        #endregion

        #region 10-1F

        public static void _10(Cpu cpu)
        {
            // byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            // just stop the GameBoy
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            throw new Exception("GameBoy Stop Running");
        }

        public static void _11(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.DE = Op.Make16(w, z);
        }

        public static void _12(Cpu cpu)
        {
            Op.Write(cpu, cpu.Reg.DE, cpu.Reg.A);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _13(Cpu cpu)
        {
            cpu.Reg.DE += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _14(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.D += 1;
            cpu.Reg.z = cpu.Reg.D == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = (cpu.Reg.D & 0x0F) == 0;
        }

        public static void _15(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.D -= 1;
            cpu.Reg.z = cpu.Reg.D == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (cpu.Reg.D & 0x0F) == 0x0F;
        }

        public static void _16(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.D = z;
        }

        public static void _17(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            int cMask = cpu.Reg.c ? 0x01 : 0x00;
            cpu.Reg.z = false;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = (cpu.Reg.A & 0x80) == 1;
            cpu.Reg.A = (byte)((cpu.Reg.A << 1) | cMask);
        }

        public static void _18(Cpu cpu)
        {
            sbyte z = (sbyte)Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.PC += (ushort)z;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _19(Cpu cpu)
        {
            ushort v1 = cpu.Reg.HL;
            ushort v2 = cpu.Reg.DE;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(v1, v2);
            cpu.Reg.c = Op.D_ADD(v1, v2);
            cpu.Reg.HL += v2;
        }

        public static void _1A(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.DE);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.A = z;
        }

        public static void _1B(Cpu cpu)
        {
            cpu.Reg.DE -= 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _1C(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.E += 1;
            cpu.Reg.z = cpu.Reg.E == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = (cpu.Reg.E & 0x0F) == 0;
        }

        public static void _1D(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.E -= 1;
            cpu.Reg.z = cpu.Reg.E == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (cpu.Reg.E & 0x0F) == 0x0F;
        }

        public static void _1E(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.E = z;
        }

        public static void _1F(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            int cMask = cpu.Reg.c ? 0x80 : 0x00;
            cpu.Reg.z = false;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = (cpu.Reg.A & 0x01) == 1;
            cpu.Reg.A = (byte)((cpu.Reg.A >> 1) | cMask);
        }

        #endregion

        #region 20-2F

        public static void _20(Cpu cpu)
        {
            sbyte z = (sbyte)Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (!cpu.Reg.z)
            {
                cpu.Reg.PC += (ushort)z;
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _21(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.HL = Op.Make16(w, z);
        }

        public static void _22(Cpu cpu)
        {
            Op.Write(cpu, cpu.Reg.HL, cpu.Reg.A);
            cpu.Reg.HL += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _23(Cpu cpu)
        {
            cpu.Reg.HL += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _24(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.H += 1;
            cpu.Reg.z = cpu.Reg.H == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = (cpu.Reg.H & 0x0F) == 0;
        }

        public static void _25(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.H -= 1;
            cpu.Reg.z = cpu.Reg.H == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (cpu.Reg.H & 0x0F) == 0x0F;
        }

        public static void _26(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.H = z;
        }

        public static void _27(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            if (cpu.Reg.n)
            {
                if (cpu.Reg.c)
                {
                    if (cpu.Reg.h)
                    {
                        cpu.Reg.A += 0x9A;
                    }
                    else
                    {
                        cpu.Reg.A += 0xA0;
                    }
                }
                else
                {
                    if (cpu.Reg.h)
                    {
                        cpu.Reg.A += 0xFA;
                    }
                }
            }
            else
            {
                if (cpu.Reg.c || cpu.Reg.A > 0x99)
                {
                    if (cpu.Reg.h || (cpu.Reg.A & 0x0F) > 0x09)
                    {
                        cpu.Reg.A += 0x66;
                    }
                    else
                    {
                        cpu.Reg.A += 0x60;
                    }

                    cpu.Reg.c = true;
                }
                else
                {
                    if (cpu.Reg.h || (cpu.Reg.A & 0x0F) > 0x09)
                    {
                        cpu.Reg.A += 0x06;
                    }
                }
            }

            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.h = false;
        }

        public static void _28(Cpu cpu)
        {
            sbyte z = (sbyte)Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (cpu.Reg.z)
            {
                cpu.Reg.PC += (ushort)z;
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _29(Cpu cpu)
        {
            ushort v1 = cpu.Reg.HL;
            ushort v2 = cpu.Reg.HL;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(v1, v2);
            cpu.Reg.c = Op.D_ADD(v1, v2);
            cpu.Reg.HL += v2;
        }

        public static void _2A(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.Reg.HL += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.A = z;
        }

        public static void _2B(Cpu cpu)
        {
            cpu.Reg.HL -= 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _2C(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.L += 1;
            cpu.Reg.z = cpu.Reg.L == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = (cpu.Reg.L & 0x0F) == 0;
        }

        public static void _2D(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.L -= 1;
            cpu.Reg.z = cpu.Reg.L == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (cpu.Reg.L & 0x0F) == 0x0F;
        }

        public static void _2E(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.L = z;
        }

        public static void _2F(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.A = (byte)~cpu.Reg.A;
            cpu.Reg.n = true;
            cpu.Reg.h = true;
        }

        #endregion

        #region 30-3F

        public static void _30(Cpu cpu)
        {
            sbyte z = (sbyte)Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (!cpu.Reg.c)
            {
                cpu.Reg.PC += (ushort)z;
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _31(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.SP = Op.Make16(w, z);
        }

        public static void _32(Cpu cpu)
        {
            Op.Write(cpu, cpu.Reg.HL, cpu.Reg.A);
            cpu.Reg.HL -= 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _33(Cpu cpu)
        {
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _34(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            z += 1;
            Op.Write(cpu, cpu.Reg.HL, z);
            cpu.ClockCounter += 4;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            cpu.Reg.z = z == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = (z & 0x0F) == 0;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _35(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            z -= 1;
            Op.Write(cpu, cpu.Reg.HL, z);
            cpu.ClockCounter += 4;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            cpu.Reg.z = z == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = (z & 0x0F) == 0x0F;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _36(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.HL, z);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _37(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = true;
        }

        public static void _38(Cpu cpu)
        {
            sbyte z = (sbyte)Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (cpu.Reg.c)
            {
                cpu.Reg.PC += (ushort)z;
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _39(Cpu cpu)
        {
            ushort v1 = cpu.Reg.HL;
            ushort v2 = cpu.Reg.SP;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(v1, v2);
            cpu.Reg.c = Op.D_ADD(v1, v2);
            cpu.Reg.HL += v2;
        }

        public static void _3A(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.Reg.HL -= 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.A = z;
        }

        public static void _3B(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _3C(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.A += 1;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = (cpu.Reg.A & 0x0F) == 0;
        }

        public static void _3D(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.A -= 1;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (cpu.Reg.A & 0x0F) == 0x0F;
        }

        public static void _3E(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.A = z;
        }

        public static void _3F(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = !cpu.Reg.c;
        }

        #endregion

        #region 40-4F

        public static void _40(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            // LD B,B: 自赋值, 空操作
        }

        public static void _41(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.B = cpu.Reg.C;
        }

        public static void _42(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.B = cpu.Reg.D;
        }

        public static void _43(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.B = cpu.Reg.E;
        }

        public static void _44(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.B = cpu.Reg.H;
        }

        public static void _45(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.B = cpu.Reg.L;
        }

        public static void _46(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.B = z;
        }

        public static void _47(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.B = cpu.Reg.A;
        }

        public static void _48(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.C = cpu.Reg.B;
        }

        public static void _49(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            // LD C,C: 自赋值, 空操作
        }

        public static void _4A(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.C = cpu.Reg.D;
        }

        public static void _4B(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.C = cpu.Reg.E;
        }

        public static void _4C(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.C = cpu.Reg.H;
        }

        public static void _4D(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.C = cpu.Reg.L;
        }

        public static void _4E(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.C = z;
        }

        public static void _4F(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.C = cpu.Reg.A;
        }

        #endregion

        #region 50-5F

        public static void _50(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.D = cpu.Reg.B;
        }

        public static void _51(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.D = cpu.Reg.C;
        }

        public static void _52(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            // LD D,D: 自赋值, 空操作
        }

        public static void _53(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.D = cpu.Reg.E;
        }

        public static void _54(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.D = cpu.Reg.H;
        }

        public static void _55(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.D = cpu.Reg.L;
        }

        public static void _56(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.D = z;
        }

        public static void _57(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.D = cpu.Reg.A;
        }

        public static void _58(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.E = cpu.Reg.B;
        }

        public static void _59(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.E = cpu.Reg.C;
        }

        public static void _5A(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.E = cpu.Reg.D;
        }

        public static void _5B(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            // LD E,E: 自赋值, 空操作
        }

        public static void _5C(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.E = cpu.Reg.H;
        }

        public static void _5D(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.E = cpu.Reg.L;
        }

        public static void _5E(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.E = z;
        }

        public static void _5F(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.E = cpu.Reg.A;
        }

        #endregion

        #region 60-6F

        public static void _60(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.H = cpu.Reg.B;
        }

        public static void _61(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.H = cpu.Reg.C;
        }

        public static void _62(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.H = cpu.Reg.D;
        }

        public static void _63(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.H = cpu.Reg.E;
        }

        public static void _64(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            // LD H,H: 自赋值, 空操作
        }

        public static void _65(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.H = cpu.Reg.L;
        }

        public static void _66(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.H = z;
        }

        public static void _67(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.H = cpu.Reg.A;
        }

        public static void _68(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.L = cpu.Reg.B;
        }

        public static void _69(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.L = cpu.Reg.C;
        }

        public static void _6A(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.L = cpu.Reg.D;
        }

        public static void _6B(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.L = cpu.Reg.E;
        }

        public static void _6C(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.L = cpu.Reg.H;
        }

        public static void _6D(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            // LD L,L: 自赋值, 空操作
        }

        public static void _6E(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.L = z;
        }

        public static void _6F(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.L = cpu.Reg.A;
        }

        #endregion

        #region 70-7F

        public static void _70(Cpu cpu)
        {
            Op.Write(cpu, cpu.Reg.HL, cpu.Reg.B);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _71(Cpu cpu)
        {
            Op.Write(cpu, cpu.Reg.HL, cpu.Reg.C);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _72(Cpu cpu)
        {
            Op.Write(cpu, cpu.Reg.HL, cpu.Reg.D);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _73(Cpu cpu)
        {
            Op.Write(cpu, cpu.Reg.HL, cpu.Reg.E);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _74(Cpu cpu)
        {
            Op.Write(cpu, cpu.Reg.HL, cpu.Reg.H);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _75(Cpu cpu)
        {
            Op.Write(cpu, cpu.Reg.HL, cpu.Reg.L);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _76(Cpu cpu)
        {
            cpu.Halted = true;
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _77(Cpu cpu)
        {
            Op.Write(cpu, cpu.Reg.HL, cpu.Reg.A);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _78(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.A = cpu.Reg.B;
        }

        public static void _79(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.A = cpu.Reg.C;
        }

        public static void _7A(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.A = cpu.Reg.D;
        }

        public static void _7B(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.A = cpu.Reg.E;
        }

        public static void _7C(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.A = cpu.Reg.H;
        }

        public static void _7D(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.A = cpu.Reg.L;
        }

        public static void _7E(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.A = z;
        }

        public static void _7F(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            // LD A,A: 自赋值, 空操作
        }

        #endregion

        #region 80-8F

        public static void _80(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.B;
            cpu.Reg.A += n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(a, n);
            cpu.Reg.c = Op.D_ADD(a, n);
        }

        public static void _81(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.C;
            cpu.Reg.A += n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(a, n);
            cpu.Reg.c = Op.D_ADD(a, n);
        }

        public static void _82(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.D;
            cpu.Reg.A += n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(a, n);
            cpu.Reg.c = Op.D_ADD(a, n);
        }

        public static void _83(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.E;
            cpu.Reg.A += n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(a, n);
            cpu.Reg.c = Op.D_ADD(a, n);
        }

        public static void _84(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.H;
            cpu.Reg.A += n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(a, n);
            cpu.Reg.c = Op.D_ADD(a, n);
        }

        public static void _85(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.L;
            cpu.Reg.A += n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(a, n);
            cpu.Reg.c = Op.D_ADD(a, n);
        }

        public static void _86(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = z;
            cpu.Reg.A += n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(a, n);
            cpu.Reg.c = Op.D_ADD(a, n);
        }

        public static void _87(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.A;
            cpu.Reg.A += n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(a, n);
            cpu.Reg.c = Op.D_ADD(a, n);
        }

        public static void _88(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.B;
            cpu.Reg.A += (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_ADC(a, n, cpu.Reg.c);
        }

        public static void _89(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.C;
            cpu.Reg.A += (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_ADC(a, n, cpu.Reg.c);
        }

        public static void _8A(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.D;
            cpu.Reg.A += (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_ADC(a, n, cpu.Reg.c);
        }

        public static void _8B(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.E;
            cpu.Reg.A += (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_ADC(a, n, cpu.Reg.c);
        }

        public static void _8C(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.H;
            cpu.Reg.A += (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_ADC(a, n, cpu.Reg.c);
        }

        public static void _8D(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.L;
            cpu.Reg.A += (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_ADC(a, n, cpu.Reg.c);
        }

        public static void _8E(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = z;
            cpu.Reg.A += (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_ADC(a, n, cpu.Reg.c);
        }

        public static void _8F(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.A;
            cpu.Reg.A += (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_ADC(a, n, cpu.Reg.c);
        }

        #endregion

        #region 90-9F

        public static void _90(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.B;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SUB(a, n);
            cpu.Reg.c = Op.D_SUB(a, n);
        }

        public static void _91(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.C;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SUB(a, n);
            cpu.Reg.c = Op.D_SUB(a, n);
        }

        public static void _92(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.D;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SUB(a, n);
            cpu.Reg.c = Op.D_SUB(a, n);
        }

        public static void _93(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.E;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SUB(a, n);
            cpu.Reg.c = Op.D_SUB(a, n);
        }

        public static void _94(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.H;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SUB(a, n);
            cpu.Reg.c = Op.D_SUB(a, n);
        }

        public static void _95(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.L;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SUB(a, n);
            cpu.Reg.c = Op.D_SUB(a, n);
        }

        public static void _96(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = z;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SUB(a, n);
            cpu.Reg.c = Op.D_SUB(a, n);
        }

        public static void _97(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.A;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SUB(a, n);
            cpu.Reg.c = Op.D_SUB(a, n);
        }

        public static void _98(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.B;
            cpu.Reg.A -= (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SBC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_SBC(a, n, cpu.Reg.c);
        }

        public static void _99(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.C;
            cpu.Reg.A -= (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SBC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_SBC(a, n, cpu.Reg.c);
        }

        public static void _9A(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.D;
            cpu.Reg.A -= (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SBC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_SBC(a, n, cpu.Reg.c);
        }

        public static void _9B(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.E;
            cpu.Reg.A -= (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SBC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_SBC(a, n, cpu.Reg.c);
        }

        public static void _9C(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.H;
            cpu.Reg.A -= (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SBC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_SBC(a, n, cpu.Reg.c);
        }

        public static void _9D(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.L;
            cpu.Reg.A -= (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SBC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_SBC(a, n, cpu.Reg.c);
        }

        public static void _9E(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = z;
            cpu.Reg.A -= (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SBC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_SBC(a, n, cpu.Reg.c);
        }

        public static void _9F(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.A;
            cpu.Reg.A -= (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SBC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_SBC(a, n, cpu.Reg.c);
        }

        #endregion

        #region A0-AF

        public static void _A0(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A &= cpu.Reg.B;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
            cpu.Reg.c = false;
        }

        public static void _A1(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A &= cpu.Reg.C;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
            cpu.Reg.c = false;
        }

        public static void _A2(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A &= cpu.Reg.D;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
            cpu.Reg.c = false;
        }

        public static void _A3(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A &= cpu.Reg.E;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
            cpu.Reg.c = false;
        }

        public static void _A4(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A &= cpu.Reg.H;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
            cpu.Reg.c = false;
        }

        public static void _A5(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A &= cpu.Reg.L;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
            cpu.Reg.c = false;
        }

        public static void _A6(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A &= z;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
            cpu.Reg.c = false;
        }

        public static void _A7(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A &= cpu.Reg.A;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
            cpu.Reg.c = false;
        }

        public static void _A8(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A ^= cpu.Reg.B;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _A9(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A ^= cpu.Reg.C;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _AA(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A ^= cpu.Reg.D;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _AB(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A ^= cpu.Reg.E;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _AC(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A ^= cpu.Reg.H;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _AD(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A ^= cpu.Reg.L;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _AE(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A ^= z;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _AF(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A ^= cpu.Reg.A;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        #endregion

        #region B0-BF

        public static void _B0(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte a = cpu.Reg.A |= cpu.Reg.B;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _B1(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte a = cpu.Reg.A |= cpu.Reg.C;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _B2(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte a = cpu.Reg.A |= cpu.Reg.D;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _B3(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte a = cpu.Reg.A |= cpu.Reg.E;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _B4(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte a = cpu.Reg.A |= cpu.Reg.H;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _B5(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte a = cpu.Reg.A |= cpu.Reg.L;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _B6(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A |= z;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _B7(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte a = cpu.Reg.A |= cpu.Reg.A;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _B8(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.B;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (a & 0x0F) < (n & 0x0F);
            cpu.Reg.c = a < n;
        }

        public static void _B9(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.C;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (a & 0x0F) < (n & 0x0F);
            cpu.Reg.c = a < n;
        }

        public static void _BA(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.D;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (a & 0x0F) < (n & 0x0F);
            cpu.Reg.c = a < n;
        }

        public static void _BB(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.E;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (a & 0x0F) < (n & 0x0F);
            cpu.Reg.c = a < n;
        }

        public static void _BC(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.H;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (a & 0x0F) < (n & 0x0F);
            cpu.Reg.c = a < n;
        }

        public static void _BD(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.L;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (a & 0x0F) < (n & 0x0F);
            cpu.Reg.c = a < n;
        }

        public static void _BE(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.HL);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = z;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (a & 0x0F) < (n & 0x0F);
            cpu.Reg.c = a < n;
        }

        public static void _BF(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = cpu.Reg.A;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (a & 0x0F) < (n & 0x0F);
            cpu.Reg.c = a < n;
        }

        #endregion

        #region C0-CF

        public static void _C0(Cpu cpu)
        {
            cpu.ClockCounter += 4;

            if (!cpu.Reg.z)
            {
                byte z = Op.Read(cpu, cpu.Reg.SP);
                cpu.Reg.SP += 1;
                cpu.ClockCounter += 4;

                byte w = Op.Read(cpu, cpu.Reg.SP);
                cpu.Reg.SP += 1;
                cpu.ClockCounter += 4;

                cpu.Reg.PC = Op.Make16(w, z);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _C1(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.BC = Op.Make16(w, z);
        }

        public static void _C2(Cpu cpu)
        {
            byte nn_lsb = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte nn_msb = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (!cpu.Reg.z)
            {
                cpu.Reg.PC = Op.Make16(nn_msb, nn_lsb);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _C3(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.PC = Op.Make16(w, z);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _C4(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (!cpu.Reg.z)
            {
                cpu.Reg.SP -= 1;
                cpu.ClockCounter += 4;

                Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
                cpu.Reg.SP -= 1;
                cpu.ClockCounter += 4;

                Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
                cpu.Reg.PC = Op.Make16(w, z);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _C5(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, cpu.Reg.B);
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, cpu.Reg.C);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _C6(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = z;
            cpu.Reg.A += n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(a, n);
            cpu.Reg.c = Op.D_ADD(a, n);
        }

        public static void _C7(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
            cpu.Reg.PC = 0x00;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _C8(Cpu cpu)
        {
            cpu.ClockCounter += 4;

            if (cpu.Reg.z)
            {
                byte z = Op.Read(cpu, cpu.Reg.SP);
                cpu.Reg.SP += 1;
                cpu.ClockCounter += 4;

                byte w = Op.Read(cpu, cpu.Reg.SP);
                cpu.Reg.SP += 1;
                cpu.ClockCounter += 4;

                cpu.Reg.PC = Op.Make16(w, z);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _C9(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.PC = Op.Make16(w, z);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _CA(Cpu cpu)
        {
            byte nn_lsb = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte nn_msb = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (cpu.Reg.z)
            {
                cpu.Reg.PC = Op.Make16(nn_msb, nn_lsb);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _CB(Cpu cpu)
        {
            // PREFIX CB
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte opcode = cpu.Reg.IR;
            byte z;
            byte opReg = (byte)(opcode & 0x07);
            switch (opReg)
            {
                case 0x00:
                    z = cpu.Reg.B;
                    break;
                case 0x01:
                    z = cpu.Reg.C;
                    break;
                case 0x02:
                    z = cpu.Reg.D;
                    break;
                case 0x03:
                    z = cpu.Reg.E;
                    break;
                case 0x04:
                    z = cpu.Reg.H;
                    break;
                case 0x05:
                    z = cpu.Reg.L;
                    break;
                case 0x06:
                    z = Op.Read(cpu, cpu.Reg.HL);
                    cpu.ClockCounter += 4;
                    break;
                case 0x07:
                    z = cpu.Reg.A;
                    break;
                default: throw new Exception("Invalid opReg");
            }

            byte opBit = (byte)((opcode & 0xF8) >> 3);
            switch (opBit)
            {
                case 0x00: // 00000
                    z = Op.CB_RLC(cpu, z);
                    break;
                case 0x01: // 00001
                    z = Op.CB_RRC(cpu, z);
                    break;
                case 0x02: // 00010
                    z = Op.CB_RL(cpu, z);
                    break;
                case 0x03: // 00011
                    z = Op.CB_RR(cpu, z);
                    break;
                case 0x04: // 00100
                    z = Op.CB_SLA(cpu, z);
                    break;
                case 0x05: // 00101
                    z = Op.CB_SRA(cpu, z);
                    break;
                case 0x06: // 00110
                    z = Op.CB_SWAP(cpu, z);
                    break;
                case 0x07: // 00111
                    z = Op.CB_SRL(cpu, z);
                    break;
                default:
                    if (opBit <= 0x0F)
                    {
                        z = Op.CB_BIT(cpu, z, (byte)(opBit - 0x08));
                        break;
                    }
                    else if (opBit <= 0x1F)
                    {
                        z = Op.CB_RES(cpu, z, (byte)(opBit - 0x10));
                        break;
                    }
                    else if (opBit <= 0x2F)
                    {
                        z = Op.CB_SET(cpu, z, (byte)(opBit - 0x18));
                        break;
                    }
                    else
                    {
                        throw new Exception("Invalid opBit");
                    }
            }

            if (opReg == 0x06)
            {
                if (0x07 < opBit && opBit <= 0x0F)
                {
                    // bit not write
                }
                else
                {
                    Op.Write(cpu, cpu.Reg.HL, z);
                    cpu.ClockCounter += 4;
                }

                cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
                cpu.ProgramCounter += 1;
                cpu.ClockCounter += 4;
            }
            else
            {
                cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
                cpu.ProgramCounter += 1;
                cpu.ClockCounter += 4;
                switch (opReg)
                {
                    case 0x00:
                        cpu.Reg.B = z;
                        break;
                    case 0x01:
                        cpu.Reg.C = z;
                        break;
                    case 0x02:
                        cpu.Reg.D = z;
                        break;
                    case 0x03:
                        cpu.Reg.E = z;
                        break;
                    case 0x04:
                        cpu.Reg.H = z;
                        break;
                    case 0x05:
                        cpu.Reg.L = z;
                        break;
                    case 0x07:
                        cpu.Reg.A = z;
                        break;
                }
            }
        }

        public static void _CC(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (cpu.Reg.z)
            {
                cpu.Reg.SP -= 1;
                cpu.ClockCounter += 4;

                Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
                cpu.Reg.SP -= 1;
                cpu.ClockCounter += 4;

                Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
                cpu.Reg.PC = Op.Make16(w, z);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _CD(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
            cpu.Reg.PC = Op.Make16(w, z);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _CE(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = z;
            cpu.Reg.A += (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_ADC(a, n, cpu.Reg.c);
        }

        public static void _CF(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
            cpu.Reg.PC = 0x08;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        #endregion

        #region D0-DF

        public static void _D0(Cpu cpu)
        {
            cpu.ClockCounter += 4;

            if (!cpu.Reg.c)
            {
                byte z = Op.Read(cpu, cpu.Reg.SP);
                cpu.Reg.SP += 1;
                cpu.ClockCounter += 4;

                byte w = Op.Read(cpu, cpu.Reg.SP);
                cpu.Reg.SP += 1;
                cpu.ClockCounter += 4;

                cpu.Reg.PC = Op.Make16(w, z);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _D1(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.DE = Op.Make16(w, z);
        }

        public static void _D2(Cpu cpu)
        {
            byte nn_lsb = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte nn_msb = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (!cpu.Reg.c)
            {
                cpu.Reg.PC = Op.Make16(nn_msb, nn_lsb);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _D3(Cpu cpu)
        {
            throw new NotImplementedException();
        }

        public static void _D4(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (!cpu.Reg.c)
            {
                cpu.Reg.SP -= 1;
                cpu.ClockCounter += 4;

                Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
                cpu.Reg.SP -= 1;
                cpu.ClockCounter += 4;

                Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
                cpu.Reg.PC = Op.Make16(w, z);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _D5(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, cpu.Reg.D);
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, cpu.Reg.E);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _D6(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = z;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SUB(a, n);
            cpu.Reg.c = Op.D_SUB(a, n);
        }

        public static void _D7(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
            cpu.Reg.PC = 0x10;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _D8(Cpu cpu)
        {
            cpu.ClockCounter += 4;

            if (cpu.Reg.c)
            {
                byte z = Op.Read(cpu, cpu.Reg.SP);
                cpu.Reg.SP += 1;
                cpu.ClockCounter += 4;

                byte w = Op.Read(cpu, cpu.Reg.SP);
                cpu.Reg.SP += 1;
                cpu.ClockCounter += 4;

                cpu.Reg.PC = Op.Make16(w, z);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _D9(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.PC = Op.Make16(w, z);
            cpu.IME = true;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _DA(Cpu cpu)
        {
            byte nn_lsb = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte nn_msb = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (cpu.Reg.c)
            {
                cpu.Reg.PC = Op.Make16(nn_msb, nn_lsb);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _DB(Cpu cpu)
        {
            throw new NotImplementedException();
        }

        public static void _DC(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            if (cpu.Reg.c)
            {
                cpu.Reg.SP -= 1;
                cpu.ClockCounter += 4;

                Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
                cpu.Reg.SP -= 1;
                cpu.ClockCounter += 4;

                Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
                cpu.Reg.PC = Op.Make16(w, z);
                cpu.ClockCounter += 4;
            }

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _DD(Cpu cpu)
        {
            throw new NotImplementedException();
        }

        public static void _DE(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = z;
            cpu.Reg.A -= (byte)(n + (cpu.Reg.c ? 1 : 0));
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = Op.DH_SBC(a, n, cpu.Reg.c);
            cpu.Reg.c = Op.D_SBC(a, n, cpu.Reg.c);
        }

        public static void _DF(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
            cpu.Reg.PC = 0x18;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        #endregion

        #region E0-EF

        public static void _E0(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, Op.Make16(0xFF, z), cpu.Reg.A);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _E1(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.HL = Op.Make16(w, z);
        }

        public static void _E2(Cpu cpu)
        {
            Op.Write(cpu, Op.Make16(0xFF, cpu.Reg.C), cpu.Reg.A);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _E3(Cpu cpu)
        {
            throw new NotImplementedException();
        }

        public static void _E4(Cpu cpu)
        {
            throw new NotImplementedException();
        }

        public static void _E5(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, cpu.Reg.H);
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, cpu.Reg.L);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _E6(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A &= z;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = true;
            cpu.Reg.c = false;
        }

        public static void _E7(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
            cpu.Reg.PC = 0x20;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _E8(Cpu cpu)
        {
            ushort v1 = cpu.Reg.SP;
            ushort v2 = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.z = false;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(v1, v2);
            cpu.Reg.c = Op.D_ADD(v1, v2);
            cpu.ClockCounter += 4;

            ushort r = (ushort)(v1 + v2);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.HL = r;
        }

        public static void _E9(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.Reg.HL);
            cpu.Reg.PC = cpu.Reg.HL;
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _EA(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, Op.Make16(w, z), cpu.Reg.A);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _EB(Cpu cpu)
        {
            throw new NotImplementedException();
        }

        public static void _EC(Cpu cpu)
        {
            throw new NotImplementedException();
        }

        public static void _ED(Cpu cpu)
        {
            throw new NotImplementedException();
        }

        public static void _EE(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A ^= z;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _EF(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
            cpu.Reg.PC = 0x28;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        #endregion

        #region F0-FF

        public static void _F0(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            z = Op.Read(cpu, Op.Make16(0xFF, z));
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.A = z;
        }

        public static void _F1(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.Reg.SP);
            cpu.Reg.SP += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.AF = Op.Make16(w, z);
        }

        public static void _F2(Cpu cpu)
        {
            byte z = Op.Read(cpu, Op.Make16(0xFF, cpu.Reg.C));
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.A = z;
        }

        public static void _F3(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.DisableInterruptMaster();
        }

        public static void _F4(Cpu cpu)
        {
            throw new NotImplementedException();
        }

        public static void _F5(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, cpu.Reg.A);
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, cpu.Reg.F);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _F6(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A |= z;
            cpu.Reg.z = a == 0;
            cpu.Reg.n = false;
            cpu.Reg.h = false;
            cpu.Reg.c = false;
        }

        public static void _F7(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
            cpu.Reg.PC = 0x30;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _F8(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            ushort sp = cpu.Reg.SP;
            ushort r = (ushort)(sp + z);
            cpu.Reg.z = false;
            cpu.Reg.n = false;
            cpu.Reg.h = Op.DH_ADD(sp, z);
            cpu.Reg.c = Op.D_ADD(sp, z);
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.HL = r;
        }

        public static void _F9(Cpu cpu)
        {
            cpu.Reg.SP = cpu.Reg.HL;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        public static void _FA(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            byte w = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            z = Op.Read(cpu, Op.Make16(w, z));

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.Reg.A = z;
        }

        public static void _FB(Cpu cpu)
        {
            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            cpu.EnableInterruptMaster();
        }

        public static void _FC(Cpu cpu)
        {
            throw new NotImplementedException();
        }

        public static void _FD(Cpu cpu)
        {
            throw new NotImplementedException();
        }

        public static void _FE(Cpu cpu)
        {
            byte z = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            byte a = cpu.Reg.A;
            byte n = z;
            cpu.Reg.A -= n;
            cpu.Reg.z = cpu.Reg.A == 0;
            cpu.Reg.n = true;
            cpu.Reg.h = (a & 0x0F) < (n & 0x0F);
            cpu.Reg.c = a < n;
        }

        public static void _FF(Cpu cpu)
        {
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeHigh(cpu.Reg.PC));
            cpu.Reg.SP -= 1;
            cpu.ClockCounter += 4;

            Op.Write(cpu, cpu.Reg.SP, Op.MakeLow(cpu.Reg.PC));
            cpu.Reg.PC = 0x38;
            cpu.ClockCounter += 4;

            cpu.Reg.IR = Op.Read(cpu, cpu.ProgramCounter);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
        }

        #endregion
    }
}