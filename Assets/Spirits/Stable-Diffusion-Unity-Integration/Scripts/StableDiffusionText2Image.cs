﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Collections;

/// <summary>
/// Component to help generate a UI Image or RawImage using Stable Diffusion.
/// </summary>
[ExecuteAlways]
public class StableDiffusionText2Image : StableDiffusionGenerator
{
    [ReadOnly]
    public string guid = "";
    
    
    public string prompt;
    public string negativePrompt;

    /// <summary>
    /// List of samplers to display as Drop-Down in the inspector
    /// </summary>
    [SerializeField]
    public string[] samplersList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.samplers;
        }
    }
    /// <summary>
    /// Actual sampler selected in the drop-down list
    /// </summary>
    [HideInInspector]
    public int selectedSampler = 0;

    public int width = 512;
    public int height = 512;
    public int steps = 90;
    public float cfgScale = 7;
    public long seed = -1;

    public long generatedSeed = -1;

    string filename = "";



    /// <summary>
    /// List of models to display as Drop-Down in the inspector
    /// </summary>
    [SerializeField]
    public string[] modelsList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.modelNames;
        }
    }
    /// <summary>
    /// Actual model selected in the drop-down list
    /// </summary>
    [HideInInspector]
    public int selectedModel = 0;


    /// <summary>
    /// On Awake, fill the properties with default values from the selected settings.
    /// </summary>
    protected virtual void Awake()
    {
        if (width < 0 || height < 0)
        {
            StableDiffusionConfiguration sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            if (sdc != null)
            {
                SDSettings settings = sdc.settings;
                if (settings != null)
                {

                    width = settings.width;
                    height = settings.height;
                    steps = settings.steps;
                    cfgScale = settings.cfgScale;
                    seed = settings.seed;
                    return;
                }
            }

            width = 512;
            height = 512;
            steps = 50;
            cfgScale = 7;
            seed = -1;
        }
    }


    protected virtual void Update()
    {
        // Clamp image dimensions values between 128 and 2048 pixels
        if (width < 128) width = 128;
        if (height < 128) height = 128;
        if (width > 2048) width = 2048;
        if (height > 2048) height = 2048;

        // If not setup already, generate a GUID (Global Unique Identifier)
        if (guid == "")
            guid = Guid.NewGuid().ToString();
    }
    public string Getguid()
    {
        guid =Guid.NewGuid().ToString();
        return guid;
    }
    // Internally keep tracking if we are currently generating (prevent re-entry)
    public bool generating = false;

    /// <summary>
    /// Callback function for the inspector Generate button.
    /// </summary>
    public void Generate(SpriteRenderer spriteRenderer)
    {

        // Start generation asynchronously
        if (!string.IsNullOrEmpty(prompt))
        {
            if (PaintingManager._Instance.useSD)
            {
                StartCoroutine(GenerateAsync(spriteRenderer));
            }
            else
            {
                StartCoroutine(NoSDLoadImage(spriteRenderer));
            }
        }
    }
    public bool UseSD=true;

    /// <summary>
    /// Setup the output path and filename for image generation
    /// </summary>
    void SetupFolders()
    {
        // Get the configuration settings
        if (sdc == null)
            sdc = this.GetComponent<StableDiffusionConfiguration>();

        try
        {
            // Determine output path
            string root = Application.dataPath + sdc.settings.OutputFolder;
            if (root == "" || !Directory.Exists(root))
                root = Application.streamingAssetsPath;
            string mat = Path.Combine(root, "SDImages");
            filename = Path.Combine(mat, guid + ".png");

            // If folders not already exists, create them
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            if (!Directory.Exists(mat))
                Directory.CreateDirectory(mat);

            // If the file already exists, delete it
            if (File.Exists(filename))
                File.Delete(filename);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }
    }
    public event Action OnRequireSpirte;

    public void RenewSpirte()
    {
        OnRequireSpirte?.Invoke();
    }
    protected IEnumerator GenerateAsync(SpriteRenderer spriteRenderer)
    {

        yield return new WaitUntil(() => !generating);
        generating = true;
        SetupFolders();
        Debug.Log("开始生成");
        // Set the model parameters
        yield return sdc.SetModelAsync(modelsList[selectedModel]);

        // Generate the image
        HttpWebRequest httpWebRequest = null;
        Debug.Log("开始发送");
        try
        {
            // Make a HTTP POST request to the Stable Diffusion server
            httpWebRequest = (HttpWebRequest)WebRequest.Create(sdc.settings.StableDiffusionServerURL + sdc.settings.TextToImageAPI);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            // add auth-header to request
            if (sdc.settings.useAuth && !sdc.settings.user.Equals("") && !sdc.settings.pass.Equals(""))
            {
                httpWebRequest.PreAuthenticate = true;
                byte[] bytesToEncode = Encoding.UTF8.GetBytes(sdc.settings.user + ":" + sdc.settings.pass);
                string encodedCredentials = Convert.ToBase64String(bytesToEncode);
                httpWebRequest.Headers.Add("Authorization", "Basic " + encodedCredentials);
            }
            
            // Send the generation parameters along with the POST request
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                SDParamsInTxt2Img sd = new SDParamsInTxt2Img();
                sd.prompt = prompt;
                sd.negative_prompt = negativePrompt;
                sd.steps = steps;
                sd.cfg_scale = cfgScale;
                sd.width = width;
                sd.height = height;
                sd.seed = seed;
                sd.tiling = false;

                if (selectedSampler >= 0 && selectedSampler < samplersList.Length)
                    sd.sampler_name = samplersList[selectedSampler];

                // Serialize the input parameters
                string json = JsonConvert.SerializeObject(sd);

                // Send to the server
                streamWriter.Write(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
            NoSDLoadImage(spriteRenderer);
            StopCoroutine(GenerateAsync(spriteRenderer));
        }
        Debug.Log("发送");
        // Read the output of generation
        if (httpWebRequest != null)
        {
            // Wait that the generation is complete before procedding
            Task<WebResponse> webResponse = httpWebRequest.GetResponseAsync();

            while (!webResponse.IsCompleted)
            {
                if (sdc.settings.useAuth && !sdc.settings.user.Equals("") && !sdc.settings.pass.Equals(""))
                    UpdateGenerationProgressWithAuth();
                else
                    UpdateGenerationProgress();
                yield return new WaitForSeconds(0.5f);
            }
            Debug.Log("收到");
            // Stream the result from the server
            var httpResponse = webResponse.Result;


            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                // Decode the response as a JSON string
                string result = streamReader.ReadToEnd();

                // Deserialize the JSON string into a data structure
                SDResponseTxt2Img json = JsonConvert.DeserializeObject<SDResponseTxt2Img>(result);

                // If no image, there was probably an error so abort
                if (json.images == null || json.images.Length == 0)
                {
                    Debug.LogError("No image was return by the server. This should not happen. Verify that the server is correctly setup.");

                    generating = false;

                    yield break;
                }
                bool RequiriedtaskCompleted = false;
                OnRequireSpirte += () => RequiriedtaskCompleted = true;
                yield return new WaitUntil(() => RequiriedtaskCompleted);

                // Decode the image from Base64 string into an array of bytes
                byte[] imageData = Convert.FromBase64String(json.images[0]);

                // Write it in the specified project output folder
                using (FileStream imageFile = new FileStream(filename, FileMode.Create))
                {
                    yield return imageFile.WriteAsync(imageData, 0, imageData.Length);
                }
                Debug.Log("开始读取");
                try
                {
                    // Read back the image into a texture
                    if (File.Exists(filename))
                    {
                        Texture2D texture = new Texture2D(16,16 );
                        texture.LoadImage(imageData);
                        texture.Apply();

                        LoadIntoImage(texture, spriteRenderer);
                    }

                    // Read the generation info back (only seed should have changed, as the generation picked a particular seed)
                    if (json.info != "")
                    {
                        SDParamsOutTxt2Img info = JsonConvert.DeserializeObject<SDParamsOutTxt2Img>(json.info);

                        // Read the seed that was used by Stable Diffusion to generate this result
                        generatedSeed = info.seed;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + "\n\n" + e.StackTrace);
                }
            }
        }

        generating = false;
        yield return null;
    }


    IEnumerator NoSDLoadImage(SpriteRenderer spriteRenderer)
    {
        string root = Application.streamingAssetsPath;
        string mat = Path.Combine(root, "SDImages");
        string filename = Path.Combine(mat, "testuse.png");

        if (!File.Exists(filename))
        {
            Debug.LogError("File does not exist: " + filename);
        }
        bool RequiriedtaskCompleted = false;
        OnRequireSpirte += () => RequiriedtaskCompleted = true;
        yield return new WaitUntil(() => RequiriedtaskCompleted);

        byte[] fileData = File.ReadAllBytes(filename);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        // 创建精灵并设置给 UI 元素
        Sprite temp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        spriteRenderer.sprite = temp;
        spriteRenderer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);


        // 保存纹理数据为新的 PNG 文件
        SaveTextureAsPNG(texture, guid + ".png");
    }

    /// <summary>
    /// resave the picture
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="fileName"></param>
     void SaveTextureAsPNG(Texture2D texture, string fileName)
    {
        byte[] pngData = texture.EncodeToPNG();

        string savePath = Path.Combine(Application.streamingAssetsPath, "SDImages", fileName);

        try
        {
            File.WriteAllBytes(savePath, pngData);
            Debug.Log("Image saved as: " + savePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save image: " + e.Message);
        }
    }
    /// <summary>
    /// Load the texture into an Image or RawImage.
    /// </summary>
    /// <param name="texture">Texture to setup</param>
    void LoadIntoImage(Texture2D texture, SpriteRenderer spriteRenderer)
    {

        // Find the image component
        if (spriteRenderer != null)
        {
            // Create a new Sprite from the loaded image
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            // Set the sprite as the source for the UI Image
            spriteRenderer.sprite = sprite;
            spriteRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            spriteRenderer.transform.localPosition = new Vector3(-1.1f, -0.8f, 0f);
        }
        // If no image found, try to find a RawImage component


    }
}
