
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class Skode_DialogueBehaviour : PlayableBehaviour
{
    // このクリップが再生された時、UIに表示されるテキスト
    public string dialogueLine;

    [Tooltip("このクリップが再生完了後、タイムラインを一時停止する必要があるかどうか")]
    public bool hasToPause = false;

    /// <summary>
    /// このクリップが未再生か？ falseは未再生を意味します。
    /// </summary>
    bool clipPlayed = false;

    /// <summary>
    /// このクリップ再生後に一時停止が必要かどうか。falseは不要を意味します。
    /// hasToPauseの値に基づいて、タイムラインがこのクリップに到達した時に値が割り当てられるキャッシュされたbool変数。
    /// </summary>
    bool pauseScheduled = false;

    /// <summary>
    /// 現在のPlayableDirectorを取得します。
    /// </summary>
    PlayableDirector director;

    public override void OnPlayableCreate(Playable playable)
    {
        Debug.Log("OnPlayableCreate");
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Debug.Log("ProcessFrame");

        // このクリップがまだ再生されていない場合、現在のクリップのウェイト > 0
        // 効果は：タイムラインがこのクリップにちょうど入った時に一度実行されます。
        if (!clipPlayed && info.weight > 0f)
        {
            Debug.Log("ProcessFrame true");
            UIManager._Instance.dialogueUI.ShowTimeline(null, dialogueLine);

            // このクリップの再生完了後に一時停止が必要かどうかを検出し、値を割り当てます。
            if (Application.isPlaying && hasToPause)
            {
                pauseScheduled = true;
                SystemInstanceManager._Instance.timeLineController.isTimeLine = true;
            }

            clipPlayed = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // このコールバックは、TimeLineがちょうど実行され始めた時と、クリップが実行完了した時の両方で実行されます。
        // したがって、hasToPauseを直接判断することはできません。そうすると、TimeLineが実行され始めたばかりで一時停止してしまいます。

        if (pauseScheduled)
        {
            pauseScheduled = false;
            SystemInstanceManager._Instance.timeLineController.PauseTimeline(director);
        }
    }
}