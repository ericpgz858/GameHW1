using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public GameObject dialoguePanel;
    public GameObject NPC_E;
    public Transform Player;
    public float FirstRange;
    [SerializeField] public TextMeshProUGUI dialogText;
    [SerializeField] public TextMeshProUGUI Continue;
    public string[] dialog;
    public GameObject enemy;
    public float typeSpeed;
    public bool playerIsClose = false;
    public int index;
    public UIState state = UIState.EnemyAlive;
    private Coroutine spawnCoroutine;
    public enum UIState
    {
        EnemyAlive,
        FirstNPC
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(Player.position, transform.position) > FirstRange)
        {
            dialoguePanel.SetActive(false);
        }
        else if(Vector2.Distance(Player.position, transform.position) <= FirstRange && state == UIState.EnemyAlive)
        {
            if(enemy.GetComponent<ArcherController>().inDead)
            {
                dialoguePanel.SetActive(false);
                dialogText.text = "";
                state = UIState.FirstNPC;
            }
            else
            {
                dialoguePanel.SetActive(true);
                dialogText.text = "Please help me !!!!";
            }
        }
        
        if(Input.GetKeyDown(KeyCode.E) && playerIsClose && state == UIState.FirstNPC) 
        {
            if(dialoguePanel.activeInHierarchy)
            {
                if (dialogText.text == dialog[index])
                {
                    NextLine();
                }
            }
            else
            {
                dialoguePanel.SetActive(true);
                spawnCoroutine = StartCoroutine(Typing());
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

        if(playerIsClose && !dialoguePanel.activeInHierarchy && state == UIState.FirstNPC)
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
            StopCoroutine(spawnCoroutine);
            dialogText.text = "";
            dialoguePanel.SetActive(false);
        }
    }
    IEnumerator Typing()
    {
        foreach(char letter in dialog[index].ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
    private void NextLine()
    {
        if(index < dialog.Length - 1)
        {
            index++;
            dialogText.text = "";
            spawnCoroutine = StartCoroutine(Typing());
        }
        else
        {
            dialoguePanel.SetActive(false);
            dialogText.text = "";
        }
    }
}
