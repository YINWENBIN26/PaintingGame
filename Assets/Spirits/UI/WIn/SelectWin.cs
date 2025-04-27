using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectWin : UIBase
{
    Button YesButton;
    Button NoButton;
    Text Text;
    protected override void Awake()
    {
        base.Awake();
        YesButton = transform.Find("YesButton").GetComponent<Button>();
        NoButton = transform.Find("NoButton").GetComponent<Button>();
        NoButton.onClick.AddListener(delegate { });
        Text= transform.Find("Text").GetComponent<Text>();
    }

    public void show(SelectWinDelegate selectWinDelegate,string text)
    {
        base.show();
        Text.text = text;
        YesButton.onClick.RemoveAllListeners();
        NoButton.onClick.RemoveAllListeners();
        if (selectWinDelegate.NeedClose)
        {
            if (selectWinDelegate.yesDelegate != null)
                YesButton.onClick.AddListener(delegate { selectWinDelegate.yesDelegate(); selectWinDelegate.UIWin.Hide(); Hide(); });
            else
                YesButton.onClick.AddListener(delegate { selectWinDelegate.UIWin.Hide(); Hide(); });
            if(selectWinDelegate.noDelegate != null)
            NoButton.onClick.AddListener(delegate { selectWinDelegate.noDelegate(); selectWinDelegate.UIWin.Hide(); Hide(); });
            else
                NoButton.onClick.AddListener(delegate { selectWinDelegate.UIWin.Hide(); Hide(); });
        }
        else
        {
            if (selectWinDelegate.yesDelegate != null)
                YesButton.onClick.AddListener(delegate { selectWinDelegate.yesDelegate(); Hide(); });
            else
                NoButton.onClick.AddListener(delegate { Hide(); });
            if (selectWinDelegate.noDelegate != null)
                NoButton.onClick.AddListener(delegate { selectWinDelegate.noDelegate(); Hide(); });
            else
                NoButton.onClick.AddListener(delegate { Hide(); });
        }
    }
}
public class SelectWinDelegate//注册selectwin委托 不同参数不同作用
{
    public delegate void SelectDelegate(params object[] objs);
    public SelectDelegate yesDelegate;
    public SelectDelegate noDelegate;
    public UIBase UIWin;
    private bool needClose;

    public bool NeedClose { get => needClose; }

    public SelectWinDelegate(UIBase UIWin, SelectDelegate yesDelegate, SelectDelegate noDelegate)
    {
        this.yesDelegate = yesDelegate;
        this.noDelegate = noDelegate;
        needClose = true;
        this.UIWin = UIWin;
    }
    public SelectWinDelegate(UIBase UIWin, SelectDelegate yesDelegate)
    {
        this.yesDelegate = yesDelegate;
        this.noDelegate = null;
        needClose = true;
        this.UIWin = UIWin;
    }
    public SelectWinDelegate(SelectDelegate yesDelegate, SelectDelegate noDelegate)
    {
        this.yesDelegate = yesDelegate;
        this.noDelegate = noDelegate;
        needClose = false;
        UIWin = null;
    }
    public SelectWinDelegate(SelectDelegate yesDelegate)
    {
        this.yesDelegate = yesDelegate;
        noDelegate = null;
        needClose = false;
        UIWin = null;
    }
}
