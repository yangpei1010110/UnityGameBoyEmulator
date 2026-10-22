using System;

namespace GameBoy.Emulators.Common.Cpus
{
    /// <summary>
    ///     DMG 四个声道. 逻辑按方波, 波形, 噪声和帧音序器来, 不追每条边角测试.
    /// </summary>
    public sealed class Apu
    {
        const int SampleCapacity = 8192;
        static readonly byte[] Duties = { 0x01, 0x81, 0x87, 0x7E };
        static readonly int[] NoiseDivisors = { 8, 16, 32, 48, 64, 80, 96, 112 };

        readonly float[] _samples = new float[SampleCapacity];
        readonly byte[] _wave = new byte[16];
        readonly Square[] _square = { new Square(), new Square() };
        Wave _waveCh = new Wave();
        Noise _noise = new Noise();

        int _sampleWrite;
        int _sampleRead;
        int _sampleTimer;
        int _samplePeriod = 95;
        int _frameTimer;
        int _frameStep;
        bool _enabled = true;
        byte _nr50 = 0x77;
        byte _nr51 = 0xF3;

        public int OutputRate
        {
            get => 4194304 / Math.Max(1, _samplePeriod);
            set
            {
                if (value < 8000)
                {
                    value = 8000;
                }

                _samplePeriod = Math.Max(1, 4194304 / value);
            }
        }

        public void Tick(Cpu cpu)
        {
            if (!_enabled)
            {
                return;
            }

            StepSquare(_square[0], true);
            StepSquare(_square[1], false);
            StepWave();
            StepNoise();

            _frameTimer++;
            if (_frameTimer >= 8192)
            {
                _frameTimer = 0;
                ClockFrame();
            }

            _sampleTimer++;
            if (_sampleTimer >= _samplePeriod)
            {
                _sampleTimer = 0;
                PushSample();
            }
        }

        public byte Read(int address)
        {
            address &= 0xFFFF;
            if (address >= 0xFF30 && address <= 0xFF3F)
            {
                return _wave[address - 0xFF30];
            }

            switch (address)
            {
                case 0xFF10: return (byte)(0x80 | _square[0].nr0);
                case 0xFF11: return (byte)(0x3F | _square[0].nr1);
                case 0xFF12: return _square[0].nr2;
                case 0xFF13: return 0xFF;
                case 0xFF14: return (byte)(0xBF | _square[0].nr4);
                case 0xFF15: return 0xFF;
                case 0xFF16: return (byte)(0x3F | _square[1].nr1);
                case 0xFF17: return _square[1].nr2;
                case 0xFF18: return 0xFF;
                case 0xFF19: return (byte)(0xBF | _square[1].nr4);
                case 0xFF1A: return (byte)(0x7F | _waveCh.nr0);
                case 0xFF1B: return 0xFF;
                case 0xFF1C: return (byte)(0x9F | _waveCh.nr2);
                case 0xFF1D: return 0xFF;
                case 0xFF1E: return (byte)(0xBF | _waveCh.nr4);
                case 0xFF1F: return 0xFF;
                case 0xFF20: return 0xFF;
                case 0xFF21: return _noise.nr2;
                case 0xFF22: return _noise.nr3;
                case 0xFF23: return (byte)(0xBF | _noise.nr4);
                case 0xFF24: return _nr50;
                case 0xFF25: return _nr51;
                case 0xFF26:
                    byte status = (byte)((_enabled ? 0x80 : 0)
                                       | (_square[0].dac && _square[0].enabled ? 0x01 : 0)
                                       | (_square[1].dac && _square[1].enabled ? 0x02 : 0)
                                       | (_waveCh.dac && _waveCh.enabled ? 0x04 : 0)
                                       | (_noise.dac && _noise.enabled ? 0x08 : 0));
                    return (byte)(status | 0x70);
                default: return 0xFF;
            }
        }

