using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public Database database;
    private TMPro.TextMeshProUGUI textHolder;
    public int beatingCharacterIndex = -1;
    public Character beatingCharacter;
    private bool isTargetIconHiding = false, isHidingCalled = false;

    private void Start()
    {
        database = GameObject.Find("Database").GetComponent<Database>();
        textHolder = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
        textHolder.transform.SetParent(GameObject.Find("Canvas").transform);
        textHolder.text = null;
    }

    void Update()
    {
        if (database.enemyDetails.Count - 1 < database.beatCharacterSelectionIndex)
        {
            database.beatCharacterSelectionIndex = database.enemyDetails.Count - 1;
            if (database.beatCharacterSelectionIndex < 0)
            {
                database.beatCharacterSelectionIndex = 0;
            }
            else
            {
                beatingCharacterIndex = database.beatCharacterSelectionIndex;
                beatingCharacter = database.enemyDetails[database.beatCharacterSelectionIndex].GetComponent<Character>();

            }
        }
        if (isTargetIconHiding == true)
        {
            if (isHidingCalled == false)
            {
                isHidingCalled = true;
                database.targetIconHolder.Hide();
                beatingCharacterIndex = -1;
            }
        }
        else
        {
            if (isHidingCalled == true)
            {
                isHidingCalled = false;
                database.targetIconHolder.Show();
            }
        }

        if (database.isHandling == false)
        {
            if (database.enemyDetails.Count > 1)
            {
                textHolder.text = "[A][D] to select stopping enemy";
                isTargetIconHiding = false;
                if (Input.GetKeyDown(KeyCode.A) && database.beatCharacterSelectionIndex - 1 >= 0)
                {
                    database.beatCharacterSelectionIndex--;
                }

                if (Input.GetKeyDown(KeyCode.D) && database.beatCharacterSelectionIndex + 1 < database.enemyDetails.Count)
                {
                    database.beatCharacterSelectionIndex++;
                }

                if (beatingCharacterIndex != database.beatCharacterSelectionIndex)
                {
                    beatingCharacterIndex = database.beatCharacterSelectionIndex;
                    beatingCharacter = database.enemyDetails[database.beatCharacterSelectionIndex].GetComponent<Character>();
                }
                if (beatingCharacter != null)
                    beatingCharacter.sceneCharacter.barCharacter.progress -= (beatingCharacter.speed + beatingCharacter.extraSpeed) * Time.deltaTime;
                else
                    beatingCharacterIndex = -1;
            }
            else
            {
                isTargetIconHiding = true;
                textHolder.text = null;
            }
        }
        else
        {
            textHolder.text = null;
        }
    }

}
