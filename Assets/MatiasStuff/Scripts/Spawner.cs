using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] _enemies;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Spawn()
    {
        int randomInt = Random.Range(0, _enemies.Length);

        GameObject some = Instantiate(_enemies[randomInt]);
        some.transform.position = this.transform.position;

        some.AddComponent<MovingObject>();
    }
}