        public void Write(int address, byte value)
        {
            address &= 0xFFFF;
            if (!_enabled && address != 0xFF26 && !(address >= 0xFF30 && address <= 0xFF3F))
            {
                return;
            }

            if (address >= 0xFF30 && address <= 0xFF3F)
            {
                _wave[address - 0xFF30] = value;
                return;
            }

            switch (address)
            {
                case 0xFF10: _square[0].nr0 = value; break;
                case 0xFF11:
                    _square[0].nr1 = value;
                    _square[0].length = 64 - (value & 0x3F);
                    break;
                case 0xFF12:
                    _square[0].nr2 = value;
                    _square[0].dac = (value & 0xF8) != 0;
                    if (!_square[0].dac)
                    {
                        _square[0].enabled = false;
                    }

                    break;
                case 0xFF13: _square[0].freq = (_square[0].freq & 0x700) | value; break;
                case 0xFF14:
                    WriteSquareHigh(_square[0], value, true);
                    break;
                case 0xFF16:
                    _square[1].nr1 = value;
                    _square[1].length = 64 - (value & 0x3F);
                    break;
                case 0xFF17:
                    _square[1].nr2 = value;
                    _square[1].dac = (value & 0xF8) != 0;
                    if (!_square[1].dac)
                    {
                        _square[1].enabled = false;
                    }

                    break;
                case 0xFF18: _square[1].freq = (_square[1].freq & 0x700) | value; break;
                case 0xFF19:
                    WriteSquareHigh(_square[1], value, false);
                    break;
                case 0xFF1A:
                    _waveCh.nr0 = (byte)(value & 0x80);
                    _waveCh.dac = (value & 0x80) != 0;
                    if (!_waveCh.dac)
                    {
                        _waveCh.enabled = false;
                    }

                    break;
                case 0xFF1B:
                    _waveCh.length = 256 - value;
                    break;
                case 0xFF1C: _waveCh.nr2 = (byte)(value & 0x60); break;
                case 0xFF1D: _waveCh.freq = (_waveCh.freq & 0x700) | value; break;
                case 0xFF1E:
                    _waveCh.freq = (_waveCh.freq & 0xFF) | ((value & 0x07) << 8);
                    ArmLength(ref _waveCh.lengthEnable, ref _waveCh.length, ref _waveCh.enabled, value);
                    _waveCh.nr4 = (byte)(value & 0x40);
                    if ((value & 0x80) != 0)
                    {
                        _waveCh.enabled = _waveCh.dac;
                        if (_waveCh.length == 0)
                        {
                            _waveCh.length = 256;
                        }

                        _waveCh.timer = PeriodWave(_waveCh.freq);
                        _waveCh.position = 0;
                    }

                    break;
                case 0xFF20:
                    _noise.length = 64 - (value & 0x3F);
                    break;
                case 0xFF21:
                    _noise.nr2 = value;
                    _noise.dac = (value & 0xF8) != 0;
                    if (!_noise.dac)
                    {
                        _noise.enabled = false;
                    }

                    break;
                case 0xFF22: _noise.nr3 = value; break;
                case 0xFF23:
                    ArmLength(ref _noise.lengthEnable, ref _noise.length, ref _noise.enabled, value);
                    _noise.nr4 = (byte)(value & 0x40);
                    if ((value & 0x80) != 0)
                    {
                        _noise.enabled = _noise.dac;
                        if (_noise.length == 0)
                        {
                            _noise.length = 64;
                        }

                        _noise.volume = _noise.nr2 >> 4;
                        _noise.envTimer = _noise.nr2 & 0x07;
                        _noise.lfsr = 0x7FFF;
                        _noise.timer = NoisePeriod();
                    }

                    break;
                case 0xFF24: _nr50 = value; break;
                case 0xFF25: _nr51 = value; break;
                case 0xFF26:
                    bool on = (value & 0x80) != 0;
                    if (!on && _enabled)
                    {
                        Silence();
                    }
                    else if (on && !_enabled)
                    {
                        _frameStep = 7;
                        _frameTimer = 0;
                    }

                    _enabled = on;
                    break;
            }
        }

        public void Drain(float[] data, int channels)
        {
            channels = Math.Max(1, channels);
            lock (_samples)
            {
                for (int i = 0; i < data.Length; i += channels)
                {
                    float sample = 0;
                    if (_sampleRead != _sampleWrite)
                    {
                        sample = _samples[_sampleRead];
                        _sampleRead = (_sampleRead + 1) % SampleCapacity;
                    }

                    for (int c = 0; c < channels; c++)
                    {
                        data[i + c] = sample;
                    }
                }
            }
        }

        void Silence()
        {
            _square[0] = new Square();
            _square[1] = new Square();
            _waveCh = new Wave();
            _noise = new Noise();
            _nr50 = 0;
            _nr51 = 0;
            _frameTimer = 0;
            _frameStep = 7;
        }

        bool NextClocksLength => ((_frameStep + 1) & 1) == 0;

        void WriteSquareHigh(Square ch, byte value, bool sweep)
        {
            ch.freq = (ch.freq & 0xFF) | ((value & 0x07) << 8);
            ArmLength(ref ch.lengthEnable, ref ch.length, ref ch.enabled, value);
            ch.nr4 = (byte)(value & 0x40);
            if ((value & 0x80) != 0)
            {
                TriggerSquare(ch, sweep);
            }
        }

