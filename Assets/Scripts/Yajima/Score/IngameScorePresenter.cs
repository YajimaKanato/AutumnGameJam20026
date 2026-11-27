using R3;
using UnityEngine;

namespace Yajima
{
    public class IngameScorePresenter : MvpPresenter<IngameScoreModel>
    {
        [SerializeField] IngameScoreView _view;
        [SerializeField] IngameScoreFactory _factory;
        [SerializeField] float _comboTimeLimit = 1.5f;

        protected override MvpModelFactory<IngameScoreModel> Factory => _factory;

        protected override IngameScoreModel CreateDefaultModel()
        {
            return new IngameScoreModel(_comboTimeLimit);
        }

        protected override void BindView(IngameScoreModel model)
        {
            if (_view == null) return;

            Track(model.Score.Subscribe(_view.GetScore));
            Track(model.Combo.Subscribe(_view.AddCombo));
        }

        public void AddScore(int score)
        {
            if (!IsStopped) Model?.GetScore(score);
        }

        private void Update()
        {
            if (!IsStopped) Model?.MeasureComboTime(Time.deltaTime);
        }
    }
}
