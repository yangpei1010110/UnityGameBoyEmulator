using System;
using System.Collections.Generic;
using GameBoy.Emulators.Common.Cpus;
using GameBoy.Emulators.Common.Cpus.Structs;

namespace GameBoy.Emulators.Common.Opcodes
{
    // https://www.pastraiser.com/cpu/gameboy/gameboy_opcodes.html
    // https://rgbds.gbdev.io/docs/v0.7.0/gbz80.7
    // https://gbdev.io/gb-opcodes/optables/
    public sealed class CpuOp
    {
        public static readonly Action<Cpu>[] Instruction = new Action<Cpu>[]
        {
            // 0x00 - 0x0F
            Op0X.X00_NOP, Op0X.X01_LD_BC_N16, Op0X.X02_LD_BC_A, Op0X.X03_INC_BC,
            Op0X.X04_INC_B, Op0X.X05_DEC_B, Op0X.X06_LD_B_N8, Op0X.X07_RLCA,
            Op0X.X08_LD_A16_SP, Op0X.X09_ADD_HL_BC, Op0X.X0A_LD_A_BC, Op0X.X0B_DEC_BC,
            Op0X.X0C_INC_C, Op0X.X0D_DEC_C, Op0X.X0E_LD_C_N8, Op0X.X0F_RRCA,
            // 0x10 - 0x1F
            Op1X.X10_STOP_N8, Op1X.X11_LD_DE_N16, Op1X.X12_LD_DE_A, Op1X.X13_INC_DE,
            Op1X.X14_INC_D, Op1X.X15_DEC_D, Op1X.X16_LD_D_N8, Op1X.X17_RLA,
            Op1X.X18_JR_E8, Op1X.X19_ADD_HL_DE, Op1X.X1A_LD_A_DE, Op1X.X1B_DEC_DE,
            Op1X.X1C_INC_E, Op1X.X1D_DEC_E, Op1X.X1E_LD_E_N8, Op1X.X1F_RRA,
            // 0x20 - 0x2F
            Op2X.X20_JR_NZ_E8, Op2X.X21_LD_HL_N16, Op2X.X22_LD_HLi_A, Op2X.X23_INC_HL,
            Op2X.X24_INC_H, Op2X.X25_DEC_H, Op2X.X26_LD_H_N8, Op2X.X27_DAA,
            Op2X.X28_JR_Z_E8, Op2X.X29_ADD_HL_HL, Op2X.X2A_LD_A_HLi, Op2X.X2B_DEC_HL,
            Op2X.X2C_INC_L, Op2X.X2D_DEC_L, Op2X.X2E_LD_L_N8, Op2X.X2F_CPL,
            // 0x30 - 0x3F
            Op3X.X30_JR_NZ_E8, Op3X.X31_LD_SP_N16, Op3X.X32_LD_HLd_A, Op3X.X33_INC_SP,
            Op3X.X34_INC_HL, Op3X.X35_DEC_HL, Op3X.X36_LD_HL_N8, Op3X.X37_SCF,
            Op3X.X38_JR_NZ_E8, Op3X.X39_ADD_HL_SP, Op3X.X3A_LD_A_HLd, Op3X.X3B_DEC_SP,
            Op3X.X3C_INC_A, Op3X.X3D_DEC_A, Op3X.X3E_LD_A_N8, Op3X.X3F_CCF,
            // 0x40 - 0x4F
            Op4X.X40_LD_B_B, Op4X.X41_LD_B_C, Op4X.X42_LD_B_D, Op4X.X43_LD_B_E,
            Op4X.X44_LD_B_H, Op4X.X45_LD_B_L, Op4X.X46_LD_B_HL, Op4X.X47_LD_B_A,
            Op4X.X48_LD_C_B, Op4X.X49_LD_C_C, Op4X.X4A_LD_C_D, Op4X.X4B_LD_C_E,
            Op4X.X4C_LD_C_H, Op4X.X4D_LD_C_L, Op4X.X4E_LD_C_HL, Op4X.X4F_LD_C_A,
            // 0x50 - 0x5F
            Op5X.X50_LD_D_B, Op5X.X51_LD_D_C, Op5X.X52_LD_D_D, Op5X.X53_LD_D_E,
            Op5X.X54_LD_D_H, Op5X.X55_LD_D_L, Op5X.X56_LD_D_HL, Op5X.X57_LD_D_A,
            Op5X.X58_LD_E_B, Op5X.X59_LD_E_C, Op5X.X5A_LD_E_D, Op5X.X5B_LD_E_E,
            Op5X.X5C_LD_E_H, Op5X.X5D_LD_E_L, Op5X.X5E_LD_E_HL, Op5X.X5F_LD_E_A,
            // 0x60 - 0x6F
            Op6X.X60_LD_H_B, Op6X.X61_LD_H_C, Op6X.X62_LD_H_D, Op6X.X63_LD_H_E,
            Op6X.X64_LD_H_H, Op6X.X65_LD_H_L, Op6X.X66_LD_H_HL, Op6X.X67_LD_H_A,
            Op6X.X68_LD_L_B, Op6X.X69_LD_L_C, Op6X.X6A_LD_L_D, Op6X.X6B_LD_L_E,
            Op6X.X6C_LD_L_H, Op6X.X6D_LD_L_L, Op6X.X6E_LD_L_HL, Op6X.X6F_LD_L_A,
            // 0x70 - 0x7F
            Op7X.X70_LD_HL_B, Op7X.X71_LD_HL_C, Op7X.X72_LD_HL_D, Op7X.X73_LD_HL_E,
            Op7X.X74_LD_HL_H, Op7X.X75_LD_HL_L, NOT_IMP, Op7X.X77_LD_HL_A,
            Op7X.X78_LD_A_B, Op7X.X79_LD_A_C, Op7X.X7A_LD_A_D, Op7X.X7B_LD_A_E,
            Op7X.X7C_LD_A_H, Op7X.X7D_LD_A_L, Op7X.X7E_LD_A_HL, Op7X.X7F_LD_A_A,
            // 0x80 - 0x8F
            Op8X.X80_ADD_A_B, Op8X.X81_ADD_A_C, Op8X.X82_ADD_A_D, Op8X.X83_ADD_A_E,
            Op8X.X84_ADD_A_H, Op8X.X85_ADD_A_L, Op8X.X86_ADD_A_HL, Op8X.X87_ADD_A_A,
            Op8X.X88_ADC_A_B, Op8X.X89_ADC_A_C, Op8X.X8A_ADC_A_D, Op8X.X8B_ADC_A_E,
            Op8X.X8C_ADC_A_H, Op8X.X8D_ADC_A_L, Op8X.X8E_ADC_A_HL, Op8X.X8F_ADC_A_A,
            // 0x90 - 0x9F
            Op9X.X90_SUB_A_B, Op9X.X91_SUB_A_C, Op9X.X92_SUB_A_D, Op9X.X93_SUB_A_E,
            Op9X.X94_SUB_A_H, Op9X.X95_SUB_A_L, Op9X.X96_SUB_A_HL, Op9X.X97_SUB_A_A,
            Op9X.X98_SUC_A_B, Op9X.X99_SUC_A_C, Op9X.X9A_SUC_A_D, Op9X.X9B_SUC_A_E,
            Op9X.X9C_SUC_A_H, Op9X.X9D_SUC_A_L, Op9X.X9E_SUC_A_HL, Op9X.X9F_SUC_A_A,
            // 0xA0 - 0xAF
            OpAX.XA0_AND_A_B, OpAX.XA1_AND_A_C, OpAX.XA2_AND_A_D, OpAX.XA3_AND_A_E,
            OpAX.XA4_AND_A_H, OpAX.XA5_AND_A_L, OpAX.XA6_AND_A_HL, OpAX.XA7_AND_A_A,
            OpAX.XA8_XOR_A_B, OpAX.XA9_XOR_A_C, OpAX.XAA_XOR_A_D, OpAX.XAB_XOR_A_E,
            OpAX.XAC_XOR_A_H, OpAX.XAD_XOR_A_L, OpAX.XAE_XOR_A_HL, OpAX.XAF_XOR_A_A,
            // 0xB0 - 0xBF
            OpBX.XB0_OR_A_B, OpBX.XB1_OR_A_C, OpBX.XB2_OR_A_D, OpBX.XB3_OR_A_E,
            OpBX.XB4_OR_A_H, OpBX.XB5_OR_A_L, OpBX.XB6_OR_A_HL, OpBX.XB7_OR_A_A,
            OpBX.XB8_CP_A_B, OpBX.XB9_CP_A_C, OpBX.XBA_CP_A_D, OpBX.XBB_CP_A_E,
            OpBX.XBC_CP_A_H, OpBX.XBD_CP_A_L, OpBX.XBE_CP_A_HL, OpBX.XBF_CP_A_A,
            // 0xC0 - 0xCF
            OpCX.XC0_RET_NZ, OpCX.XC1_POP_BC, OpCX.XC2_JP_NZ_A16, OpCX.XC3_JP_A16,
            OpCX.XC4_CALL_NZ_A16, OpCX.XC5_PUSH_BC, OpCX.XC6_ADD_A_N8, OpCX.XC7_RST_00H,
            OpCX.XC8_RET_Z, OpCX.XC9_RET, OpCX.XCA_JP_Z_A16, OpCX.XCB_PREFIX,
            OpCX.XCC_CALL_Z_A16, OpCX.XCD_CALL_A16, NOT_IMP, OpCX.XCF_RST_08H,
            // 0xD0 - 0xDF
            OpDX.XD0_RET_NC, OpDX.XD1_POP_DE, OpDX.XD2_JP_NC_A16, NOT_IMP,
            OpDX.XD4_CALL_NC_A16, OpDX.XD5_PUSH_DE, OpDX.XD6_SUB_N8, OpDX.XD7_RST_10H,
            OpDX.XD8_RET_C, OpDX.XD9_RETI, OpDX.XDA_JP_C_A16, NOT_IMP,
            OpDX.XDC_CALL_C_A16, NOT_IMP, OpDX.XDE_SUC_A_N8, OpDX.XDF_RST_18H,
            // 0xE0 - 0xEF
            OpEX.XE0_LDH_A8_A, OpEX.XE1_POP_HL, OpEX.XE2_LDH_C_A, NOT_IMP,
            NOT_IMP, OpEX.XE5_PUSH_HL, NOT_IMP, OpEX.XE7_RST_20H,
            OpEX.XE8_ADD_SP_E8, OpEX.XE9_JP_HL, OpEX.XEA_LD_A16_A, NOT_IMP,
            NOT_IMP, NOT_IMP, OpEX.XEE_XOR_A_N8, OpEX.XEF_RST_28H,
            // 0xF0 - 0xFF
            OpFX.XF0_LDH_A_A8, OpFX.XF1_POP_AF, OpFX.XF2_LDH_A_C, OpFX.XF3_DI,
            NOT_IMP, OpFX.XF5_PUSH_AF, OpFX.XF6_OR_A_N8, OpFX.XF7_RST_30H,
            OpFX.XF8_LD_HL_SP_N8, OpFX.XF9_LD_SP_HL, OpFX.XFA_LD_A_A16, NOT_IMP,
            NOT_IMP, NOT_IMP, OpFX.XFE_CP_A_N8, OpFX.XFF_RST_38H,
        };

