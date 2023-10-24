using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageMonster : MonoBehaviour
{
    public int childNumber = 10;
    public int killMonster = 0;
    private GameObject[] child;
    // Start is called before the first frame update
    void Start()
    {
        GameObject originalGameObject = GameObject.Find("EnemySpawn");
        child = new GameObject[childNumber];
        for (int i = 0; i < childNumber; i++)
        {
            child[i] = originalGameObject.transform.GetChild(i).gameObject;
        }
    }
    public void Restart()
    {
        for (int i = 0; i < childNumber; i++)
        {
            var M_controller = child[i].GetComponent<MeleeController>();
            if(M_controller != null)
            {
                M_controller.respawn();
            }
            var A_controller = child[i].GetComponent<ArcherController>();
            if (A_controller != null)
            {
                A_controller.respawn();
            }
        }
        killMonster = 0;
    }
}
