using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

public class PaintingDecideWin : UIBase
{
    Button DecideBtn;

    Toggle PaingtingTog;
    Toggle ScreenShootTog;
    Toggle LongTog;
    Toggle ShortTog;
    Toggle NormalTog;

    Image PaingtingTogImage;
    Image ScreenShootTogImage;
    Image LongTogImage;
    Image NormalTogImage;
    Image ShortTogImage;
    Dictionary<Toggle, E_PaintingPrompt> paintPromptTogDict = new Dictionary<Toggle, E_PaintingPrompt>();
    Transform PromptTran;
    public E_PaintingType type=E_PaintingType.None;
    public E_TimeType timeType=E_TimeType.None;

    public List<Toggle> selcetPrompt=new List<Toggle>();

    public void Start()
    {
        CloseBtn = transform.Find("CloseBtn").GetComponent<Button>();
        CloseBtn.onClick.AddListener(delegate { closewin(); });
        DecideBtn = transform.Find("DecideBut").GetComponent<Button>();
        PaingtingTog =transform.Find("PaintingStatus/Type/Painting").GetComponent<Toggle>() ;
        ScreenShootTog = transform.Find("PaintingStatus/Type/Signing").GetComponent<Toggle>();
        LongTog = transform.Find("PaintingStatus/Time/Long").GetComponent<Toggle>();
        NormalTog = transform.Find("PaintingStatus/Time/Normal").GetComponent<Toggle>();
        ShortTog = transform.Find("PaintingStatus/Time/Short").GetComponent<Toggle>();

        PaingtingTogImage = transform.Find("PaintingStatus/Type/Painting/Background").GetComponentInChildren<Image>();
        ScreenShootTogImage = transform.Find("PaintingStatus/Type/Signing/Background").GetComponentInChildren<Image>();
        LongTogImage = transform.Find("PaintingStatus/Time/Long/Background").GetComponentInChildren<Image>();
        NormalTogImage = transform.Find("PaintingStatus/Time/Normal/Background").GetComponentInChildren<Image>();
        ShortTogImage = transform.Find("PaintingStatus/Time/Short/Background").GetComponentInChildren<Image>();
        PromptTran = transform.Find("PaintingStatus/Prompt/Scroll View/Viewport/Content");

        PaingtingTog.onValueChanged.AddListener(delegate { ToggleValueChange(1); });
        ScreenShootTog.onValueChanged.AddListener(delegate { ToggleValueChange(2); });
        LongTog.onValueChanged.AddListener(delegate { ToggleValueChange(3); });
        NormalTog.onValueChanged.AddListener(delegate { ToggleValueChange(4); });
        ShortTog.onValueChanged.AddListener(delegate { ToggleValueChange(5); });
       // CreativeSlider.onValueChanged.AddListener(delegate { SliderChange(); });
        DecideBtn.onClick.AddListener(delegate { DecidePainting(); });
        ResetColor(1);
        ResetColor(2);

        // CreativeSlider.value = 0;
        PlayerModel playerModel = PlayerModel.GetModel;
        playerModel.AddPaintingPromptSetListener(AddPromptDict);
        playerModel.AddPaintingPromptSet(E_PaintingPrompt.OneGirl);
        playerModel.AddPaintingPromptSet(E_PaintingPrompt.Kawai);
        playerModel.AddPaintingPromptSet(E_PaintingPrompt.OneDog);
        playerModel.AddPaintingPromptSet(E_PaintingPrompt.OneCat);
    }


    /// <summary>
    /// if have new prompt,plus toggle 
    /// </summary>
    /// <param name="playerModel"></param>
    public void AddPromptDict(PlayerModel playerModel)
    {
        GameObject go = Resources.Load<GameObject>("UI/Gird/PromptToggle");
        go=Instantiate(go);
        go.transform.parent = PromptTran;
        go.transform.GetChild(1).GetComponent<Text>().text= playerModel.GetNewPrompt.ToString();
        Toggle toggle = go.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener((value) => {PromptToggleValueChange(toggle);});
        paintPromptTogDict.Add(go.GetComponent<Toggle>(), playerModel.GetNewPrompt);
    }

