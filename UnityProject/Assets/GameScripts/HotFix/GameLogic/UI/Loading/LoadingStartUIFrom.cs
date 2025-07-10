using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    public class LoadingStartUIFrom : UguiForm
    {
        protected override void OnOpen(object userData)
        {
            Log.Info("LoadingStartUIFrom OnOpen");
            base.OnOpen(userData);
            WaitToOpenMain();
        }

        private async UniTask WaitToOpenMain()
        {
            Log.Warning("LoadingStartUIFrom Debug.LogWarning");
            await UniTask.Delay(1000);
            GameModule.UI.OpenUIForm("LoadingMainUIFrom", 0);
        }


        protected override void OnInit(object userData)
        {
            Log.Info("LoadingStartUIFrom OnInit");
            base.OnInit(userData);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            Log.Info("LoadingStartUIFrom OnClose");
            base.OnClose(isShutdown, userData);
        }


        protected override void OnRecycle()
        {
            Log.Info("LoadingStartUIFrom OnRecycle");
            base.OnRecycle();
        }

        protected override void OnPause()
        {
            Log.Info("LoadingStartUIFrom OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Log.Info("LoadingStartUIFrom OnResume");
            base.OnResume();
        }

        protected override void OnCover()
        {
            Log.Info("LoadingStartUIFrom OnCover");
            base.OnCover();
        }
    }
}
