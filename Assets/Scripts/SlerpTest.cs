using UnityEngine;

class SlerpTest : MonoBehaviour
{
    public float speed;
    private Vector3 destination;
    public Vector3 moveTo;

    public void Start()
    {
        destination = transform.position;
    }

    public void Update()
    {
        if(Input.anyKeyDown)
        {
            destination = moveTo;
        }
        transform.position = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);
    }
}

