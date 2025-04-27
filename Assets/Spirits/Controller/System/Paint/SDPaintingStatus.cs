using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SDPaintingStatus : MonoBehaviour
{
    [HideInInspector]
    public float PaintingTime; // 描画時間 (绘画时间)
    [HideInInspector]
    public bool IsPainting; // 描画中かどうか (是否正在绘画)
    [HideInInspector]
    float timer; // タイマー (计时器)
    [HideInInspector]
    public float process = 1.0f; // 処理速度 (处理速度)

    public Transform paintingTrans; // 描画トランスフォーム (绘画变换)

    public string generalPrompt; // 共通プロンプト (通用提示词)
    public string generalNegativePrompt; // 共通ネガティブプロンプト (通用负面提示词)

    public string prompt; // プロンプト (提示词)
    [HideInInspector]
    public E_PaintingType type; // 絵のタイプ (绘画类型)

    private SDPaintingClass painting; // 現在の絵画オブジェクト (当前绘画对象)
    public WorkingNpc npc; // 現在作業中のNPC (当前工作的NPC)
    StableDiffusionText2Image stableDiffusion; // Stable Diffusionコンポーネント (Stable Diffusion组件)

    public StableDiffusionText2Image StableDiffusion
    {
        get
        {
            if (stableDiffusion == null)
                stableDiffusion = PaintingManager._Instance.GetComponent<StableDiffusionText2Image>();
            return stableDiffusion;
        }
    }

    public SDPaintingClass Painting
    {
        get
        {
            if (painting == null)
                painting = new SDPaintingClass(StableDiffusion.Getguid(), null, 1, 0, null, 0, 0, 0, 0);
            return painting;
        }
        set => painting = value;
    }

    void Awake()
    {
        PaintingTime = 0; // 描画時間を初期化 (初始化绘画时间)
        transform.Find("Painting").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Icon/white");
        WorkNPCManager._Instance.PaintingStatusesList.Add(this); // 絵画ステータスリストに追加 (添加到绘画状态列表)
    }

    void Update()
    {
        // NPCがある場合、描画プロセスを進行させます。(如果存在NPC，则推进绘画过程)
        if (npc != null)
        {
            timer += Time.deltaTime * 5;
            if (timer >= process)
            {
                timer -= process;
                SystemInstanceManager._Instance.audioController.ChangceAudio(1, 2); // オーディオの変更を行います。(更改音频)
                PaintingTime += 1 * SimulateClass.PaintingSpeed(); // 描画時間を進行させます。(推进绘画时间)
                if (PaintingTime > Painting.costTime / 1.5)
                {
                    PaintingManager._Instance.stableDiffusionText2Image.RenewSpirte(); // 描画が一定の進行を達成したら、スプライトを更新します。(达到一定进度时更新精灵)
                }
                if (PaintingTime >= Painting.costTime)
                {
                    FinishPainting(); // 描画が完了したら、FinishPaintingメソッドを呼び出します。(绘画完成时调用FinishPainting方法)
                }
            }
        }
    }
    // 描画開始 (开始绘画)
    public void StartSDPainting() 
    {
        // Stable Diffusionモデルにプロンプトを渡し、画像生成を開始します。
        // (将提示词传递给Stable Diffusion模型并开始生成图像)
        IsPainting = true;
        Painting.paintingType = type;
        prompt = Painting.prompt1.ToString() + "," + Painting.prompt2.ToString() + "," + generalPrompt + "," + type.ToString();
        StableDiffusion.prompt = prompt;
        StableDiffusion.negativePrompt = generalNegativePrompt;
        StableDiffusion.Generate(transform.GetChild(0).GetComponent<SpriteRenderer>());
        //
        Painting.price = Painting.costTime * Random.Range(10, 50) / 200; // 描画価格を計算 (计算绘画价格)
        Painting.attraction = Painting.price * Random.Range(1, 100) / 100; // 魅力を計算 (计算吸引力)
    }
    // 描画停止 (停止绘画)
    public void StopPainting() 
    {
        IsPainting = false;
    }

    // 描画完了 (绘画完成)
    void FinishPainting()
    {
        Painting.completeTime = SystemInstanceManager._Instance.timeManager.GetTime(); 
        KnapsackModel.GetModel.paintingListAdd(Painting); 
        npc.SetWait(); // NPCを待機状態にする (将NPC设置为等待状态)
        WorkNPCManager._Instance.PaintingStatusesList.Remove(this); // 絵画ステータスリストから削除 (从绘画状态列表中移除)
        Destroy(gameObject); 
    }

    // 描画キャンセル (取消绘画)
    public void CancelPainting()
    {
        Destroy(this.gameObject); // このオブジェクトを破棄 (销毁该对象)
    }
}

