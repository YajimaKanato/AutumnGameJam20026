using Cysharp.Threading.Tasks;
using Kato;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yajima;

namespace Aizawa
{
    /// <summary>
    /// リザルトシーンのマネージャークラス
    /// </summary>
    public class ResultManager : MonoBehaviour
    {
        [SerializeField] RectTransform _resultCanvas;
        [SerializeField] FadeManager _fadeManager;
        [SerializeField] OptionWindow _settings;
        bool _transitioning;

        private void Start() => OpenResult().Forget();

        async UniTask OpenResult()
        {
            _transitioning = true;
            AudioManager.instance?.Play("ResultBGM");
            BindButton("ResultRestart", Replay);
            BindButton("BackTitle", BackTitle);
            if (_settings != null) await _settings.CloseWindow();
            if (_fadeManager != null && await _fadeManager.FadeIn(destroyCancellationToken).SuppressCancellationThrow()) return;
            OpenResultUIs();
            _transitioning = false;
        }

        void BindButton(string name, UnityEngine.Events.UnityAction action)
        {
            if (_resultCanvas == null) return;

            var button = _resultCanvas.Find(name)?.GetComponentInChildren<Button>(true);
            if (button != null) button.onClick.AddListener(action);
        }

        /// <summary>
        /// リザルト画面のUIを展開する
        /// </summary>
        public void OpenResultUIs()
        {
            if (_resultCanvas == null) return;

            var rankingWindow = _resultCanvas.GetComponentInChildren<RankingWindowPerformance>(true);
            if (rankingWindow != null) rankingWindow.OpenWindow().Forget();
            else _resultCanvas.GetComponentInChildren<RankingUI>(true)?.Open();
            _resultCanvas.GetComponentInChildren<ResultScoreUI>(true)?.Open();
        }

        private void Update()
        {
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true) ToggleSettings();
        }

        /// <summary>
        /// もう一度遊ぶ
        /// </summary>
        public void Replay() => Leave(Scene.Ingame).Forget();

        /// <summary>
        /// タイトルへ戻る
        /// </summary>
        public void BackTitle() => Leave(Scene.Title).Forget();

        public void ToggleSettings()
        {
            if (_transitioning || _settings == null) return;
            if (_settings.PanelIsOpen) _settings.CloseWindow().Forget();
            else _settings.OpenWindow().Forget();
        }

        async UniTask Leave(Scene scene)
        {
            if (_transitioning || (_settings != null && _settings.PanelIsOpen)) return;

            _transitioning = true;
            if (_fadeManager != null && await _fadeManager.FadeOut(destroyCancellationToken).SuppressCancellationThrow()) return;
            await SceneTransitionManager.TransitionScene(scene);
        }
    }
}
