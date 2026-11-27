using Cysharp.Threading.Tasks;
using UnityEngine;

public class OptionWindow : WindowPerformance
{
    [SerializeField] GameObject _window;

    public override async UniTask CloseWindow()
    {
        _window.SetActive(false);
    }

    public override async UniTask OpenWindow()
    {
        _window.SetActive(true);
    }
}
