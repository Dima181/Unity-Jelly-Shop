using Mine.CodeBase.Framework.Manager.UINavigator.Runtime;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Mine.CodeBase.Framework.Presenter
{
    public class BackPresenter : IStartable
    {
        public void Start() => 
            Observable.EveryUpdate()
                .Where(_ =>
                {
                    return Input.GetKeyDown(KeyCode.Escape);
                })
                .Subscribe(_ =>
                {
                    UIContainer.BackAsync().Forget();
                });
    }
}
