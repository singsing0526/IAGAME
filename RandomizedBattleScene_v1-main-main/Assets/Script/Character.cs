using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int maxHP, currentHP, maxMP, currentMP, defense, dodgeRate, speed, attackDamage, ID, wave;
    public int extraDefense, extraDodgeRate, extraSpeed, extraAttackDamage, shieldPoint;
    [HideInInspector]public enum Element {none, fire, water, wind, earth, electricity, ice, rain, stone, wildfire, chaos};
    public Element element;
    public bool isAlly, isDead = false;
    [HideInInspector]public SceneCharacter sceneCharacter;
    public List<Skill> skills;
    [HideInInspector]public GameObject elementPrefab;
    public List<GameObject> effects;
    [HideInInspector]public GameObject textPrefab;
    public List<StatsEffect> statsEffects;
    [HideInInspector] public Database database;

    public Character(int maxHP, int maxMP, int defense, int dodgeRate, int speed, int attackDamage, Element element, int ID, int wave)
    {

        this.maxHP = maxHP;
        currentHP = maxHP;
        this.maxMP = maxMP;
        currentMP = 0;
        this.defense = defense;
        this.dodgeRate = dodgeRate;
        this.speed = speed;
        this.attackDamage = attackDamage;
        this.element = element;
        this.ID = ID;
        this.wave = wave;
    }

    public void SetCharacter(int maxHP, int maxMP, int defense, int dodgeRate, int speed, int attackDamage, Element element, int ID, int wave = 0)
    {
        this.maxHP = maxHP;
        currentHP = maxHP;
        this.maxMP = maxMP;
        currentMP = 0;
        this.defense = defense;
        this.dodgeRate = dodgeRate;
        this.speed = speed;
        this.attackDamage = attackDamage;
        this.element = element;
        this.ID = ID;
        this.wave = wave;
    }

    public void AddSkill(Element element, int MPCost, int ID)
    {
        skills.Add(new Skill(element, MPCost, ID));
    }

    public void AddEffect(int round, Element element)
    {
        if (element != Element.none)
        {
            GameObject temp = Instantiate(elementPrefab);
            temp.GetComponent<ElementEffect>().setValue(round, element, this);
            effects.Add(temp);
        }
    }

    public void AddStatsEffect(int round, int extraDefense, int extraDodgeRate, int extraSpeed, int extraAttackDamage)
    {
        statsEffects.Add(new StatsEffect(round, this, extraDefense, extraDodgeRate, extraSpeed, extraAttackDamage));
    }

    public void DealDamage(int damage)
    {
        int finalDamage = damage - (defense + extraDefense);
        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }
        currentHP -= finalDamage;
        
        AddPopText(finalDamage.ToString());
    }

    public void AddPopText(string text, float waitTime = 0.5f)
    {
        PopText tempText = Instantiate(database.popText).GetComponent<PopText>();
        tempText.SetText(sceneCharacter.transform.position, text, waitTime);
        tempText.SetFloat();
    }
}
