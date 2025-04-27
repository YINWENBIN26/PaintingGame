using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BeginSceneController : MonoBehaviour
{
    public Button StartButton;
    public Button StopButton;
    public Button SetButton;
    public DialogueSystem dialogueSystem;
    public GameObject ImageObject;

    private void Awake()
    {
        StartButton = transform.Find("StartButton").GetComponent<Button>();
        StopButton= transform.Find("StopButton").GetComponent<Button>();
        SetButton = transform.Find("SetButton").GetComponent<Button>();
        StartButton.onClick.AddListener(delegate { StartGame(); });
        StopButton.onClick.AddListener(delegate { StoptGame(); });
        SetButton.onClick.AddListener(delegate { });

    }
    private void StartGame()
    {
        StartCoroutine(I_StartGAME());

    }
    IEnumerator I_StartGAME()
    {
        ImageObject.SetActive(false);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Scenes/OpeningScence");
    }
    private void StoptGame()
    {
     Application.Quit();
    }



}
