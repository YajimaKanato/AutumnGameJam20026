using R3;
using System;
using UnityEngine;

namespace Yajima
{
    public class LifeEnergyPresenter : MvpPresenter<LifeEnergyModel>
    {
        [SerializeField] LifeEnergyView _view;
        [SerializeField] LifeEnergyFactory _factory;
        [SerializeField, Min(0.001f)] float _maxEnergy = 1f;
        [SerializeField, Min(0.001f)] float _recoveryDuration = 1f;
        [SerializeField, Min(0.001f)] float _drainDuration = 15f;
        [SerializeField, Min(1)] int _recoverEvery = LifeEnergyModel.DefaultRecoverEvery;
        [SerializeField, Range(0f, 1f)] float _recoveryFraction = LifeEnergyModel.DefaultRecoveryFraction;

        bool _criticalCondition;

        public event Action Depleted;

        protected override MvpModelFactory<LifeEnergyModel> Factory => _factory;

        protected override LifeEnergyModel CreateDefaultModel()
        {
            return new LifeEnergyModel(_maxEnergy, _recoveryDuration, _drainDuration, _recoverEvery, _recoveryFraction);
        }

        protected override void BindView(LifeEnergyModel model)
        {
            if (_view == null) return;

            Track(model.Energy.Subscribe(_view.UpdateEnergy));
        }

        public void ChangeEnergy(bool lightMoving, float delta)
        {
            if (IsStopped || Model == null || Model.Energy.CurrentValue <= 0f) return;

            Model.ChangeEnergy(lightMoving, delta, _criticalCondition);
            if (Model.Energy.CurrentValue > 0f) return;

            Depleted?.Invoke();
        }

        public void SetCriticalCondition(bool active)
        {
            _criticalCondition = active;
            _view?.SetCriticalCondition(active);
        }

        public void OnUnfinishedResolved(int score)
        {
            if (!IsStopped) Model?.ResolveUnfinished();
        }
    }
}
