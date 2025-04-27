
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
 
[Serializable]
public class DialogueClipWithName : PlayableAsset, ITimelineClipAsset
{
    public DialogueBehaviourWithName template = new DialogueBehaviourWithName();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviourWithName>.Create(graph, template);
        return playable;
    }
}

