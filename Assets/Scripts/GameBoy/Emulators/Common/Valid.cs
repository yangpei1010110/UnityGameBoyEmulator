namespace GameBoy.Emulators.Common
{
    public static class Valid
    {
        private static readonly byte[] CHECK_SUM = new byte[]
        {
            0xCE,
            0xED,
            0x66,
            0x66,
            0xCC,
            0x0D,
            0x00,
            0x0B,
            0x03,
            0x73,
            0x00,
            0x83,
            0x00,
            0x0C,
            0x00,
            0x0D,
            0x00,
            0x08,
            0x11,
            0x1F,
            0x88,
            0x89,
            0x00,
            0x0E,
            0xDC,
            0xCC,
            0x6E,
            0xE6,
            0xDD,
            0xDD,
            0xD9,
            0x99,
            0xBB,
            0xBB,
            0x67,
            0x63,
            0x6E,
            0x0E,
            0xEC,
            0xCC,
            0xDD,
            0xDC,
            0x99,
            0x9F,
            0xBB,
            0xB9,
            0x33,
            0x3E,
        };

        private static bool GlobalCheckSum(in byte[] romData, out string errorMessage)
        {
            errorMessage = string.Empty;

            ushort globalCheckSum = 0;
            for (int i = 0; i < Rom.MAP_GLOBAL_CHECKSUM; i++)
            {
                globalCheckSum += romData[i];
            }

            for (int i = Rom.MAP_GLOBAL_CHECKSUM_END + 1; i < romData.Length; i++)
            {
                globalCheckSum += romData[i];
            }

            ushort checkSum = (ushort)((romData[Rom.MAP_GLOBAL_CHECKSUM] << 8)
                                     | romData[Rom.MAP_GLOBAL_CHECKSUM_END]);
            if (globalCheckSum != checkSum)
            {
                errorMessage = $"ROM数据全局校验失败 0x{globalCheckSum:X4} != 0x{checkSum:X4}";
                return false;
            }

            return true;
        }

        private static bool HeaderCheckSum(in byte[] romData, out string errorMessage)
        {
            errorMessage = string.Empty;

            byte headerCheckSum = 0;
            for (ushort i = Rom.MAP_TITLE_START; i < Rom.MAP_HEADER_CHECKSUM; i++)
            {
                headerCheckSum = (byte)(headerCheckSum - romData[i] - 1);
            }

            if (headerCheckSum != romData[Rom.MAP_HEADER_CHECKSUM])
            {
                errorMessage = "ROM数据头校验失败";
                return false;
            }

            return true;
        }

        public static bool LogoCheckSum(in byte[] romData, out string errorMessage)
        {
            errorMessage = string.Empty;
            for (ushort i = Rom.MAP_LOGO_TOP_START; i <= Rom.MAP_LOGO_BOTTOM_END; i++)
            {
                ushort checkIndex = (ushort)(i - Rom.MAP_LOGO_TOP_START);
                if (romData[i] != CHECK_SUM[checkIndex])
                {
                    errorMessage = $"ROM数据校验失败，索引{i:X4}的数据不匹配";
                    return false;
                }
            }

            return true;
        }

        public static bool CheckSum(in byte[] romData, out string errorMessage)
        {
            if (romData == null || romData.Length <= Rom.MAP_GLOBAL_CHECKSUM_END)
            {
                errorMessage = "ROM数据为空或者长度不够";
                return false;
            }

            if (!LogoCheckSum(romData, out errorMessage))
            {
                return false;
            }

            if (!HeaderCheckSum(romData, out errorMessage))
            {
                return false;
            }

            // 开机 ROM 不检查全局校验. 很多自制和测试 ROM 这里对不上, 不能因此拒绝启动.
            GlobalCheckSum(romData, out errorMessage);
            errorMessage = string.Empty;
            return true;
        }

        public static bool BitTest(byte      value, int bit) => (value & (1 << bit)) != 0;
        public static void BitReset(ref byte value, int bit) => value &= (byte)~(1 << bit);
    }
}