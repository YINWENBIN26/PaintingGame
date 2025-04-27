using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// SD絵画の制御を担当するコントローラークラス
public class SDPaintController : MonoBehaviour
{
    public GameObject Nowpaiting = null;

    // 現在操作している絵画UIのコントローラー (当前正在操作的绘画UI控制器)
    public PaintingControllerWin paintingControllerWin;

    // SD絵画のプレハブ (SD绘画的预制体)
    GameObject SDPainting;

    void Start()
    {
        // SD絵画のプレハブを読み込む (加载SD绘画的预制体)
        SDPainting = Resources.Load<GameObject>("Perfab/Work/Painting/SDPaintingOBJ");
    }

    void Update()
    {
        // 画面外のクリックを無視 (忽略屏幕外的点击)
        if (!Screen.safeArea.Contains(Input.mousePosition)) return;

        // UI上のクリックを無視 (忽略在UI上的点击)
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000, 1 << LayerMask.NameToLayer("PaintingUI"));
            if (hit.collider != null)
            {
                // 絵画にヒットした場合 (点击到了绘画)
                if (hit.collider.tag == "Painting")
                {
                    if (Input.GetMouseButtonDown(0)) // 左クリックされた場合 (左键点击的情况下)
                    {
                        if (paintingControllerWin != null) // 以前のPaintUIが閉じていない場合は閉じる (如果之前的绘画UI未关闭，则关闭)
                        {
                            paintingControllerWin.gameObject.SetActive(false);
                        }
                        paintingControllerWin = hit.collider.transform.GetChild(0).GetComponent<PaintingControllerWin>();

                        // 既に絵が描かれている場合は続きから、そうでなければ新規に開始 (如果已绘制则继续，否则重新开始)
                        if (paintingControllerWin.Painting != null)
                        {
                            paintingControllerWin.ShowContinue();
                        }
                        else
                        {
                            paintingControllerWin.Show();
                        }
                    }
                }
            }
            else
            {
                // 絵画以外をクリックした場合は、開いている絵画UIを閉じる (点击非绘画区域时关闭打开的绘画UI)
                if (Input.GetMouseButtonDown(0) && paintingControllerWin != null)
                {
                    paintingControllerWin.Hide();
                }
            }
        }
    }

    /// <summary>
    /// 新しい絵画を作成します。(创建新绘画)
    /// </summary>
    /// <param name="type1">絵画のプロンプト1 (绘画的提示1)</param>
    /// <param name="type2">絵画のプロンプト2 (绘画的提示2)</param>
    /// <param name="timeType">制作時間のタイプ (制作时间类型)</param>
    /// <param name="paintingType">絵画のタイプ (绘画类型)</param>
    public void CreatePaint(E_PaintingPrompt type1, E_PaintingPrompt type2, E_TimeType timeType, E_PaintingType paintingType)
    {
        // 絵画の初期設定 (初始化绘画)
        print(type1.ToString() + " and " + type2.ToString() + " painting is being created.");
        GameObject newPainting = Instantiate(SDPainting);
        newPainting.transform.SetParent(paintingControllerWin.transform.parent.GetChild(1).GetChild(0), false);
        newPainting.transform.localPosition = Vector3.zero;
        newPainting.transform.Find("Painting").localPosition = Vector3.zero;

        // 絵画の状態を設定 (设置绘画状态)
        SDPaintingStatus paintingStatus = newPainting.GetComponent<SDPaintingStatus>();
        paintingStatus.Painting.prompt1 = type1;
        paintingStatus.Painting.prompt2 = type2;
        paintingStatus.Painting.name = "The " + type1.ToString() + " of " + type2.ToString();
        paintingStatus.Painting.paintingType = paintingType;
        paintingStatus.Painting.costTime = 1000; // 制作時間の基本値 (制作时间的基本值)

        // 制作時間のタイプに応じて時間を調整 (根据制作时间类型调整时间)
        if (timeType == E_TimeType.Long)
        {
            paintingStatus.Painting.costTime = (int)(paintingStatus.Painting.costTime * 1.5f);
        }
        else if (timeType == E_TimeType.Normal)
        {
            paintingStatus.Painting.costTime = (int)(paintingStatus.Painting.costTime * 1.0f);
        }

        // 絵画の制作を開始 (开始绘画的制作)
        paintingStatus.StartSDPainting();
    }
}