        void ArmLength(ref bool lengthEnable, ref int length, ref bool enabled, byte value)
        {
            bool was = lengthEnable;
            lengthEnable = (value & 0x40) != 0;
            if (!was && lengthEnable && !NextClocksLength && length > 0)
            {
                length--;
                if (length == 0)
                {
                    enabled = false;
                }
            }
        }

        void TriggerSquare(Square ch, bool sweep)
        {
            ch.enabled = ch.dac;
            if (ch.length == 0)
            {
                ch.length = 64;
            }

            ch.volume = ch.nr2 >> 4;
            ch.envTimer = ch.nr2 & 0x07;
            ch.timer = PeriodSquare(ch.freq);
            if (!sweep)
            {
                return;
            }

            int period = (ch.nr0 >> 4) & 0x07;
            int shift = ch.nr0 & 0x07;
            ch.shadow = ch.freq;
            ch.sweepTimer = period == 0 ? 8 : period;
            ch.sweepEnable = period != 0 || shift != 0;
            if (shift != 0)
            {
                SweepCalculate(ch, false);
            }
        }

        void StepSquare(Square ch, bool sweep)
        {
            if (!ch.enabled || !ch.dac)
            {
                return;
            }

            if (--ch.timer <= 0)
            {
                ch.timer = PeriodSquare(ch.freq);
                ch.dutyPos = (ch.dutyPos + 1) & 7;
            }
        }

        void StepWave()
        {
            if (!_waveCh.enabled || !_waveCh.dac)
            {
                return;
            }

            if (--_waveCh.timer <= 0)
            {
                _waveCh.timer = PeriodWave(_waveCh.freq);
                _waveCh.position = (_waveCh.position + 1) & 31;
            }
        }

        void StepNoise()
        {
            if (!_noise.enabled || !_noise.dac)
            {
                return;
            }

            if (--_noise.timer <= 0)
            {
                _noise.timer = NoisePeriod();
                int xor = (_noise.lfsr & 1) ^ ((_noise.lfsr >> 1) & 1);
                _noise.lfsr = (_noise.lfsr >> 1) | (xor << 14);
                if ((_noise.nr3 & 0x08) != 0)
                {
                    _noise.lfsr = (_noise.lfsr & ~(1 << 6)) | (xor << 6);
                }
            }
        }

        void ClockFrame()
        {
            _frameStep = (_frameStep + 1) & 7;
            if ((_frameStep & 1) == 0)
            {
                ClockLength(_square[0], 64);
                ClockLength(_square[1], 64);
                if (_waveCh.lengthEnable && _waveCh.length > 0 && --_waveCh.length == 0)
                {
                    _waveCh.enabled = false;
                }

                if (_noise.lengthEnable && _noise.length > 0 && --_noise.length == 0)
                {
                    _noise.enabled = false;
                }
            }

            if (_frameStep == 2 || _frameStep == 6)
            {
                ClockSweep();
            }

            if (_frameStep == 7)
            {
                ClockEnvelope(_square[0]);
                ClockEnvelope(_square[1]);
                ClockEnvelopeNoise();
            }
        }

        static void ClockLength(Square ch, int max)
        {
            if (!ch.lengthEnable || ch.length <= 0)
            {
                return;
            }

            if (--ch.length == 0)
            {
                ch.enabled = false;
            }
        }

        void ClockSweep()
        {
            Square ch = _square[0];
            if (!ch.sweepEnable)
            {
                return;
            }

            if (--ch.sweepTimer > 0)
            {
                return;
            }

            int period = (ch.nr0 >> 4) & 0x07;
            ch.sweepTimer = period == 0 ? 8 : period;
            if (period != 0)
            {
                SweepCalculate(ch, true);
            }
        }

        void SweepCalculate(Square ch, bool update)
        {
            int shift = ch.nr0 & 0x07;
            int shifted = ch.shadow >> shift;
            int next = (ch.nr0 & 0x08) != 0 ? ch.shadow - shifted : ch.shadow + shifted;
            if (next > 2047)
            {
                ch.enabled = false;
                ch.sweepEnable = false;
                return;
            }

            if (next < 0 || shift == 0 || !update)
            {
                return;
            }

            ch.shadow = next;
            ch.freq = next & 0x7FF;
            SweepCalculate(ch, false);
        }

