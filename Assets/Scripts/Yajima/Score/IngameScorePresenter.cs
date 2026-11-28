using R3;
using UnityEngine;

namespace Yajima
{
    public class IngameScorePresenter : MvpPresenter<IngameScoreModel>
    {
        [SerializeField] IngameScoreView _view;
        [SerializeField] IngameScoreFactory _factory;
        [SerializeField] float _comboTimeLimit = 1.5f;
        public int TotalScore => Model?.Score.CurrentValue ?? 0;

        public void AddSurvivalBonus(int points) => Model?.AddBonus(points);

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

        public void AddScore(int score) => AwardScore(score);

        public int AwardScore(int score)
        {
            if (IsStopped || Model == null) return 0;

            int previousScore = TotalScore;
            Model.GetScore(score);
            return TotalScore - previousScore;
        }

        private void Update()
        {
            if (!IsStopped) Model?.MeasureComboTime(Time.deltaTime);
        }
    }
}
