using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDictionary : MonoBehaviour
{
    public Dictionary<int, ItemClass> ItemDict = new Dictionary<int, ItemClass>(); // アイテム辞書
    public TextAsset KnapsackData; // バックパックデータ
    public Dictionary<int, NPCprop> NPCPrefabDict = new Dictionary<int, NPCprop>(); // NPC辞書
    public TextAsset NpcData; // NPCデータ

    private void Awake()
    {
        // データの読み込み
        ReadKnapsackData();
        ReadNPCData();
    }

    // バックパックデータを読み込む
    void ReadKnapsackData()
    {
        string str = KnapsackData.ToString(); // テキストデータを文字列に変換
        string[] PerKnapsackStrArray = str.Split('\n'); // 改行で分割
        foreach (string tempStr in PerKnapsackStrArray)
        {
            string[] proArray = tempStr.Split('|'); // '|'でプロパティを分割
            ItemClass gc = new ItemClass(int.Parse(proArray[0]), proArray[1], proArray[2], int.Parse(proArray[3]), proArray[4], int.Parse(proArray[5]), int.Parse(proArray[6]));
            ItemDict.Add(int.Parse(proArray[0]), gc);
        }
    }

    // NPCデータを読み込む
    void ReadNPCData()
    {
        string str = NpcData.ToString(); // テキストデータを文字列に変換
        string[] PerNPCStrArray = str.Split('\n'); // 改行で分割
        foreach (string tempStr in PerNPCStrArray)
        {
            string[] proArray = tempStr.Split('|'); // '|'でプロパティを分割
            NPCprop temp = new NPCprop(proArray[1], int.Parse(proArray[2]), int.Parse(proArray[3]), int.Parse(proArray[4]), int.Parse(proArray[5]));
            NPCPrefabDict.Add(int.Parse(proArray[0]), temp);
        }
    }
}
