using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationText : MonoBehaviour
{
    public string KEY_NAME = "";
    Text text;
    int initFontSize;
    private void Awake()
    {
        var text = GetComponent<Text>();
        if (text != null)
            initFontSize = this.GetComponent<Text>().fontSize;
    }
    private void OnEnable()
    {
        var text = GetComponent<Text>();
        if (text != null)
        {
            if (User.language == "en")
                text.GetComponent<Text>().font = Resources.Load<Font>("Font/en");
            else
                text.GetComponent<Text>().font = Resources.Load<Font>("Font/ko");

            if (KEY_NAME == "ISOCode")
                text.text = LocalizationManager.GetLanguage();
            else
            {
                try
                {
                    text.text = LocalizationManager.Fields[KEY_NAME];
                }
                catch(KeyNotFoundException e)
                {
                    Debugging.LogWarning(KEY_NAME + " 의 로컬라이징 텍스트를 발견하지못함");
                }
            }
        }
    }
    public void ReDraw()
    {
        var text = GetComponent<Text>();
        if (text != null)
        {
            if (User.language == "en")
                text.GetComponent<Text>().font = Resources.Load<Font>("Font/en");
            else
                text.GetComponent<Text>().font = Resources.Load<Font>("Font/ko");
            if (KEY_NAME == "ISOCode")
                text.text = LocalizationManager.GetLanguage();
            else
            {
                try
                {
                    text.text = LocalizationManager.Fields[KEY_NAME];
                }
                catch (KeyNotFoundException e)
                {
                    Debugging.LogWarning(KEY_NAME + " 의 로컬라이징 텍스트를 발견하지못함");
                }
            }
        }
    }
}
