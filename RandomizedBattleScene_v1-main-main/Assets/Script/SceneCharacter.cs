﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCharacter : MonoBehaviour
{
    public bool isBarCharacter;
    public Character characterStats;
    public Database database;
    public Animator animator;
    public SceneCharacter sceneCharacter, barCharacter;
    public SpriteRenderer myRenderer;
    public int repeatRate = 0;
    public float progress = 0;

    public char characterIdentifirerIndex = '0';
    private BattleMenu battleMenu;
    private static int speedBarLength = 16;
    private Shader shaderGUIText, shaderSpriteDefault;
    public TMPro.TextMeshProUGUI HPIndicatorHolder;

    private void Start()
    {
        if (isBarCharacter == false)
        {
            GameObject cloner1 = Instantiate(database.characterSprites[characterStats.ID], transform.position, Quaternion.identity);
            cloner1.transform.SetParent(transform);
            animator = cloner1.GetComponent<Animator>();
            myRenderer = cloner1.GetComponent<SpriteRenderer>();
            myRenderer.sortingLayerName = "character";
            StartCoroutine(FadeIn());

            GameObject cloner2 = Instantiate(gameObject);
            barCharacter = cloner2.GetComponent<SceneCharacter>();
            barCharacter.isBarCharacter = true;
            barCharacter.name = "barIcon";
            barCharacter.transform.SetParent(transform);

            HPIndicatorHolder = Instantiate(characterStats.textPrefab).GetComponent<TMPro.TextMeshProUGUI>();
            HPIndicatorHolder.transform.position = transform.position;
            HPIndicatorHolder.transform.position += new Vector3(1, -2.1f, 0);
            HPIndicatorHolder.transform.SetParent(GameObject.Find("Canvas").transform);

            if (characterStats.isAlly == true)
            {
                HPIndicatorHolder.text = "HP" + characterStats.currentHP + "\nMP" + characterStats.currentMP;
            }
            else
            {
                characterIdentifirerIndex = char.Parse(database.enemyDetails.IndexOf(characterStats.gameObject).ToString());
                HPIndicatorHolder.text = "HP" + characterStats.currentHP + "\n" + characterIdentifirerIndex;
            }
            HPIndicatorHolder.fontSize = 0.4f;

            if (tag == "Enemy")
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
            }

        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector2(0.4f, 0.4f);
            animator = transform.GetChild(0).GetComponent<Animator>();
            myRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            myRenderer.sortingLayerName = "character";
            myRenderer.color = new Color32(255, 255, 255, 255);

            if (characterStats.isAlly == false)
            {
                characterIdentifirerIndex = char.Parse(database.enemyDetails.IndexOf(characterStats.gameObject).ToString());
                HPIndicatorHolder = Instantiate(characterStats.textPrefab).GetComponent<TMPro.TextMeshProUGUI>();
                HPIndicatorHolder.transform.position = transform.position;
                HPIndicatorHolder.transform.position += new Vector3(0, 0.5f);
                HPIndicatorHolder.transform.SetParent(GameObject.Find("Canvas").transform);
                HPIndicatorHolder.text = characterIdentifirerIndex.ToString();
                HPIndicatorHolder.fontSize = 0.4f;
            }

            battleMenu = GameObject.Find("BattleMenu").GetComponent<BattleMenu>();
            sceneCharacter = transform.parent.GetComponent<SceneCharacter>();
        }
        shaderGUIText = Shader.Find("GUI/Text Shader");
        shaderSpriteDefault = Shader.Find("Sprites/Default");
    }

    private void FixedUpdate()
    {
        if (database.isHandling == false)
        {
            if (isBarCharacter == true)
            {
                if (characterStats.isDead == false)
                {
                    if (progress <= 0)
                    {
                        transform.position = new Vector2(-8, 4);
                        progress += 0.1f;
                    }
                    else if (progress >= speedBarLength)
                    {
                        if (database.isHandling == false)
                        {
                            database.isHandling = true;
                            if (characterStats.isAlly == true)
                            {
                                StartCoroutine("WaitOption");
                            }
                            else
                            {
                                int tempAttackIndex = 0;
                                do
                                {
                                    tempAttackIndex = Random.Range(0, database.allyDetails.Count);
                                } while (database.allyDetails[tempAttackIndex].GetComponent<Character>().isDead == true);

                                performAttackPattern(characterStats.ID, getAttackID(characterStats.ID, repeatRate), tempAttackIndex, true);
                                StartCoroutine("attack");
                            }
                        }
                    }
                    else
                    {
                            if (characterStats.speed + characterStats.extraSpeed > 0)
                            {
                                progress += (characterStats.speed + characterStats.extraSpeed) * Time.deltaTime;
                            }
                            else
                            {
                                progress += 1 * Time.deltaTime;
                            }
                            transform.position = new Vector2(-8 + progress, 4);
                    }

                    if (characterStats.isAlly == false)
                    {
                        HPIndicatorHolder.transform.position = transform.position;
                        HPIndicatorHolder.transform.position += new Vector3(0, 0.5f);
                    }
                }
            }
            else
            {
                if (characterStats.currentHP <= 0 && characterStats.isDead == false)
                {
                    database.AddSound(8, false, 1);
                    HPIndicatorHolder.text = "Dead";
                    if (characterStats.isAlly == true)
                    {
                        database.allyDetails.Remove(characterStats.gameObject);
                        database.allyDetails.Add(characterStats.gameObject);
                        characterStats.isDead = true;
                        myRenderer.color = new Color32(255, 255, 255, 100);
                        barCharacter.myRenderer.color = new Color32(255, 255, 255, 100);
                        barCharacter.progress = 0;
                        barCharacter.transform.position = new Vector2(-4, 4);

                        int deathNumber = RepositionCharacter(true);
                        if (deathNumber == database.allyDetails.Count)
                        {
                            for(int i = 0; i < database.allyDetails.Count; i++)
                            {
                                Character tempCharacter = database.allyDetails[i].GetComponent<Character>();
                                Destroy(tempCharacter.sceneCharacter.HPIndicatorHolder.gameObject);
                                Destroy(tempCharacter.sceneCharacter.gameObject);
                                Destroy(tempCharacter.gameObject);
                            }
                            database.allyDetails.Clear();

                            for (int i = 0; i < database.enemyDetails.Count; i++)
                            {
                                Character tempCharacter = database.enemyDetails[i].GetComponent<Character>();
                                Destroy(tempCharacter.sceneCharacter.HPIndicatorHolder.gameObject);
                                Destroy(tempCharacter.sceneCharacter.gameObject);
                                Destroy(tempCharacter.gameObject);
                                Destroy(tempCharacter.sceneCharacter.barCharacter.HPIndicatorHolder.gameObject);
                            }
                            database.enemyDetails.Clear();

                            database.currentWave = 0;
                            database.waitingEnemies.Clear();
                            database.logMessage.DeleteLog();
                            database.logMessage.AddMessage("All allies died!");
                            database.deathTime++;
                            database.logMessage.Print(LogMessage.closeStatus.backToCharacterSelection);
                        }
                    }
                    else
                    {
                        StartCoroutine(FadeOut());

                        if (characterStats.ID == 13) // 3 Doggies
                        {
                            database.CreateEnemy(database.CharacterLibrary(14));
                        }
                        if (characterStats.ID == 14) // 2 Doogies
                        {
                            database.CreateEnemy(database.CharacterLibrary(15));
                        }

                        int reward = (int)(characterStats.maxHP * 0.8f + (characterStats.defense + characterStats.dodgeRate) * 0.4f + (characterStats.speed + characterStats.attackDamage) * 1.5f);
                        database.coinGainInOneRound += reward;

                        database.enemyDetails.Remove(characterStats.gameObject);
                        characterStats.isDead = true;

                        int deathNumber = RepositionCharacter(false);
                        if (deathNumber == database.enemyDetails.Count)
                        {
                            database.isHandling = true;
                            for (int i = 0; i < database.enemyDetails.Count; i++) // Destroy All Enemies
                            {
                                Character tempCharacter = database.enemyDetails[i].GetComponent<Character>();
                                if (characterStats != database.enemyDetails[i].GetComponent<Character>())
                                {
                                    Destroy(tempCharacter.sceneCharacter.HPIndicatorHolder.gameObject);
                                    //Destroy(tempCharacter.sceneCharacter.barCharacter.HPIndicatorHolder.gameObject);
                                    Destroy(tempCharacter.sceneCharacter.gameObject);
                                    Destroy(database.enemyDetails[i]);
                                }
                            }
                            database.enemyDetails.Clear();

                            for (int i = 0; i < database.allyDetails.Count; i++) // Set Allies Progress Back To Zero
                            {
                                database.allyDetails[i].GetComponent<Character>().sceneCharacter.barCharacter.progress = 0;
                            }

                            if (database.currentWave + 1 == database.totalWave)
                            {
                                for (int i = 0; i < database.allyDetails.Count; i++) // Clear All Effects On Allies
                                {
                                    Character tempCharacter = database.allyDetails[i].GetComponent<Character>();
                                    tempCharacter.effects.Clear();
                                    tempCharacter.statsEffects.Clear();

                                    tempCharacter.currentHP = getOverThread(tempCharacter.currentHP, tempCharacter.currentMP, tempCharacter.maxHP);
                                    tempCharacter.currentMP = 0;

                                    if (tempCharacter.isDead == true) // Destroy Ally If Dead
                                    {
                                        database.allyDetails.Remove(tempCharacter.gameObject);
                                        Destroy(tempCharacter.sceneCharacter.HPIndicatorHolder.gameObject);
                                        Destroy(tempCharacter.sceneCharacter.gameObject);
                                        Destroy(tempCharacter.gameObject);
                                    }
                                    else
                                    {
                                        Destroy(tempCharacter.sceneCharacter.HPIndicatorHolder.gameObject);
                                        Destroy(tempCharacter.sceneCharacter.barCharacter.gameObject);
                                        tempCharacter.sceneCharacter.enabled = false;
                                    }
                                }
                                database.currentWave = 0;
                                database.waitingEnemies.Clear();
                                database.logMessage.AddMessage("Current Gold: " + database.coin + " G + " + database.coinGainInOneRound + " G => " + (database.coin + database.coinGainInOneRound) + " G");
                                database.coin = database.coin + database.coinGainInOneRound;


                                database.logMessage.Print(LogMessage.closeStatus.backToShop);
                            }
                            else
                            {
                                database.moveMap.StartLerping();
                            }
                        }
                    }

                }
                else
                {
                    if (characterStats.isAlly == true)
                    {
                        HPIndicatorHolder.text = "HP" + characterStats.currentHP + "\nMP" + characterStats.currentMP;
                    }
                    else
                    {
                        if (characterStats != null)
                        {
                            HPIndicatorHolder.text = "HP" + characterStats.currentHP + "\n" + characterIdentifirerIndex.ToString();
                        }
                        else
                        {
                            HPIndicatorHolder.text = "HP" + characterStats.currentHP;
                        }
                    }
                }
            }
        }
    }

    public int RepositionCharacter(bool isAlly)
    {
        int deathNumber = 0;
        if (isAlly == true)
        {
            for (int i = 0; i < database.allyDetails.Count; i++)
            {
                Character tempCharacter = database.allyDetails[i].GetComponent<Character>();
                if (tempCharacter.isDead == true)
                {
                    deathNumber++;
                }
                tempCharacter.sceneCharacter.transform.position = new Vector2(-2 + i * -2.5f, -2);
                TMPro.TextMeshProUGUI tempHPIndicator = tempCharacter.sceneCharacter.GetComponent<SceneCharacter>().HPIndicatorHolder;
                tempHPIndicator.transform.position = tempCharacter.sceneCharacter.transform.position;
                tempHPIndicator.transform.position += new Vector3(1, -2.1f, 0);
            }
        }
        else
        {
            for (int i = 0; i < database.enemyDetails.Count; i++)
            {
                Character tempCharacter = database.enemyDetails[i].GetComponent<Character>();
                if (tempCharacter.isDead == true)
                {
                    deathNumber++;
                }
                tempCharacter.sceneCharacter.transform.position = new Vector2(2 + i * 2.5f, -2);
                TMPro.TextMeshProUGUI tempHPIndicator = tempCharacter.sceneCharacter.GetComponent<SceneCharacter>().HPIndicatorHolder;
                tempHPIndicator.transform.position = tempCharacter.sceneCharacter.transform.position;
                tempHPIndicator.transform.position += new Vector3(1, -2.1f, 0);
            }
        }
        return deathNumber;
    }

    public void isHit()
    {
        StartCoroutine("Flash");
    }

    IEnumerator Flash()
    {
        myRenderer.material.shader = shaderGUIText;
         myRenderer.color = Color.red;
         yield return new WaitForSeconds(0.2f);
         if (characterStats.isDead == true)
         {
             myRenderer.material.shader = shaderGUIText;
             myRenderer.color = new Color32(255, 255, 255, 100);
        }
         else
         {
             myRenderer.material.shader = shaderSpriteDefault;
             myRenderer.color = Color.white;
         }
    }

    IEnumerator attack()
    {
        database.AddSound(6,false,0.5f);
        sceneCharacter.animator.SetBool("isAttack", true);
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + 0.3f);
        repeatRate++;
        sceneCharacter.animator.SetBool("isAttack", false);
        animator.SetBool("isAttack", false);

        database.isHandling = false;

        for (int i = 0; i < characterStats.statsEffects.Count; i++)
        {
            characterStats.statsEffects[i].FinsihOneRound();
        }
        progress = 0;
    }

    private Character getTarget(int index, bool isAlly)
    {
        if (isAlly)
        {
            return database.allyDetails[index].GetComponent<Character>();
        }
        else
        {
            return database.enemyDetails[index].GetComponent<Character>();
        }
    }

    private int getOverThread(int current, int addValue, int max = 9999)
    {
        if (current + addValue > max)
        {
            return max;
        }
        if (current + addValue <= 0)
        {
            return 1;
        }
        return current + addValue;
    }

    // For Enemy Only
    private int getAttackID(int ID, int repeatRate)
    {
        int maxSkillPattern = 0;
        switch (ID)
        {
            case 6:
                maxSkillPattern = 1;
                break;
            case 16:
                maxSkillPattern = 4;
                break;
            default:
                maxSkillPattern = -1;
                break;
        }
        if (maxSkillPattern == -1)
        {
            return -1;
        }
        return repeatRate % maxSkillPattern;
    }

    // For Enemy Only, AttackID Sets To -1 = Basic Attack
    private void performAttackPattern(int ID, int attackID, int index, bool isAlly)
    {
        Character target = getTarget(index, isAlly);
        bool isDodged = false;

        if (target != characterStats)
        {
            if (Random.Range(getOverThread(target.dodgeRate, target.extraDodgeRate, 100), 100) == getOverThread(target.dodgeRate, target.extraDodgeRate, 100))
            {
                isDodged = true;
                target.AddPopText("Dodged!");
            }
        }

        if (isDodged == false)
        {
            if (attackID == -1)
            {
                characterStats.currentMP += 10;
                if (characterStats.currentMP > characterStats.maxMP)
                {
                    characterStats.currentMP = characterStats.maxMP;
                }
                target.DealDamage(characterStats.attackDamage + characterStats.extraAttackDamage);
            }
            else
            {
                switch (ID)
                {
                    case 6:
                        if (attackID == 0)
                        {
                            if (database.enemyDetails.Count + 1 <= 4)
                            {
                                Character tempCharacter = database.CharacterLibrary(7);
                                database.CreateEnemy(tempCharacter);
                            }
                        }
                        break;
                    case 16:
                        if (attackID == 0)
                        {
                            
                            target.DealDamage(characterStats.attackDamage + characterStats.extraAttackDamage);
                        }
                        if (attackID == 1)
                        {
                            characterStats.AddPopText("Heal!");
                            characterStats.currentHP = getOverThread(characterStats.currentHP, Random.Range(20, 40), characterStats.maxHP);
                        }
                        if (attackID == 2)
                        {
                            characterStats.AddPopText(".......(**his next attack is stronger now !)", 1.5f);
                            characterStats.AddStatsEffect(1, 0, 0, 0, 25);
                        }
                        if (attackID == 3)
                        {
                            characterStats.AddPopText("Die!");
                            target.DealDamage(characterStats.attackDamage + characterStats.extraAttackDamage);
                        }
                        break;
                }
            }
            target.AddEffect(2, characterStats.element);
            target.sceneCharacter.isHit();
        }
    }

    IEnumerator WaitOption()
    {
        database.isSelectedOption = false;
        database.selector = database.allyDetails.IndexOf(characterStats.gameObject);
        battleMenu.Show();
        sceneCharacter.animator.speed = 0;
        sceneCharacter.animator.Play("c" + characterStats.ID.ToString() + "_attack", 0, 0);
        yield return new WaitUntil(() => database.isSelectedOption == true);
        sceneCharacter.animator.Play("c" + characterStats.ID.ToString() + "_idle", 0, 0);
        sceneCharacter.animator.speed = 1;
        database.isSelectedOption = false;
        Character target = getTarget(database.selectedIndex, database.isAllySelected);
        int finalDamage = 0;
        switch (database.selectedState)
        {
            case 0:
                performAttackPattern(0, -1, database.selectedIndex, false);
                break;
            case 1: // Skill
                database.AddSound(9, false, 0.8f);
                int tempMPCost = 0;
                for (int i = 0; i < characterStats.skills.Count; i++)
                {
                    if (characterStats.skills[i].ID == database.selectedItem)
                    {
                        tempMPCost = characterStats.skills[i].MPCost;
                    }
                }
                characterStats.currentMP -= tempMPCost;
                Debug.Log("Caster: " + database.selector + ", Skill: " + database.selectedItem + ", Is Ally Side: " + database.isAllySelected + ", Target Index: " + database.selectedIndex);
                switch (database.selectedItem)
                 {
                     case 0:
                        target.AddStatsEffect(1, 0, 0, -3, 0);
                        target.AddEffect(2, Character.Element.wind);
                        break;
                    case 1:
                        target.AddStatsEffect(2, 0, 100, 0, 0);
                        break;
                    case 2:
                        target.currentHP -= getOverThread(characterStats.attackDamage, characterStats.extraAttackDamage - (target.defense + target.extraDefense));
                        target.AddStatsEffect(2, 0, 0, 6, 0);
                        target.AddEffect(2, Character.Element.electricity);
                        break;
                     case 3:
                         finalDamage = getOverThread(characterStats.attackDamage, characterStats.extraAttackDamage - (target.defense + target.extraDefense));
                         finalDamage += (int)(finalDamage * 0.05f);
                         for (int i = 0; i < database.enemyDetails.Count; i++)
                         {
                             database.enemyDetails[i].GetComponent<Character>().currentHP -= finalDamage;
                             database.enemyDetails[i].GetComponent<Character>().AddEffect(2, Character.Element.fire);
                         }
                         break;
                     case 4:
                        finalDamage = getOverThread(characterStats.attackDamage, characterStats.extraAttackDamage - (target.defense + target.extraDefense));
                        finalDamage += (int)(finalDamage * 0.15f);
                        target.currentHP -= finalDamage;
                        target.AddEffect(2, Character.Element.fire);
                        break;
                    case 5:
                         finalDamage = getOverThread(characterStats.attackDamage, characterStats.extraAttackDamage - (target.defense + target.extraDefense));
                         finalDamage = (int)(finalDamage * 0.7f);
                         for (int i = 0; i < database.enemyDetails.Count; i++)
                         {
                             database.enemyDetails[i].GetComponent<Character>().currentHP -= finalDamage;
                             database.enemyDetails[i].GetComponent<Character>().AddEffect(2, Character.Element.water);
                         }
                         break;
                     case 6:
                        target.currentHP = target.maxHP;
                        break;
                    case 7:
                        characterStats.currentHP -= (int)(characterStats.currentHP * 0.9f);
                        finalDamage = getOverThread(characterStats.attackDamage, characterStats.extraAttackDamage - (target.defense + target.extraDefense));
                        finalDamage += (int)(finalDamage * 1.5f);
                        target.AddEffect(2, Character.Element.electricity);
                        break;
                     case 8:
                        target.shieldPoint += 50;
                        break;
                    case 9:
                        for (int i = 0; i < database.allyDetails.Count; i++)
                        {
                            database.allyDetails[i].GetComponent<Character>().shieldPoint += 30;
                        }
                        break;
                }
                break;
            case 2: // Item
                database.AddSound(7, false, 1);
                Debug.Log("Caster: " + database.selector + ", Item: " + database.inventory[database.selectedItem].itemName + ", Is Ally Side: " + database.isAllySelected + ", Target Index: " + database.selectedIndex);
                switch (database.inventory[database.selectedItem].ID)
                {
                    case 0: // HP Potion
                        target.currentHP = getOverThread(target.currentHP, 35, target.maxHP);

                        break;
                    case 1: // MP Potion
                        target.currentMP = getOverThread(target.currentMP, 30, target.maxHP);
                        break;
                    case 2: // Speed Potion
                        target.AddStatsEffect(3, 0, 0, 3, 0);
                        break;
                    case 3: // Strength Potion
                        target.AddStatsEffect(3, 0, 0, 0, 25);
                        break;
                    case 4: // Revive Potion
                        target.isDead = false;
                        target.currentHP = getOverThread(target.currentHP, 50, target.maxHP);
                        target.sceneCharacter.isHit();
                        break;
                }
                database.inventory[database.selectedItem].itemAmount--;
                if (database.inventory[database.selectedItem].itemAmount <= 0)
                {
                    database.inventory.RemoveAt(database.selectedItem);
                }
                break;
        }
        StartCoroutine("attack");
    }

    private IEnumerator FadeIn()
    {
        myRenderer.color = new Color32(255, 255, 255, 0);
        for (int i = 0; i < 10; i++)
        {
            myRenderer.color += new Color32(0, 0, 0, 26);
            yield return new WaitForSeconds(0.04f);
        }
    }

    private IEnumerator FadeOut()
    {
        myRenderer.color = new Color32(255, 255, 255, 255);
        for (int i = 0; i < 10; i++)
        {
            myRenderer.color -= new Color32(0, 0, 0, 26);
            yield return new WaitForSeconds(0.04f);
        }

        Destroy(characterStats.sceneCharacter.HPIndicatorHolder.gameObject);
        Destroy(characterStats.sceneCharacter.barCharacter.HPIndicatorHolder.gameObject);
        Destroy(characterStats.sceneCharacter.gameObject);
        database.enemyDetails.Remove(characterStats.gameObject);
        Destroy(characterStats.gameObject);
    }
}
