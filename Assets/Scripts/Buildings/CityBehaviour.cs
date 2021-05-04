using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject explosionAnim;

    // Start is called before the first frame update
    void Start()
    {
        CometBehaviour.OnCityHit += SelfDestruct;
    }

    private void OnDestroy()
    {
        CometBehaviour.OnCityHit -= SelfDestruct;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelfDestruct(GameObject go)
    {
        if (go == this.gameObject)
        {
            GameObject exp = Instantiate(explosionAnim, transform.position, Quaternion.identity);
            exp.transform.localScale += new Vector3(2, 2, 0);
            Destroy(gameObject);
        }
    }
}
