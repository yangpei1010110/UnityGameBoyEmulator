using System;
using System.IO;
using System.Linq;
using System.Text;
using GameBoy.Emulators.Common;
using MyUtils.Extensions;
using MyUtils.R3;
using R3;
using R3.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameBoy.Emulators.Debugs
{
    public class DebugWindow : ObservableMonoBehaviour
    {
        [SerializeField]
        private bool _showDebugWindow;

        [SerializeField]
        private GameObject _debugWindow;
        [SerializeField]
        private Slider _gameSpeedSlider;
        [SerializeField]
        private TextMeshProUGUI _tmp__gameSpeedSlider;

        [SerializeField]
        private Emulator _emulator;

        [SerializeField]
        private Transform _romListContent;
        [SerializeField]
        private GameObject _buttonPrefab;

        // [SerializeField]
        // private string romPathRoot = @"Assets/Resources/ROMs";
        // private string rootPath = "Assets/Resources/ROMs";
        [SerializeField]
        private TextMeshProUGUI _tmp_serial_debug;

        protected override void OnEnableInit(ref DisposableBuilder builder)
        {
            _romListContent.DestroyAllChildren();
            Debug.Log(Path.GetFullPath(@"Assets/Resources"));
            var fileInfos = new DirectoryInfo(Path.GetFullPath(@"Assets/Resources"))
               .GetFiles("*.gb", SearchOption.AllDirectories);
            foreach (FileInfo fileInfo in fileInfos)
            {
                GameObject go = Instantiate(_buttonPrefab, _romListContent);
                go.GetComponentInChildren<TextMeshProUGUI>().SetText(fileInfo.Name);
                var button = go.GetComponent<Button>();
                button.OnPointerClickAsObservable()
                      .Subscribe(_ => _emulator.ReloadRom(fileInfo.FullName))
                      .AddTo(ref builder);
            }

            Observable.EveryValueChanged(_gameSpeedSlider, x => x.value)
                      .Select(v => Mathf.Clamp01(v))
                      .Subscribe(v =>
                       {
                           Cpu.ClockSpeedScale = v;
                           _tmp__gameSpeedSlider.SetText($"speed:{v:P2}");
                       })
                      .AddTo(ref builder);

            Observable.EveryValueChanged(this, _ => _showDebugWindow)
                      .Where(_ => _debugWindow)
                      .Subscribe(v => _debugWindow.SetActive(v))
                      .AddTo(ref builder);

            Observable.EveryValueChanged(_emulator, x => x.cpu.Serial.output_buffer.Count)
                      .Subscribe(_ =>
                       {
                           var arr = _emulator.cpu.Serial.output_buffer.ToArray();
                           // Array.Reverse(arr);
                           var text = Encoding.UTF8.GetString(arr);
                           // var arr = _emulator.cpu.Serial.output_buffer.Select(b => b.ToString("X2")).ToArray();
                           // var text = string.Join("", arr);
                           _tmp_serial_debug.SetText(text);
                       })
                      .AddTo(ref builder);
        }

        // private FileInfo[] GetRoms(string path)
        // {
        //     path = Path.GetFullPath(path);
        //     Debug.Log(path);
        //     if (File.Exists(path) && Path.GetExtension(path) == ".gb")
        //     {
        //         return new[] {new FileInfo(path)};
        //     }
        //
        //     if (Directory.Exists(path))
        //     {
        //         return new DirectoryInfo(path).GetFiles("*.gb", SearchOption.AllDirectories);
        //     }
        //
        //     return Array.Empty<FileInfo>();
        // }
    }
}