using UnityEngine;
using Yajima;

[CreateAssetMenu(fileName = "LifeTimeFactory", menuName = "TeamGemini/LifeTimeFactory")]
public class LifeTimeFactory : MvpModelFactory<LifeTimeModel>
{
    [SerializeField] float _lifeTime = 10f;

    public override LifeTimeModel CreateModel()
    {
        return new LifeTimeModel(_lifeTime);
    }
}
