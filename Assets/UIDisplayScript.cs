using Oculus.Interaction;
using UnityEngine;

public class UIDisplayScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private InteractorActiveState activeState;
    void Start()
    {
        this.activeState = this.GetComponent<InteractorActiveState>();
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.SetActive(activeState.Active);
    }
}