    /// <summary>
    /// toogle value change
    /// </summary>
    /// <param name="toggle"></param>
    public void PromptToggleValueChange(Toggle toggle)
    {
        if (!toggle.isOn)
        {
            // 当关闭一个开关时，重置颜色并确保状态是关闭的
            ToggleResetColor(toggle);
            if (selcetPrompt.Contains(toggle))
                selcetPrompt.Remove(toggle);
            return;
        }

        // 如果已经有两个或更多开关打开，关闭最早的那个
        if (selcetPrompt.Count >= 2)
        {
            Toggle toRemove = selcetPrompt[0];
            ToggleResetColor(toRemove);
            selcetPrompt.RemoveAt(0);
        }

        // 添加新的开关到列表并设置颜色和状态
        selcetPrompt.Add(toggle);
        toggle.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
        toggle.isOn = true; // 确保新添加的开关状态为打开
    }

    // 重置开关的颜色和状态
    void ToggleResetColor(Toggle toggle)
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        toggle.isOn = false; // 确保状态是关闭的
        toggle.onValueChanged.AddListener((value) => { PromptToggleValueChange(toggle); });
    }

    //change type toogle
    void ToggleValueChange(int a)
    {
        switch (a)
        {
            case 1: type = E_PaintingType.Illustration  ;  ResetColor(1); PaingtingTogImage.color = Color.grey; break;
            case 2: type = E_PaintingType.Icon; ResetColor(1); ScreenShootTogImage.color = Color.grey; break;
            case 3: timeType = E_TimeType.Long; ResetColor(2); LongTogImage.color = Color.grey; break;
            case 4: timeType = E_TimeType.Normal; ResetColor(2); NormalTogImage.color = Color.grey; break;
            case 5: timeType = E_TimeType.Short; ResetColor(2); ShortTogImage.color = Color.grey; break;
        }
        //StartCoroutine(DecideWorkingVlaue());
    }
    //reset toogle color
    private void ResetColor(int a)
    {
        switch(a)
        {
            case 1:
                PaingtingTogImage.color = Color.white;
                ScreenShootTogImage.color = Color.white;
                break;
            case 2:
                LongTogImage.color = Color.white;
                NormalTogImage.color = Color.white;
                ShortTogImage.color = Color.white;
                break;
        }
    }
    //decide painting button event
    void DecidePainting()
    {
        if(selcetPrompt.Count<2) return;
        if (timeType <= 0) return;
        if (type <= 0) return;

        PaintingManager._Instance.sDPaintingController.CreatePaint(paintPromptTogDict[selcetPrompt[0]], paintPromptTogDict[selcetPrompt[1]],timeType, type);
        Hide();
    }


    /*
    IEnumerator DecideWorkingVlaue()
    {
        float a = 0;
        switch (timeType)
        {
            case E_TimeType.Long: a=1.3f ; break;
            case E_TimeType.Normal: a= 1.1f;break;
            case E_TimeType.Short: a= 1f;  ; break;
            default:a=0 ;break;
        }
        yield return new WaitForSeconds(0.1f);/*
        switch (type)
        {
            case E_PaintingType.Illustration: WorkingValue = AlwaysInstanceManager._Instance.paintController.CanPaintingList[AlwaysInstanceManager._Instance.paintController.CanPaintingList.Count - 1].CostTime; break;
            case E_PaintingType.Icon: WorkingValue = AlwaysInstanceManager._Instance.paintController.CanSigningList[AlwaysInstanceManager._Instance.paintController.CanSigningList.Count - 1].CostTime; ; break;
            default: WorkingValue = 0; break;
        }
        WorkingValue =(int)((WorkingValue*a- CreativeVlaue));
        WorkingText.text = WorkingValue.ToString();
}
*/
}

public enum E_TimeType
{
    None=0,
    Long = 1,
    Normal=2,
    Short = 3,
    
}
