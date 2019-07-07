using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class StartManagement : MonoBehaviour
{
    private void Awake()
    {
        string path = Application.persistentDataPath + "/player.fun";
        PlayerData data = null;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
        }
        if(data!=null)
        {
            LocalizationManager.LoadLanguage(data.language);
        }
        else
        {
            LocalizationManager.LoadLanguage();
        }

    }
    private void Start()
    {
        DontDestroyOnLoad(this);
        if(Common.GetSceneCompareTo(Common.SCENE.MAIN))
        {
            if (this.transform.childCount < 1)
                Destroy(this.gameObject);
        }
    }
}
