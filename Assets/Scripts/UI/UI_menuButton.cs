using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_menuButton: MonoBehaviour
{
    Color imageColor;
    private void Awake()
    {
        imageColor = GetComponentInChildren<Text>().color;
    }
    public void OnMouseOverEvent()
    {
        GetComponentInChildren<Text>().color = new Color(1, 1, 1);
    }
    public void OnMouseExitEvent()
    {
        GetComponentInChildren<Text>().color = imageColor;
    }
    public IEnumerator ColorChange(Color startColor, Color endColor, GameObject gameObject)
    {
        Color rgb = startColor;
        while (rgb != endColor)
        {
            rgb = Color.Lerp(rgb, endColor, 0.5f);
            gameObject.GetComponentInChildren<Text>().color = rgb;
            Debug.Log(gameObject.GetComponentInChildren<Text>().color);
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
