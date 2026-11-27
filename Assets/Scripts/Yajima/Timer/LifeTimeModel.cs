using R3;
using UnityEngine;

public class LifeTimeModel
{
    readonly ReactiveProperty<float> _lifeTime = new(0);

    public ReadOnlyReactiveProperty<float> LifeTime => _lifeTime;

    public LifeTimeModel(float lifeTime)
    {
        _lifeTime.Value = lifeTime;
    }

    public void Update(float delta)
    {
        if (delta < 0) return;

        _lifeTime.Value = Mathf.Max(0, _lifeTime.Value - delta);
    }
}
