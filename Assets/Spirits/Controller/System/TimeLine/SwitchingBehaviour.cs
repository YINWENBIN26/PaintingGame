
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

[Serializable]
public class SwitchingBehaviour : PlayableBehaviour
{  

    /// <summary>
    /// �õ���ǰ PlayableDirector
    /// </summary>
    PlayableDirector director;
    bool isNeedSwitch = false;
    public override void OnPlayableCreate(Playable playable)
    {
        Debug.Log("OnPlayableCreate");
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Debug.Log("ProcessFrame");

        //����δ���Ÿ�clip����ǰclip��Ȩ��>0
        //Ч���ǣ�ʱ����ս����clipʱִ��һ��
        if (info.weight > 0f)
        {

            //��ⲥ�����clip���Ƿ���Ҫ��ͣ������ֵ
            if (Application.isPlaying )
            {
                isNeedSwitch = true;
            }
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        //��Ϊ�ûص����ڸ�����TimeLine��ִ����clipʱ����ִ��һ�Ρ�
        //��˲���ֱ���ж� hasToPause �����������TimeLine�͸���ͣ�ˡ�

        if (isNeedSwitch)
        {
            isNeedSwitch = false;
            SceneManager.LoadScene("Scenes/MainSense");
        }
    }
}