using R3;
using UnityEngine;
using Yajima;

public class LifeTimePresenter : MvpPresenter<LifeTimeModel>
{
    [SerializeField] LifeTimeView _view;
    [SerializeField] LifeTimeFactory _factory;
    [SerializeField] float _lifeTime = 10f;

    public ReadOnlyReactiveProperty<float> RemainingTime => Model?.LifeTime;

    protected override MvpModelFactory<LifeTimeModel> Factory => _factory;

    protected override LifeTimeModel CreateDefaultModel()
    {
        return new LifeTimeModel(_lifeTime);
    }

    protected override void BindView(LifeTimeModel model)
    {
        if (_view == null) return;

        Track(model.LifeTime.Subscribe(_view.UpdateTimer));
    }

    public void UpdateTimer(float delta)
    {
        if (!IsStopped) Model?.Update(delta);
    }
}
