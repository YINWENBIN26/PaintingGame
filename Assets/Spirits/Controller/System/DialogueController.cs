using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueUI dialogueUI;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator DialogueStart(Queue<DialogueData> Dialogues)
    {
        while (Dialogues.Count > 0)
        {
            DialogueData temp = Dialogues.Dequeue();
            yield return StartCoroutine(dialogueUI.Show(temp.name,temp.Dialogue));
            
        }
        yield return new WaitForSeconds(2);
        dialogueUI.Hide();
    }

}

public class DialogueData
{
    public string name;
    public string Dialogue;
   public DialogueData(string name,string Dialogue)
    {
        this.name = name;
        this.Dialogue = Dialogue;
    }
    public DialogueData(string Dialogue)
    {
        name = null;
        this.Dialogue = Dialogue;
    }
}