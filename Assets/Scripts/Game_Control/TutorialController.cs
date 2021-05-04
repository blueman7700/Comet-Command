using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TutorialController : MonoBehaviour
{

    int imageIndex = 0;
    [SerializeField] GameObject image1;
    [SerializeField] GameObject image2;
    [SerializeField] GameObject image3;
    [SerializeField] Fade fader;
    [SerializeField] Animator anim;

    // Start is called before the first frame update
    void Start()
    {

        //show welcome message
        //show controls message
        //show ui description panel
        //spawn comet
        //destroy comet
        //spawn comet
        //get hit
        //show end screen
        //explain upgrades
        //exit

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Progress()
    {
        if (imageIndex == 0)
        {
            imageIndex++;
            image2.SetActive(true);
        }
        else if (imageIndex == 1)
        {
            imageIndex++;
            image3.SetActive(true);
        }
        else if (imageIndex == 2)
        {
            imageIndex++;
            fader.loadScene("MainMenu");
        }
    }
}
