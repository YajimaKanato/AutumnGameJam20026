using R3;
using UnityEngine;

namespace Yajima
{
    public class LifeEnergyModel
    {
        internal const int DefaultRecoverEvery = 5;
        internal const float DefaultRecoveryFraction = 0.25f;

        readonly ReactiveProperty<float> _energy;

        public ReadOnlyReactiveProperty<float> Energy => _energy;
        readonly float _maxEnergy;
        float _currentEnergy;
        int _resolvedCount;
        readonly float _recoveryDuration;
        readonly float _drainDuration;
        readonly int _recoverEvery;
        readonly float _recoveryFraction;

        float Progress => _currentEnergy / _maxEnergy;

        public LifeEnergyModel(float maxEnergy, float recoveryDuration, float drainDuration,
            int recoverEvery = DefaultRecoverEvery, float recoveryFraction = DefaultRecoveryFraction)
        {
            _maxEnergy = Mathf.Max(0.001f, maxEnergy);
            _currentEnergy = _maxEnergy;
            _energy = new(Progress);
            _recoveryDuration = Mathf.Max(0.001f, recoveryDuration);
            _drainDuration = Mathf.Max(0.001f, drainDuration);
            _recoverEvery = Mathf.Max(1, recoverEvery);
            _recoveryFraction = Mathf.Clamp01(recoveryFraction);
        }

        public void ChangeEnergy(bool lightMoving, float delta, bool criticalCondition = false)
        {
            if (delta <= 0f) return;

            float rate = lightMoving ? -1f / _drainDuration : 1f / _recoveryDuration;
            ApplyEnergy(rate * delta * (criticalCondition ? Mathf.Pow(2, Mathf.Sign(-rate)) : 1f));
        }

        public void ResolveUnfinished()
        {
            if (_currentEnergy <= 0f || ++_resolvedCount < _recoverEvery) return;

            _resolvedCount = 0;
            ApplyEnergy(_recoveryFraction);
        }

        void ApplyEnergy(float fraction)
        {
            if (_currentEnergy <= 0f) return;

            _currentEnergy = Mathf.Clamp(_currentEnergy + _maxEnergy * fraction, 0f, _maxEnergy);
            _energy.Value = Progress;
        }
    }
}
