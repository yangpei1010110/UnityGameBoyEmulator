using System;
using System.Text;
using GameBoy.Emulators.Common;
using GameBoy.Emulators.Common.Opcodes;
using TMPro;
using UnityEngine;

namespace GameBoy.Emulators.Debugs
{
    public class TileDataDebug : MonoBehaviour
    {
        public SpriteRenderer _SpriteRenderer;
        public Sprite         _Sprite;
        public Texture2D      _Texture;

        public Emulator _Emulator;

        public TextMeshProUGUI _cpuInfo;

        private void Start()
        {
            _SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (_SpriteRenderer)
            {
                _Emulator = GetComponentInChildren<Emulator>();
                _Texture = new Texture2D(16 * 8, 24 * 8);
                Color[] pixels = _Texture.GetPixels();
                Array.Fill(pixels, Color.black);
                _Texture.SetPixels(pixels);
                _Texture.name = "图块数据";
                _Texture.filterMode = FilterMode.Point;
                _Texture.Apply();
                _Sprite = Sprite.Create(_Texture, new Rect(0, 0, 16 * 8, 24 * 8), new Vector2(0.5f, 0.5f));
                _Sprite.name = "图块数据精灵";
                _SpriteRenderer.sprite = _Sprite;
            }
        }

        private void Update()
        {
            if (_Emulator && _Texture)
            {
                Color32[] color32s = _Texture.GetPixels32();
                for (int yIndex = 0; yIndex < 24; yIndex++)
                for (int xIndex = 0; xIndex < 16; xIndex++)
                {
                    int tileIndex = yIndex * 16 + xIndex;
                    // 16 byte to 64 color32
                    int memoryIndex = 0x8000 + tileIndex * 16;
                    for (int line = 0; line < 8; line++)
                    {
                        byte l = Op.Read(_Emulator.cpu, (ushort)(memoryIndex + line * 2));
                        byte h = Op.Read(_Emulator.cpu, (ushort)(memoryIndex + line * 2 + 1));
                        Color32[] colors = DecodeTileLine(l, h);
                        for (int i = 0; i < 8; i++)
                        {
                            int offset = yIndex * 16 * 8 * 8 + line * 16 * 8 + xIndex * 8 + i;
                            color32s[offset] = colors[i];
                        }
                    }
                }

                _Texture.SetPixels32(color32s);
                _Texture.Apply();
            }

            if (_Emulator && _cpuInfo)
            {
                Cpu.GameBoyEmulatorCpuRegister reg = _Emulator.cpu.Reg;
                StringBuilder sb = new();
                sb.AppendLine($"AF:{reg.AF:X4} A:{reg.A:X2} F:{reg.F:X2}");
                sb.AppendLine($"BC:{reg.BC:X4} B:{reg.B:X2} C:{reg.C:X2}");
                sb.AppendLine($"DE:{reg.DE:X4} D:{reg.D:X2} E:{reg.E:X2}");
                sb.AppendLine($"HL:{reg.HL:X4} H:{reg.H:X2} L:{reg.L:X2}");
                sb.AppendLine($"PC:{reg.PC:X4} SP:{reg.SP:X4}");
                sb.AppendLine($"Z:{(reg.z ? 'T' : 'F')} N:{(reg.n ? 'T' : 'F')} H:{(reg.h ? 'T' : 'F')} C:{(reg.c ? 'T' : 'F')}");
                sb.AppendLine($"IR:{reg.IR:X2}");
                sb.AppendLine($"IE:{reg.IE:X2}");

                _cpuInfo.SetText(sb);
            }
        }

        private static Color32[] DecodeTileLine(byte l, byte h)
        {
            Color32[] result = new Color32[8];
            for (int b = 7; b >= 0; b--)
            {
                byte lo = (l & (1 << b)) == 0 ? (byte)0 : (byte)(1 << 0);
                byte hi = (h & (1 << b)) == 0 ? (byte)0 : (byte)(1 << 1);
                byte color = (byte)(lo | hi);
                switch (color)
                {
                    case 0:
                        result[7 - b] = new Color32(255, 255, 255, 255);
                        break;
                    case 1:
                        result[7 - b] = new Color32(170, 170, 170, 255);
                        break;
                    case 2:
                        result[7 - b] = new Color32(85, 85, 85, 255);
                        break;
                    case 3:
                        result[7 - b] = new Color32(0, 0, 0, 255);
                        break;
                    default:
                        throw new Exception("Invalid color");
                }
            }

            // Debug.Log($"l:{l:X2},h:{h:X2},result:{string.Join(",", result)}");

            return result;
        }
    }
}