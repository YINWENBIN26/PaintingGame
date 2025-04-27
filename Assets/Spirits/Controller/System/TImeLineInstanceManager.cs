using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TImeLineInstanceManager : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public TimeLineController timeLineController;
    public GameObject ReadingCanvas;

    public static TImeLineInstanceManager _Instance;

    void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

}
