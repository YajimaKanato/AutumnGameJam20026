using DG.Tweening;
using UnityEngine;

namespace Yajima
{
    public class CriticalEventView : MonoBehaviour
    {
        [SerializeField] GameObject _indicator;
        [SerializeField, Min(0f)] float _showDuration = 0.32f;
        [SerializeField, Min(0f)] float _hideDuration = 0.24f;

        Tween _scaleTween;

        void OnDisable()
        {
            _scaleTween?.Kill();
            if (_indicator == null) return;

            _indicator.transform.localScale = Vector3.zero;
            _indicator.SetActive(false);
        }

        public void SetCritical(bool active)
        {
            if (_indicator == null || !isActiveAndEnabled) return;

            _scaleTween?.Kill();
            if (active)
            {
                if (!_indicator.activeSelf)
                {
                    _indicator.transform.localScale = Vector3.zero;
                    _indicator.SetActive(true);
                }

                _scaleTween = _indicator.transform.DOScale(Vector3.one, _showDuration)
                    .SetEase(Ease.OutBack);
            }
            else if (_indicator.activeSelf)
            {
                _scaleTween = _indicator.transform.DOScale(Vector3.zero, _hideDuration)
                    .SetEase(Ease.InCubic)
                    .OnComplete(() => _indicator.SetActive(false));
            }
        }
    }
}
