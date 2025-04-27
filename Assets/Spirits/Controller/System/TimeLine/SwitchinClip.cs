
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
 
[Serializable]
public class SwitchinClip : PlayableAsset, ITimelineClipAsset
{
    public SwitchingBehaviour template = new SwitchingBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SwitchingBehaviour>.Create(graph, template);
        return playable;
    }
}

