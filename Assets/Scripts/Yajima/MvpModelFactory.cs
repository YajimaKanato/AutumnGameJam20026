using UnityEngine;

namespace Yajima
{
    public abstract class MvpModelFactory<TModel> : ScriptableObject where TModel : class
    {
        public abstract TModel CreateModel();
    }
}
