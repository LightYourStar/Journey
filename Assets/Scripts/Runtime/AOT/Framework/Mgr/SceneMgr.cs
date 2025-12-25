using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UIManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JO
{
    public sealed class SceneMgr
    {
        public string CurrentSceneName { get; private set; }
        public bool IsLoading => m_Cts != null;
        public event Action<string, string> OnBeforeSceneChange;
        public event Action<string> OnAfterSceneChange;

        private CancellationTokenSource m_Cts;
        private float m_MinVisibleSeconds = 0.6f;
        private float m_MaxFillSpeedPerSec = 1.8f;

        public void InitCurrentScene()
        {
            CurrentSceneName = SceneManager.GetActiveScene().name;
        }

        public void CancelCurrentLoad()
        {
            m_Cts?.Cancel();
        }

        public async void ChangeScene(string sceneName)
        {
            // UIManager.UIMgr.CloseAllUI();  todo等

            UIManager.UIMgr.OpenUI("LoadingUI");
            var view = (LoadingUI)UIManager.UIMgr.GetUIView("LoadingUI");
            IProgress<float> progress = view.CreateProgress();

            await SwitchToAsync(sceneName, progress);

            await UniTask.NextFrame();
            UIManager.UIMgr.CloseUI("LoadingUI");
        }

        public async UniTask SwitchToAsync(string nextSceneName, IProgress<float> progress = null, bool unloadUnusedAssets = true, bool allowSameSceneReload = false)
        {
            if (!allowSameSceneReload && string.Equals(CurrentSceneName, nextSceneName, StringComparison.Ordinal))
                return;

            if (IsLoading) return;

            m_Cts = new CancellationTokenSource();
            CancellationToken ct = m_Cts.Token;

            float shown = 0f;
            float t0 = Time.realtimeSinceStartup;

            void ReportSmoothTo(float target)
            {
                float dt = Time.unscaledDeltaTime;
                shown = Mathf.MoveTowards(shown, target, m_MaxFillSpeedPerSec * dt);
                progress?.Report(shown);
            }

            try
            {
                string preSceneName = CurrentSceneName;
                OnBeforeSceneChange?.Invoke(preSceneName, nextSceneName);

                AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
                op.allowSceneActivation = false;

                while (op.progress < 0.9f)
                {
                    ct.ThrowIfCancellationRequested();
                    progress?.Report(Mathf.Clamp01(op.progress / 0.9f) * 0.95f);
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                while ((Time.realtimeSinceStartup - t0) < m_MinVisibleSeconds || shown < 0.98f)
                {
                    ct.ThrowIfCancellationRequested();
                    ReportSmoothTo(0.98f);
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                op.allowSceneActivation = true;
                await op.ToUniTask(cancellationToken: ct);

                CurrentSceneName = nextSceneName;

                if (unloadUnusedAssets)
                {
                    await Resources.UnloadUnusedAssets().ToUniTask(cancellationToken: ct);
                    Global.gApp.gResMgr.UnLoadAssets();
                }

                progress?.Report(1f);
                OnAfterSceneChange?.Invoke(CurrentSceneName);
            }
            finally
            {
                m_Cts?.Dispose();
                m_Cts = null;
            }
        }
    }
}