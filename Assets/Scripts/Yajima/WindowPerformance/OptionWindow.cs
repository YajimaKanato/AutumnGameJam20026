using Cysharp.Threading.Tasks;
using System;
using Kato;
using UnityEngine;
using UnityEngine.UI;

public class OptionWindow : WindowPerformance
{
    [SerializeField] GameObject _window;
    [SerializeField] Slider _bgmSlider;
    [SerializeField] Slider _seSlider;
    public bool PanelIsOpen => _window != null && _window.activeSelf;
    public event Action Closed;

    public void Close()
    {
        if (!PanelIsOpen) return;

        _window.SetActive(false);
        Closed?.Invoke();
    }

    public void SetBgmVolume(float volume) => AudioManager.instance?.SetBgmVolume(volume);
    public void SetSoundEffectVolume(float volume) => AudioManager.instance?.SetSoundEffectVolume(volume);

    public override UniTask CloseWindow()
    {
        if (_window != null) _window.SetActive(false);
        return UniTask.CompletedTask;
    }

    public override UniTask OpenWindow()
    {
        var audio = AudioManager.instance;
        if (audio != null)
        {
            if (_bgmSlider != null) _bgmSlider.SetValueWithoutNotify(audio.BgmVolume);
            if (_seSlider != null) _seSlider.SetValueWithoutNotify(audio.SoundEffectVolume);
        }
        _window.SetActive(true);
        return UniTask.CompletedTask;
    }
}
