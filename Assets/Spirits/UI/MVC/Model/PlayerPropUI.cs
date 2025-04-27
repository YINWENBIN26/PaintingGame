using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// プレイヤーのプロパティを表示するUIコンポーネント
public class PlayerPropUI : MonoBehaviour
{
    // 通貨表示用テキスト (用于显示货币的文本)
    Text CoinText;
    // 時間表示用テキスト (用于显示时间的文本)
    Text TimeText;
    private void Awake()
    {
        // TimeTextとCoinTextの取得と初期化 (获取并初始化TimeText和CoinText)
        TimeText = transform.Find("Time/Text").GetComponent<Text>();
        CoinText = transform.Find("Coin/Coin").GetComponent<Text>();
        // 通貨変更リスナーの追加。通貨が変更されたときにUIを更新し、音を変更する。 (添加货币更改监听器。当货币变化时更新UI并更改音效)
        PlayerModel.GetModel.AddCoinChangeListener(delegate
        {
            UpdateCoin();
            SystemInstanceManager._Instance.audioController.ChangceAudio(1, 3);
        });
    }

    private void OnEnable()
    {
    }

    /// <summary>
    /// UIを表示します。 (显示UI)
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// UIを非表示にします。 (隐藏UI)
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 通貨テキストを更新します。 (更新货币文本)
    /// </summary>
    public void UpdateCoin()
    {
        CoinText.text = PlayerModel.GetModel.GetCoin.ToString();
    }

    /// <summary>
    /// 時間テキストを更新します。 (更新时间文本)
    /// </summary>
    /// <param name="min">分 </param>
    /// <param name="hour">時</param>
    /// <param name="day">日</param>
    /// <param name="month">月)</param>
    /// <param name="year">年</param>
    public void UpdateTime(int min, int hour, int day, int month, int year)
    {
        string minute = min.ToString().PadLeft(2, '0');
        string formattedTime = string.Format("{0}/{1}/{2}  {3}:{4}", day, month, year, hour, minute);

        TimeText.text = formattedTime;
    }
}
