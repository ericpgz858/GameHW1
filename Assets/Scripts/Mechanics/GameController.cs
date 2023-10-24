using JetBrains.Annotations;
using UnityEngine.Events;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NPCController;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This class exposes the the game model in the inspector, and ticks the
    /// simulation.
    /// </summary> 
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }
        public GameObject player;
        public GameObject UI_start;
        public GameObject UI_end;
        public GameObject StartButton;
        public GameObject RestartButton;
        public TextMeshProUGUI EndText;
        public TextMeshProUGUI EndScoreText;
        public GameObject NPC;
        public GameObject MG_potion;
        public GameObject MG_monster;

        //This model field is public and can be therefore be modified in the 
        //inspector.
        //The reference actually comes from the InstanceRegister, and is shared
        //through the simulation and events. Unity will deserialize over this
        //shared reference when the scene loads, allowing the model to be
        //conveniently configured inside the inspector.
        public PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        private void Awake()
        {
            Physics2D.IgnoreLayerCollision(6, 6);
            UI_start.SetActive(true);
            StartButton.GetComponent<Button>().onClick.AddListener(GameStart);
            RestartButton.GetComponent<Button>().onClick.AddListener(ReStart);
        }

        void OnEnable()
        {
            Instance = this;
        }

        void OnDisable()
        {
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            if (Instance == this) Simulation.Tick();
        }
        public void GameStart()
        {
            player.GetComponent<PlayerController>().controlEnabled = true;
            UI_start.SetActive(false);
        }
        public void ReStart()
        {
            UI_end.SetActive(false);
            player.GetComponent<PlayerController>().controlEnabled = true;
            player.GetComponent<PlayerController>().hasFlower = false;
            NPC.GetComponent<NPCController>().index = 0;
            NPC.GetComponent<NPCController>().state = UIState.EnemyAlive;
            player.GetComponent<PlayerController>().Respawn(true);
            MG_potion.GetComponent<ManagePotion>().Restart();
            MG_monster.GetComponent<ManageMonster>().Restart();
        }
        public void GameOver(int finishscore = 0)
        {
            player.GetComponent<PlayerController>().controlEnabled = false;
            int score = (player.GetComponent<PlayerController>().currentHealth * 10) + (MG_monster.GetComponent<ManageMonster>().killMonster * 100) + finishscore;
            if (finishscore == 1000)
            {
                
                EndText.text = "Remain " + (player.GetComponent<PlayerController>().currentHealth / 10).ToString() + " Heart\n"
                    + "Kill " + MG_monster.GetComponent<ManageMonster>().killMonster.ToString() + " Monsters\n"
                    + "Mission Complete\n"
                    + "Total Score\n";
                EndScoreText.text = "--------> + " + (player.GetComponent<PlayerController>().currentHealth * 10).ToString() + "\n"
                    + "--------> + " + (MG_monster.GetComponent<ManageMonster>().killMonster * 100).ToString() + "\n"
                    + "--------> + " + finishscore.ToString() + "\n"
                    + "--------> " + score.ToString() + "\n";
            }
            else
            {
                EndText.text = "Remain " + (player.GetComponent<PlayerController>().currentHealth / 10).ToString() + " Heart\n"
                    + "Kill " + MG_monster.GetComponent<ManageMonster>().killMonster.ToString() + " Monsters\n"
                    + "Mission Fail\n"
                    + "Total Score\n";
                EndScoreText.text = "--------> + " + (player.GetComponent<PlayerController>().currentHealth * 10).ToString() + "\n"
                    + "--------> + " + (MG_monster.GetComponent<ManageMonster>().killMonster * 100).ToString() + "\n"
                    + "--------> + " + finishscore.ToString() + "\n"
                    + "--------> " + score.ToString() + "\n";
            }
            UI_end.SetActive(true);
        }
    }
}