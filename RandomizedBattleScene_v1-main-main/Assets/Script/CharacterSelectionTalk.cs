using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionTalk : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textHolder, instructionHolder;
    private CrossSceneManagement CSM;

    private void Awake()
    {
        CSM = GameObject.Find("CrossSceneManager").GetComponent<CrossSceneManagement>();
        textHolder.text = null;
        instructionHolder.text = null;
        StartCoroutine(Talk());
    }

    private IEnumerator Talk()
    {
        textHolder.text = "\"A new day arrives in the land of eternal darkness...\"";

        yield return new WaitForSeconds(0.5f);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        instructionHolder.text = null;
        textHolder.text = "\"Waves after waves, men with courage and ambition rush to the cursed soil.\"";

        yield return new WaitForSeconds(0.5f);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        instructionHolder.text = null;
        int deathTime = CSM.transform.GetChild(0).GetComponent<Database>().deathTime;
        if (deathTime == 0)
        {
            textHolder.text = "\"I haven't seen one in a while...\"";
        }
        else
        {
            textHolder.text = "\"There's few gone without news...\"";
        }

        yield return new WaitForSeconds(0.5f);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        instructionHolder.text = null;
        textHolder.text = "\"Good luck, adventurer.\"";

        yield return new WaitForSeconds(0.5f);
        instructionHolder.text = "[Z] to flip";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        CSM.LoadScene("BigMap");
    }

}
