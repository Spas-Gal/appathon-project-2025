using UnityEngine;

public class ObjectReceiver : MonoBehaviour
{

    [SerializeField]
    private GameController gameController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter(Collider collider)
    {
        Debug.LogError("collision with obj" + collider.tag);
        gameController.handleObject(collider.gameObject);
    }
}
