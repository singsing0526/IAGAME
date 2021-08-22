using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Sprite[] diceImages;
    private SpriteRenderer sr;
    private Vector2 originalPos;
    [HideInInspector]public Database database;
    public bool isDicingComplete;
    public int diceNumber;
    [HideInInspector] public TMPro.TextMeshProUGUI textHolder, instructionHolder;
    private Transform canvasTransform;

    private void Awake()
    {
        canvasTransform = GameObject.Find("Canvas").transform;
        sr = GetComponent<SpriteRenderer>();
        isDicingComplete = true;
        originalPos = transform.position;
    }

    private void CreateText()
    {
        if (textHolder == null && database != null)
        {
            textHolder = Instantiate(database.instruction, transform.position, Quaternion.identity).GetComponent<TMPro.TextMeshProUGUI>();
            textHolder.transform.SetParent(canvasTransform);
            textHolder.text = null;
            textHolder.fontSize = 4;
            textHolder.alignment = TMPro.TextAlignmentOptions.Midline;
        }
    }

    private void Update()
    {
        transform.Rotate(0, 0, 30 * Time.deltaTime);
        transform.position = new Vector3(originalPos.x, originalPos.y + Mathf.Cos(Time.time * 20) * 0.5f, 0);
    }

    public void ThrowDice(int min, int max)
    {
        CreateText();
        StartCoroutine(DiceRandom(min, max));
    }

    public void ThrowPretendDice(int finalResult)
    {
        CreateText();
        StartCoroutine(PretendDiceRandom(finalResult));
    }

    private IEnumerator PretendDiceRandom(int finalResult)
    {
        enabled = true;
        isDicingComplete = false;
        database.isHandling = true;
        database.logMessage.PrintPreviousMessages();
        database.logMessage.AddMessage("<Start Throwing Dice!>");
        database.logMessage.PrintLatestMessage();
        database.logMessage.AddMessage("...");
        database.logMessage.PrintLatestMessage();
        float waitTime = 0.4f;
        for (int i = 0; i < 20; i++)
        {
            sr.sprite = diceImages[i % 6];
            yield return new WaitForSeconds(0.07f + waitTime);
            waitTime = waitTime * 0.7f;
        }
        if (finalResult < 6)
        {
            sr.sprite = diceImages[finalResult];
        }
        else
        {
            textHolder.transform.rotation = transform.localRotation;
            textHolder.transform.position = transform.position;
            sr.sprite = diceImages[6];
            textHolder.text = finalResult.ToString();
        }
        database.logMessage.AddMessage("Value is " + (finalResult + 1) + "!");
        database.logMessage.PrintLatestMessage();
        enabled = false;
        database.logMessage.SetImage(1);
        instructionHolder = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
        instructionHolder.transform.SetParent(canvasTransform);
        instructionHolder.text = "[Z] to continue";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        Destroy(instructionHolder.gameObject);
        database.logMessage.DeleteLog();
        database.logMessage.Hide();
        database.isHandling = false;
        isDicingComplete = true;
        diceNumber = finalResult;
    }

    private IEnumerator DiceRandom(int min, int max)
    {
        enabled = true;
        isDicingComplete = false;
        database.isHandling = true;
        database.logMessage.PrintPreviousMessages();
        database.logMessage.AddMessage("<Start Throwing Dice!>");
        database.logMessage.PrintLatestMessage();
        database.logMessage.AddMessage("...");
        database.logMessage.PrintLatestMessage();
        int diceValue = Random.Range(min, max);
        float waitTime = 0.3f;
        for (int i = 0; i < 6; i++)
        {
            sr.sprite = diceImages[i % 6];
            yield return new WaitForSeconds(0.07f + waitTime);
            waitTime = waitTime * 0.7f;
        }
        if (diceValue < 6)
        {
            sr.sprite = diceImages[diceValue];
        }
        else
        {
            textHolder.transform.rotation = transform.localRotation;
            textHolder.transform.position = transform.position;
            sr.sprite = diceImages[6];
            textHolder.text = (diceValue + 1).ToString();
        }
        database.logMessage.AddMessage("Value is " + (diceValue + 1) + "!");
        database.logMessage.PrintLatestMessage();
        enabled = false;
        database.logMessage.SetImage(1);
        instructionHolder = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
        instructionHolder.transform.SetParent(canvasTransform);
        instructionHolder.text = "[Z] to continue";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        Destroy(instructionHolder.gameObject);
        database.logMessage.DeleteLog();
        database.logMessage.Hide();
        database.isHandling = false;
        isDicingComplete = true;
        diceNumber = diceValue + 1;
    }
}
