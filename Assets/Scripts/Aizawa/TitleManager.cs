using UnityEngine;
using TMPro;

namespace Aizawa
{
    /// <summary>
    /// タイトルシーンのマネージャークラス
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        /// <summary> 名前の入力欄 </summary>
        [SerializeField]
        private TMP_InputField _nameField;

        /// <summary>
        /// 名前を決定してインゲームに入る
        /// </summary>
        public async void StartGame()
        {
            if (ScoreManager.SetPlayer(_nameField.text))
            {
                await SceneTransitionManager.TransitionScene(Scene.Ingame);
            }
        }
    }
}