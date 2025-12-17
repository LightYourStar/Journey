using System;
using System.Threading;
using System.Threading.Tasks;

public class DelayedCallback
{
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// 延迟指定的时间后执行回调，并返回一个可用于取消延迟的任务的对象。
    /// </summary>
    /// <param name="delay">延迟的时间</param>
    /// <param name="callback">要执行的回调动作</param>
    /// <returns>可用于取消延迟的CancellationTokenSource</returns>
    public CancellationTokenSource SetTimeout(TimeSpan delay, Action callback)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Delay(delay, _cancellationTokenSource.Token)
            .ContinueWith(task =>
            {
                if (!task.IsCanceled)
                {
                    callback?.Invoke();
                }
            }, TaskScheduler.Default);

        return _cancellationTokenSource;
    }

    /// <summary>
    /// 取消延迟回调
    /// </summary>
    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
    }
}