using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TriggerEvent
{
    public float startTimeAtTargetPositionX;
    [HideInInspector]
    public bool isTriggerEnd = false;
    public bool isOnceTrigger = true;
    public Common.EventType eventType;

    public void CreateEvent()
    {
        if(!isTriggerEnd)
        {
            switch (eventType)
            {
                case Common.EventType.CameraShake:
                    SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.rumble);
                    Common.isShake = true;
                    break;

                case Common.EventType.CameraSlow:
                    break;

                case Common.EventType.CameraBlack:
                    Common.isBlackUpDown = true;
                    break;
            }
            isTriggerEnd = true;
        }
    }
}
