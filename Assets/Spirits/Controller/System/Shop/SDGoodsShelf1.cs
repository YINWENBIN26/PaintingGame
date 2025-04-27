using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Command;
using System.IO;
using NPC;
public class SDGoodsShelf : MonoBehaviour
{
    SpriteRenderer goods; // 商品のスプライトレンダラー
    public SDPaintingClass painting; // 現在の絵
    public List<int> DestinationTranslist = new List<int>(); // 移動先リスト     
    public PlayerModel playerModel = null; // プレイヤーモデル

    private void Awake()
    {
        goods = transform.Find("goods").GetComponent<SpriteRenderer>(); // スプライトレンダラーの参照を取得
        playerModel = PlayerModel.GetModel; // プレイヤーモデルを取得
    }


    public SDPaintingClass Painting
    {
        get => painting;
        set
        {
            if (painting != null)
            {
                // 商品を取り除くコマンドを実行
                RemoveGoodsCommand removeGoodsCommand = new RemoveGoodsCommand();
                removeGoodsCommand.GoodsTraget = DestinationTranslist;
                removeGoodsCommand.Execute();
                print("Remove");
                goods.sprite = null; // スプライトをクリア
            }
            painting = value;
            if (value == null) return;
            if (painting.guid != null)
            {
                LoadImage(painting.guid); // 画像をロード

                // 商品を設定するコマンドを実行
                SetGoodsCommand setGoodsCommand = new SetGoodsCommand();
                setGoodsCommand.GoodsTraget = DestinationTranslist;
                setGoodsCommand.Execute();
            }
            else
            {
                goods.sprite = null; // スプライトをクリア
            }
        }
    }

    public void LoadImage(string guid)
    {
        // 画像ファイルのパスを組み立て
        string root = Application.streamingAssetsPath;
        string mat = Path.Combine(root, "SDImages");
        string filename = Path.Combine(mat, guid + ".png");

        if (!File.Exists(filename))
        {
            Debug.LogError("File does not exist: " + filename);
            return;
        }

        // 画像ファイルを読み込み、スプライトを設定
        byte[] fileData = File.ReadAllBytes(filename);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        Sprite temp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.2f, 0.2f));
        goods.sprite = temp;
        goods.transform.localPosition = new Vector2(-0.75f, -0.79f);
        goods.transform.localScale = new Vector2(0.22f, 0.31f);
    }

    public void UpdateBuyList()
    {
        // 販売リストを更新
        if (painting != null && painting.guid != null)
        {
            GameStateManager._Instance.nPCManager.sdBuyList.addSellList(this);
        }
    }
}

public enum ShelfState
{
    Set, // 設定済み
    None, // なし
}
