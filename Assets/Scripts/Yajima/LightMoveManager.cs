using UnityEngine;
using Yajima;

public class LightMoveManager : IngameBehaviour
{
    [SerializeField] float _worldSpeedLightMoving = 1 / 15f;
    [SerializeField] LifeTimePresenter _presenter;
    [SerializeField] LifeEnergyPresenter _lifeEnergyPresenter;
    [SerializeField, Range(0.01f, 1f)] float _unfinishedSpeedLightMoving = 0.25f;
    [SerializeField] UnfinishedPool _unfinishedPool;
    bool _lightMoving;

    public void ChangeLightMoving(bool lightMoving)
    {
        _lightMoving = lightMoving;
        _unfinishedPool?.SetWorldSpeed(lightMoving ? _unfinishedSpeedLightMoving : 1f);
    }

    public void SetUnfinishedPool(UnfinishedPool pool) => _unfinishedPool = pool;

    private void Update()
    {
        if (IsStopped) return;

        var delta = Time.deltaTime * (_lightMoving ? _worldSpeedLightMoving : 1);
        _presenter?.UpdateTimer(delta);
        if (IsStopped) return;
        _lifeEnergyPresenter?.ChangeEnergy(_lightMoving, Time.deltaTime);
    }
}
