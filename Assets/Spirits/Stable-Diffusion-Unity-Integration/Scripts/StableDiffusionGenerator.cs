using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class StableDiffusionGenerator : MonoBehaviour
{
    static protected StableDiffusionConfiguration sdc = null;
    private Coroutine _updateProgressRunning = null;

    /// <summary>
    /// Update a generation progress bar
    /// </summary>
    protected void UpdateGenerationProgress()
    {
        // Stable diffusion API url for setting a model
        string url = sdc.settings.StableDiffusionServerURL + sdc.settings.ProgressAPI;

        float progress = 0;

        using (WebClient client = new WebClient())
        {
            // Send the GET request
            string responseBody = client.DownloadString(url);
            
            // Deserialize the response to a class
            SDProgress sdp = JsonConvert.DeserializeObject<SDProgress>(responseBody);
            progress = sdp.progress;

        }
    }

    /// <summary>
    /// Update a generation progress bar with auth
    /// </summary>
    protected void UpdateGenerationProgressWithAuth()
    {
        if (_updateProgressRunning != null) return;
        _updateProgressRunning = StartCoroutine(UpdateGenerationProgressWithAuthCor());
    }

    private IEnumerator UpdateGenerationProgressWithAuthCor()
    {
        // Stable diffusion API url for setting a model
        string url = sdc.settings.StableDiffusionServerURL + sdc.settings.ProgressAPI;
        float progress = 0;

        using (UnityWebRequest modelInfoRequest = UnityWebRequest.Get(url))
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(sdc.settings.user + ":" + sdc.settings.pass);
            string encodedText = Convert.ToBase64String(bytesToEncode);

            modelInfoRequest.SetRequestHeader("Authorization", "Basic " + encodedText);
            yield return modelInfoRequest.SendWebRequest();

            // Deserialize the response to a class
            SDProgress sdp = JsonConvert.DeserializeObject<SDProgress>(modelInfoRequest.downloadHandler.text);
            progress = sdp.progress;

        }

        _updateProgressRunning = null;
        yield return null;
    }
    
}
