using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class DataManager : MonoBehaviour
{
    public Mapdata mapdata;
    public Mapdata WorkingMapdata;
    public GameDictionary gameDictionary;

    public static DataManager _Instance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
