using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
    [SerializeField] private Vector3 spawnArea = new Vector3(8, 4, 8);
    //[SerializeField] private ObjectPool pool;
    //[SerializeField] private float minSpawnDelay = 1f;
    //[SerializeField] private float maxSpawnDelay = 3f;
    //[SerializeField] private float spawnDelay = 3f; //Bruges hvis man ønsker fast spawnrate


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            //Random spawntime
            yield return new WaitForSeconds(Random.Range(type.minDelay, type.maxDelay));

            //Fast spawntime
            //yield return new WaitForSeconds(spawnDelay);

            Vector3 randomPos = GetRandomPos();
            var obj = type.pool.GetPooledObject(randomPos);

            obj.transform.position = randomPos;
        }
    }

    public Vector3 GetRandomPos()
    {
        return new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y+2, spawnArea.y), 
                Random.Range(-spawnArea.z, spawnArea.z));
        
    }

     

}
