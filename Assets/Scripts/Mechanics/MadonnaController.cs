using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MadonnaController : MonoBehaviour
{
    public GameObject Game_Cont;
    public GameObject dialoguePanel;
    public GameObject NPC_E;
    public GameObject player;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI Continue;
    public string[] dialog;
    public float typeSpeed;
    public bool playerIsClose = false;
    private int index;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose)
        {
            if (dialoguePanel.activeInHierarchy)
            {
                dialoguePanel.SetActive(false);
                dialogText.text = "";
                if(index == 1)
                {
                    Game_Cont.GetComponent<GameController>().GameOver(1000);
                }
            }
            else
            {
                if (!player.GetComponent<PlayerController>().hasFlower)
                    index = 0;
                else
                    index = 1;
                dialogText.text = "";
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
            }
        }
        if (dialogText.text == dialog[index])
        {
            Continue.enabled = true;
        }
        else
        {
            Continue.enabled = false;
        }
        Continue.enabled = true;
        if (playerIsClose && !dialoguePanel.activeInHierarchy)
        {
            NPC_E.SetActive(true);
        }
        else
        {
            NPC_E.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            dialoguePanel.SetActive(false);
        }
    }
    IEnumerator Typing()
    {
        foreach (char letter in dialog[index].ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
}
