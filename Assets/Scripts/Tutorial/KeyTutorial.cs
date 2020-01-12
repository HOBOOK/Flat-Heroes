using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTutorial : Tutorial
{
    public List<string> Keys = new List<string>();

    public override void CheckIfHappening()
    {
        for(int i = 0; i < Keys.Count; i++)
        {
            if(Input.inputString.Contains(Keys[i]))
            {
                Keys.RemoveAt(i);
                break;
            }
        }

        if (Keys.Count == 0)
            TutorialManager.Instance.CompletedTutorial();
    }
}
