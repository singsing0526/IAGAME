using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public Database database;
    [SerializeField] private int page = 0, selectionIndex = 0, maxSelectionIndex = 0, learnableIndex = 0;
    public GameObject spriteHolder;
    public List<SpriteRenderer> sr, characterExistingSkills;
    public List<TMPro.TextMeshProUGUI> price;
    public TMPro.TextMeshProUGUI coinText, characterStatsText, instructionHolder;
    public List<Character.Element> existingCharacterElement;
    public List<GameObject> characterForSkill;
    private SpriteRenderer skillNotice;
    private bool isSkillLearnable = false;
    private ShopDoorController door;
    public bool isShopOpened = false;
    public List<int> fixedRandomSkillPrices;

    private void Start()
    {
        database = GameObject.Find("Database").GetComponent<Database>();
        coinText = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
        coinText.transform.SetParent(GameObject.Find("Canvas").transform);
        coinText.transform.position = new Vector2(-3, 4.5f);
        coinText.fontSize = 2;

        instructionHolder = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>(); ;
        instructionHolder.transform.SetParent(GameObject.Find("Canvas").transform);

        for (int i = 0; i < database.allyDetails.Count; i++)
        {
            Character.Element tempElement = database.allyDetails[i].GetComponent<Character>().element;
            if (!existingCharacterElement.Contains(tempElement))
            {
                existingCharacterElement.Add(tempElement);
            }
        }

        fixedRandomSkillPrices.Clear();
        for (int i = 0; i < database.skillSprites.Length; i++)
        {
            fixedRandomSkillPrices.Add(Random.Range(150, 400));
        }

        door = GameObject.Find("DoorController").GetComponent<ShopDoorController>();
        door.SM = this;
        door.OpenDoor(0);
    }

    private void Update()
    {
        if (isShopOpened == true)
        {
            if (page == 0)
            {
                maxSelectionIndex = database.itemSprites.Length;
            }
            else if (page == 1)
            {
                maxSelectionIndex = database.skillSprites.Length;
            }
            else if (page == 2)
            {
                maxSelectionIndex = characterForSkill.Count;
            }

            if (page != 2)
            {
                if (Input.GetKeyDown(KeyCode.W) && selectionIndex - 1 >= 0)
                {
                    database.AddSound(9, false, 1);
                    selectionIndex--;
                    sr[selectionIndex].color = new Color32(255, 255, 255, 255);
                    sr[selectionIndex + 1].color = new Color32(170, 70, 200, 255);

                    SetText(page, selectionIndex, 1);
                    ScrollDown();
                }
                if (Input.GetKeyDown(KeyCode.S) && selectionIndex + 1 < maxSelectionIndex)
                {
                    database.AddSound(9, false, 1);
                    selectionIndex++;
                    sr[selectionIndex].color = new Color32(255, 255, 255, 255);
                    sr[selectionIndex - 1].color = new Color32(170, 70, 200, 255);

                    SetText(page, selectionIndex, -1);
                    ScrollUp();
                }
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    database.AddSound(13, false, 1);
                    if (page == 0)
                    {
                        if (database.coin >= GetPrice(page, selectionIndex))
                        {
                            PopText tempText = Instantiate(database.popText).GetComponent<PopText>();
                            tempText.SetText(sr[selectionIndex].transform.position, "Bought", 0.5f);
                            tempText.SetFloat();
                            database.coin -= GetPrice(page, selectionIndex);
                            coinText.text = "$" + database.coin.ToString();
                            database.AddItemToInventory(database.GetItemName(selectionIndex), 1);
                            price[selectionIndex].text = "$" + GetPrice(page, selectionIndex).ToString() + "\nx" + GetInventoryAmount(selectionIndex).ToString();
                        }
                        else
                        {
                            PopText tempText = Instantiate(database.popText).GetComponent<PopText>();
                            tempText.SetText(sr[selectionIndex].transform.position, "Insufficient Coins", 0.5f);
                            tempText.SetFloat();
                        }
                    }
                    else if (page == 1)
                    {
                        instructionHolder.text = "[A][D] to select character, [Z] to learn skill, [X] to cancel";
                        if (existingCharacterElement.Contains(database.GetSkillElement(selectionIndex)))
                        {
                            skillNotice = Instantiate(spriteHolder).GetComponent<SpriteRenderer>();
                            skillNotice.sprite = database.skillSprites[selectionIndex];
                            skillNotice.transform.position = new Vector2(-5.7f, 2.6f);
                            learnableIndex = selectionIndex;

                            Character.Element tempElement = database.GetSkillElement(selectionIndex);
                            page = 2;
                            selectionIndex = 0;

                            for (int i = 0; i < sr.Count; i++)
                            {
                                Destroy(sr[i].gameObject);
                                Destroy(price[i].gameObject);
                            }
                            characterForSkill.Clear();
                            sr.Clear();
                            price.Clear();

                            for (int i = 0; i < database.allyDetails.Count; i++)
                            {
                                Character tempCharacter = database.allyDetails[i].GetComponent<Character>();
                                if (tempElement == tempCharacter.element)
                                {
                                    characterForSkill.Add(Instantiate(database.characterSprites[tempCharacter.ID]));
                                    price.Add(Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>());
                                    price[price.Count - 1].text = i.ToString();
                                    sr.Add(characterForSkill[characterForSkill.Count - 1].GetComponent<SpriteRenderer>());
                                }
                            }
                            for (int i = 0; i < characterForSkill.Count; i++)
                            {
                                characterForSkill[i].transform.position = new Vector2(-8 + i * 1.5f, -3);
                                price[i].transform.position = new Vector2(-8 + i * 1.5f, -1);
                                price[i].transform.SetParent(GameObject.Find("Canvas").transform);
                                price[i].alignment = TMPro.TextAlignmentOptions.Midline;
                                sr[i].color = new Color32(170, 70, 200, 100);
                            }

                            ShowCharacterStats(0);
                            sr[0].color = new Color32(255, 255, 255, 255);
                            page = 2;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.A) && page - 1 >= 0)
                {
                    database.AddSound(9, false, 1);
                    page--;
                    SetUpShop(page);
                }
                if (Input.GetKeyDown(KeyCode.D) && page + 1 < 2)
                {
                    database.AddSound(9, false, 1);
                    page++;
                    SetUpShop(page);
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    database.AddSound(10, false, 1);
                    if (coinText != null)
                        Destroy(coinText.gameObject);
                    if (characterStatsText != null)
                        Destroy(characterStatsText.gameObject);
                    if (price.Count > 0)
                    {
                        for (int i = 0; i < price.Count; i++)
                        {
                            Destroy(price[i].gameObject);
                        }
                    }
                    door.CloseDoor();
                    enabled = false;
                }
            }
            else // page = 2
            {
                if (Input.GetKeyDown(KeyCode.A) && selectionIndex - 1 >= 0)
                {
                    database.AddSound(9, false, 1);
                    selectionIndex--;
                    sr[selectionIndex].color = new Color32(255, 255, 255, 255);
                    sr[selectionIndex + 1].color = new Color32(170, 70, 200, 100);

                    ShowCharacterStats(int.Parse(price[selectionIndex].text));
                }
                if (Input.GetKeyDown(KeyCode.D) && selectionIndex + 1 < maxSelectionIndex)
                {
                    database.AddSound(9, false, 1);
                    selectionIndex++;
                    sr[selectionIndex].color = new Color32(255, 255, 255, 255);
                    sr[selectionIndex - 1].color = new Color32(170, 70, 200, 100);

                    ShowCharacterStats(int.Parse(price[selectionIndex].text));
                }
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    database.AddSound(10, false, 1);
                    if (isSkillLearnable == true)
                    {
                        if (database.coin >= GetPrice(1, learnableIndex))
                        {
                            PopText tempText = Instantiate(database.popText).GetComponent<PopText>();
                            tempText.SetText(sr[selectionIndex].transform.position, "Learnt", 0.5f);
                            tempText.SetFloat();
                            int characterIndex = int.Parse(price[selectionIndex].text);
                            database.allyDetails[characterIndex].GetComponent<Character>().AddSkill(database.GetSkillElement(learnableIndex), 10, learnableIndex);
                            ShowCharacterStats(characterIndex);
                            database.coin -= GetPrice(1, learnableIndex);
                            SetTextForLearningPage(database.allyDetails[characterIndex].GetComponent<Character>());
                        }
                        else
                        {
                            PopText tempText = Instantiate(database.popText).GetComponent<PopText>();
                            tempText.SetText(sr[selectionIndex].transform.position, "Insufficient Coins", 0.5f);
                            tempText.SetFloat();
                        }
                    }
                    else
                    {
                        PopText tempText = Instantiate(database.popText).GetComponent<PopText>();
                        tempText.SetText(sr[selectionIndex].transform.position, "You've Learnt", 0.5f);
                        tempText.SetFloat();
                    }
                }

                if (Input.GetKeyDown(KeyCode.X))
                {
                    database.AddSound(10, false, 1);
                    for (int i = 0; i < sr.Count; i++)
                    {
                        Destroy(characterForSkill[i]);
                        Destroy(sr[i].gameObject);
                        Destroy(price[i].gameObject);
                    }
                    for (int i = 0; i < characterExistingSkills.Count; i++)
                    {
                        Destroy(characterExistingSkills[i].gameObject);
                    }

                    Destroy(skillNotice.gameObject);
                    Destroy(characterStatsText.gameObject);
                    characterForSkill.Clear();
                    sr.Clear();
                    price.Clear();
                    characterExistingSkills.Clear();
                    coinText.text = "$" + database.coin.ToString();
                    SetUpShop(1);
                    instructionHolder.text = "[W][S] to select, [Z] to buy, [A][D] to flip, [X] to exit";
                }
            }
        }
    }

    public void SetTextForLearningPage(Character characterStats)
    {
        if (characterExistingSkills.Count > 0)
        {
            if (IsSkillLearnt(learnableIndex, characterStats) == true)
            {
                isSkillLearnable = true;
                coinText.text = "$" + database.coin.ToString() + "\nLearnable Skill:\t\t\t\t\t\t\t\t\tExisting Skills:";
            }
            else
            {
                isSkillLearnable = false;
                coinText.text = "$" + database.coin.ToString() + "\nLearnt Skill:\t\t\t\t\t\t\t\t\tExisting Skills:";
            }
        }
        else
        {
            if (IsSkillLearnt(learnableIndex, characterStats) == true)
            {
                isSkillLearnable = true;
                coinText.text = "$" + database.coin.ToString() + "\nLearnable Skill:";
            }
            else
            {
                isSkillLearnable = false;
                coinText.text = "$" + database.coin.ToString() + "\nLearnt Skill:";

            }
        }
    }

    public bool IsSkillLearnt(int skillID, Character characterStats)
    {
        for(int i = 0; i < characterStats.skills.Count; i++)
        {
            if (characterStats.skills[i].ID == skillID)
            {
                return false;
            }
        }
        return true;
    }

    public void ShowCharacterStats(int index)
    {
        for (int i = 0; i < characterExistingSkills.Count; i++)
        {
            Destroy(characterExistingSkills[i].gameObject);
        }
        characterExistingSkills.Clear();
        if (characterStatsText != null)
        {
            Destroy(characterStatsText.gameObject);
            characterStatsText = null;
        }

        Character tempCharacter = database.allyDetails[index].GetComponent<Character>();
        for (int i = 0; i < tempCharacter.skills.Count; i++)
        {
            SpriteRenderer tempSR = Instantiate(spriteHolder).GetComponent<SpriteRenderer>();
            tempSR.sprite = database.skillSprites[tempCharacter.skills[i].ID];
            characterExistingSkills.Add(tempSR);
        }
        for (int i = 0; i < characterExistingSkills.Count; i++)
        {
            characterExistingSkills[i].transform.position = new Vector2(6, 2.6f + i * -1.5f);
        }

        SetTextForLearningPage(tempCharacter);

        characterStatsText = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
        characterStatsText.transform.SetParent(GameObject.Find("Canvas").transform);
        characterStatsText.text = "HP: " + tempCharacter.currentHP + "/" + tempCharacter.maxHP + " \tMP: " + tempCharacter.currentMP + "/" + tempCharacter.maxMP + "\nStrength: " + tempCharacter.attackDamage + " \tSpeed: " + tempCharacter.speed + "\nDefense: " + tempCharacter.defense + " \t Dodge Rate: " + tempCharacter.dodgeRate + "\nElement: " + tempCharacter.element;
        characterStatsText.transform.position = new Vector2(-3, 1.6f);
        characterStatsText.color = new Color32(250, 200, 55, 255);
    }

    public void SetText(int page, int index, int offset)
    {
        if (page == 0)
        {
            price[index].text = "$" + GetPrice(page, index).ToString() + "\nx" + GetInventoryAmount(index).ToString();
            price[index + offset].text = "x" + GetInventoryAmount(index + offset).ToString();
        }
        else if (page == 1)
        {
            if (existingCharacterElement.Contains(database.GetSkillElement(index)))
            {
                price[index].text = "$" + GetPrice(page, index).ToString();
            }
            else
            {
                price[index].text = "Locked";
            }

            if (existingCharacterElement.Contains(database.GetSkillElement(index + offset)))
            {
                price[index + offset].text = "$" + GetPrice(page, index + offset).ToString();
            }
            else
            {
                price[index + offset].text = "Locked";
            }
        }
    }

    public void ScrollUp()
    {
        if (sr.Count >= 6)
        {
            for (int i = 0; i < sr.Count; i++)
            {
                sr[i].transform.position = (Vector2)sr[i].transform.position + new Vector2(0, 1.5f);
                price[i].transform.position = (Vector2)price[i].transform.position + new Vector2(0, 1.5f);
                if (price[i].transform.position.y >= 6)
                {
                    sr[i].enabled = false;
                    price[i].enabled = false;
                }
                else if (price[i].transform.position.y <= -6)
                {
                    sr[i].enabled = false;
                    price[i].enabled = false;
                }
                else
                {
                    sr[i].enabled = true;
                    price[i].enabled = true;
                }

            }
        }
    }

    public void ScrollDown()
    {
        if (sr.Count >= 6)
        {
            for (int i = 0; i < sr.Count; i++)
            {
                sr[i].transform.position = (Vector2)sr[i].transform.position + new Vector2(0, -1.5f);
                price[i].transform.position = (Vector2)price[i].transform.position + new Vector2(0, -1.5f);
                if (price[i].transform.position.y >= 6)
                {
                    sr[i].enabled = false;
                    price[i].enabled = false;
                }
                else if (price[i].transform.position.y <= -6)
                {
                    sr[i].enabled = false;
                    price[i].enabled = false;
                }
                else
                {
                    sr[i].enabled = true;
                    price[i].enabled = true;
                }
            }
        }
    }


    public int GetPrice(int page, int index)
    {
        if (page == 0)
        {
            switch (index)
            {
                case 0: // HP Potion
                    return 100;
                case 1: // HP Potion
                    return 70;
                case 2: // Speed Potion
                    return 125;
                case 3: // Strength Potion
                    return 150;
                case 4: // Revive Potion
                    return 300;
            }
        }
        else if (page == 1)
        {
            return fixedRandomSkillPrices[index];
        }
        return 0;
    }

    public int GetInventoryAmount(int ID)
    {
        for (int i = 0; i< database.inventory.Count; i++)
        {
            if (database.inventory[i].ID == ID)
            {
                return database.inventory[i].itemAmount;
            }
        }
        return 0;
    }

    public void SetUpShop(int page)
    {

        this.page = page;

        if (sr.Count > 0)
        {
            for (int i = 0; i < sr.Count; i++)
            {
                Destroy(sr[i].gameObject);
                Destroy(price[i].gameObject);
            }
            sr.Clear();
            price.Clear();
        }

        if (page == 0)
        {
            maxSelectionIndex = database.itemSprites.Length;
        }
        else
        {
            maxSelectionIndex = database.skillSprites.Length;
        }
        selectionIndex = maxSelectionIndex / 2;

        switch (page)
        {
            case 0:
                for (int i = 0; i < database.itemSprites.Length; i++)
                {
                    SpriteRenderer tempSR = Instantiate(spriteHolder).GetComponent<SpriteRenderer>();
                    tempSR.sprite = database.itemSprites[i];
                    tempSR.transform.position = new Vector2(0, selectionIndex * 1.5f + i * -1.5f);
                    tempSR.color = new Color32(170, 70, 200, 255);
                    sr.Add(tempSR);

                    TMPro.TextMeshProUGUI tempPrice = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
                    tempPrice.transform.position = new Vector2(1.3f, selectionIndex * 1.5f + i * -1.5f);
                    tempPrice.transform.SetParent(GameObject.Find("Canvas").transform);
                    tempPrice.fontSize = 2;
                    price.Add(tempPrice);
                    SetText(page, i, 0);
                }
                break;
            case 1:
                for (int i = 0; i < database.skillSprites.Length; i++)
                {
                    SpriteRenderer tempSR = Instantiate(spriteHolder).GetComponent<SpriteRenderer>();
                    tempSR.sprite = database.skillSprites[i];
                    tempSR.transform.position = new Vector2(0, selectionIndex * 1.5f + i * -1.5f);
                    tempSR.color = new Color32(170, 70, 200, 255);
                    sr.Add(tempSR);

                    TMPro.TextMeshProUGUI tempPrice = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
                    tempPrice.transform.position = new Vector2(0, selectionIndex * 1.5f + i * -1.5f);
                    tempPrice.transform.SetParent(GameObject.Find("Canvas").transform);
                    tempPrice.fontSize = 2;
                    price.Add(tempPrice);
                    SetText(page, i, 0);
                }
                ScrollUp();
                ScrollDown();
                break;
        }

        sr[selectionIndex].color = new Color32(255, 255, 255, 255);
        if (page == 0)
        {
            price[selectionIndex].text = "$" + GetPrice(page, selectionIndex).ToString() + "\nx" + GetInventoryAmount(selectionIndex).ToString();
        }
    }
}
