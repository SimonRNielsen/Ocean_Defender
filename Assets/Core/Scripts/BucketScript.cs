using UnityEngine;

public class BucketScript : MonoBehaviour, IClickable
{
    [SerializeField, Tooltip("The gameobject which is goind to be instantiate ")]
    public GameObject eelgrassNail;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnPrimaryClick()
    {
        //Instantiate a new eelgrassNail gameobject
        eelgrassNail.transform.position = Vector3.zero;
        Instantiate(eelgrassNail);

    }

    public void OnPrimaryHold(Vector3 movement)
    {
        //It throw Exception if movement was set to Vector3.zero
        movement = Vector3.zero; 

    }

    public void OnPrimaryRelease()
    {
        //throw new System.NotImplementedException();
    }

}
