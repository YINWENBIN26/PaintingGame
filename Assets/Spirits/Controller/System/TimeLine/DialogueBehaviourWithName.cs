
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class DialogueBehaviourWithName : PlayableBehaviour
{
    //���ŵ���clipʱ��UIҪ��ʾ������
    public string dialogueLine;
    public string Speakername;

    [Tooltip("�������clip���Ƿ���Ҫ��ͣTimeLine")]
    public bool hasToPause = false;

    /// <summary>
    /// δ���Ÿ�clip��  falseΪδ����
    /// </summary>
    bool clipPlayed = false;

    /// <summary>
    /// �������clip���Ƿ���Ҫ��ͣ��falseΪ����Ҫ��
    /// �����bool����������ʱ�������е���clipʱ������ hasToPause ֵ��ֵ��
    /// </summary>
    bool pauseScheduled = false;

    bool HideUI = false;

    /// <summary>
    /// �õ���ǰ PlayableDirector
    /// </summary>
    PlayableDirector director;

    public override void OnPlayableCreate(Playable playable)
    {
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

        //����δ���Ÿ�clip����ǰclip��Ȩ��>0
        //Ч���ǣ�ʱ����ս����clipʱִ��һ��
        if (!clipPlayed && info.weight > 0f)
        {
            TImeLineInstanceManager._Instance.dialogueUI.show(Speakername, dialogueLine);

            //��ⲥ�����clip���Ƿ���Ҫ��ͣ������ֵ
            if (Application.isPlaying && hasToPause)
            {
                pauseScheduled = true;
                TImeLineInstanceManager._Instance.timeLineController.isTimeLine = true;
            }
            if (Application.isPlaying)
            {
                HideUI = true;
            }

            clipPlayed = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        //��Ϊ�ûص����ڸ�����TimeLine��ִ����clipʱ����ִ��һ�Ρ�
        //��˲���ֱ���ж� hasToPause �����������TimeLine�͸���ͣ�ˡ�

        if (pauseScheduled)
        {
            pauseScheduled = false;
            TImeLineInstanceManager._Instance.timeLineController.PauseTimeline(director);
        }
        if (HideUI)
        {
            HideUI = false;
            TImeLineInstanceManager._Instance.dialogueUI.Hide();
        }
    }
}