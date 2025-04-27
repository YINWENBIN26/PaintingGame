using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Foundation;

public class DialogueUI : MonoBehaviour
{
    Text NameText;
    Text DialogueText;
    GameObject NameImageObj;
    public float waittime;
    public string Dialoguethis;
    public string Dialogue;
    int index;
    public bool isuse = false;


    private void Awake()
    {
        NameText = transform.Find("NameImage/NameText").GetComponent<Text>();
        DialogueText=transform.Find("DialogueImage/DialogueText").GetComponent<Text>();
        NameImageObj=transform.Find("NameImage").gameObject;
        Hide();
    }
    public void show(string name, string Dialogue)
    {
        gameObject.SetActive(true);
        StartCoroutine(Show(name, Dialogue));
    }
    public IEnumerator Show(string name, string Dialogue)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        isuse = true;
        index = 0;
        this.Dialogue = Dialogue;//空2竵E
        Dialoguethis = null;
        if (name == null||name == "".ToString())
        {
            NameImageObj.SetActive(false);
        }
        else
        {
            NameImageObj.SetActive(true);
            NameText.text = name;
        }
        if (isuse)
        {
            for (int i = 0; i < Dialogue.Length; i++)
            {
                UpdateDialogue();
                yield return new WaitForSecondsRealtime(waittime);
            }
        }
        yield return new WaitForSecondsRealtime(1.0f);
        isuse = false; 
    }
    public void Update()
    {
        if (isuse)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DialogueText.text = Dialogue;
                isuse = false;
            }
        }
    }

    public void ShowTimeline(string name, string Dialogue)
    {
        this.Dialogue = Dialogue;
        Dialoguethis = null;
        index = 0;
        gameObject.SetActive(true);
        if (name == null || name == "".ToString())
        {
            NameImageObj.SetActive(false);
        }
        else
        {
            NameImageObj.SetActive(true);
            NameText.text = name;
        }
        SystemInstanceManager._Instance.timer.Register("Dialogue", waittime, Dialogue.Length, true, delegate { UpdateDialogue(); });
    }

    private void UpdateDialogue()
    {
        
        Dialoguethis += Dialogue[index++];
        DialogueText.text = Dialoguethis;
       
    }
    IEnumerator ReadyHide()
    {
        yield return new WaitForSeconds(3);
        Hide();
    }
    public void Hide()
    {
       if(gameObject.activeSelf)
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
