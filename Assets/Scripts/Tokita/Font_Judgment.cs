using UnityEngine;
using TMPro;
using Aizawa;

namespace Tokita
{
    public class Font_Judgment : MonoBehaviour
    {

        [SerializeField]
        private TMP_InputField _PlayerInput;


        // プレイヤー名の入力＆ランキングデータへの譲渡
        public void Judgment()
        {
            string name = _PlayerInput.text;

            bool result = ScoreManager.SetPlayer(name);
        }
    }
}

