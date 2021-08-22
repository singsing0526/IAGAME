using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public bool isHandling = true, isSelectedOption = false, isAllySelected = false;
    [HideInInspector] public int selectedState = 0, selectedIndex = 0, selector = 0, selectedItem = 0, currentWave = 0, mapLerpingNumber = 0, totalWave = 0, level = 0, coinGainInOneRound = 0, beatCharacterSelectionIndex = 0, deathTime = 0;
    public int coin = 0;
    public GameObject characterData, sceneCharacter, instruction, map, log, dice, targetIcon, popText, audioPrefab;
    public GameObject[] characterSprites;
    [HideInInspector]public List<GameObject> allyDetails, enemyDetails;
    [HideInInspector]public List<Character> waitingEnemies;
    public List<Item> inventory;
    public Sprite[] backgroundMap, foregroundMap;
    [HideInInspector]public MoveForeground moveMap;
    [HideInInspector]public LogMessage logMessage;
     public Dice diceHolder;
    [HideInInspector] public TargetSelection targetIconHolder;
    public Sprite[] itemSprites, skillSprites;
    public AudioClip[] Sounds;

    private BattleMenu battleMenu;

    public void AddCharacterToAllyList(Character characterStats)
    {
        GameObject cloner = Instantiate(characterData);
        cloner.GetComponent<Character>().SetCharacter(characterStats.maxHP, characterStats.maxMP, characterStats.defense, characterStats.dodgeRate, characterStats.speed, characterStats.attackDamage, characterStats.element, characterStats.ID);
        Character tempCharacter = cloner.GetComponent<Character>();
        tempCharacter.isAlly = true;
        tempCharacter.database = this;
        cloner.name = characterStats.ID.ToString();
        cloner.transform.SetParent(transform);
        allyDetails.Add(cloner);
    }

    public void AddCharacterToEnemyList(Character characterStats)
    {
        GameObject cloner = Instantiate(characterData);
        cloner.GetComponent<Character>().SetCharacter(characterStats.maxHP, characterStats.maxMP, characterStats.defense, characterStats.dodgeRate, characterStats.speed, characterStats.attackDamage, characterStats.element, characterStats.ID, characterStats.wave);
        Character tempCharacter = cloner.GetComponent<Character>();
        tempCharacter.isAlly = false;
        tempCharacter.database = this;
        cloner.name = characterStats.ID.ToString();
        cloner.transform.SetParent(transform);
        enemyDetails.Add(cloner);
    }

    public void CreateAlly()
    {
        for (int i = 0; i < allyDetails.Count; i++)
        {
            GameObject cloner = Instantiate(sceneCharacter, new Vector2(-2 + i * -2.5f, -2), Quaternion.identity);
            SceneCharacter temp = cloner.GetComponent<SceneCharacter>();
            temp.characterStats = allyDetails[i].GetComponent<Character>();
            temp.database = this;
            temp.isBarCharacter = false;
            allyDetails[i].GetComponent<Character>().sceneCharacter = temp;
            cloner.tag = "Ally";
        }
    }

    public void CreateEnemy(Character characterStats)
    {
        characterStats.wave = currentWave;
        AddCharacterToEnemyList(characterStats);

        GameObject cloner = Instantiate(sceneCharacter, new Vector2(2 + (enemyDetails.Count - 1) * 2.5f, -2), Quaternion.identity);
        SceneCharacter temp = cloner.GetComponent<SceneCharacter>();
        temp.characterStats = enemyDetails[enemyDetails.Count - 1].GetComponent<Character>();
        temp.database = this;
        temp.isBarCharacter = false;
        enemyDetails[enemyDetails.Count - 1].GetComponent<Character>().sceneCharacter = temp;
        cloner.tag = "Enemy";
    }

    public void CreateEnemy()
    {
        int t = 0;
        for (int i = 0; i < waitingEnemies.Count; i++)
        {
            if (waitingEnemies[i].wave == currentWave)
            {
                AddCharacterToEnemyList(waitingEnemies[i]);
                t++;
            }
        }

        for (int i = 0; i < waitingEnemies.Count; i++)
        {
            if (t > 0)
            {
                waitingEnemies.RemoveAt(t - 1);
            }
            t--;
        }
        for (int i = 0; i < enemyDetails.Count; i++)
        {
            GameObject cloner = Instantiate(sceneCharacter, new Vector2(2 + i * 2.5f, -2), Quaternion.identity);
            SceneCharacter temp = cloner.GetComponent<SceneCharacter>();
            temp.characterStats = enemyDetails[i].GetComponent<Character>();
            temp.database = this;
            temp.isBarCharacter = false;
            enemyDetails[i].GetComponent<Character>().sceneCharacter = temp;
            cloner.tag = "Enemy";
        }
    }

    public void AddItemToInventory(string name, int amount)
    {
        bool isExist = false;
        int existingIndex = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (name == inventory[i].itemName)
            {
                isExist = true;
                existingIndex = i;
            }
        }
        if (isExist == true)
        {
            inventory[existingIndex].itemAmount += amount;
        }
        else
        {
            inventory.Add(new Item(name, amount, GetItemID(name)));
        }
    }

    public void CreateMap(int sceneNumber)
    {
        GameObject tempBackground = Instantiate(map);
        tempBackground.transform.position = new Vector3(0, 0, 0);
        SpriteRenderer backgroundSR = tempBackground.GetComponent<SpriteRenderer>();
        backgroundSR.sprite = backgroundMap[sceneNumber];
        backgroundSR.sortingLayerName = "background";
        tempBackground.transform.localScale = new Vector3(2.5f, 2.5f, 0);

        GameObject tempForeground = Instantiate(map);
        tempForeground.transform.position = new Vector3(0, 0, 0);
        SpriteRenderer foregroundSR = tempForeground.GetComponent<SpriteRenderer>();
        foregroundSR.sprite = foregroundMap[sceneNumber];
        foregroundSR.sortingLayerName = "foreground";
        tempForeground.transform.localScale = new Vector3(2.5f, 2.5f, 0);
        moveMap = tempForeground.GetComponent<MoveForeground>();
        moveMap.isForeground = true;
        moveMap.database = this;
    }

    public void AddEnemyDetailsToWaitingEnemies(int maxHP, int maxMP, int defense, int dodgeRate, int speed, int attackDamage, Character.Element element, int ID, int wave)
    {
        waitingEnemies.Add(new Character(maxHP, maxMP, defense, dodgeRate, speed, attackDamage, element, ID, wave));
    }

    public Character.Element GetSkillElement(int ID)
    {
        switch (ID)
        {
            case 0:
                return Character.Element.wind;
            case 1:
                return Character.Element.wind;
            case 2:
                return Character.Element.electricity;
            case 3:
                return Character.Element.fire;
            case 4:
                return Character.Element.fire;
            case 5:
                return Character.Element.water;
            case 6:
                return Character.Element.water;
            case 7:
                return Character.Element.electricity;
            case 8:
                return Character.Element.earth;
            case 9:
                return Character.Element.earth;
        }
        return Character.Element.none;
    }

    public int GetItemID(string name)
    {
        switch (name)
        {
            case "HP Potion":
                return 0;
            case "MP Potion":
                return 1;
            case "Speed Potion":
                return 2;
            case "Strength Potion":
                return 3;
            case "Revive Potion":
                return 4;
        }
        return 0;
    }

    public string GetItemName(int index)
    {
        switch (index)
        {
            case 0:
                return "HP Potion";
            case 1:
                return "MP Potion";
            case 2:
                return "Speed Potion";
            case 3:
                return "Strength Potion";
            case 4:
                return "Revive Potion";
        }
        return "HP Potion";
    }

    public void SetUp()
    {
        coin = 0;
    }

    public void CreateDice()
    {
        if (diceHolder == null)
        {
            if (logMessage == null)
            {
                logMessage = Instantiate(log).GetComponent<LogMessage>();
                logMessage.DeleteLog();
                logMessage.database = this;
            }
            diceHolder = Instantiate(dice, new Vector2(-9, 0), Quaternion.identity).GetComponent<Dice>();
            diceHolder.database = this;
        }
    }


    private IEnumerator WaitForTotalWave(int minWave, int maxWave, int level, int minEnemy, int maxEnemy)
    {
        CreateDice();
        diceHolder.ThrowDice(minWave - 1, maxWave);
        yield return new WaitUntil(() => diceHolder.isDicingComplete == true);
        totalWave = diceHolder.diceNumber;
        Destroy(diceHolder.textHolder.gameObject);
        Destroy(diceHolder.gameObject);
        diceHolder = null;

        for (int i = 0; i < totalWave; i++)
        {

            int enemyNumber = Random.Range(minEnemy, maxEnemy + 1);
            for (int j = 0; j < enemyNumber; j++)
            {
                Character tempCharacter = CharacterLibrary(GetPossibleEnemyInLevel(level));
                AddEnemyDetailsToWaitingEnemies(tempCharacter.maxHP, 0, tempCharacter.defense, tempCharacter.dodgeRate, tempCharacter.speed, tempCharacter.attackDamage, tempCharacter.element, tempCharacter.ID, i);
            }
        }

        CreateAlly();
        CreateEnemy();
    }

    public void GetAllyNPC()
    {
        StartCoroutine(WaitForAllyCharacterGift());
    }

    public IEnumerator WaitForAllyCharacterGift()
    {
        CreateDice();
        logMessage.AddMessage("<Determining Ally Character Gain!>");
        diceHolder.ThrowDice(1, 4);
        yield return new WaitUntil(() => diceHolder.isDicingComplete == true);
        diceHolder.isDicingComplete = false;
        int tempDiceNumber = diceHolder.diceNumber;
        if (tempDiceNumber != 4)
        {
            TMPro.TextMeshProUGUI tempInstruction = Instantiate(instruction).GetComponent<TMPro.TextMeshProUGUI>();
            tempInstruction.text = "[Z] to continue";
            tempInstruction.transform.SetParent(GameObject.Find("Canvas").transform);
            logMessage.AddMessage("Failed!");
            logMessage.PrintLatestMessage();
            logMessage.SetImage(1);
            Destroy(diceHolder.textHolder.gameObject);
            Destroy(diceHolder.gameObject);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            Destroy(tempInstruction.gameObject);
        }
        else
        {
            logMessage.AddMessage("Success!");
            logMessage.AddMessage("<Determining Possible Ally Character Index!>");
            int characterIndex = 0;
            do
            {
                characterIndex = Random.Range(0, characterSprites.Length);
            } while (isCharacterAnAlly(characterIndex) == false);

            diceHolder.ThrowPretendDice(characterIndex);
            if (allyDetails.Count < 4)
            {
                AddCharacterToAllyList(CharacterLibrary(characterIndex));
            }
            else
            {
                logMessage.AddMessage("<Detected Reaching Maximum Ally Cap>");
                logMessage.AddMessage("Adding ally failed!");
            }
            yield return new WaitUntil(() => diceHolder.isDicingComplete == true);
            Destroy(diceHolder.textHolder.gameObject);
            Destroy(diceHolder.gameObject);
        }

        diceHolder.isDicingComplete = true;
        logMessage.DeleteLog();
        Destroy(logMessage.gameObject);
        diceHolder = null;
        transform.parent.GetChild(1).GetComponent<PointHolder>().SetText();
    }

    public bool isCharacterAnAlly(int index)
    {
        switch (index)
        {
            case 0:
                return true;
            case 3:
                return true;
            case 4:
                return true;
            case 5:
                return true;
            case 20:
                return true;
        }
        return false;
    }

    private int GetPossibleEnemyInLevel(int level)
    {
        switch (level)
        {
            case 0:
                if (isFulfilledPossibility(2))
                {
                    return 1;
                }
                if (isFulfilledPossibility(2))
                {
                    return 2;
                }
                break;

            case 1:
                if (isFulfilledPossibility(3))
                {
                    return 2;
                }
                if (isFulfilledPossibility(2))
                {
                    return 17;
                }
                break;

            case 2:
                if (isFulfilledPossibility(1))
                {
                    return 12; // Knights
                }
                break;

            case 3:
                if (isFulfilledPossibility(2))
                {
                    return 1;
                }
                if (isFulfilledPossibility(3))
                {
                    return 17;
                }
                if (isFulfilledPossibility(3))
                {
                    return 18;
                }
                break;

            case 4:
                if (isFulfilledPossibility(2))
                {
                    return 1;
                }
                if (isFulfilledPossibility(2))
                {
                    return 18;
                }
                if (isFulfilledPossibility(3))
                {
                    return 19;
                }
                break;

            case 5:
                if (isFulfilledPossibility(3))
                {
                    return 6;
                }
                if (isFulfilledPossibility(3))
                {
                    return 7;
                }
                if (isFulfilledPossibility(2))
                {
                    return 19;
                }
                break;

            case 6:
                if (isFulfilledPossibility(2))
                {
                    return 8;
                }
                if (isFulfilledPossibility(4))
                {
                    return 6;
                }
                if (isFulfilledPossibility(3))
                {
                    return 7;
                }
                break;

            case 7: // Doogy
                if (isFulfilledPossibility(1))
                {
                    return 13;
                }
                break;

            case 8: // FInal Boss
                if (isFulfilledPossibility(1))
                {
                    return 16;
                }
                break;

            case 9: // Bird Cage
                if (isFulfilledPossibility(2))
                {
                    return 9;
                }
                if (isFulfilledPossibility(2))
                {
                    return 10;
                }
                if (isFulfilledPossibility(2))
                {
                    return 11;
                }
                break;

            case 10: // Beacon
                if (isFulfilledPossibility(2))
                {
                    return 18;
                }
                if (isFulfilledPossibility(2))
                {
                    return 19;
                }
                if (isFulfilledPossibility(4))
                {
                    return 6;
                }
                break;

            case 11:
                if (isFulfilledPossibility(3))
                {
                    return 18;
                }
                if (isFulfilledPossibility(4))
                {
                    return 19;
                }
                if (isFulfilledPossibility(2))
                {
                    return 6;
                }
                if (isFulfilledPossibility(3))
                {
                    return 9;
                }
                if (isFulfilledPossibility(3))
                {
                    return 10;
                }
                if (isFulfilledPossibility(3))
                {
                    return 11;
                }
                if (isFulfilledPossibility(2))
                {
                    return 6;
                }
                break;
        }

        return 1;
    }

    public Character CharacterLibrary(int index) // Set All Character Statisics
    {
        switch (index)
        {
            case 0: // HP, MP, Defense, Dodge Rate, Speed, Attack Damage, Element
                return new Character(180, 20, 9, 12, 10, 25, GetRandomElement(), index, 0); //main
            case 1: // Base Enemy
                return new Character(30, 0, 2, 10, 5, 10, GetRandomElement(), index, 0);
            case 2:
                return new Character(40, 0, 4, 10, 4, 18, GetRandomElement(), index, 0);
            case 3:
                return new Character(200, 20, 8, 10, 7, 30, GetRandomElement(), index, 0);//big guy
            case 4: // Have More Default Skills
                return new Character(130, 60, 5, 10, 14, 8, GetRandomElement(), index, 0); //woman
            case 5:
                return new Character(80, 70, 5, 10, 5, 50, GetRandomElement(), index, 0); //ging
            case 6:
                return new Character(70, 0, 3, 10, 10, 0, GetRandomElement(), index, 0);//heart
            case 7:
                return new Character(35, 0, 3, 11, 14, 10, GetRandomElement(), index, 0);//bird
            case 8:
                return new Character(100, 0, 3, 10, 9, 15, GetRandomElement(), index, 0);//cake
            case 9:
                return new Character(90, 0, 5, 10, 9, 25, GetRandomElement(), index, 0);//face1
            case 10:
                return new Character(90, 0, 5, 10, 12, 15, GetRandomElement(), index, 0);//face2
            case 11:
                return new Character(70, 0, 6, 10, 6, 35, GetRandomElement(), index, 0);//face3
            case 12: // knight
                return new Character(240, 0, 6, 10, 9, 20, GetRandomElement(), index, 0);
            case 13: //3headdogXD 1
                return new Character(150, 0, 5, 10, 12, 20, GetRandomElement(), index, 0);
            case 14: //3headdogXD 2
                return new Character(150, 0, 5, 10, 10, 30, GetRandomElement(), index, 0);
            case 15: //3headdogXD 3
                return new Character(110, 0, 4, 10, 8, 40, GetRandomElement(), index, 0);
            case 16: //KING
                return new Character(777, 0, 4, 10, 10, 33, Character.Element.wildfire, index, 0);
            case 17: //worm._.
                return new Character(30, 0, 3, 10, 9, 12, GetRandomElement(), index, 0);
            case 18: //BigEye
                return new Character(90, 0, 3, 10, 10, 25, GetRandomElement(), index, 0);
            case 19: //idontknowwhatisthis
                return new Character(100, 0, 4, 10, 4, 35, GetRandomElement(), index, 0);
            case 20: //bow man
                return new Character(110, 0, 3, 15, 13, 25, GetRandomElement(), index, 0);

        }
        return new Character(100, 100, 100, 100, 100, 100, GetRandomElement(), index, 0);
    }

    private Character.Element GetRandomElement()
    {
        int randomIndex = Random.Range(0, 5);

        switch (randomIndex)
        {
            case 0:
                return Character.Element.fire;
            case 1:
                return Character.Element.water;
            case 2:
                return Character.Element.wind;
            case 3:
                return Character.Element.earth;
            case 4:
                return Character.Element.electricity;
        }
        return Character.Element.none;
    }

    private bool isFulfilledPossibility(int denominator)
    {
        if (Random.Range(1, denominator + 1) == 1)
        {
            return true;
        }
        return false;
    }

    public void Initialize()
    {
        battleMenu = GameObject.Find("BattleMenu").GetComponent<BattleMenu>();
        battleMenu.sr = battleMenu.GetComponent<SpriteRenderer>();
        battleMenu.database = this;
        battleMenu.Hide();

        logMessage = Instantiate(log).GetComponent<LogMessage>();
        logMessage.DeleteLog();
        logMessage.database = this;
        logMessage.AddMessage("<Determining Total Wave!>");

        currentWave = 0;
        coinGainInOneRound = 0;
        targetIconHolder = Instantiate(targetIcon).GetComponent<TargetSelection>();
        targetIconHolder.database = this;
        beatCharacterSelectionIndex = 0;


        CreateMap(level);
        switch (level)
        {
            case 0:
                AddSound(0, true, 0.5f);
                StartCoroutine(WaitForTotalWave(2, 3, level, 2, 3)); // min , max
                break;
            case 1:
                AddSound(0, true, 0.5f);
                StartCoroutine(WaitForTotalWave(2, 3, level, 2, 3));
                break;
            case 2:     //boss
                AddSound(1, true, 0.5f);
                StartCoroutine(WaitForTotalWave(1, 1, level, 1, 1));
                break;
            case 3:
                AddSound(0, true, 0.5f);
                StartCoroutine(WaitForTotalWave(1, 3, level, 2, 4));
                break;
            case 4:
                AddSound(0, true, 0.5f);
                StartCoroutine(WaitForTotalWave(2, 3, level, 2, 4));
                break;
            case 5:
                AddSound(3, true, 0.5f);
                StartCoroutine(WaitForTotalWave(2, 3, level, 2, 4));
                break;
            case 6:
                AddSound(3, true, 0.5f);
                StartCoroutine(WaitForTotalWave(2, 3, level, 2, 4));
                break;
            case 7:  // Doogy
                AddSound(4, true, 0.5f);
                StartCoroutine(WaitForTotalWave(1, 1, level, 1, 1));
                break;
            case 8:  // Final Boss
                AddSound(5, true, 0.5f);
                StartCoroutine(WaitForTotalWave(1, 1, level, 1, 1));
                break;
            case 9:  // Bird Cage
                AddSound(3, true, 0.5f);
                StartCoroutine(WaitForTotalWave(2, 3, level, 2, 4));
                break;
            case 10: // Beacon
                AddSound(3, true, 0.5f);
                StartCoroutine(WaitForTotalWave(2, 3, level, 2, 3));
                break;
            case 11:
                AddSound(3, true, 0.5f);
                StartCoroutine(WaitForTotalWave(2, 4, level, 2, 3));
                break;
            default: // 11
                AddSound(3, true, 0.5f);
                StartCoroutine(WaitForTotalWave(2, 4, level, 2, 3));
                break;
        }
    }

    public void AddSound(int index, bool isLoopable, float vol)
    {
        AudioPlayer tempAudio = Instantiate(audioPrefab).GetComponent<AudioPlayer>();
        tempAudio.SetAudioDetails(Sounds[index], isLoopable, vol);
    }
}
