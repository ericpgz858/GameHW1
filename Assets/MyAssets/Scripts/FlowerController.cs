using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlowerController : MonoBehaviour
{
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
                if (dialogText.text == dialog[index])
                {
                    NextLine();
                }
            }
            else
            {
                player.GetComponent<PlayerController>().hasFlower = true;
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
    private void NextLine()
    {
        if (index < dialog.Length - 1)
        {
            index++;
            dialogText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            dialoguePanel.SetActive(false);
            dialogText.text = "";
        }
    }
}