        public static void NOT_IMP(Cpu cpu)
        {
            throw new Exception("NotImplemented");
        }


        #region Step Increments

        public const byte INT_VBLANK   = 1;
        public const byte INT_LCD_STAT = 2;
        public const byte INT_TIMER    = 4;
        public const byte INT_SERIAL   = 8;
        public const byte INT_JOYPAD   = 16;

        public static void ServiceInterrupt(Cpu cpu)
        {
            byte intFlags = (byte)(cpu._intFlags & cpu._intEnableFlags);
            byte serviceInt = 0;
            if ((intFlags & INT_VBLANK) != 0)
            {
                serviceInt = INT_VBLANK;
            }
            else if ((intFlags & INT_LCD_STAT) != 0)
            {
                serviceInt = INT_LCD_STAT;
            }
            else if ((intFlags & INT_TIMER) != 0)
            {
                serviceInt = INT_TIMER;
            }
            else if ((intFlags & INT_SERIAL) != 0)
            {
                serviceInt = INT_SERIAL;
            }
            else if ((intFlags & INT_JOYPAD) != 0)
            {
                serviceInt = INT_JOYPAD;
            }

            if (serviceInt == 0)
            {
                throw new Exception("No interrupt to service");
            }

            cpu._intFlags &= (byte)~serviceInt;
            cpu.DisableInterruptMaster();
            cpu.ClockCounter += 8;
            Op.Push16(cpu, cpu.ProgramCounter);
            cpu.ClockCounter += 8;
            switch (serviceInt)
            {
                case INT_VBLANK:
                    cpu.ProgramCounter = 0x40;
                    break;
                case INT_LCD_STAT:
                    cpu.ProgramCounter = 0x48;
                    break;
                case INT_TIMER:
                    cpu.ProgramCounter = 0x50;
                    break;
                case INT_SERIAL:
                    cpu.ProgramCounter = 0x58;
                    break;
                case INT_JOYPAD:
                    cpu.ProgramCounter = 0x60;
                    break;
            }

            cpu.ClockCounter += 4;
        }

