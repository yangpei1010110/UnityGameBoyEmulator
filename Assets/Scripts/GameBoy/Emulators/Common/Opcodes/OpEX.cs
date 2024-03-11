﻿namespace GameBoy.Emulators.Common.Opcodes
{
    public static class OpEX
    {
        public static void XE0_LDH_A8_A(Cpu cpu)
        {
            byte address = Op.Read(cpu, cpu.ProgramCounter + 1);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 4;
            Op.Write(cpu, Ram.MAP_IO_REGISTERS + address, cpu.Reg.A);
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }

        public static void XE2_LDH_C_A(Cpu cpu)
        {
            Op.Write(cpu, Ram.MAP_IO_REGISTERS + cpu.Reg.C, cpu.Reg.A);
            cpu.ProgramCounter += 1;
            cpu.ClockCounter += 8;
        }

        public static void XEA_LD_A16_A(Cpu cpu)
        {
            ushort address = Op.Read16(cpu, cpu.ProgramCounter + 1);
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
            Op.Write(cpu, Ram.MAP_IO_REGISTERS + address, cpu.Reg.A);
            cpu.ProgramCounter += 2;
            cpu.ClockCounter += 8;
        }
    }
}