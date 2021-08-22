using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDoorController : MonoBehaviour
{
    public ShopManager SM;

    public void OpenDoor(int page)
    {
        SM.database.AddSound(11, false, 1);
        StartCoroutine(OpenDoorAnimation(page));
    }

    public void CloseDoor()
    {
        SM.database.AddSound(12, false, 1);
        StartCoroutine(CloseDoorAnimation());
    }

    private IEnumerator OpenDoorAnimation(int shopPage)
    {
        SM.isShopOpened = false;
        SM.coinText.text = null;
        SM.instructionHolder.text = null;
        transform.GetChild(0).position = new Vector2(-5.76f, 0);
        transform.GetChild(1).position = new Vector2(5.72f, 0);

        for (int i = 0; i < 11; i++)
        {
            transform.GetChild(0).position = (Vector2)transform.GetChild(0).position + new Vector2(-1, 0);
            transform.GetChild(1).position = (Vector2)transform.GetChild(1).position + new Vector2(1, 0);
            yield return new WaitForSeconds(0.08f);
        }
        if (shopPage != -1)
            SM.SetUpShop(shopPage);
        SM.coinText.text = "$" + SM.database.coin.ToString();
        SM.instructionHolder.text = "[W][S] to select, [Z] to buy, [A][D] to flip, [X] to exit";
        SM.isShopOpened = true;
    }

    private IEnumerator CloseDoorAnimation()
    {
        SM.isShopOpened = false;
        for (int i = 0; i < 11; i++)
        {
            transform.GetChild(0).position = (Vector2)transform.GetChild(0).position + new Vector2(1, 0);
            transform.GetChild(1).position = (Vector2)transform.GetChild(1).position + new Vector2(-1, 0);
            yield return new WaitForSeconds(0.08f);
        }

        SM.database.transform.parent.GetComponent<CrossSceneManagement>().LoadScene("BigMap");
    }
}
