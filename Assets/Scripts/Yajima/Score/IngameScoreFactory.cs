using UnityEngine;

namespace Yajima
{
    [CreateAssetMenu(fileName = "IngameScoreFactory", menuName = "TeamGemini/IngameScoreFactory")]
    public class IngameScoreFactory : MvpModelFactory<IngameScoreModel>
    {
        [SerializeField] float _comboTimeLimit = 1.5f;

        public override IngameScoreModel CreateModel()
        {
            return new IngameScoreModel(_comboTimeLimit);
        }
    }
}
