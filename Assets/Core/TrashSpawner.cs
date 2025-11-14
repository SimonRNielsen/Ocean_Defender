using System.Collections;
using UnityEngine;

[System.Serializable]
public class TrashType
{
    public ObjectPool pool;
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 3f;
}

public class TrashSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool pool;
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    //[SerializeField] private float spawnDelay = 3f; //Bruges hvis man ønsker fast spawnrate
    [SerializeField] private Vector3 spawnArea = new Vector3(8, 4, 8);


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

            Vector3 randomPos = GetRandomPos();
            var obj = pool.GetPooledObject(randomPos);

            obj.transform.position = randomPos;
        }
    }

    public Vector3 GetRandomPos()
    {
        Vector3 randomPos = new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                /*Random.Range(*/-spawnArea.y/*, spawnArea.y)*/, 
                Random.Range(-spawnArea.z, spawnArea.z));
        return randomPos;
    }


    



}