        public static void StepByExecutor(Cpu cpu, HashSet<byte> opcodes = null)
        {
            Joypad.JoypadTick(cpu);
            Sm83.Step(cpu);
            if (opcodes != null)
            {
                opcodes.Add(cpu.Reg.IR);
            }
        }

        // public static void Step(Cpu cpu, HashSet<byte> opcodes = null)
        // {
        //     Joypad.JoypadTick(cpu);
        //     Cpu.TimerTick(cpu);
        //     // Ppu.PpuTick(cpu);
        //     if (!cpu.Halted)
        //     {
        //         if (cpu._ime && cpu._intFlags != 0 && cpu._intEnableFlags != 0)
        //         {
        //             ServiceInterrupt(cpu);
        //         }
        //         else
        //         {
        //             byte opcode = Op.Read(cpu, cpu.ProgramCounter);
        //             if (opcodes != null)
        //             {
        //                 opcodes.Add(opcode);
        //             }
        //
        //             try
        //             {
        //                 Instruction[opcode](cpu);
        //             }
        //             catch (Exception e)
        //             {
        //                 throw new Exception($"Opcode {opcode:X2} -> {e}");
        //             }
        //         }
        //     }
        //     else
        //     {
        //         cpu.ClockCounter += 4;
        //         if (cpu._intFlags != 0 && cpu._intEnableFlags != 0)
        //         {
        //             cpu.Halted = false;
        //         }
        //     }
        //
        //     if (cpu._imeCountdown != 0)
        //     {
        //         cpu._imeCountdown -= 1;
        //         if (cpu._imeCountdown == 0)
        //         {
        //             cpu._ime = true;
        //         }
        //     }
        // }

        public static void Step(Cpu cpu, double deltaTime, HashSet<byte> opcodes = null)
        {
            if (deltaTime < 0)
            {
                deltaTime = 0;
            }
            else if (deltaTime > 1.0 / 30.0)
            {
                deltaTime = 1.0 / 30.0;
            }

            int budget = (int)(Cpu.CLOCK_SPEED * deltaTime);
            int ran = 0;
            while (ran < budget)
            {
                Joypad.JoypadTick(cpu);
                ran += Sm83.Step(cpu);
            }

            if (opcodes != null)
            {
                opcodes.Add(cpu.Reg.IR);
            }
        }

        #endregion
    }
}