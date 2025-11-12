using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool pool;
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    //[SerializeField] private float spawnDelay = 3f; //Bruges hvis man ønsker fast spawnrate
    [SerializeField] private Vector3 spawnArea = new Vector3(8, 4, 8); //Passer cirka til hele scenen
    [SerializeField] private bool spawnRandomTrash = true;
    [SerializeField] private List<TrashType> prefabTypes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
               
            if (spawnRandomTrash)
            {

                //Random spawntime
                yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

                //Fast spawntime
                //yield return new WaitForSeconds(spawnDelay);


                var obj = pool.GetPooledObject();

                if (obj != null)
                {
                    obj.transform.position = GetRandomSpawnPos();
                }
            }
            else
            {
                foreach (var type in prefabTypes)
                {
                    yield return new WaitForSeconds(type.spawnInterval);

                    var obj = type.pool.GetPooledObject();

                    if (obj != null)
                    {
                       
                        obj.transform.position = GetRandomSpawnPos();
                    }
                }
            }

        }
    }

    public Vector3 GetRandomSpawnPos()
    {
        return new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y),
            Random.Range(-spawnArea.z, spawnArea.z));
    }

    ////Implementeres hvis vi skal bruge max capacitet
    //private void OnEnable()
    //{
    //    pool.OnObjectReturned += RestartSpawn;
    //}

    //private void OnDisable()
    //{
    //    pool.OnObjectReturned -= RestartSpawn;
    //}

    //private void RestartSpawn()
    //{
    //    StartCoroutine (SpawnLoop());
    //}

}

[System.Serializable]
public class TrashType
{
    public ObjectPool pool;
    public float spawnInterval;
}
