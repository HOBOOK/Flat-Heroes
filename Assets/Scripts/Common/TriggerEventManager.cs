using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventManager : Singleton<ObjectPool>
{
    [Header("Object")]
    public List<TriggerObject> triggerObjectList = new List<TriggerObject>();
    [Header("Event")]
    public List<TriggerEvent> triggerEventList = new List<TriggerEvent>();

    public void TriggerEventInitialize()
    {
        if(Common.triggerObjectInitialize)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                if(!transform.GetChild(i).name.Equals("Once"))
                    Destroy(transform.GetChild(i).gameObject);
            }
            for (int ix = 0; ix < triggerObjectList.Count; ++ix)
            {
                if (triggerObjectList[ix].isTriggerEnd&&!triggerObjectList[ix].isOnceTrigger)
                {
                    triggerObjectList[ix].isTriggerEnd = false;
                }
            }
            for (int ix = 0; ix < triggerEventList.Count; ++ix)
            {
                if (triggerEventList[ix].isTriggerEnd&&!triggerEventList[ix].isOnceTrigger)
                {
                    triggerEventList[ix].isTriggerEnd = false;
                }
            }
            Common.triggerObjectInitialize = false;
        }
    }

	//void Update ()
 //   {
 //       for (int ix = 0; ix < triggerObjectList.Count; ++ix)
 //       {
 //           if(triggerObjectList[ix].startTimeAtTargetPositionX<= target.transform.position.x && triggerObjectList[ix].startTimeAtTargetPositionX + 2 > target.transform.position.x && !triggerObjectList[ix].isTriggerEnd)
 //           {
 //               triggerObjectList[ix].Initialize(transform);
 //           }
 //       }
 //       for (int ix = 0; ix < triggerEventList.Count; ++ix)
 //       {
 //           if (triggerEventList[ix].startTimeAtTargetPositionX <= target.transform.position.x && triggerEventList[ix].startTimeAtTargetPositionX + 2 > target.transform.position.x && !triggerEventList[ix].isTriggerEnd)
 //           {
 //               triggerEventList[ix].CreateEvent();
 //           }
 //       }
 //       TriggerEventInitialize();
 //   }
}
