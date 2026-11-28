using R3;
using UnityEngine;

namespace Yajima
{
    public class IngameScoreModel
    {
        public const int MaxScore = 99999;
        readonly ReactiveProperty<int> _score = new(0);
        readonly ReactiveProperty<int> _combo = new(0);
        float _comboTimeLimit;
        float _delta;

        public ReadOnlyReactiveProperty<int> Score => _score;
        public ReadOnlyReactiveProperty<int> Combo => _combo;

        public IngameScoreModel(float comboTimeLimit = 0)
        {
            _score.Value = 0;
            _combo.Value = 0;
            _comboTimeLimit = comboTimeLimit;
            _delta = 0;
        }

        public void GetScore(int score)
        {
            if (score <= 0) return;

            _combo.Value++;
            AddBonus((long)score + ComboCalculator.CalculateBonus(_combo.Value));
            _delta = _comboTimeLimit;
        }

        public void AddBonus(long points)
        {
            if (points > 0)
                _score.Value = (int)System.Math.Min(_score.Value + points, MaxScore);
        }

        public void MeasureComboTime(float deltaTime)
        {
            if (deltaTime <= 0) return;

            _delta = Mathf.Max(_delta - deltaTime, 0);

            if (_delta <= 0)
                _combo.Value = 0;
        }
    }
}
