using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    public PaintingDecideWin paintingDecideWin;
    public SDPaintController sDPaintingController;
    public bool useSD=false;
    public static PaintingManager _Instance;
    public StableDiffusionText2Image stableDiffusionText2Image;

    void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

}
