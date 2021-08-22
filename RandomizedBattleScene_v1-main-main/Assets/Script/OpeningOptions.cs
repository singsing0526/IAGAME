using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningOptions : MonoBehaviour
{
    public SpriteRenderer[] sr;
    public AudioClip[] buttonsounds;
    private int selectionIndex = 0;
    private AudioSource audiosource;

    private void Start()
    {
        audiosource = GetComponent<AudioSource>();
        sr[1].color = new Color32(145, 15, 175, 255);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && selectionIndex - 1 >= 0)
        {
            audiosource.clip = buttonsounds[0];
            audiosource.Play();
            sr[selectionIndex].color = new Color32(145, 15, 175, 255);
            selectionIndex--;
            sr[selectionIndex].color = new Color32(255, 255, 255, 255);
        }
        if (Input.GetKeyDown(KeyCode.S) && selectionIndex + 1 < 2)
        {
            audiosource.clip = buttonsounds[0];
            audiosource.Play();
            sr[selectionIndex].color = new Color32(145, 15, 175, 255);
            selectionIndex++;
            sr[selectionIndex].color = new Color32(255, 255, 255, 255);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            audiosource.clip = buttonsounds[1];
            audiosource.Play();
            if (selectionIndex == 0)
            {
                SceneManager.LoadScene("SelectInitialCharacter");
            }
            else
            {
                Application.Quit();
            }
        }
    }
}
