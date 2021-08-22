using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolySword : MonoBehaviour
{
    private Database database;
    private TMPro.TextMeshProUGUI instructionHolder, talk;
    private Transform canvasTransform, background;
    public Sprite[] options;
    public SpriteRenderer[] twoOptionsHolder = new SpriteRenderer[2];
    public GameObject spriteHolder;

    private bool allowChoosingOptions = false, isBreaking = false;
    private int selectionIndex = 0;
    private SpriteRenderer selfSR;

    private void Awake()
    {
        database = GameObject.Find("Database").GetComponent<Database>();
        canvasTransform = GameObject.Find("Canvas").transform;
        background = GameObject.Find("EndingBackground").transform; ;

        instructionHolder = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
        instructionHolder.text = null;
        instructionHolder.transform.SetParent(canvasTransform);

        talk = Instantiate(database.instruction).GetComponent<TMPro.TextMeshProUGUI>();
        talk.text = null;
        talk.transform.SetParent(canvasTransform);
        talk.alignment = TMPro.TextAlignmentOptions.Midline;
        talk.fontSize = 2;
        talk.transform.position = new Vector2(0, 5.7f);
        selfSR = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(DecideEnding());
    }

    void Update()
    {
        if (isBreaking == false)
        {
            transform.position = new Vector2(0, Mathf.Sin(Time.time * 5) * 0.8f);
            if (allowChoosingOptions == true)
            {
                if (Input.GetKeyDown(KeyCode.A) && selectionIndex - 1 >= 0)
                {
                    twoOptionsHolder[selectionIndex].color = new Color32(170, 70, 200, 255);
                    selectionIndex--;
                    twoOptionsHolder[selectionIndex].color = new Color32(255, 255, 255, 255);
                }
                if (Input.GetKeyDown(KeyCode.D) && selectionIndex + 1 < 2)
                {
                    twoOptionsHolder[selectionIndex].color = new Color32(170, 70, 200, 255);
                    selectionIndex++;
                    twoOptionsHolder[selectionIndex].color = new Color32(255, 255, 255, 255);
                    if (selectionIndex == 1)
                    {
                        twoOptionsHolder[1].sprite = options[2];
                    }
                }
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    instructionHolder.text = null;
                    if (selectionIndex == 0) // Break it
                    {
                        twoOptionsHolder[1].sprite = null;
                        StartCoroutine(BreakIt());
                    }
                    else // Take it
                    {
                        twoOptionsHolder[0].sprite = null;
                        StartCoroutine(TakeIt());
                    }
                    allowChoosingOptions = false;
                }
            }
        }
        else
        {
            transform.position = new Vector2(Mathf.Cos(Time.time * 50) * 1.2f, Mathf.Sin(Time.time * 50) * 1.2f);
        }
    }

    private IEnumerator DecideEnding()
    {
        instructionHolder.text = null;
        talk.text = "\"You have came so far, human...\"";
        yield return new WaitForSeconds(1);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        instructionHolder.text = null;
        talk.text = "\"You should know what you have to do.\"";
        yield return new WaitForSeconds(1);
        instructionHolder.text = "[W][S] to select, [Z] to comfirm";

        SpriteRenderer tempSR = Instantiate(spriteHolder).GetComponent<SpriteRenderer>();
        tempSR.sprite = options[0];
        tempSR.transform.position = new Vector2(-6, 0);
        twoOptionsHolder[0] = tempSR;

        tempSR = Instantiate(spriteHolder).GetComponent<SpriteRenderer>();
        tempSR.sprite = options[1];
        tempSR.transform.position = new Vector2(6, 0);
        twoOptionsHolder[1] = tempSR;
        tempSR.color = new Color32(170, 70, 200, 255);
        allowChoosingOptions = true;
    }

    private IEnumerator BreakIt()
    {
        talk.text = "...";
        yield return new WaitForSeconds(1);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        instructionHolder.text = null;
        talk.text = "\"Only the strongest will can deter the temptation, human.\"";
        yield return new WaitForSeconds(1);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        instructionHolder.text = null;
        talk.text = "\"The curse is unsealed.\nBut the aftermath can not be undone.\"";
        yield return new WaitForSeconds(1);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        twoOptionsHolder[0].sprite = null;
        StartCoroutine(ResizeBackground(false));
        instructionHolder.text = null;
        talk.text = "\"May the new kingdom shines its glory once more.\"";
        yield return new WaitForSeconds(1);
        instructionHolder.text = "[Z] to break";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        instructionHolder.text = null;
        talk.text = null;
        isBreaking = true;
        yield return new WaitForSeconds(4);
        Camera.main.backgroundColor = new Color32(0, 0, 0, 255);
        Destroy(background.gameObject);
        selfSR.sprite = null;

        yield return new WaitForSeconds(3);
        database.transform.parent.GetComponent<CrossSceneManagement>().LoadScene("credit scene");
    }

    private IEnumerator TakeIt()
    {
        talk.text = "...";
        yield return new WaitForSeconds(1);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        instructionHolder.text = null;
        talk.text = "\"As a reward for taking down the devil king, \n the sword is yours.\"";

        yield return new WaitForSeconds(1);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        twoOptionsHolder[1].sprite = null;
        instructionHolder.text = null;
        talk.text = "\"Although you have gained the power,\nthe curse still remains.\"";

        yield return new WaitForSeconds(1);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        instructionHolder.text = null;
        talk.text = "\"The Doomdays won't end anytime soon...\"";

        yield return new WaitForSeconds(1);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        instructionHolder.text = null;
        talk.text = null;
        isBreaking = true;
        yield return new WaitForSeconds(4);

        Camera.main.backgroundColor = new Color32(0, 0, 0, 255);
        Destroy(background.gameObject);
        gameObject.SetActive(false);
        selfSR.sprite = null;

        yield return new WaitForSeconds(3);
        database.transform.parent.GetComponent<CrossSceneManagement>().LoadScene("credit scene");
    }

    private IEnumerator ResizeBackground(bool isZoom)
    {
        for(int i = 0; i < 15; i++)
        {
            if (isZoom == true)
            {
                background.localScale = (Vector2)background.localScale - new Vector2(0.3f, 0.3f);
            }
            else
            {
                background.localScale = (Vector2)background.localScale + new Vector2(0.3f, 0.3f);

            }
            yield return new WaitForSeconds(0.08f);
        }
    }
}
