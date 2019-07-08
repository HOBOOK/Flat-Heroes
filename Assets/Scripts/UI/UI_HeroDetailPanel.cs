using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroDetailPanel : MonoBehaviour
{
    private HeroData heroData;
    public Transform ShowPoint;
    public Text nameText;
    public Text descriptionText;
    public GameObject statusInfoPanel;
    public GameObject skillInfoPanel;
    private GameObject showHeroObj;

    public void OpenUI(HeroData data)
    {
        heroData = data;
        showHeroObj = Instantiate(PrefabsDatabaseManager.instance.GetHeroPrefab(heroData.id), ShowPoint.transform);
        showHeroObj.transform.localScale = new Vector3(200, 200, 200);
        showHeroObj.transform.localPosition = Vector3.zero;

        if (showHeroObj.GetComponent<Hero>() != null)
            Destroy(showHeroObj.GetComponent<Hero>());
        if (showHeroObj.GetComponent<Rigidbody2D>() != null)
            Destroy(showHeroObj.GetComponent<Rigidbody2D>());
        foreach (var sp in showHeroObj.GetComponentsInChildren<SpriteRenderer>())
        {
            sp.sortingLayerName = "ShowObject";
            sp.gameObject.layer = 16;
        }
        showHeroObj.gameObject.SetActive(true);

        nameText.text = HeroSystem.GetHeroName(data.id);
        descriptionText.text = HeroSystem.GetHeroDescription(data.id);

        //Status 정보
        if (statusInfoPanel != null)
        {
            statusInfoPanel.transform.GetChild(0).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusAttack(ref heroData).ToString();
            statusInfoPanel.transform.GetChild(1).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusDefence(ref heroData).ToString();
            statusInfoPanel.transform.GetChild(2).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusMaxHp(ref heroData).ToString();
            statusInfoPanel.transform.GetChild(3).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusCriticalPercent(ref heroData).ToString();
            statusInfoPanel.transform.GetChild(4).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusAttackSpeed(ref heroData).ToString();
            statusInfoPanel.transform.GetChild(5).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusMoveSpeed(ref heroData).ToString();
            statusInfoPanel.transform.GetChild(6).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusKnockbackResist(ref heroData).ToString("N1");
            statusInfoPanel.transform.GetChild(7).GetComponentInChildren<Text>().text = HeroSystem.GetHeroStatusSkillEnergy(ref heroData).ToString();
        }

        // 스킬정보
        Skill heroSkill = SkillSystem.GetSkill(heroData.skill);
        if (skillInfoPanel != null && heroSkill != null)
        {
            var skillImage = skillInfoPanel.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            skillImage.sprite = SkillSystem.GetSkillImage(heroSkill.id);
            skillInfoPanel.transform.GetComponentInChildren<Text>().text = string.Format("<size='27'>{0} : {1}  {2}</size>\r\n\r\n<color='grey'>{3}</color>", LocalizationManager.GetText("SkillLevel"), 1, SkillSystem.GetSkillName(heroSkill.id),SkillSystem.GetSkillDescription(heroSkill.id));
        }
    }

    private void OnDisable()
    {
        if (ShowPoint.transform.childCount > 0)
            Destroy(ShowPoint.transform.GetChild(0).gameObject);
    }
}
