using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Fields
    [Header("Pool settings")]
    [SerializeField] private ClearableScript objectToPool;
    [SerializeField] private int trashPoolSize = 20;
    [SerializeField] private int MaxActiveTrash = 10;
    //[SerializeField] private int trashedScore = 0;

    //private Stack<ClearableScript> stack;
    private Stack<ClearableScript> inActiveStack;
    private int activeCount = 0;
    private int Activecount => activeCount;
    //private int currentActiveTrash = 0;

    #endregion
    #region Methods
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupPool();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Creates the pool
    private void SetupPool()
    {
        stack = new Stack<ClearableScript>();
        ClearableScript instance = null;

        for (int i = 0; i < trashPoolSize; i++)
        {
            instance = Instantiate(objectToPool);
            instance.Pool = this;
            instance.gameObject.SetActive(false);
            stack.Push(instance);
        }
    }

    //return the first active GameObject from the pool
    public ClearableScript GetPooledObject(Vector3 position)
    {

        //if (currentActiveTrash >= MaxActiveTrash)
        //    return null;

        ClearableScript clearableObject;

        //if the pool is not large enough, instantiate a new PooledObject
        if (stack.Count == 0)
        {
            clearableObject = Instantiate(objectToPool);
            clearableObject.Pool = this;
            //ClearableScript newInstance = Instantiate(objectToPool);
            //newInstance.Pool = this;
            ////return newInstance;
        }
        else
        {
            clearableObject = stack.Pop();
        }
        ////Otherwise, grab the next one from the list
        //ClearableScript nextInstance = stack.Pop();
        //nextInstance.gameObject.SetActive(true);
        //return nextInstance;

        clearableObject.transform.position = position;
        clearableObject.gameObject.SetActive(true);
        return clearableObject;
    }

    public void ReturnToPool(ClearableScript pooledObject)
    {
        stack.Push(pooledObject);
        pooledObject.gameObject.SetActive(false);
        trashedScore++;
    }
}


#endregion
