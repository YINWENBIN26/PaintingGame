using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// シミュレーションに関連する静的な計算を行うクラスです。
public static class SimulateClass
{
    // 魅力値の基本倍率
    public static float AttracionSeries = 0.5f;
    // NPCスコアの基本値
    public static float NPCSSeries = 20f;
    // プレイヤーの速度スコアの基本値
    public static int PlayerSpeedSSeries = 1;
    // NPCの基本数
    public static float NPCCount = 100f;

    /// <summary>
    /// 魅力によるシミュレーションを行います。
    /// </summary>
    /// <param name="attraction">プレイヤーの魅力値</param>
    /// <returns>計算後の値</returns>
    public static float AttracionSimulate(int attraction)
    {
        float a = 10.0f;
        if (SystemInstanceManager._Instance.timeManager.day >= 22)
        {
            a = 80 - attraction / NPCSSeries;
            if (a < 3)
            {
                a = 3;
            }
        }
        else
        {
            a = 20 - attraction / NPCSSeries;
            if (a < 5)
            {
                a = 5;
            }
        }
        return a;
    }

    /// <summary>
    /// NPCの数に対するシミュレーションを行います。
    /// </summary>
    /// <param name="attraction">プレイヤーの魅力値</param>
    /// <returns>計算後のNPCの数</returns>
    public static float NPCSimulate(int attraction)
    {
        float a = 10.0f;
        if (SystemInstanceManager._Instance.timeManager.day >= 22)
        {
            a = 80 - attraction / NPCSSeries;
            if (a < 3)
            {
                a = 3;
            }
        }
        else
        {
            a = 20 - attraction / NPCSSeries;
            if (a < 5)
            {
                a = 5;
            }
        }
        return a;
    }

    /// <summary>
    /// 絵の描画速度に対するシミュレーションを行います。
    /// </summary>
    /// <returns>計算後の描画速度</returns>
    public static int PaintingSpeed()
    {
        // 100ごとに速度を1点上げます。
        int a = (int)(PlayerModel.GetModel.GetSpeed * PlayerSpeedSSeries / 100);
        return a;
    }

    /// <summary>
    /// NPCの数を魅力値に基づいてシミュレーションします。
    /// </summary>
    /// <param name="attraction">プレイヤーの魅力値</param>
    /// <returns>計算後のNPCの数</returns>
    public static int NPCSCount(int attraction)
    {
        float a = 10.0f;
        if (SystemInstanceManager._Instance.timeManager.day >= 22)
        {
            a = attraction / NPCCount + 3;
        }
        else
        {
            a = attraction / NPCCount + 5;
        }

        if (a > 20)
        {
            a = 20;
        }
        return (int)a;
    }
}
