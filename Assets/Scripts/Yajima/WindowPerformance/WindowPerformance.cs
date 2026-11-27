using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class WindowPerformance : MonoBehaviour
{
    public abstract UniTask OpenWindow();


    public abstract UniTask CloseWindow();
}
