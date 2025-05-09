
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class ReadingBehaviour : PlayableBehaviour
{  

    /// <summary>
    /// 得到当前 PlayableDirector
    /// </summary>
    PlayableDirector director;
    bool isNeedStop = false;
    public override void OnPlayableCreate(Playable playable)
    {
        Debug.Log("OnPlayableCreate");
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Debug.Log("ProcessFrame");

        //若还未播放该clip，当前clip的权重>0
        //效果是：时间轴刚进入该clip时执行一次
        if (info.weight > 0f)
        {
            TImeLineInstanceManager._Instance.ReadingCanvas.SetActive(true);

            //检测播放完该clip后是否需要暂停，并赋值
            if (Application.isPlaying )
            {
                isNeedStop = true;
            }
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        //因为该回调会在刚运行TimeLine、执行完clip时都会执行一次。
        //因此不能直接判断 hasToPause ，否则刚运行TimeLine就给暂停了。

        if (isNeedStop)
        {
            isNeedStop = false;
            TImeLineInstanceManager._Instance.timeLineController.PauseTimeline(director);
        }
    }
}