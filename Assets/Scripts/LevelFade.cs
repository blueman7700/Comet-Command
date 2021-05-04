using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFade : MonoBehaviour
{
    [SerializeField] public Animator transitions;

    public IEnumerator DoFadeOut()
    {
        transitions.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
    }
}
