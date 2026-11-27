using R3;
using UnityEngine;

namespace Yajima
{
    public class CriticalEventPresenter : MvpPresenter<CriticalEventModel>
    {
        [SerializeField] CriticalEvent _factory;
        [SerializeField] CriticalEventView _view;

        public ReadOnlyReactiveProperty<bool> IsCritical => Model?.IsCritical;

        protected override MvpModelFactory<CriticalEventModel> Factory => _factory;

        protected override CriticalEventModel CreateDefaultModel()
        {
            return new CriticalEventModel(0, 0f);
        }

        protected override void BindView(CriticalEventModel model)
        {
            if (_view != null)
                Track(model.IsCritical.Subscribe(_view.SetCritical));
        }

        public void StartSchedule(float timeLimit)
        {
            Init();
            Model.StartSchedule(timeLimit);
        }

        public void UpdateRemainingTime(float remainingTime)
        {
            Model?.UpdateRemainingTime(remainingTime);
        }
    }
}
