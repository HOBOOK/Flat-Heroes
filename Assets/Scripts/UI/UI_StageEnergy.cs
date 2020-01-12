using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageEnergy : MonoBehaviour
{
    Text energyText;
    Image energyImage;
    float imageSize = 1;
    bool isSizeOver = false;
    ParticleSystem ChargeEffect;

    float curEnergy = 0.0f;
    float maxEnergy = 0.0f;
    private void Awake()
    {
        energyText = this.GetComponentInChildren<Text>();
        energyImage = this.GetComponentInChildren<Image>();
        ChargeEffect = this.transform.GetChild(3).GetComponent<ParticleSystem>();
    }
    void LateUpdate()
    {
        if(energyText != null&& StageManagement.instance!=null&&StageManagement.instance.stageInfo!=null)
        {
            curEnergy = StageManagement.instance.stageInfo.stageEnergy;
            maxEnergy = StageManagement.instance.stageInfo.stageMaxEnergy;
            if(ChargeEffect!=null)
            {
                if (curEnergy > 0 && curEnergy < maxEnergy && !ChargeEffect.isPlaying)
                    ChargeEffect.Play();
                else if (ChargeEffect.isPlaying)
                    ChargeEffect.Stop();
            }
            energyText.text = string.Format("{0}<color='white'>/<size='36'>{1}</size></color>", curEnergy.ToString("N0"), maxEnergy.ToString("N0"));
        }
        if (energyImage != null)
        {
            if (imageSize > 1.1f)
                isSizeOver = true;
            else if (imageSize < 1)
                isSizeOver = false;
            imageSize = isSizeOver ? imageSize - Time.deltaTime * 0.1f : imageSize + Time.deltaTime*0.1f;
            energyImage.transform.localScale = new Vector3(imageSize, imageSize, imageSize);
        }
    }
}
