using System;
using System.IO;

namespace GameBoy.Emulators.Common
{
    /// <summary>
    ///     卡带. 负责 ROM/RAM 换银行. 覆盖 ROM ONLY, MBC1, MBC2, MBC3, MBC5.
    /// </summary>
    public sealed class Cartridge
    {
        public enum MapperKind
        {
            None,
            Mbc1,
            Mbc2,
            Mbc3,
            Mbc5,
        }

        readonly byte[] _rom;
        readonly byte[] _ram;
        readonly string _savPath;
        readonly int _romBanks;

        MapperKind _mapper;
        bool _ramEnable;
        bool _hasBattery;
        bool _hasRtc;
        bool _mbc1RamMode;
        int _romBank = 1;
        int _ramBank;
        int _mbc1Low = 1;
        int _mbc1High;
        int _mbc5High;

        byte _rtcSeconds;
        byte _rtcMinutes;
        byte _rtcHours;
        byte _rtcDayLow;
        byte _rtcDayHigh;
        byte _rtcLatchSeconds;
        byte _rtcLatchMinutes;
        byte _rtcLatchHours;
        byte _rtcLatchDayLow;
        byte _rtcLatchDayHigh;
        byte _rtcLatchGate;
        bool _rtcLatched;

        public Cartridge(byte[] rom, string path = null)
        {
            _rom = rom ?? Array.Empty<byte>();
            byte type = _rom.Length > 0x147 ? _rom[0x147] : (byte)0;
            byte romCode = _rom.Length > 0x148 ? _rom[0x148] : (byte)0;
            byte ramCode = _rom.Length > 0x149 ? _rom[0x149] : (byte)0;
            Classify(type);
            _romBanks = Math.Max(1, _rom.Length / 0x4000);
            if (romCode <= 8)
            {
                int declared = 2 << romCode;
                if (declared > _romBanks)
                {
                    _romBanks = declared;
                }
            }

            int ramSize = RamSize(ramCode);
            if (_mapper == MapperKind.Mbc2)
            {
                ramSize = 512;
            }
            else if (ramSize == 0 && (_mapper == MapperKind.Mbc1 || _mapper == MapperKind.Mbc3 || _mapper == MapperKind.Mbc5))
            {
                ramSize = 0x2000;
            }

            _ram = new byte[ramSize];
            if (_hasBattery && !string.IsNullOrEmpty(path))
            {
                _savPath = Path.ChangeExtension(path, ".sav");
                if (File.Exists(_savPath))
                {
                    byte[] sav = File.ReadAllBytes(_savPath);
                    Array.Copy(sav, _ram, Math.Min(sav.Length, _ram.Length));
                }
            }
        }

        public MapperKind Mapper => _mapper;

        public void Save()
        {
            if (!_hasBattery || string.IsNullOrEmpty(_savPath) || _ram.Length == 0)
            {
                return;
            }

            File.WriteAllBytes(_savPath, _ram);
        }

        public byte Read(int address)
        {
            address &= 0xFFFF;
            if (address <= 0x7FFF)
            {
                return ReadRom(address);
            }

            if (address < 0xA000 || address > 0xBFFF)
            {
                return 0xFF;
            }

            if (_mapper == MapperKind.Mbc3 && _ramBank >= 0x08 && _ramBank <= 0x0C)
            {
                return ReadRtc(_ramBank);
            }

            if (!_ramEnable || _ram.Length == 0)
            {
                return 0xFF;
            }

            int index = RamIndex(address);
            if (index < 0 || index >= _ram.Length)
            {
                return 0xFF;
            }

            byte value = _ram[index];
            if (_mapper == MapperKind.Mbc2)
            {
                return (byte)(value | 0xF0);
            }

            return value;
        }

