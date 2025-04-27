using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using NPC;

public class OpenShopState : MonoBehaviour, IGameState
{
    private GameObject nPCIncubator;

    private NPCManager nPCManager;

    // 現在のインタラクション中のNPC (当前正在交互的NPC)
    public NPC.NPC nPC;

    public GameState gameState => GameState.OpenningShop;

  
    public void Exit()
    {
        Object.Destroy(nPCIncubator);
        foreach (NPC.NPC nPC in nPCIncubator.GetComponent<NPCManager>().npcList)
        {
            nPC.ChangeState(new NpcLeaveState());
        }
    }

    public void Enter()
    {
        nPCIncubator = Resources.Load<GameObject>("Perfab/system/shop/NPCIncubator");
        nPCIncubator = Instantiate(nPCIncubator);
        nPCIncubator.transform.SetParent(GameStateManager._Instance.NPCIncubator);
        nPCIncubator.transform.localPosition = new Vector3(0, 0, 0);
        GameStateManager._Instance.nPCManager = nPCIncubator.GetComponent<NPCManager>();
        nPCManager = GameStateManager._Instance.nPCManager;
    }

   
    public void Execute()
    {
        // マウスが安全エリア内になければ、戻る (如果鼠标不在安全区域内，返回)
        if (!Screen.safeArea.Contains(Input.mousePosition)) return;

        // マウスがどのUI要素も指していない場合 (如果鼠标未指向任何UI元素)
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // 商品棚にクリックされたかどうかを検出するためのレイを発射 (发射射线检测是否点击到商品货架)
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1 << LayerMask.NameToLayer("GoodShelf"));
            if (hit.collider != null) 
            {
                if (hit.collider.tag == "goods") 
                {
                    // 商品情報ヒントボックスを表示し、クリックロジックを処理 (显示商品信息提示框并处理点击逻辑)
                    GameStateManager._Instance.currentGoodsShelf = hit.collider.GetComponent<SDGoodsShelf>();
                    GameStateManager._Instance.SelectedGGoodsShelfTip.SetActive(true);
                    GameStateManager._Instance.SelectedGGoodsShelfTip.transform.SetParent(hit.transform);
                    GameStateManager._Instance.SelectedGGoodsShelfTip.transform.localPosition = new Vector3(0, 0, 0);
                    if (Input.GetMouseButtonDown(0)) // マウス左クリック時 (鼠标左键点击时)
                    {
                        // 商品取引ウィンドウを開く (打开商品交易窗口)
                        GameStateManager._Instance.currentSelectedGoodsShelf = GameStateManager._Instance.currentGoodsShelf;
                        if (GameStateManager._Instance.currentSelectedGoodsShelf != null)
                        {
                            UIManager._Instance.secletedPaintingWin.show();
                        }
                    }
                }
                else // 商品でない場合 (如果不是商品)
                {
                    // 商品情報ヒントボックスを非表示 (隐藏商品信息提示框)
                    GameStateManager._Instance.SelectedGGoodsShelfTip.SetActive(false);
                    GameStateManager._Instance.currentGoodsShelf = null;
                }
            }
            else // 衝突検出なし (未检测到碰撞)
            {
                // 商品情報ヒントボックスを非表示 (隐藏商品信息提示框)
                GameStateManager._Instance.SelectedGGoodsShelfTip.SetActive(false);
                GameStateManager._Instance.currentGoodsShelf = null;
            }
        }

        // 現在のインタラクション中のNPCがいなく、待機キューにNPCがいる場合
        // (当前没有正在交互的NPC且等待队列中有NPC时)
        if (nPC == null)
        {
            if (nPCManager.waitQueue.waitQueue.Count > 0)
            {
                //Debug.Log("Start Deal1");
                if (GameStateManager._Instance.candeal)
                {
                    //Debug.Log("Start Deal2");
                    if (WorkNPCManager._Instance.dealNPC != null) 
                    {
                        //Debug.Log("Start Deal3");
                        if (Input.GetKeyDown(KeyCode.F) && GameStateManager._Instance.nPCManager.dealController == null) // Fキーを押した時 (按下F键时)
                        {
                            // NPC取引ロジックを処理 (处理NPC交易逻辑)
                            GameStateManager._Instance.candeal = false;
                            nPC = nPCManager.waitQueue.waitQueue.Peek(); // キューからNPCを取り出す (从队列中取出NPC)
                            GameObject dealOBJ = Resources.Load<GameObject>("DealOBJ");
                            dealOBJ = GameObject.Instantiate(dealOBJ);
                            DealController dealController = dealOBJ.GetComponent<DealController>(); // NPCに取引コンポーネントを追加 (为NPC添加交易组件)
                            GameStateManager._Instance.nPCManager.dealController = dealController;
                            dealController.NPC = nPC;
                        }
                    }
                }
            }
        }
    }
}
