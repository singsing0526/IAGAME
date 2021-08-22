using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelection : MonoBehaviour
{
    public BattleMenu battleMenu;
    private int previousIndex = 0;
    private SpriteRenderer sr;
    private Sprite currentSprite;
    public Database database;
    public Sprite targetIcon, lightIcon;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (database == null)
        {
            sr.sprite = targetIcon;
            transform.position = GetPositionFromBattleMenu(battleMenu.isTargetAlly, battleMenu.currentTarget);
        }
        else
        {
            sr.sprite = lightIcon;
            transform.localScale = new Vector3(1, 0.8f, 1);
            transform.position = new Vector2(2, 0.75f);
            sr.color = new Color32(255, 255, 255, 120);
        }

        currentSprite = sr.sprite;
    }

    private void LateUpdate()
    {
        if (battleMenu != null)
        {
            transform.Rotate(new Vector3(0, 0, -10 * Time.deltaTime));
            if (previousIndex != battleMenu.currentTarget)
            {
                transform.position = GetPositionFromBattleMenu(battleMenu.isTargetAlly, battleMenu.currentTarget);
            }
        }
        else
        {
            if (database.isHandling == false)
            {
                if (previousIndex != database.beatCharacterSelectionIndex)
                {
                    transform.position = GetPositionFromDatabase(database.beatCharacterSelectionIndex);
                }
            }
        }
    }

    public Vector2 GetPositionFromBattleMenu(bool isAlly, int index)
    {
        previousIndex = index;
        if (isAlly)
        {
            return new Vector2(-2 + index * -2.5f, -2);
        }
        else
        {
            return new Vector2(2 + index * 2.5f, -2);
        }
    }

    public Vector2 GetPositionFromDatabase (int index)
    {
        previousIndex = index;
        return new Vector2(2 + index * 2.5f, 0.75f);
    }

    public void Hide()
    {
        sr.sprite = null;
        enabled = false;
    }

    public void Show()
    {
        sr.sprite = currentSprite;
        enabled = true;
    }
}
