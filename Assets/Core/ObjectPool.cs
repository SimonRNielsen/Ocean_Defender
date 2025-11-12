using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Fields
    [SerializeField] private int trashPoolSize = 10;
    [SerializeField] private ClearableScript objectToPool;
    [SerializeField] private int trashCounter = 0; //Aflæses per pool/trash
    private Stack<ClearableScript> stack;
    //private int currentActiveTrash = 0; //implementeres ved max capacitet

    public event System.Action OnObjectReturned;

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
            instance.gameObject.SetActive(false);
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
        //currentActiveTrash++; //implementeres ved max capacitet
        return instance;
    }

    public void ReturnToPool(ClearableScript pooledObject)
    {
        trashCounter++; //Aflæses per pool/trash
        stack.Push(pooledObject);
        pooledObject.gameObject.SetActive(false);
        //currentActiveTrash--; //implementeres ved max capacitet

        ////implementeres hvis vi skal bruge max capacitet
        //Starter spawn igen, hvis den har været stoppet pga. max capacitet
        //OnObjectReturned?.Invoke();
    }
}


#endregion
