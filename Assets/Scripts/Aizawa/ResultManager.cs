using UnityEngine;
using Yajima;

namespace Aizawa
{
    /// <summary>
    /// リザルトシーンのマネージャークラス
    /// </summary>
    public class ResultManager : MonoBehaviour
    {
        [SerializeField]
        private RankingUI _rankingUI;
        [SerializeField]
        private ResultScoreUI _resultScoreUI;
        [SerializeField] FadeManager _fadeManager;

        private async void Awake()
        {
            await _fadeManager.FadeIn();
        }

        /// <summary>
        /// リザルト画面のUIを展開する
        /// </summary>
        public void OpenResultUIs()
        {
            _rankingUI.Open();
            _resultScoreUI.Open();
        }

        /// <summary>
        /// もう一度遊ぶ
        /// </summary>
        public async void Replay()
        {
            await _fadeManager.FadeOut();
            await SceneTransitionManager.TransitionScene(Scene.Ingame);
        }

        /// <summary>
        /// タイトルへ戻る
        /// </summary>
        public async void BackTitle()
        {
            await _fadeManager.FadeOut();
            await SceneTransitionManager.TransitionScene(Scene.Title);
        }
    }
}