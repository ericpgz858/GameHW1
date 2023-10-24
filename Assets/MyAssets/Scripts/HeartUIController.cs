using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor.Rendering;

public class HeartUIController : MonoBehaviour
{
    // Start is called before the first frame update
    private Image[] images;
    void Start()
    {
        images = GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpHeart(int currentHealth)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if(currentHealth/10 >= (i + 1))
            {
                images[i].gameObject.SetActive(true);
            }
            else
            {
                images[i].gameObject.SetActive(false);
            }
        }
    }
}
