using GameBoy.Emulators.Common.Opcodes;

namespace GameBoy.Emulators.Common.Cpus
{
    public class Joypad
    {
        public byte selectBits = 0x30;
        byte _lines = 0x0F;

        public bool select;
        public bool start;
        public bool a;
        public bool b;
        public bool down;
        public bool left;
        public bool right;
        public bool up;

        public bool AnyPressed => a || b || start || select || left || right || up || down;

        public byte p1 => Read();

        public byte Read()
        {
            byte value = (byte)(0xCF | selectBits);
            if ((selectBits & 0x10) == 0)
            {
                if (right)
                {
                    value &= unchecked((byte)~0x01);
                }

                if (left)
                {
                    value &= unchecked((byte)~0x02);
                }

                if (up)
                {
                    value &= unchecked((byte)~0x04);
                }

                if (down)
                {
                    value &= unchecked((byte)~0x08);
                }

                value &= unchecked((byte)~0x10);
            }

            if ((selectBits & 0x20) == 0)
            {
                if (a)
                {
                    value &= unchecked((byte)~0x01);
                }

                if (b)
                {
                    value &= unchecked((byte)~0x02);
                }

                if (select)
                {
                    value &= unchecked((byte)~0x04);
                }

                if (start)
                {
                    value &= unchecked((byte)~0x08);
                }

                value &= unchecked((byte)~0x20);
            }

            return value;
        }

        public void Write(byte value) => selectBits = (byte)(value & 0x30);

        public static void JoypadTick(Cpu cpu)
        {
            byte lines = (byte)(cpu.Joypad.Read() & 0x0F);
            byte falling = (byte)(cpu.Joypad._lines & ~lines);
            if (falling != 0)
            {
                cpu._intFlags |= CpuOp.INT_JOYPAD;
            }

            cpu.Joypad._lines = lines;
        }
    }
}
