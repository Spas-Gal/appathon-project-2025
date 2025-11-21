using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{

    // currently is serializeField maybe change since NPCs are instantiated since different?
    [SerializeField]
    private NPCScript npc;

    private GameObject npcObject;

    private float startTime;

    private int currentPrompt = 0;

    private bool firstLineSaid = false;

    GameObject lastObj = null;

    Vector3 lastObjPos = Vector3.zero;
    Quaternion lastObjRot = Quaternion.identity;

    bool isReturned = true;

    [SerializeField]
    private Animator anim;


    [SerializeField]
    private GameObject audioAnchors;

    [SerializeField]
    private TMP_Text successCounter;

    [SerializeField]
    private TMP_Text failureCounter;

    [SerializeField]
    private GameObject restartMenu;

    private int successCount = 0;

    private int failureCount = 0;

    private int maxSuccesses = 20;

    private int maxFailures = 15;

    //0 = hasnt walked, 1 = at counter, asking quesitons, 2 = walking out happy, 3 is walking out mad
    int npcState = 0;

    [SerializeField]
    UnityEngine.UI.Button restartButton;


    //prompt and the answer, check answer against list of properties
    class PromptAnswer
    {
        public string Prompt { get; set; }
        public string Answer { get; set; }

        public PromptAnswer(string prompt, string answer)
        {
            Prompt = prompt;
            Answer = answer;
        }

        /*
        public PromptAnswers(string prompt, List<string> answers)
        {
            Prompt = prompt;
            Answers = answers;
        }*/
    }

    private List<PromptAnswer> promptanswers = new() {
        new("しょうゆをください   shōyu o kudasai", "soy sauce"), //shoyu wo kudasai
        new("コショウをください   koshō o kudasai", "pepper"),
        new("しお をください   shio o kudasai", "salt"),
        new("ナプキンをください   napukin o kudasai", "napkin"),
        new("はしをください   hashi o kudasai", "chopstick"),
        new("ケーキをください   kēki o kudasai", "cake"),
        new("コーヒーをください   kōhī o kudasai", "coffee")
    } ;

    bool doNextPrompt = false;

    PromptAnswer currentPA;

    bool isDismissed = false;


    bool hasTurned = false;
    private UnityAction action;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

      
        action += restart;

        restartButton.onClick.AddListener(action);

        restartMenu.SetActive(false);
        npcObject = npc.gameObject;

        currentPA = promptanswers[0];

        startTime = Time.time;

        //npc.sayLine("first line i say!! what is going on? I dont really know....");

        npc.startWalking();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDismissed)
        {
            if (npc.readyNextLine)
            {
                npc.sayLine(currentPA.Prompt);
                isDismissed = false;
            }

            return;
        }

      
        //if freater than a certian amt of seconds
        if (Time.time - startTime > 5 && !firstLineSaid)
        {



            firstLineSaid = true;


            npc.sayLine(promptanswers[0].Prompt);
            npc.playVoice(2);



        }


        //if is buggy remove the if statement
        if (lastObj)
        {
            lastObj.SetActive(true);
        }

        if (!isReturned)
        {
            lastObj.transform.position = lastObjPos;
            lastObj.transform.rotation = lastObjRot;
            isReturned = true;
        }

        switch (npcState) {
            case 0:
                if (npcObject.transform.position.x > 5.5)
                {
                npc.stopWalking();
                }
                npcState = 1;
                break;
            case 1:

                if (doNextPrompt)
                {
                    if (npc.readyNextLine)
                    {
                        int randVal = Random.Range(0, promptanswers.Count);
                        npc.sayLine(promptanswers[randVal].Prompt);

                        currentPA = promptanswers[randVal];

                        npc.playVoice(randVal + 2);


                        doNextPrompt = false;
                    }
                }

                break;
            case 2:
                if (npc.readyNextLine)
                {
                    npc.hackyEndFix = true;
                    if (!hasTurned)
                    {
                        Quaternion rot = npcObject.transform.rotation;
                        rot.eulerAngles = new Vector3(0, 270.0f, 0);

                        npcObject.transform.rotation = rot;

                        hasTurned = true;
                        anim.SetTrigger("DoWalking");
                        npc.sayHappy();

                        restartMenu.SetActive(true);
                    }
                    npcObject.transform.position = npcObject.transform.position + (npcObject.transform.forward * 0.02f);
                }
                break;
            case 3:
                if (npc.readyNextLine)
                {
                    npc.hackyEndFix = true;
                    if (!hasTurned)
                    {
                        Quaternion rot = npcObject.transform.rotation;
                        rot.eulerAngles = new Vector3(0, 270.0f, 0);

                        npcObject.transform.rotation = rot;
                        hasTurned = true;
                        anim.SetTrigger("DoWalking");
                        npc.sayAngry();
                        restartMenu.SetActive(true);
                    }
                    npcObject.transform.position = npcObject.transform.position + (npcObject.transform.forward * 0.02f);
                }
                break;

        }
       
    }

    public void handleObject(GameObject obj)
    {
        
        DisplayObjectName objScript = obj.GetComponent<DisplayObjectName>();

        //check if it has DisplayObjectName component
        if (objScript != null && npcState == 1)
        {
            //if successful comparison made
            if (obj.CompareTag(currentPA.Answer))
            {
                lastObj = obj;
                lastObjPos = objScript.originalPosition();
                lastObjRot = objScript.originalRotation();

                obj.SetActive(false);
                objScript.disableText();

                isReturned = false;

                doBow();


                doNextPrompt = true;

                successCount++;

                successCounter.text = successCount.ToString() + "/" + maxSuccesses.ToString();

                if (successCount >= maxSuccesses)
                {
                    npcState = 2;
                    npc.sayLine("それでいい, ほんとうにありがとうございました sore de ī, hontō ni arigatōgozaimashita");
                    npc.playVoice(9);
                    
                }

            }
            //if unsuccessful comparison made
            else
            {
                doDismiss();

                failureCount++;
                failureCounter.text = failureCount.ToString() + "/" + maxFailures.ToString();

                if (failureCount >= maxFailures)
                {
                    npcState = 3;
                    npc.sayLine("なんだこれわ? なにをするんですか? かえるぞ! nanda kore we? nani wo surundesuka? kaeruzo!");
                    npc.playVoice(10);
                    
                }
            }
        }

    }

    void doBow()
    {
        npc.sayLine("ありがとうございます Arigatō gozaimasu");
        npc.playVoice(0);
        anim.SetTrigger("DoBow");
    }

    void doDismiss()
    {
        npc.sayLine("ちがうよ!? chigau yo!?");
        npc.playVoice(1);
        anim.SetTrigger("DoDismiss");
        isDismissed = true;
    }

    void restart()
    {
        npcState = 0;
        doNextPrompt = false;



        isDismissed = false;


        hasTurned = false;


        currentPrompt = 0;

        firstLineSaid = false;

        lastObj = null;

        lastObjPos = Vector3.zero;
        lastObjRot = Quaternion.identity;

        isReturned = true;

        successCount = 0;

        failureCount = 0;


        currentPA = promptanswers[0];

        startTime = Time.time;

        //npc.sayLine("first line i say!! what is going on? I dont really know....");

    


        npc.restart();
        npc.stopWalking();

        successCounter.text = successCount.ToString() + "/" + maxSuccesses.ToString();
        failureCounter.text = failureCount.ToString() + "/" + maxFailures.ToString();


        npc.startWalking();

        restartMenu.SetActive(false);
    }

}
