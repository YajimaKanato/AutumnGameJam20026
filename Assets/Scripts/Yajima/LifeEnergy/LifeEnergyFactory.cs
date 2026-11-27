using UnityEngine;

namespace Yajima
{
    [CreateAssetMenu(fileName = "LifeEnergyFactory", menuName = "TeamGemini/LifeEnergyFactory")]
    public class LifeEnergyFactory : MvpModelFactory<LifeEnergyModel>
    {
        [SerializeField, Min(0.001f)] float _maxEnergy = 1f;
        [SerializeField, Min(0.001f)] float _recoveryDuration = 1f;
        [SerializeField, Min(0.001f)] float _drainDuration = 15f;
        [SerializeField, Min(1)] int _recoverEvery = LifeEnergyModel.DefaultRecoverEvery;
        [SerializeField, Range(0f, 1f)] float _recoveryFraction = LifeEnergyModel.DefaultRecoveryFraction;

        public override LifeEnergyModel CreateModel()
        {
            return new LifeEnergyModel(_maxEnergy, _recoveryDuration, _drainDuration, _recoverEvery, _recoveryFraction);
        }
    }
}
