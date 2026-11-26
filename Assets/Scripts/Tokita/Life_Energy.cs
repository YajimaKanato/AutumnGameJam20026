using UnityEngine;
using Yajima;


namespace Tokita
{
    public class Life_Energy : MonoBehaviour
    {
        [Range(0,1)]
        [Tooltip("生命エネルギーパラメータ")]
        public float _Energy;

        [SerializeField]
        public float w;


        Player OnLightMove;


        [Header("エネルギー消費時間")]
        [Min(0.01f)]
        [SerializeField]
        public float _AirTime = 15f;

        [Header("エネルギー回復時間")]
        [Min(0.01f)]
        [SerializeField]
        public float _FullTime = 1f;


        private void Update()
        {
            if (OnLightMove)
            {
                UpdateEnergy(-1 / _AirTime);
            }
            else
            {
                UpdateEnergy(1 /_FullTime);
            }
        }


        private void UpdateEnergy(float f)
        {
            _Energy = Mathf.Clamp01(
                _Energy + Time.deltaTime * f
            );
        }
    }
}
