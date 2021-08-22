using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditSceneController : MonoBehaviour
{
    private CrossSceneManagement CSM;
    private float animationLength = 0;
    private void Awake()
    {
        CSM = GameObject.Find("CrossSceneManager").GetComponent<CrossSceneManagement>();
        animationLength = GameObject.Find("GameObject").GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(WaitDestroy());
    }

    private IEnumerator WaitDestroy()
    {
        yield return new WaitForSeconds(animationLength);
        CSM.LoadScene("Opening");
        Destroy(gameObject);
    }
}
