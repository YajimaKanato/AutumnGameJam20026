using UnityEngine;

namespace Yajima
{
    public class CriticalEventView : MonoBehaviour
    {
        [SerializeField] GameObject _indicator;

        private void Awake()
        {
            _indicator?.gameObject.SetActive(false);
        }

        public void SetCritical(bool active)
        {
            if (_indicator != null)
                _indicator.SetActive(active);
        }
    }
}
