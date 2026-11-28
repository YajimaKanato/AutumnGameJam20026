using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Yajima
{
    public class LifeEnergyView : IngameBehaviour
    {
        [SerializeField] Image _energyImage;
        [SerializeField] Image _tankImage;
        [SerializeField] Sprite _normalTank;
        [SerializeField] Sprite _warningTank;
        [SerializeField] Sprite _criticalTank;
        [SerializeField] Sprite _criticalEventTank;
        [SerializeField, Min(0f)] float _fillDuration = 0.1f;

        float _targetEnergy;
        float _fillVelocity;
        int _band = -1;
        bool _criticalEventActive;
        Tween _tankEffect;
        Vector2 _tankPosition;
        Vector3 _tankScale;
        Sprite _initialTank;

        protected override void OnPaused()
        {
            _tankEffect?.Pause();
        }

        protected override void OnResumed()
        {
            _tankEffect?.Play();
        }

        protected override void OnFinished()
        {
            ResetTankEffect();
            if (_energyImage != null) _energyImage.fillAmount = _targetEnergy;
        }

        public void UpdateEnergy(float energy)
        {
            if (_energyImage == null) return;

            _targetEnergy = energy;
            if (_band < 0) _energyImage.fillAmount = energy;
            int band = energy > 0.5f ? 2 : energy > 0.2f ? 1 : 0;
            if (band == _band) return;

            _energyImage.color = band == 2
                ? new Color(0.31f, 0.92f, 0.73f)
                : band == 1 ? new Color(1f, 0.72f, 0.32f) : new Color(1f, 0.34f, 0.38f);

            if (_tankImage != null)
            {
                if (_band < 0)
                {
                    _tankPosition = _tankImage.rectTransform.anchoredPosition;
                    _tankScale = _tankImage.rectTransform.localScale;
                    _initialTank = _tankImage.sprite;
                }

                if (_band >= 0)
                {
                    ResetTankEffect();
                    var rect = _tankImage.rectTransform;
                    _tankEffect = (band < _band
                        ? rect.DOShakeAnchorPos(0.25f, new Vector2(5f, 0f), 14)
                        : rect.DOPunchScale(Vector3.one * 0.07f, 0.28f))
                        .SetUpdate(true)
                        .OnKill(() => _tankEffect = null);
                    if (IsStopped) _tankEffect.Pause();
                }
            }

            _band = band;
            UpdateTankSprite();
        }

        public void SetCriticalCondition(bool active)
        {
            _criticalEventActive = active;
            if (_band >= 0) UpdateTankSprite();
        }

        void UpdateTankSprite()
        {
            if (_tankImage == null) return;

            var sprite = _band == 2 ? _normalTank : _band == 1 ? _warningTank : _criticalTank;
            if (_criticalEventActive && _criticalEventTank != null)
                sprite = _criticalEventTank;
            _tankImage.sprite = sprite != null ? sprite : _initialTank;
        }

        private void LateUpdate()
        {
            if (IsStopped || _energyImage == null) return;

            float current = _energyImage.fillAmount;
            _energyImage.fillAmount = _fillDuration <= 0f || Mathf.Abs(current - _targetEnergy) < 0.0005f
                ? _targetEnergy
                : Mathf.SmoothDamp(current, _targetEnergy, ref _fillVelocity, _fillDuration,
                    Mathf.Infinity, Time.unscaledDeltaTime);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ResetTankEffect();
        }

        void ResetTankEffect()
        {
            _tankEffect?.Kill();
            if (_tankImage == null || _band < 0) return;

            _tankImage.rectTransform.anchoredPosition = _tankPosition;
            _tankImage.rectTransform.localScale = _tankScale;
        }
    }
}
