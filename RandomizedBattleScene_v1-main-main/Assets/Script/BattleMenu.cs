﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMenu : MonoBehaviour
{
    public Sprite[] battleOptions;
    public GameObject skillMenu, itemMenu;
    private char targetRange = 'e';
    private GameObject targetIconHolder, skillMenuHolder, itemMenuHolder;
    public TMPro.TextMeshProUGUI instructionHolder;
    public Database database;
    public SpriteRenderer sr;

    public bool isShowing = false, isSelectedOption = false, isTargetAlly = false, isSelectedTarget = false, isSelectedItem = false, hasTargetIconCreated = false, hasTargetRangeSet = false;
    public int currentOption = 0, currentTarget = 0, currentItem = 0, previousOption = 0;

    public void Show()
    {
        enabled = true;
        isShowing = true;
        isSelectedOption = false;
        isTargetAlly = false;
        isSelectedTarget = false;
        isSelectedItem = false;
        hasTargetIconCreated = false;
        hasTargetRangeSet = false;
        sr.sprite = battleOptions[0];
        currentOption = 0;
        currentTarget = 0;
        currentItem = 0;
        instructionHolder = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
        instructionHolder.transform.SetParent(GameObject.Find("Canvas").transform);
        instructionHolder.text = "[W], [Up] and [S], [Down] to scroll, [Z] to comfirm";
    }
    public void Hide()
    {
        enabled = false;
        isShowing = false;
        if (instructionHolder != null)
            Destroy(instructionHolder.gameObject);
        if (targetIconHolder != null)
            Destroy(targetIconHolder);
        database.isAllySelected = isTargetAlly;
        database.selectedIndex = currentTarget;
        database.selectedState = currentOption - 1;
        database.selectedItem = currentItem;
        database.isSelectedOption = true;
        sr.sprite = null;
    }

    private void Update()
    {
        if (isShowing == true)
        {
            if (isSelectedOption == false)
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    database.AddSound(9, false, 1);
                    if (currentOption - 1 > 1)
                    {
                        currentOption--;
                        sr.sprite = battleOptions[currentOption];
                    }
                    else
                    {
                        currentOption = 1;
                        sr.sprite = battleOptions[currentOption];
                    }
                }
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    database.AddSound(9, false, 1);
                    if (currentOption + 1 < 4)
                    {
                        currentOption++;
                        sr.sprite = battleOptions[currentOption];
                    }
                    else
                    {
                        currentOption = 3;
                        sr.sprite = battleOptions[currentOption];
                    }
                }
                if (Input.GetKeyDown(KeyCode.Z) && currentOption != 0)
                {
                    database.AddSound(10, false, 1);
                    switch (currentOption)
                    {
                        case 1: //Attack Selected
                            targetRange = 'e';
                            isTargetAlly = false;
                            CreateTargetIcon();
                            break;
                        case 2: // Skill Selected
                            skillMenuHolder = Instantiate(skillMenu);
                            SkillMenu tempSkillMenu = skillMenuHolder.GetComponent<SkillMenu>();
                            tempSkillMenu.battleMenu = this;
                            tempSkillMenu.isDescription = false;
                            enabled = false;
                            break;
                        case 3: // Item Selected
                            itemMenuHolder = Instantiate(itemMenu);
                            itemMenuHolder.GetComponent<ItemMenu>().battleMenu = this;
                            itemMenuHolder.GetComponent<ItemMenu>().isDescription = false;
                            enabled = false;
                            break;
                    }
                    isSelectedOption = true;
                    sr.sprite = null;
                    previousOption = currentOption;
                }
            }
            else
            {
                switch (currentOption)
                {
                    case 1://Attack
                        selectTarget();
                        break;
                    case 2://Skill
                        if(isSelectedItem == true)
                        {
                            getSkillTargetRange();
                            CreateTargetIcon();
                            selectTarget();
                        }
                        break;
                    case 3://Item
                        if (isSelectedItem == true)
                        {
                            getItemTargetRange(currentItem);
                            CreateTargetIcon();
                            selectTarget();
                        }
                        break;
                }
            }
        }
    }

    private void CreateTargetIcon()
    {
        if (hasTargetIconCreated == false && database.enemyDetails.Count > 0)
        {
            instructionHolder.text = "[A], [Left] and [D], [Right] to change selection, [Z] to comfirm, [X] to cancel";
            hasTargetIconCreated = true;
            targetIconHolder = Instantiate(database.targetIcon);
            targetIconHolder.GetComponent<TargetSelection>().battleMenu = this;
        }
    }

    private void selectTarget()
    {
        if (isSelectedTarget == false)
        {
            if (targetRange != 's')
            {
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    database.AddSound(9, false, 1);
                    if (isTargetAlly == false)
                    {
                        if (currentTarget - 1 >= 0)
                        {
                            currentTarget--;
                        }
                        else if (targetRange != 'e')
                        {
                            currentTarget = 0;
                            isTargetAlly = true;
                        }
                    }
                    else
                    {
                        if (currentTarget + 1 < database.allyDetails.Count)
                        {
                            currentTarget++;
                        }
                        else
                        {
                            currentTarget = database.allyDetails.Count - 1;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    database.AddSound(9, false, 1);
                    if (isTargetAlly == false)
                    {
                        if (currentTarget + 1 < database.enemyDetails.Count)
                        {
                            currentTarget++;
                        }
                        else
                        {
                            currentTarget = database.enemyDetails.Count - 1;
                        }
                    }
                    else
                    {
                        if (currentTarget - 1 >= 0)
                        {
                            currentTarget--;
                        }
                        else if (targetRange != 'a')
                        {
                            currentTarget = 0;
                            isTargetAlly = false;
                        }
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                database.AddSound(10, false, 1);
                Debug.Log(currentTarget + ", " + currentItem);
                if (currentOption != 3) // Not Selecting Item
                {
                    if (isTargetAlly == true)
                    {
                        if (database.allyDetails[currentTarget].GetComponent<Character>().isDead == false)
                        {
                            isSelectedTarget = true;
                        }
                    }
                    else
                    {
                        if (database.enemyDetails[currentTarget].GetComponent<Character>().isDead == false)
                        {
                            isSelectedTarget = true;
                        }
                    }
                }
                else 
                {
                    if (database.inventory[currentItem].ID != 4) // Not Revive Potion
                    {
                        if (database.allyDetails[currentTarget].GetComponent<Character>().isDead == false)
                        {
                            isSelectedTarget = true;
                        }
                    }
                    else // Is Revive Potion
                    {
                        if (database.allyDetails[currentTarget].GetComponent<Character>().isDead == true)
                        {
                            isSelectedTarget = true;
                        }
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.X) && isSelectedTarget == false)
            {
                database.AddSound(10, false, 1);
                sr.sprite = battleOptions[previousOption];
                instructionHolder.text = "[W], [Up] and [S], [Down] to scroll, [Z] to comfirm";
                Destroy(targetIconHolder);
                isSelectedOption = false;
                isSelectedItem = false;
                hasTargetIconCreated = false;
                hasTargetRangeSet = false;
                targetRange = 'e';
            }
        }
        else
        {
            Hide();
        }
    }

    private void getSkillTargetRange()
    {
        if (hasTargetRangeSet == false)
        {
            hasTargetRangeSet = true;
            switch (currentItem)
            {
                case 0:
                    targetRange = 'e';
                    break;
                case 1:
                    targetRange = 's';
                    break;
                case 2:
                    targetRange = 'u';
                    break;
                case 3:
                    targetRange = 'e';
                    break;
                case 4:
                    targetRange = 'e';
                    break;
                case 5:
                    targetRange = 'e';
                    break;
                case 6:
                    targetRange = 'a';
                    break;
                case 7:
                    targetRange = 'e';
                    break;
                case 8:
                    targetRange = 's';
                    break;
                case 9:
                    targetRange = 'a';
                    break;
            }
            switch (targetRange)
            {
                case 's': // s = self
                    isTargetAlly = true;
                    currentTarget = database.selector;
                    break;
                case 'u': // u = both side
                case 'e': // e = enemy
                    isTargetAlly = false;
                    currentTarget = 0;
                    break;
                case 'a': // a = ally
                    isTargetAlly = true;
                    currentTarget = 0;
                    break;
            }
        }
    }

    private void getItemTargetRange(int scrolledPage)
    {
        if (hasTargetRangeSet == false)
        {
            hasTargetRangeSet = true;
            switch (database.inventory[scrolledPage].ID)
            {
                case 0:
                    targetRange = 'a';
                    break;
                case 1:
                    targetRange = 'a';
                    break;
                case 2:
                    targetRange = 'a';
                    break;
                case 3:
                    targetRange = 'a';
                    break;
                case 4:
                    targetRange = 'a';
                    break;
            }
            switch (targetRange)
            {
                case 's':
                    isTargetAlly = true;
                    currentTarget = database.selector;
                    break;
                case 'u':
                case 'e':
                    isTargetAlly = false;
                    currentTarget = 0;
                    break;
                case 'a':
                    isTargetAlly = true;
                    currentTarget = 0;
                    break;
            }
        }
    }
}
