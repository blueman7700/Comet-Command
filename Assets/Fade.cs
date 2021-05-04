using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    [SerializeField] public Animator transitions;

    public void loadScene(string newScene)
    {
        StartCoroutine(DoFadeOut(newScene));
    }

    public IEnumerator DoFadeOut(string sceneToLoad)
    {
        transitions.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneToLoad);
    }
}