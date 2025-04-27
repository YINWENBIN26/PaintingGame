using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SDPaintingGird : MonoBehaviour
{
    Image Icon; // 絵のアイコン (绘画的图标)
    Text Name; // 絵の名前 (绘画的名称)
    Text Price; // 価格 (价格)
    Text TypeText; // 絵のタイプ (绘画的类型)
    Text CompleteTimeText; // 完成時間 (完成时间)
    Button SelectBtn; // 選択ボタン (选择按钮)
    public int price; // 公開価格 (公开价格)
    public string guid; // 絵の一意識別子 (绘画的唯一标识符)
    private SDPaintingClass painting; // 現在の絵オブジェクト (当前绘画对象)
    public int ListIndex; // リスト内のインデックス (列表中的索引)

    private void Awake()
    {
        // 各コンポーネントの参照を取得 (获取各组件的引用)
        Icon = transform.Find("Icon").GetComponent<Image>();
        Name = transform.Find("Name").GetComponent<Text>();
        Price = transform.Find("Price").GetComponent<Text>();
        TypeText = transform.Find("Type/Text").GetComponent<Text>();
        CompleteTimeText = transform.Find("CompleteTime/Text").GetComponent<Text>();
        SelectBtn = transform.Find("SelectBtn").GetComponent<Button>();
    }

    /// <summary>
    /// 絵を売る (出售绘画)
    /// </summary>
    public void Sell()
    {
        if (guid != null)
        {
            // 絵を売りリストに追加 (将绘画添加到出售列表)
            GameStateManager._Instance.nPCManager.sdBuyList.addSellList(GameStateManager._Instance.currentSelectedGoodsShelf);
            // 選択された棚から絵を削除 (从选中的货架中移除绘画)
            GameStateManager._Instance.currentSelectedGoodsShelf.Painting = KnapsackModel.GetModel.paintingListDelete(guid);
        }
        // 選択ウィンドウを閉じる (关闭选择窗口)
        UIManager._Instance.secletedPaintingWin.Hide();
    }

    /// <summary>
    /// 絵を取り除く (移除绘画)
    /// </summary>
    void RemovePainting()
    {
        if (guid != null)
        {
            // 絵をリストに戻す (将绘画返回到列表)
            KnapsackModel.GetModel.paintingListAdd(painting);
            // 売りリストから絵を削除 (从出售列表中移除绘画)
            GameStateManager._Instance.nPCManager.sdBuyList.RomoveSellList(GameStateManager._Instance.currentSelectedGoodsShelf);
            // 現在選択されている棚の絵をクリア (清除当前选中货架的绘画)
            GameStateManager._Instance.currentSelectedGoodsShelf.Painting = null;
        }
        // 選択ウィンドウを閉じる (关闭选择窗口)
        UIManager._Instance.secletedPaintingWin.Hide();
    }

    /// <summary>
    /// 絵のグリッドを表示 (显示绘画网格)
    /// </summary>
    /// <param name="paintingClass">絵のクラス (绘画类)</param>
    public void UpdateShow(SDPaintingClass paintingClass)
    {
        // 絵の情報を表示用に更新 (更新绘画信息以供显示)
        guid = paintingClass.guid;
        painting = paintingClass;
        price = paintingClass.price;
        Price.text = paintingClass.price.ToString();
        CompleteTimeText.text = painting.completeTime.ToString();
        TypeText.text = paintingClass.paintingType.ToString();
        LoadImage(); // 画像をロード (加载图像)
        Name.text = paintingClass.name;
        SelectBtn.onClick.AddListener(delegate { Sell(); }); // 売る機能をボタンに割り当て (将出售功能分配给按钮)
    }

    /// <summary>
    /// 絵の画像をロード (加载绘画图像)
    /// </summary>
    public void LoadImage()
    {
        string root = Application.streamingAssetsPath; // 基本パス (基础路径)
        string mat = Path.Combine(root, "SDImages"); // 画像フォルダー (图像文件夹)
        string filename = Path.Combine(mat, guid + ".png"); // 画像ファイル名 (图像文件名)

        if (!File.Exists(filename))
        {
            Debug.LogError("File does not exist: " + filename);
            return;
        }

        byte[] fileData = File.ReadAllBytes(filename);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        Sprite temp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        Icon.sprite = temp; // アイコンに画像を設定 (将图像设置为图标)
    }

    /// <summary>
    /// 取り除くグリッドを表示 (显示移除网格)
    /// </summary>
    public void UpdateShow()
    {
        // 現在選択されている棚から絵の情報を取得し、表示を更新 (从当前选中的货架获取绘画信息并更新显示)
        SDPaintingClass paintingClass = GameStateManager._Instance.currentSelectedGoodsShelf.Painting;
        painting = paintingClass;
        guid = paintingClass.guid;
        price = paintingClass.price;
        Price.text = paintingClass.price.ToString();
        CompleteTimeText.text = painting.completeTime.ToString();
        TypeText.text = paintingClass.paintingType.ToString();
        LoadImage(); 
        Name.text = paintingClass.name;
        SelectBtn.GetComponentInChildren<Text>().text = "Remove"; // ボタンのテキストを「取り除く」に変更 (将按钮文本更改为“移除”)
        SelectBtn.onClick.AddListener(delegate { RemovePainting(); }); // 取り除く機能をボタンに割り当て (将移除功能分配给按钮)
    }
}