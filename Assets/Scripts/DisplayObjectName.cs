using System.Collections.Specialized;
using UnityEngine;
using TMPro;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections.Generic;

public class DisplayObjectName : MonoBehaviour
{
    //the translated name to display of the object
    [SerializeField]
    private string name = "no_object_name_assigned!";

    [SerializeField]
    private GameObject cameraRig;

    //the prefab for the menu that displays the object's name
    public GameObject objectNameDisplayPrefab;

    private GameObject nameDisplay;

    private InteractorActiveState activeState;

    private Transform cameraEyesTransform;

    //grab interactable
    private HandGrabInteractable handGrabInteractable;

    private bool isGrabbed = false;

    [SerializeField]
    private List<string> properties;

    private Vector3 originalPos;
    private Quaternion originalRot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {




        originalPos = transform.position;
        originalRot = transform.rotation;
        
      



        cameraEyesTransform = GameObject.Find("/OVRCameraRig/TrackingSpace/CenterEyeAnchor").transform; 

        activeState = this.GetComponent<InteractorActiveState>(); //uselesss maybe delete later

        

    
    }

    // Update is called once per frame
    void Update()
    {

        int count = 0;

        if (handGrabInteractable)
        {
            count = handGrabInteractable.Interactors.Count;
        }
        else
        {
            //Debug.LogError("no handgrabinteractable");
        }

       // Debug.LogError(count);

        isGrabbed = (count > 0);

        
        nameDisplay.SetActive(isGrabbed);
       
       

        Vector3 offset = new Vector3(0, 0.22f, 0);

        //!!!
 
        if (isGrabbed)
        {
         
            
            nameDisplay.transform.position = this.transform.position + offset;
            nameDisplay.transform.LookAt(cameraEyesTransform); //cameraRig.transform);
        }

        //nameDisplay.SetActive(activeState.Active);

     
    }

    void OnEnable()
    {
        handGrabInteractable = this.transform.Find("ISDK_HandGrabInteraction").gameObject.GetComponent<HandGrabInteractable>();

        if (handGrabInteractable == null)
        {
            Debug.LogError("could not create grabinteractable");
        }
        else
        {
           // Debug.LogError(handGrabInteractable.gameObject.transform.position);
        }

        //Note: instantiating all objects is bad for efficiency, maybe only do it when interacted with?
        nameDisplay = Instantiate(objectNameDisplayPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        GameObject canvasGameObject = nameDisplay.transform.GetChild(1).gameObject;

        GameObject wordDisplayGameObject = canvasGameObject.transform.GetChild(0).gameObject;

        GameObject wordLabelGameObject = wordDisplayGameObject.transform.GetChild(0).gameObject;

        GameObject labelTextGameObject = wordLabelGameObject.transform.GetChild(0).gameObject;

        TMP_Text text_component = labelTextGameObject.GetComponent<TMP_Text>();

        text_component.text = name;



    }


    //restrore the object to its original position
    public Vector3 originalPosition()
    {
        return originalPos;
    }

    public Quaternion originalRotation()
    {
        return originalRot;
    }


    public void disableText()
    {
        nameDisplay.SetActive(false);
    }

}
