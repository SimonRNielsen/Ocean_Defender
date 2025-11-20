using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Fields
    [Header("Pool settings")]
    [SerializeField] private ClearableScript objectToPool;
    [SerializeField] private int trashPoolSize = 20;
    [SerializeField] private int MaxActiveTrash = 10;
    [SerializeField] private int trashedScore = 0;
    [SerializeField] private int activeCount = 0;

    private Stack<ClearableScript> inActiveStack;

    //Tæller aktive skralde objekter i scenen
    public int Activecount => activeCount;

    #endregion
    #region Methods
    private void Awake()
    {
        inActiveStack = new Stack<ClearableScript>();

        //instantier trashpoolsize objecter og læg dem i pool
        for (int i = 0; i < trashPoolSize; i++)
        {
            ClearableScript obj = Instantiate(objectToPool);
            obj.gameObject.SetActive(false); //sørger for at objecterne ikke kan ses før de spawner
            obj.Pool = this;
            inActiveStack.Push(obj); //lægger objektet i inactive stack
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Henter et object fra poolen og aktiverer det
    /// Hvis MaxActive er nået, returnerer det null
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public ClearableScript GetPooledObject(Vector3 position)
    {
        //Stopper før der kommer for mange aktive objekter i scenen
        if (activeCount >= MaxActiveTrash)
            return null;

        ClearableScript clearableObject;

        //if the pool is not large enough, instantiate a new PooledObject
        if (inActiveStack.Count > 0)
        {
            clearableObject = inActiveStack.Pop();
        }
        else
        {
            //Instantier nyt object, hvis der ikke er flere i stakken
            clearableObject = Instantiate(objectToPool);
            clearableObject.Pool = this;
        }

        //Flytter til random pos og aktiverer objekt
        clearableObject.transform.position = position;
        clearableObject.gameObject.SetActive(true);

        activeCount++;
        return clearableObject;
    }

    /// <summary>
    /// Returnerer objekt til pool og gør inaktivt
    /// </summary>
    /// <param name="pooledObject"></param>
    public void ReturnToPool(ClearableScript pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
        inActiveStack.Push(pooledObject);

        activeCount--;
       // trashedScore += pooledObject.Score;
    }
}


#endregion
