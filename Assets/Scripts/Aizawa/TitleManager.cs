using UnityEngine;
using TMPro;
using Yajima;
using Kato;
using UnityEngine.InputSystem;

namespace Aizawa
{
    /// <summary>
    /// タイトルシーンのマネージャークラス
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        /// <summary> 名前の入力欄 </summary>
        [SerializeField] private TMP_InputField _nameField;
        [SerializeField] WindowPerformance _optionWindow;
        [SerializeField] WindowPerformance _creditWindow;
        [SerializeField] WindowPerformance _rankingWindow;
        [SerializeField] FadeManager _fadeManager;
        bool _transitioning;

        private void Awake()
        {
            if (_nameField != null)
                _nameField.onValidateInput = ValidateNameCharacter;
        }

        private static char ValidateNameCharacter(string text, int index, char character)
        {
            char uppercase = char.ToUpperInvariant(character);
            return uppercase >= 'A' && uppercase <= 'Z' ? uppercase : '\0';
        }

        private async void Start()
        {
            _transitioning = true;
            AudioManager.instance?.Play("TitleBGM");
            if (_optionWindow != null) await _optionWindow.CloseWindow();
            if (_creditWindow != null) await _creditWindow.CloseWindow();
            if (_rankingWindow != null) await _rankingWindow.CloseWindow();
            if (_fadeManager != null) await _fadeManager.FadeIn(destroyCancellationToken);
            _transitioning = false;
        }

        /// <summary>
        /// 名前を決定してインゲームに入る
        /// </summary>
        public async void StartGame()
        {
            if (_transitioning || _nameField == null) return;
            if (!ScoreManager.SetPlayer(_nameField.text.Trim()))
            {
                _nameField.ActivateInputField();
                return;
            }

            _transitioning = true;
            if (_fadeManager != null) await _fadeManager.FadeOut(destroyCancellationToken);
            await SceneTransitionManager.TransitionScene(Scene.Ingame);
        }

        private void Update()
        {
            if (_optionWindow is OptionWindow settings && Keyboard.current?.escapeKey.wasPressedThisFrame == true)
                OperationOptionWindow(!settings.PanelIsOpen);
        }

        public void OperationOptionWindow(bool isOpen)
        {
            OperationWindow(_optionWindow, isOpen);
        }

        public void OperationCreditWindow(bool isOpen)
        {
            OperationWindow(_creditWindow, isOpen);
        }

        public void OperationRankingWindow(bool isOpen)
        {
            OperationWindow(_rankingWindow, isOpen);
        }

        async void OperationWindow(WindowPerformance window, bool isOpen)
        {
            if (_transitioning || window == null) return;

            if (isOpen)
                await window.OpenWindow();
            else
                await window.CloseWindow();
        }
    }
}
