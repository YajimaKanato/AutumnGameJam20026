using UnityEngine;
using UnityEngine.InputSystem;

namespace Aizawa
{
    /// <summary>
    /// テスト用
    /// </summary>
    public class AizawaTest : MonoBehaviour
    {
        void Start()
        {
            
        }

        void Update()
        {
            if(Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ScoreManager.SetPlayer(new System.Random().Next().ToString());
                ScoreManager.ReceiveScore(Random.Range(1, 11));
                ScoreManager.UpdateRanking();

                foreach(var r in ScoreManager.Ranking)
                {
                    Debug.Log($"「{r.name}」:{r.score} ... {r.rank}位");
                }
            }
        }
}
}