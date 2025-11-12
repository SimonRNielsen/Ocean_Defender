using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Fields
    [SerializeField] private int trashPoolSize = 20;
    [SerializeField] private int MaxActiveTrash = 10;
    [SerializeField] private ClearableScript objectToPool;
    private Stack<ClearableScript> stack;
    private int currentActiveTrash = 0;

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
            stack.Push(instance);
        }
    }

    //return the first active GameObject from the pool
    public ClearableScript GetPooledObject()
    {
        ClearableScript instance;

        //if the pool is not large enough, instantiate a new PooledObject
        if (stack.Count == 0)
        {
            instance = Instantiate(objectToPool);
            instance.Pool = this;
            //ClearableScript newInstance = Instantiate(objectToPool);
            //newInstance.Pool = this;
            ////return newInstance;
        }
        else
        {
            instance = stack.Pop();
        }
        ////Otherwise, grab the next one from the list
        //ClearableScript nextInstance = stack.Pop();
        //nextInstance.gameObject.SetActive(true);
        //return nextInstance;

        
        instance.gameObject.SetActive(true);
        return instance;
    }

    public void ReturnToPool(ClearableScript pooledObject)
    {
        stack.Push(pooledObject);
        pooledObject.gameObject.SetActive(false);
    }
}


#endregion
