using UnityEngine;
using TMPro;
using Yajima;

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

        private async void Awake()
        {
            await _fadeManager.FadeIn();
        }

        /// <summary>
        /// 名前を決定してインゲームに入る
        /// </summary>
        public async void StartGame()
        {
            if (ScoreManager.SetPlayer(_nameField.text))
            {
                await _fadeManager.FadeOut();
                await SceneTransitionManager.TransitionScene(Scene.Ingame);
            }
        }

        public async void OperationOptionWindow(bool isOpen)
        {
            OperationWindow(_optionWindow, isOpen);
        }

        public async void OperationCreditWindow(bool isOpen)
        {
            OperationWindow(_creditWindow, isOpen);
        }

        public async void OperationRankingWindow(bool isOpen)
        {
            OperationWindow(_rankingWindow, isOpen);
        }

        void OperationWindow(WindowPerformance window, bool isOpen)
        {
            if (isOpen)
                window?.OpenWindow();
            else
                window?.CloseWindow();
        }
    }
}