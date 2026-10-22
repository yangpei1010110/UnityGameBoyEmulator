using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameBoy.Emulators.Common;
using GameBoy.Emulators.Common.Cpus;
using GameBoy.Emulators.Common.Opcodes;
using MyUtils;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameBoy.Emulators
{
    public class Emulator : Singleton<Emulator>
    {
        public const string TEST_ROM_PATH1 =
            @"Assets/Resources/ROMs/Legend of Zelda, The - Link's Awakening (G) [!].gb";
        public const string TEST_ROM_PATH2 = @"Assets/Resources/ROMs/Pokemon Red-Blue 2-in-1 (Unl) [S].gb";
        public const string TEST_ROM_PATH3 = @"Assets/Resources/ROMs/Super Mario Land (JUE) (V1.1) [!].gb";
        public const string TEST_ROM_PATH4 = @"Assets/Resources/ROMs/Dr. Mario (JU) (V1.1).gb";

        [HideInInspector] public byte[] romData = Array.Empty<byte>();

        [SerializeField]
        private ulong clockCounter = 0;

        [SerializeField]
        private ushort programCounter = 0;

        // private async void Start()
        // {
        //     // var channel = GrpcChannelx.ForTarget(new GrpcChannelTarget("localhost", 5000, ChannelCredentials.Insecure));
        //     GrpcChannelx channel = GrpcChannelx.ForAddress("http://localhost:5000");
        //     IMyFirstService client = MagicOnionClient.Create<IMyFirstService>(channel);
        //
        //     Stopwatch sw = Stopwatch.StartNew();
        //     for (int i = 0; i < 1000; i++)
        //     {
        //         int left = Random.Range(1, 1000);
        //         int right = Random.Range(1, 1000);
        //         sw.Restart();
        //         int result = await client.SumAsync(left, right);
        //         sw.Stop();
        //         long microSeconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        //         Debug.Log($"{left}+{right}={result} in {microSeconds} microSeconds");
        //     }
        // }

        [SerializeField]
        private bool StepMode = false;

        [SerializeField]
        private bool StepNext;

        public Cpu cpu = new();

        private bool isException = false;
        
        public  LinkedList<string> log = new();
        [SerializeField]
        public bool isLog = false;
        Texture2D _lcdTexture;
        AudioSource _audio;
        string _romPath;
        [SerializeField]
        public bool isOutput = false;

        protected override void Awake()
        {
            base.Awake();
            DebugOpcode = new HashSet<byte>();
            _romPath = Path.GetFullPath(TEST_ROM_PATH4);
            romData = File.ReadAllBytes(_romPath);
            if (!Valid.CheckSum(romData, out string err))
            {
                Debug.LogError(err);
                return;
            }

            cpu.ProgramCounter = 0x100;
            cpu.Mount(romData, _romPath);
            isException = false;
            EnsureScreen();
            EnsureAudio();

            Debug.Log(new Info(File.ReadAllBytes(Path.GetFullPath(TEST_ROM_PATH1))).ToString());
            Debug.Log(new Info(File.ReadAllBytes(Path.GetFullPath(TEST_ROM_PATH2))).ToString());
            Debug.Log(new Info(File.ReadAllBytes(Path.GetFullPath(TEST_ROM_PATH3))).ToString());
            Debug.Log(new Info(File.ReadAllBytes(Path.GetFullPath(TEST_ROM_PATH4))).ToString());
        }

        public void ReloadRom(string fullPath)
        {
            var data = File.ReadAllBytes(Path.GetFullPath(fullPath));
            if (!Valid.CheckSum(data, out string err))
            {
                Debug.LogError(err);
                return;
            }
            else
            {
                Debug.Log(new Info(data).ToString());
            }
            
            DebugOpcode = new HashSet<byte>();
            cpu.Cart?.Save();
            romData = data;
            _romPath = Path.GetFullPath(fullPath);
            cpu = new Cpu();
            cpu.ProgramCounter = 0x100;
            cpu.Mount(romData, _romPath);
            isException = false;
            StepMode = false;
            StepNext = false;
            log.Clear();
        }

        public HashSet<byte> DebugOpcode = new();

        void PollInput()
        {
            if (cpu?.Joypad == null)
            {
                return;
            }

            Joypad pad = cpu.Joypad;
            pad.right = Input.GetKey(KeyCode.RightArrow);
            pad.left = Input.GetKey(KeyCode.LeftArrow);
            pad.up = Input.GetKey(KeyCode.UpArrow);
            pad.down = Input.GetKey(KeyCode.DownArrow);
            pad.a = Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.S);
            pad.b = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.A);
            pad.start = Input.GetKey(KeyCode.Return);
            pad.select = Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.RightShift);
        }

        void EnsureScreen()
        {
            if (_lcdTexture != null)
            {
                return;
            }

            _lcdTexture = new Texture2D(Ppu.SCREEN_WIDTH, Ppu.SCREEN_HEIGHT, TextureFormat.RGBA32, false);
            _lcdTexture.filterMode = FilterMode.Point;
            _lcdTexture.wrapMode = TextureWrapMode.Clamp;
            var sprite = Sprite.Create(_lcdTexture, new Rect(0, 0, Ppu.SCREEN_WIDTH, Ppu.SCREEN_HEIGHT), new Vector2(0.5f, 0.5f), 16f);
            var lcd = new GameObject("LCD");
            lcd.transform.position = Vector3.zero;
            var renderer = lcd.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 10;
            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.orthographic = true;
                cam.orthographicSize = 5f;
                cam.transform.position = new Vector3(0, 0, -10);
                cam.backgroundColor = Color.black;
            }
        }

        void Present()
        {
            if (_lcdTexture == null || cpu?.Ppu == null)
            {
                return;
            }

            byte[] frame = cpu.Ppu.Framebuffer;
            Color32[] pixels = _lcdTexture.GetPixels32();
            int width = Ppu.SCREEN_WIDTH;
            int height = Ppu.SCREEN_HEIGHT;
            for (int y = 0; y < height; y++)
            {
                int srcRow = (height - 1 - y) * width;
                int dstRow = y * width;
                for (int x = 0; x < width; x++)
                {
                    int src = (srcRow + x) * 4;
                    pixels[dstRow + x] = new Color32(frame[src], frame[src + 1], frame[src + 2], 255);
                }
            }

            _lcdTexture.SetPixels32(pixels);
            _lcdTexture.Apply(false);
        }

        void EnsureAudio()
        {
            _audio = GetComponent<AudioSource>();
            if (_audio == null)
            {
                _audio = gameObject.AddComponent<AudioSource>();
            }

            _audio.playOnAwake = false;
            _audio.loop = true;
            _audio.spatialBlend = 0;
            if (_audio.clip == null)
            {
                int rate = AudioSettings.outputSampleRate;
                if (rate < 8000)
                {
                    rate = 48000;
                }

                cpu.Apu.OutputRate = rate;
                _audio.clip = AudioClip.Create("GameBoy", rate, 1, rate, false);
            }

            if (!_audio.isPlaying)
            {
                _audio.Play();
            }
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            cpu?.Apu?.Drain(data, channels);
        }

        void OnApplicationQuit() => cpu?.Cart?.Save();

        private void Update()
        {
            PollInput();
            if (isOutput)
            {
                isOutput = false;
                const string path = @"C:\Users\Lenovo\Desktop\log2.txt";
                File.WriteAllLines(path, log);
            }
            
            if (cpu != null
             && !isException
             && cpu.RomData != null
             && cpu.RomData.Length > 0)
            {
                if (!StepMode)
                {
                    try
                    {
                        CpuOp.Step(cpu, Time.deltaTime, DebugOpcode);
                        clockCounter = cpu.ClockCounter;
                        programCounter = cpu.ProgramCounter;
                        Present();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        isException = true;
                        Debug.LogWarning($"pc:{cpu.ProgramCounter:X4}");
                        byte opcode1 = Op.Read(cpu, cpu.ProgramCounter);
                        Debug.LogWarning($"pc:{opcode1:X4}");
                        byte opcode2 = Op.Read(cpu, (ushort)(cpu.ProgramCounter + 1));
                        Debug.LogWarning($"pc:{opcode2:X4}");
                        byte opcode3 = Op.Read(cpu, (ushort)(cpu.ProgramCounter + 2));
                        Debug.LogWarning($"pc:{opcode3:X4}");
                        byte opcode4 = Op.Read(cpu, (ushort)(cpu.ProgramCounter + 3));
                        Debug.LogWarning($"pc:{opcode4:X4}");

                        if (DebugOpcode != null)
                        {
                            Debug.LogWarning(string.Join(',', DebugOpcode
                                                             .OrderBy(o => o)
                                                             .Select(o => o.ToString("X2"))
                                                             .ToArray()));
                        }
                    }
                }
                else
                {
                    if (StepNext)
                    {
                        if (DebugOpcode != null)
                        {
                            Debug.Log(string.Join(',', DebugOpcode
                                                      .OrderBy(o => o)
                                                      .Select(o => o.ToString("X2"))
                                                      .ToArray()));
                        }

                        StepNext = false;
                        try
                        {
                            byte opcode1 = Op.Read(cpu, cpu.ProgramCounter);
                            byte opcode2 = Op.Read(cpu, (ushort)(cpu.ProgramCounter + 1));
                            byte opcode3 = Op.Read(cpu, (ushort)(cpu.ProgramCounter + 2));
                            byte opcode4 = Op.Read(cpu, (ushort)(cpu.ProgramCounter + 3));
                            Debug.Log(
                                $"pc:{cpu.ProgramCounter:X4},opcode:{opcode1:X2},{opcode2:X2},{opcode3:X2},{opcode4:X2}");
                            CpuOp.StepByExecutor(cpu, DebugOpcode);
                            clockCounter = cpu.ClockCounter;
                            programCounter = cpu.ProgramCounter;
                            Present();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                            isException = true;
                        }
                    }
                }
            }
        }
    }
}