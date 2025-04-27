using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonUIWin : MonoBehaviour
{
    
    public Button SkipButton;
    // Start is called before the first frame update
    void Awake()
    {
        SkipButton.onClick.AddListener(delegate { Skip(); });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Skip()
    {
        SceneManager.LoadScene("Scenes/MainSense");
    }
    
}
