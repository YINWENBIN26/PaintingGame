using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using Map;

public class SystemInstanceManager : MonoBehaviour
{
    public static SystemInstanceManager _Instance;
    
    public TimeLineController timeLineController;
    public GameStateManager gameContorller;

    public TimeController timeManager;
    public Timer timer;
    public DialogueController dialogueController;
    public AudioController audioController;


    private void Awake()
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
    private void Update()
    {
    }
    void Start()
    {

    }
}
