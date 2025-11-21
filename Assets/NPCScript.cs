using UnityEngine;
using TMPro;


public class NPCScript : MonoBehaviour
{

    private int currentChar = 0; //index of the current char to print out
    private int charSpeed = 3; //higher is slower text, every charSpeed frames, the text advances one char.
    private int charCount = 0; //count of number of frames since lsat characer output. Once charCount reaches charSpeed, it is reset and new character is printed.
    private string outputStr = "";

    private GameObject chatObject;
    private GameObject playerObject;
    private Transform playerEyesTransform;
    

    private TMP_Text component;
    private string dialogueLine = "";

    private AudioSource aud;

    public AudioClip[] voiceLines;

    public bool readyNextLine = false;


    private Animator anim;
    private bool hasWalkedToCounter = false;
    private bool isWalking = false;

    [SerializeField]
    private Collider counterStopCollider;

    //if he is at the end yet
    public bool hackyEndFix = false;

    private Vector3 originalPos = Vector3.zero;
    private Quaternion originalRot = Quaternion.identity;

    private void Awake()
    {

        aud = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPos = transform.position; 
        originalRot = transform.rotation;

        component = this.transform.Find("NPCText/Canvas/WordDisplay/WordLabel/LabelText").GetComponent<TMP_Text>();
        chatObject = this.transform.Find("NPCText").gameObject;
        playerObject = GameObject.Find("OVRCameraRig");
        playerEyesTransform = playerObject.transform.Find("TrackingSpace/CenterEyeAnchor");

      

        
    }

    public void playVoice(int index)
    {
        aud.clip = voiceLines[index];
        aud.Play();
    }

    // Update is called once per frame
    void Update()
    {

        //make text face
        if (chatObject.activeSelf)
        {
            chatObject.transform.LookAt(playerEyesTransform);
        }

        if (hackyEndFix)
        {

            
        }
        else
        {
            //advance text
            if (charCount >= charSpeed)
            {
                if (currentChar < dialogueLine.Length)
                {
                    charCount = 0;
                    outputStr += dialogueLine[currentChar];
                    component.text = outputStr;
                    currentChar++;
                }
                else// else stop printing
                {
                    readyNextLine = true;
                }
            }
            else
            {
                charCount++;
            }


            if (isWalking)
            {
                transform.position = transform.position + (transform.forward * 0.02f);

                if (!hasWalkedToCounter)
                {
                    if (transform.position.x > 5.6f)
                    {
                        stopWalking();
                        hasWalkedToCounter = true;
                    }
                }

            }
        }
    }

    public void sayLine(string line)
    {
        readyNextLine = false;
        showChat();
        dialogueLine = line;
        currentChar = 0;
        charCount = 0; //can delete not neessary to rseet charcount
        outputStr = "";
    }

    public void showChat()
    {
        chatObject.SetActive(true);
    }
    public void hideChat()
    {
        chatObject.SetActive(false);
    }

    public void startWalking()
    {
        anim.SetTrigger("DoWalking");
        isWalking = true;
    }

    public void stopWalking()
    {
        anim.SetTrigger("StopWalking");
        isWalking = false;
    }

    public void resetNPC()
    {
        hasWalkedToCounter = false;
    }

    public void sayHappy()
    {
        component.text = "☺☺☺☺☺☺☺☺☺☺☺☺☺☺";
    }

    public void sayAngry()
    {
        component.text = "☹☹☹☹☹☹☹☹☹☹☹☹☹☹☹☹";
    }

    /*
    void OnTriggerEnter(Collider other)
    {
        if (other.name == counterStopCollider.name)
        {
            Debug.LogError("collided with stop, stopping");
            anim.SetTrigger("StopWalking");
            isWalking = false;
        }
    }*/

    public void restart()
    {
        transform.position = originalPos;
        transform.rotation = originalRot;

        readyNextLine = false;


        hasWalkedToCounter = false;
        isWalking = false;

 

        //if he is at the end yet
        hackyEndFix = false;

        currentChar = 0; //index of the current char to print out
        charSpeed = 3; //higher is slower text, every charSpeed frames, the text advances one char.
        charCount = 0; //count of number of frames since lsat characer output. Once charCount reaches charSpeed, it is reset and new character is printed.
        outputStr = "";
        component.text = "";
    }

}
