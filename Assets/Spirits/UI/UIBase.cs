using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    protected Button CloseBtn;
    protected virtual void Awake()
    {
        CloseBtn = transform.Find("CloseBtn").GetComponent<Button>();
        CloseBtn.onClick.AddListener(delegate { closewin(); });
        Hide();
    }
    public virtual void closewin()
    {
        Hide();
    } 
    public virtual void show()
    {
        gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
