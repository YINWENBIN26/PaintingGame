using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimeController : MonoBehaviour
{
    public static int StartHour = 8; // 開始時刻 (开始时间)
    int month = 3; // 現在の月 (当前月份)
    public int day = 20; // 現在の日 (当前日期)
    int week = 1; // 現在の週 (当前周数)
    int Year = 2022; // 現在の年 (当前年份)
    int min = 0; // 現在の分 (当前分钟)
    int hour = 6; // 現在の時刻 (当前小时)
    public static float TenMinTime = 6; // 10分ごとに2秒が経過します。(每10分钟2秒的模拟时间)

    public delegate void PerDayDelegate();
    public PerDayDelegate perDay; // 一日ごとのイベント。(每日事件)
    public delegate void PerWeekDelegate();
    public PerWeekDelegate preWeek; // 一週間ごとのイベント。(每周事件)
    public delegate void PerMonthDelegate();
    public PerDayDelegate perMonth; // 一ヶ月ごとのイベント。(每月事件)
    public delegate void PerSleepDelegate();
    public event PerSleepDelegate perSleep; // 睡眠時のイベント。(睡眠事件)
    WaitForSeconds wait = new WaitForSeconds(TenMinTime); // 時間経過をシミュレートする間隔。(模拟时间流逝的间隔)
    WaitForSeconds waitNewDay = new WaitForSeconds(TenMinTime); // 新しい日の待機時間。(新一天的等待时间)
    public GameObject BlackMask; // 画面暗転のオブジェクト。(画面遮罩对象)
    public Text text; // 日付を表示するテキスト。(显示日期的文本)

    void Start()
    {
        // 初期化処理。(初始化处理)
        BlackMask = GameObject.Find("Canvas/BlackMask");
        text = GameObject.Find("Canvas/BlackMask/Text").GetComponent<Text>();

        GotoNewDay(); // 新しい日の処理を開始します。(开始新一天的处理)
        perDay += stop; 
        perDay += GotoNewDay; // 新しい日の処理をイベントに登録します。(注册新一天的处理事件)
        text.text = "New DAY" + "  " + month + "/" + day.ToString();
        GameStateManager._Instance.ChangeState(new SetState()); // ゲームステートを変更します。(更改游戏状态)
    }

    /// <summary>
    /// 時間経過のコルーチン。(时间流逝的协程)
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeSpend()
    {
        // 一日の時間経過をシミュレートします。(模拟一天的时间流逝)
        hour = StartHour;
        min = 0;
        while (hour < 23)
        {
            // 時間と分を進めます。(推进时间和分钟)
            min += 10;
            if (min == 60)
            {
                hour += 1;
                min = 0;
            }

            // 特定の時間にイベントを実行します。(在特定时间执行事件)
            if (hour == 18)
            {
                SystemInstanceManager._Instance.audioController.ChangceAudio(0, 1);
            }
            if (hour == 22)
            {
                perSleep(); // 睡眠イベントを実行します。(触发睡眠事件)
                GameStateManager._Instance.ChangeState(new SetState());
                yield return wait;
                yield return wait;
                yield return wait;
                Turnday(); // 日付を変更します。(更改日期)
            }
            UIManager._Instance.playerPropUI.UpdateTime(min, hour, day, month, Year);
            yield return wait;
        }
    }

    /// <summary>
    /// 現在の日付を文字列で返します。(返回当前日期的字符串)
    /// </summary>
    public string ReturnTIme()
    {
        return Year + "/" + month + "/" + day;
    }

    /// <summary>
    /// 日の終わりの処理。(每日结束时的处理)
    /// </summary>
    public void stop()
    {
        // 日が終わるときの処理。(每日结束时的操作)
        BlackMask.SetActive(true);
        text.text = "New DAY" + "  " + month + "/" + day.ToString();
        GameStateManager._Instance.ChangeState(new SetState()); // ゲームステートを変更します。(更改游戏状态)
    }

    /// <summary>
    /// 新しい日への移行処理。(进入新一天的处理)
    /// </summary>
    public void GotoNewDay()
    {
        if (day > 20)
            PlayerModel.GetModel.CoinChange(-50); // 特定の日にコインを減らします。(在特定日期减少金币)
        StartCoroutine(StartNewDay()); // 新しい日の処理を開始します。(开始新一天的处理)
    }

    public GameObject GoodshelfSet; // 商品棚セット。(货架集合)
    public IEnumerator StartNewDay()
    {
        // 新しい日の開始処理。(开始新一天的处理)
        SystemInstanceManager._Instance.audioController.ChangceAudio(0, 0);
        yield return waitNewDay;
        BlackMask.SetActive(false); // 画面の暗転を解除します。(解除画面遮罩)
        yield return wait;
        GoodshelfSet.BroadcastMessage("UpdateBuyList"); // 商品リストを更新します。(更新商品列表)
        GameStateManager._Instance.ChangeState(new OpenShopState()); // ショップのステートに変更します。(切换到商店状态)
        StartCoroutine(TimeSpend()); // 時間経過の処理を再開します。(重新开始时间流逝的处理)
    }

    /// <summary>
    /// 日付を変更する処理。(更改日期的处理)
    /// </summary>
    public void Turnday()
    {
        StopCoroutine(TimeSpend()); // 時間経過のコルーチンを停止します。(停止时间流逝协程)

        day++;
        week++;
        if (week == 7)
        {
            preWeek(); // 週が変わったときのイベントを実行します。(触发每周事件)
        }
        if (day > 28)
        {
            day = 1;
            month++;

            if (month > 12)
            {
                month = 1;
                Year++;
            }
            if (perMonth != null)
            {
                perMonth(); // 月が変わったときのイベントを実行します。(触发每月事件)
            }
        }

        if (perDay != null)
        {
            perDay(); // 日が変わったときのイベントを実行します。(触发每日事件)
        }
    }

    /// <summary>
    /// 現在の時刻を文字列で返します。(返回当前时间的字符串)
    /// </summary>
    public string GetTime()
    {
        string minute = "00";
        if (min != 0)
        {
            minute = min.ToString();
        }
        return (day.ToString() + "/" + month.ToString() + "/" + Year.ToString() + " " + hour.ToString() + ":" + minute);
    }
}



