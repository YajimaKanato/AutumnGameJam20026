using UnityEngine;

namespace Aizawa
{
    /// <summary>
    /// リザルトシーンのマネージャークラス
    /// </summary>
    public class ResultManager : MonoBehaviour
    {
        /// <summary>
        /// もう一度遊ぶ
        /// </summary>
        public async void Replay()
        {
            await SceneTransitionManager.TransitionScene(Scene.Ingame);
        }

        /// <summary>
        /// タイトルへ戻る
        /// </summary>
        public async void BackTitle()
        {
            await SceneTransitionManager.TransitionScene(Scene.Title);
        }
    }
}