
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
 
[Serializable]
public class Skode_DialogueClip : PlayableAsset, ITimelineClipAsset
{
    public Skode_DialogueBehaviour template = new Skode_DialogueBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<Skode_DialogueBehaviour>.Create(graph, template);
        return playable;
    }
}