        public void Write(int address, byte value)
        {
            address &= 0xFFFF;
            if (address <= 0x7FFF)
            {
                WriteRegister(address, value);
                return;
            }

            if (address < 0xA000 || address > 0xBFFF)
            {
                return;
            }

            if (_mapper == MapperKind.Mbc3 && _ramBank >= 0x08 && _ramBank <= 0x0C)
            {
                WriteRtc(_ramBank, value);
                return;
            }

            if (!_ramEnable || _ram.Length == 0)
            {
                return;
            }

            int index = RamIndex(address);
            if (index < 0 || index >= _ram.Length)
            {
                return;
            }

            if (_mapper == MapperKind.Mbc2)
            {
                value = (byte)(value & 0x0F);
            }

            _ram[index] = value;
        }

        void Classify(byte type)
        {
            switch (type)
            {
                case 0x01:
                case 0x02:
                    _mapper = MapperKind.Mbc1;
                    break;
                case 0x03:
                    _mapper = MapperKind.Mbc1;
                    _hasBattery = true;
                    break;
                case 0x05:
                    _mapper = MapperKind.Mbc2;
                    break;
                case 0x06:
                    _mapper = MapperKind.Mbc2;
                    _hasBattery = true;
                    break;
                case 0x08:
                    _mapper = MapperKind.None;
                    break;
                case 0x09:
                    _mapper = MapperKind.None;
                    _hasBattery = true;
                    break;
                case 0x0F:
                    _mapper = MapperKind.Mbc3;
                    _hasBattery = true;
                    _hasRtc = true;
                    break;
                case 0x10:
                    _mapper = MapperKind.Mbc3;
                    _hasBattery = true;
                    _hasRtc = true;
                    break;
                case 0x11:
                case 0x12:
                    _mapper = MapperKind.Mbc3;
                    break;
                case 0x13:
                    _mapper = MapperKind.Mbc3;
                    _hasBattery = true;
                    _hasRtc = true;
                    break;
                case 0x19:
                case 0x1C:
                    _mapper = MapperKind.Mbc5;
                    break;
                case 0x1A:
                case 0x1D:
                    _mapper = MapperKind.Mbc5;
                    break;
                case 0x1B:
                case 0x1E:
                    _mapper = MapperKind.Mbc5;
                    _hasBattery = true;
                    break;
                default:
                    _mapper = MapperKind.None;
                    break;
            }
        }

        static int RamSize(byte code)
        {
            switch (code)
            {
                case 0x01: return 0x800;
                case 0x02: return 0x2000;
                case 0x03: return 0x8000;
                case 0x04: return 0x20000;
                case 0x05: return 0x10000;
                default: return 0;
            }
        }

        byte ReadRom(int address)
        {
            int bank = address < 0x4000 ? FixedBank() : SwitchableBank();
            int offset = bank * 0x4000 + (address & 0x3FFF);
            if ((uint)offset >= (uint)_rom.Length)
            {
                return 0xFF;
            }

            return _rom[offset];
        }

        int FixedBank()
        {
            if (_mapper == MapperKind.Mbc1 && _mbc1RamMode && _romBanks > 32)
            {
                return (_mbc1High << 5) % _romBanks;
            }

            return 0;
        }

        int SwitchableBank()
        {
            int bank = _romBank;
            if (_romBanks <= 0)
            {
                return 0;
            }

            bank %= _romBanks;
            if (bank < 0)
            {
                bank = 0;
            }

            return bank;
        }

        int RamIndex(int address)
        {
            int bank = 0;
            if (_mapper == MapperKind.Mbc1)
            {
                bank = _mbc1RamMode ? _mbc1High : 0;
            }
            else if (_mapper == MapperKind.Mbc3 || _mapper == MapperKind.Mbc5)
            {
                bank = _ramBank & 0x0F;
            }

            int bankSize = _mapper == MapperKind.Mbc2 ? 512 : 0x2000;
            int index = bank * bankSize + (address & 0x1FFF);
            if (_mapper == MapperKind.Mbc2)
            {
                index = address & 0x1FF;
            }

            return index;
        }

