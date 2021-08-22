using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopText : MonoBehaviour
{
    private TMPro.TextMeshProUGUI textHolder;
    private string text;
    private float waitTime;

    public void SetText(Vector2 position, string text, float waitTime)
    {
        textHolder = GetComponent<TMPro.TextMeshProUGUI>();
        textHolder.text = null;
        this.text = text;
        textHolder.fontSize = 2;
        textHolder.color = Color.red;
        transform.position = position + new Vector2(Random.Range(-1.2f, 1.2f), Random.Range(0, 2));
        transform.SetParent(GameObject.Find("Canvas").transform);
        this.waitTime = waitTime;
    }

    public void SetFloat()
    {
        StartCoroutine(FloatingText());
    }

    private IEnumerator FloatingText()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.25f));
        StartCoroutine(WaitDestroy(waitTime));
        textHolder.text = text;
        for (int i = 0; i < 25; i++)
        {
            transform.position = (Vector2)transform.position + new Vector2(0, 3) * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
            textHolder.fontSize -= 0.06f;
        }
    }

    private IEnumerator WaitDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
