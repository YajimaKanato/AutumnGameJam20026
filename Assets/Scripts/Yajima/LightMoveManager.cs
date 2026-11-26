using UnityEngine;
using Yajima;

public class LightMoveManager : IngameBehaviour
{
    [SerializeField] float _worldSpeedLightMoving = 1 / 15f;
    [SerializeField] LifeTimePresenter _presenter;
    [SerializeField] LifeEnergyPresenter _lifeEnergyPresenter;
    bool _lightMoving;

    public void ChangeLightMoving(bool lightMoving)
    {
        _lightMoving = lightMoving;
    }

    private void Update()
    {
        if (IsStopped) return;

        var delta = Time.deltaTime * (_lightMoving ? _worldSpeedLightMoving : 1);
        _presenter?.UpdateTimer(delta);
        _lifeEnergyPresenter?.ChangeEnergy(_lightMoving, Time.deltaTime);
    }
}