        void WriteRegister(int address, byte value)
        {
            if (_mapper == MapperKind.None)
            {
                return;
            }

            if (_mapper == MapperKind.Mbc2)
            {
                if (address <= 0x3FFF)
                {
                    if ((address & 0x0100) == 0)
                    {
                        _ramEnable = (value & 0x0F) == 0x0A;
                    }
                    else
                    {
                        _romBank = value & 0x0F;
                        if (_romBank == 0)
                        {
                            _romBank = 1;
                        }
                    }
                }

                return;
            }

            if (address <= 0x1FFF)
            {
                _ramEnable = (value & 0x0F) == 0x0A;
                return;
            }

            if (address <= 0x3FFF)
            {
                switch (_mapper)
                {
                    case MapperKind.Mbc1:
                        _mbc1Low = value & 0x1F;
                        if (_mbc1Low == 0)
                        {
                            _mbc1Low = 1;
                        }

                        _romBank = (_mbc1High << 5) | _mbc1Low;
                        break;
                    case MapperKind.Mbc3:
                        _romBank = value & 0x7F;
                        if (_romBank == 0)
                        {
                            _romBank = 1;
                        }

                        break;
                    case MapperKind.Mbc5:
                        if (address < 0x3000)
                        {
                            _romBank = (_mbc5High << 8) | value;
                        }
                        else
                        {
                            _mbc5High = value & 0x01;
                            _romBank = (_mbc5High << 8) | (_romBank & 0xFF);
                        }

                        break;
                }

                return;
            }

            if (address <= 0x5FFF)
            {
                switch (_mapper)
                {
                    case MapperKind.Mbc1:
                        _mbc1High = value & 0x03;
                        _romBank = (_mbc1High << 5) | _mbc1Low;
                        _ramBank = _mbc1High;
                        break;
                    case MapperKind.Mbc3:
                        _ramBank = value & 0x0F;
                        break;
                    case MapperKind.Mbc5:
                        _ramBank = value & 0x0F;
                        break;
                }

                return;
            }

            if (_mapper == MapperKind.Mbc1)
            {
                _mbc1RamMode = (value & 0x01) != 0;
            }
            else if (_mapper == MapperKind.Mbc3 && _hasRtc)
            {
                if (_rtcLatchGate == 0 && value == 1)
                {
                    LatchRtc();
                }

                _rtcLatchGate = value;
            }
            else if (_mapper == MapperKind.Mbc5 && address <= 0x3FFF)
            {
                _mbc5High = value & 0x01;
                _romBank = (_mbc5High << 8) | (_romBank & 0xFF);
            }
        }

        void LatchRtc()
        {
            if (!_rtcLatched)
            {
                DateTime now = DateTime.Now;
                _rtcSeconds = (byte)now.Second;
                _rtcMinutes = (byte)now.Minute;
                _rtcHours = (byte)now.Hour;
                int day = now.DayOfYear & 0x1FF;
                _rtcDayLow = (byte)day;
                _rtcDayHigh = (byte)((day >> 8) & 0x01);
            }

            _rtcLatchSeconds = _rtcSeconds;
            _rtcLatchMinutes = _rtcMinutes;
            _rtcLatchHours = _rtcHours;
            _rtcLatchDayLow = _rtcDayLow;
            _rtcLatchDayHigh = _rtcDayHigh;
            _rtcLatched = true;
        }

        byte ReadRtc(int reg)
        {
            switch (reg)
            {
                case 0x08: return _rtcLatchSeconds;
                case 0x09: return _rtcLatchMinutes;
                case 0x0A: return _rtcLatchHours;
                case 0x0B: return _rtcLatchDayLow;
                case 0x0C: return _rtcLatchDayHigh;
                default: return 0xFF;
            }
        }

        void WriteRtc(int reg, byte value)
        {
            switch (reg)
            {
                case 0x08: _rtcSeconds = (byte)(value & 0x3F); break;
                case 0x09: _rtcMinutes = (byte)(value & 0x3F); break;
                case 0x0A: _rtcHours = (byte)(value & 0x1F); break;
                case 0x0B: _rtcDayLow = value; break;
                case 0x0C: _rtcDayHigh = (byte)(value & 0xC1); break;
            }

            _rtcLatchSeconds = _rtcSeconds;
            _rtcLatchMinutes = _rtcMinutes;
            _rtcLatchHours = _rtcHours;
            _rtcLatchDayLow = _rtcDayLow;
            _rtcLatchDayHigh = _rtcDayHigh;
        }
    }
}