        static void ClockEnvelope(Square ch)
        {
            int period = ch.nr2 & 0x07;
            if (period == 0 || !ch.enabled)
            {
                return;
            }

            if (--ch.envTimer > 0)
            {
                return;
            }

            ch.envTimer = period;
            if ((ch.nr2 & 0x08) != 0)
            {
                if (ch.volume < 15)
                {
                    ch.volume++;
                }
            }
            else if (ch.volume > 0)
            {
                ch.volume--;
            }
        }

        void ClockEnvelopeNoise()
        {
            int period = _noise.nr2 & 0x07;
            if (period == 0 || !_noise.enabled)
            {
                return;
            }

            if (--_noise.envTimer > 0)
            {
                return;
            }

            _noise.envTimer = period;
            if ((_noise.nr2 & 0x08) != 0)
            {
                if (_noise.volume < 15)
                {
                    _noise.volume++;
                }
            }
            else if (_noise.volume > 0)
            {
                _noise.volume--;
            }
        }

        void PushSample()
        {
            int left = 0;
            int right = 0;
            Mix(LevelSquare(_square[0]), 0, ref left, ref right);
            Mix(LevelSquare(_square[1]), 1, ref left, ref right);
            Mix(LevelWave(), 2, ref left, ref right);
            Mix(LevelNoise(), 3, ref left, ref right);
            int leftVol = ((_nr50 >> 4) & 0x07) + 1;
            int rightVol = (_nr50 & 0x07) + 1;
            float sample = ((left * leftVol) + (right * rightVol)) / (15f * 8f * 4f * 2f);
            if (sample > 1f)
            {
                sample = 1f;
            }
            else if (sample < -1f)
            {
                sample = -1f;
            }

            lock (_samples)
            {
                _samples[_sampleWrite] = sample;
                int next = (_sampleWrite + 1) % SampleCapacity;
                if (next != _sampleRead)
                {
                    _sampleWrite = next;
                }
            }
        }

        void Mix(int level, int channel, ref int left, ref int right)
        {
            if ((_nr51 & (1 << channel)) != 0)
            {
                right += level;
            }

            if ((_nr51 & (1 << (channel + 4))) != 0)
            {
                left += level;
            }
        }

        static int LevelSquare(Square ch)
        {
            if (!ch.enabled || !ch.dac)
            {
                return 0;
            }

            int duty = Duties[(ch.nr1 >> 6) & 3];
            int on = (duty >> (7 - ch.dutyPos)) & 1;
            return on == 0 ? 0 : ch.volume;
        }

        int LevelWave()
        {
            if (!_waveCh.enabled || !_waveCh.dac)
            {
                return 0;
            }

            int shift = (_waveCh.nr2 >> 5) & 3;
            if (shift == 0)
            {
                return 0;
            }

            int byteIndex = _waveCh.position >> 1;
            int nibble = (_waveCh.position & 1) == 0 ? _wave[_byteIndexSafe(byteIndex)] >> 4 : _wave[_byteIndexSafe(byteIndex)] & 0x0F;
            return nibble >> (shift - 1);
        }

        int _byteIndexSafe(int index) => index & 0x0F;

        int LevelNoise()
        {
            if (!_noise.enabled || !_noise.dac)
            {
                return 0;
            }

            return (_noise.lfsr & 1) == 0 ? _noise.volume : 0;
        }

        static int PeriodSquare(int freq) => Math.Max(1, (2048 - (freq & 0x7FF)) * 4);

        static int PeriodWave(int freq) => Math.Max(1, (2048 - (freq & 0x7FF)) * 2);

        int NoisePeriod()
        {
            int divisor = NoiseDivisors[_noise.nr3 & 7];
            int shift = (_noise.nr3 >> 4) & 0x0F;
            return Math.Max(1, divisor << shift);
        }

        sealed class Square
        {
            public byte nr0, nr1, nr2, nr4;
            public bool dac = true;
            public bool enabled;
            public bool lengthEnable;
            public bool sweepEnable;
            public int length = 64;
            public int volume;
            public int envTimer;
            public int freq;
            public int timer = 1;
            public int dutyPos;
            public int sweepTimer;
            public int shadow;
        }

        sealed class Wave
        {
            public byte nr0, nr2, nr4;
            public bool dac;
            public bool enabled;
            public bool lengthEnable;
            public int length = 256;
            public int freq;
            public int timer = 1;
            public int position;
        }

        sealed class Noise
        {
            public byte nr2, nr3, nr4;
            public bool dac;
            public bool enabled;
            public bool lengthEnable;
            public int length = 64;
            public int volume;
            public int envTimer;
            public int timer = 1;
            public int lfsr = 0x7FFF;
        }
    }
}
