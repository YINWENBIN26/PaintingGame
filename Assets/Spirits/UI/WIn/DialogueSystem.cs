using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public Text DialogueText;
    public float waittime;
    public string Dialoguethis;
    public string Dialogue;
    int index;
    public bool isuse = false;
    public TextAsset dialogueAssest;


    private void Awake()
    {
        Hide();
    }
    private void Start()
    {
       
    }
    public IEnumerator Show()
    {
        isuse = true;
        Dialogue=dialogueAssest.ToString();
        print(Dialogue.Length);
        gameObject.SetActive(true);
        index = 0;
        Dialoguethis = null;
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
           // if (Input.GetMouseButtonDown(0))
            {
           //     DialogueText.text = Dialogue;
             //   isuse = false;
            }
        }
    }
    private void UpdateDialogue()
    {
        Dialoguethis += Dialogue[index++];
        DialogueText.text = Dialoguethis;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}

