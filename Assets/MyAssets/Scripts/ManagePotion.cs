using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagePotion : MonoBehaviour
{
    private GameObject child1, child2, child3;
    // Start is called before the first frame update
    void Start()
    {
        child1 = gameObject.transform.GetChild(0).gameObject;
        child2 = gameObject.transform.GetChild(1).gameObject;
        child3 = gameObject.transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Restart()
    {
        child1.SetActive(true);
        child2.SetActive(true);
        child3.SetActive(true);
    }
}
