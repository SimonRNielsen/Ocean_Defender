using System.Collections;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool pool;
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    //[SerializeField] private float spawnDelay = 3f; //Bruges hvis man ønsker fast spawnrate
    [SerializeField] private Vector3 spawnArea = new Vector3(10, 0, 10);


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
            //Random spawntime
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

            //Fast spawntime
            //yield return new WaitForSeconds(spawnDelay);

            Vector3 randomPos = new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y, spawnArea.y),
                Random.Range(-spawnArea.z, spawnArea.z));

            var obj = pool.GetPooledObject();

            obj.transform.position = randomPos;
            obj.transform.rotation = Quaternion.identity;
        }
    }



    



}
