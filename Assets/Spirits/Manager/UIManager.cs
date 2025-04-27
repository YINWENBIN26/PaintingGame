using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerPropUI playerPropUI;
    public DealWinInfo dealWinInfo;
    public SelectWin selectWin;
    public DialogueUI dialogueUI;//
    public SecletedPaintingWin secletedPaintingWin;
    public PaintingDecideWin paintingDecideWin;


    public static UIManager _Instance;
    private void Awake()
    {
        if (_Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            _Instance = this;
        }
        else
        {
            Destroy(this);



        }
    }
}