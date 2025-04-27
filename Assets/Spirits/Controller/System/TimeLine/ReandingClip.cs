using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
 
[Serializable]
public class ReandingClip : PlayableAsset, ITimelineClipAsset
{
    public ReadingBehaviour template = new ReadingBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ReadingBehaviour>.Create(graph, template);
        return playable;
    }
}

