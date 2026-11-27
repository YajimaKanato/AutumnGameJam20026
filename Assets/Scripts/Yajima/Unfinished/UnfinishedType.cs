using UnityEngine;

namespace Yajima
{
    public enum UnfinishedType
    {
        [InspectorName("共通プレハブ")] Default = 0,
        [InspectorName("ロードローラーで人をペタンコにする")] RoadRollerFlatten = 1,
        [InspectorName("時限爆弾の最後のコードを切る")] TimeBombWireCut = 2,
        [InspectorName("札束で人の顔をはたく")] MoneyBundleSlap = 3,
        [InspectorName("勇者の剣を引き抜く")] PullHeroSword = 4,
        [InspectorName("精密機器を壊す")] SmashPrecisionMachine = 5,
        [InspectorName("ロケットの発射ボタンを押す")] LaunchRocket = 6
    }
}
