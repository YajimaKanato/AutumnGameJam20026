using UnityEngine;

namespace Aizawa
{
    /// <summary>
    /// アニメーションでエフェクトを発生させる用
    /// </summary>
    public class EffectAnimator : MonoBehaviour
    {
        public void EndAnim()
        {
            gameObject.SetActive(false);
        }
    }
}