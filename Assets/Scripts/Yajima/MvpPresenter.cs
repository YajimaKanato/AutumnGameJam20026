using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yajima
{
    public abstract class MvpPresenter<TModel> : IngameBehaviour where TModel : class
    {
        readonly List<IDisposable> _subscriptions = new();

        protected TModel Model { get; private set; }

        public void Init()
        {
            Unsubscribe();
            Model = Factory?.CreateModel() ?? CreateDefaultModel();
            if (isActiveAndEnabled) Subscribe();
        }

        protected abstract MvpModelFactory<TModel> Factory { get; }
        protected abstract TModel CreateDefaultModel();
        protected abstract void BindView(TModel model);

        protected void Track(IDisposable subscription)
        {
            _subscriptions.Add(subscription);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Subscribe();
        }

        protected override void OnDisable()
        {
            Unsubscribe();
            base.OnDisable();
        }

        void Subscribe()
        {
            if (Model == null || _subscriptions.Count != 0) return;

            BindView(Model);
        }

        void Unsubscribe()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();
        }
    }
}
