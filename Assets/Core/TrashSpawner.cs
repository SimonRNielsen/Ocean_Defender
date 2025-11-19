using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Én spawner, tager flere pools med hver deres skraldtype og spawninterval
/// </summary>
public class TrashSpawner : MonoBehaviour
{
    [System.Serializable]
    public class TrashType
    {
        public ObjectPool pool;
        public float minDelay = 1f;
        public float maxDelay = 3f;
    }

    [Header("Spawn settings")]
    [SerializeField] private List<TrashType>trashTypes = new List<TrashType>();
    [SerializeField] private Vector3 spawnArea = new Vector3(8, 10, 8); //spawner trash inden for et bestemt område 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Starter et spawn-loop for hver type skrald i listen.
        // Det betyder: hvert pool-type spawner uafhængigt af hinanden.
        foreach (var type in trashTypes)
        {
            StartCoroutine(SpawnLoop(type));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    
    private IEnumerator SpawnLoop(TrashType type)
    {
        while (true)
        {
            //Venter et random spawninterval
            yield return new WaitForSeconds(Random.Range(type.minDelay, type.maxDelay));

            Vector3 randomPos = GetRandomPos();

            //Forsøger at få fat i skrald fra poolen
            var obj = type.pool.GetPooledObject(randomPos);

            // Hvis obj == null betyder det:
            // poolen har nået maxActiveTrash, derfor kan der ikke spawnes mere
            if (obj == null)
                continue; // spring over og prøv igen senere

            obj.transform.position = randomPos;
        }
    }

    /// <summary>
    /// Returnerer spawn område
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRandomPos()
    {
        return new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                /*Random.Range(-spawnArea.y+2,*/ (spawnArea.y), //+2 så skraldet ikke spawner i vores "menu"/nederste del af skærmen
                Random.Range(-spawnArea.z, spawnArea.z));
        
    }

     

}
