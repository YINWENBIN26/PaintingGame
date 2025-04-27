using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using NPC;
public class GameStateManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameStateManager _Instance;
    private IGameState currentState;
    public IGameState CurrentState { get => currentState; }
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
        ChangeState(new OpenShopState());
    }
    private void Start()
    {

    }

    private void Update()
    {
        currentState?.Execute();
    }

    public void ChangeState(IGameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    [Header("OpenShop")]
    //openshopのパラメータ
    [HideInInspector] public SDGoodsShelf currentSelectedGoodsShelf;
    [HideInInspector] public SDGoodsShelf currentGoodsShelf;
    public GameObject SelectedGGoodsShelfTip;
    [HideInInspector] public GameObject tempObject;
    public NPCManager nPCManager;
    public Transform NPCIncubator;
    [HideInInspector] public NPC.NPC nPC;
     public bool candeal = true;

}

public enum GameState
{
    OpenningShop= 1,
    Setting=2, 
    Painting = 3,
    CG=4,

}
