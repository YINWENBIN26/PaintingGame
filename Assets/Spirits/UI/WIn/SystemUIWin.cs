using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemUIWin : UIBase
{
    public Button BagButton;
    public Button LearnButton;
    public Button WorkButton;
    // Start is called before the first frame update
    protected override void Awake()
    {
        BagButton = transform.GetChild(0).GetComponent<Button>();
        LearnButton= transform.GetChild(1).GetComponent<Button>();
        WorkButton= transform.GetChild(2).GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
