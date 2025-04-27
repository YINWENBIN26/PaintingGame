using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintingControllerWin : UIBase
{
    Button BeginBut;
    Button StatusBut;
    Button StopBut;
    public GameObject Painting;
    // Start is called before the first frame update
    protected override void Awake()
    {
        BeginBut = transform.Find("BeginContinue").GetComponent<Button>();
        StatusBut = transform.Find("Status").GetComponent<Button>();
        StopBut = transform.Find("Stop").GetComponent<Button>();
        Hide();
        StatusBut.onClick.AddListener(delegate { });
        StopBut.onClick.AddListener(delegate { Stop(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowContinue()
    {
        gameObject.SetActive(true);
        BeginBut.onClick.RemoveAllListeners();
        BeginBut.gameObject.SetActive(false);
    }
    public void Show()
    {

        gameObject.SetActive(true);
        BeginBut.gameObject.SetActive(true);
        BeginBut.onClick.RemoveAllListeners();
        BeginBut.GetComponent<Image>().color = Color.white;
        BeginBut.gameObject.GetComponentInChildren<Text>().text = "Start";
        BeginBut.onClick.AddListener(delegate { UIManager._Instance.paintingDecideWin.show(); Hide(); });
    }
    public void Stop()
    {
        SelectWin selectWin = UIManager._Instance.selectWin;
        SelectWinDelegate selectWinDelegate = new SelectWinDelegate(this.GetComponent<UIBase>(), delegate { Painting.GetComponent<SDPaintingStatus>().CancelPainting(); });
        selectWin.show(selectWinDelegate, "Do you want to stop?");
    }
    
    public void ShowStatusWin()
    {
        
    }
}
