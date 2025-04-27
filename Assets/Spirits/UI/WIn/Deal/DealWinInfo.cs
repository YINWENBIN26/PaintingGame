using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DealWinInfo : MonoBehaviour
{
    // 各種ボタンの定義
    Button wanUpBtn; // 万単位の増加ボタン
    Button wanDownBtn; // 万単位の減少ボタン
    Button thousandUpBtn; // 千単位の増加ボタン
    Button thousandDownBtn; // 千単位の減少ボタン
    Button hundredUpBtn; // 百単位の増加ボタン
    Button hundredDownBtn; // 百単位の減少ボタン
    Button tenUpBtn; // 十単位の増加ボタン
    Button tenDownBtn; // 十単位の減少ボタン
    Button geUpBtn; // 一単位の増加ボタン
    Button geDownBtn; // 一単位の減少ボタン
    Button Confirm; // 確認ボタン

    Image Icon; // アイテムアイコン

    // 価格を表示するテキスト
    Text wanText;
    Text thousandText;
    Text hundredText;
    Text tenText;
    Text geText;

    // アイコンを設定するメソッド
    public void setIcon(ItemClass tempResName)
    {
        int hashCode = tempResName.itemType.GetHashCode();
        string enumParseStr = ItemType.Parse(typeof(ItemType), hashCode.ToString()).ToString();
        Icon.sprite = Resources.Load<Sprite>("Icon/" + enumParseStr + "/" + tempResName.ResName);
    }

    // 画像をロードするメソッド
    public void LoadImage(string guid)
    {
        string root = Application.streamingAssetsPath;
        string mat = Path.Combine(root, "SDImages");
        string filename = Path.Combine(mat, guid + ".png");

        if (!File.Exists(filename))
        {
            Debug.LogError("File does not exist: " + filename);
            return;
        }

        byte[] fileData = File.ReadAllBytes(filename);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        Sprite temp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        Icon.sprite = temp;
    }

    // SDPaintingClassを使用して表示を更新するメソッド
    public void Show(SDPaintingClass sDPaintingClass)
    {
        Show();
        LoadImage(sDPaintingClass.guid);
        Price = sDPaintingClass.price;
        UpdatePrice();
    }

    // SDPaintingClassと価格を指定して表示を更新するメソッド
    public void Show(SDPaintingClass sDPaintingClass, int price)
    {
        Show();
        LoadImage(sDPaintingClass.guid);
        Price = price;
        UpdatePrice();
    }

    // 価格を更新するプライベートメソッド
    private void UpdatePrice()
    {
        int temp = Price;
        geText.text = (temp % 10).ToString();
        temp /= 10;
        tenText.text = (temp % 10).ToString();
        temp /= 10;
        hundredText.text = (temp % 10).ToString();
        temp /= 10;
        thousandText.text = (temp % 10).ToString();
        temp /= 10;
        wanText.text = (temp % 10).ToString();
    }

    // コンポーネントの初期化
    void Awake()
    {
        // 各コンポーネントへの参照を取得
        Icon = transform.Find("Item/IconBG/Icon").GetComponent<Image>();
        wanUpBtn = transform.Find("Price/wan/Up").GetComponent<Button>();
        wanDownBtn = transform.Find("Price/wan/Down").GetComponent<Button>(); ;
        thousandUpBtn = transform.Find("Price/thousand/Up").GetComponent<Button>();
        thousandDownBtn = transform.Find("Price/thousand/Down").GetComponent<Button>();
        hundredUpBtn = transform.Find("Price/hundred/Up").GetComponent<Button>();
        hundredDownBtn = transform.Find("Price/hundred/Down").GetComponent<Button>();
        tenUpBtn = transform.Find("Price/ten/Up").GetComponent<Button>();
        tenDownBtn = transform.Find("Price/ten/Down").GetComponent<Button>();
        geUpBtn = transform.Find("Price/ge/Up").GetComponent<Button>();
        geDownBtn = transform.Find("Price/ge/Down").GetComponent<Button>();
        Confirm = transform.Find("Price/Confirm").GetComponent<Button>();

        wanText = transform.Find("Price/wan/icon/Text").GetComponent<Text>(); ;
        thousandText = transform.Find("Price/thousand/icon/Text").GetComponent<Text>();
        hundredText = transform.Find("Price/hundred/icon/Text").GetComponent<Text>();
        tenText = transform.Find("Price/ten/icon/Text").GetComponent<Text>();
        geText = transform.Find("Price/ge/icon/Text").GetComponent<Text>();

        wanUpBtn.onClick.AddListener(delegate { Up(wanUpBtn); });
        wanDownBtn.onClick.AddListener(delegate { Down(wanDownBtn); }); ;
        thousandUpBtn.onClick.AddListener(delegate { Up(thousandUpBtn); }); ;
        thousandDownBtn.onClick.AddListener(delegate { Down(thousandDownBtn); }); ;
        hundredUpBtn.onClick.AddListener(delegate { Up(hundredUpBtn); }); ;
        hundredDownBtn.onClick.AddListener(delegate { Down(hundredDownBtn); }); ;
        tenUpBtn.onClick.AddListener(delegate { Up(tenUpBtn); }); ;
        tenDownBtn.onClick.AddListener(delegate { Down(tenDownBtn); }); ;
        geUpBtn.onClick.AddListener(delegate { Up(geUpBtn); }); ;
        geDownBtn.onClick.AddListener(delegate { Down(geDownBtn); }); ;
        Confirm.onClick.AddListener(delegate { ConfirmPrice(); }); ;
        Hide(); // 初期状態では非表示
    }

    // ボタンが押された時の増加処理
    private void Up(Button button)
    {
        Text temp = button.transform.parent.Find("icon/Text").GetComponent<Text>();
        if (int.Parse(temp.text) < 9)
            temp.text = (int.Parse(temp.text) + 1).ToString();
    }

    // ボタンが押された時の減少処理
    private void Down(Button button)
    {
        Text temp = button.transform.parent.Find("icon/Text").GetComponent<Text>();
        if (int.Parse(temp.text) > 0)
            temp.text = (int.Parse(temp.text) - 1).ToString();
    }

    public int Price; // 現在の価格

    // 確認ボタンが押された時の処理
    void ConfirmPrice()
    {
        // 価格の計算と設定
        Price = int.Parse(wanText.text) * 10000 + int.Parse(thousandText.text) * 1000 + int.Parse(hundredText.text) * 100 + int.Parse(tenText.text) * 10 + int.Parse(geText.text);
        // DealControllerに価格を設定
        GameStateManager._Instance.nPCManager.dealController.price = Price;
        this.Hide(); // ウィンドウを非表示
    }

    // ウィンドウを表示
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // ウィンドウを非表示
    void Hide()
    {
        gameObject.SetActive(false);
    }
}