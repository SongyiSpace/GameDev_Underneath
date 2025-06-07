using System.Collections;
using UnityEngine;
// 집 씬 전체 흐름 제어

public class HomeSceneEventManager : MonoBehaviour
{
    private MonologueManager monoManager;
    private GameObject rabbit;
    private TransparentSwitcher switcher;
    private AnimatorController animatorController;
    private Animator microAnimator;
    private InteractionManager interactionManager;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject food1;
    [SerializeField] private GameObject food2;
    [SerializeField] private GameObject food3;
    [SerializeField] private GameObject microDoor;
    [SerializeField] private GameObject computer;
    [SerializeField] private GameObject showerHead;
    [SerializeField] private GameObject bathPointLight;
    [SerializeField] private GameObject bathLight;
    [SerializeField] private GameObject lightSwitch;
    [SerializeField] private ParticleSystem waterFall;



    private float microwaveWaitTime = 2f;
    private float showerTime1 = 3f;

    enum GameStep
    {
        step1_Intro,
        step2_DropRabbit,
        step3_PickUpFood_f,
        step4_DropFood_m,
        step5_PickUpFood_m,
        step6_EatFood,
        step7_Study,
        step8_Shower,
        step9_Shower2,
        step10_GoToSleep
    }
    GameStep currentStep;

    void Start()
    {
        monoManager = FindFirstObjectByType<MonologueManager>();
        switcher = FindFirstObjectByType<TransparentSwitcher>();
        rabbit = GameObject.Find("rabbit");
        microAnimator = microDoor.GetComponent<Animator>();
        animatorController = FindFirstObjectByType<AnimatorController>();
        interactionManager = FindFirstObjectByType<InteractionManager>();

        switcher.SwitchToTransparent(rabbit);
        switcher.SwitchToTransparent(food2);
        switcher.SwitchToTransparent(food3);

        StartMonologue();
    }

    //---step1---//
    public void StartMonologue()
    {
        if (currentStep == GameStep.step1_Intro)
        {
            monoManager.HideMonologue();
            monoManager.ShowMonologue(Monologue.Home_Start);

            currentStep = GameStep.step2_DropRabbit;
        }
    }

    //---step2---//
    public void DropRabbit()
    {
        if (currentStep == GameStep.step2_DropRabbit)
        {
            switcher.SwitchToOrigin(rabbit);
            monoManager.ShowMonologue(Monologue.Home_LetsEat);
            food1.GetComponent<Collider>().enabled = true;
            currentStep = GameStep.step3_PickUpFood_f;
        }

    }

    //---step3---//
    public void PickUpFridgeFood()
    {
        if (currentStep == GameStep.step3_PickUpFood_f)
        {
            food1.SetActive(false);
            food2.GetComponent<Collider>().enabled = true;
            currentStep = GameStep.step4_DropFood_m;
        }

    }

    public void MicroFood()
    {
        if (currentStep == GameStep.step4_DropFood_m) //---step4---//
        {
            switcher.SwitchToOrigin(food2);
            microAnimator.SetBool("isOpen", false);
            monoManager.ShowMonologueForSeconds(Monologue.Home_WaitMicro, microwaveWaitTime);
            Invoke(nameof(OpenMicrowave), microwaveWaitTime);

            currentStep = GameStep.step5_PickUpFood_m;
        }
        else if (currentStep == GameStep.step5_PickUpFood_m) //---step5---//
        {
            food2.SetActive(false);
            food3.GetComponent<Collider>().enabled = true;

            currentStep = GameStep.step6_EatFood;
        }
    }
    private void OpenMicrowave()
    {
        microAnimator.SetBool("isOpen", true);
    }

    //---step6---//
    public void EatFood()
    {
        if (currentStep == GameStep.step6_EatFood)
        {
            switcher.SwitchToOrigin(food3);
            animatorController.Ani_EatFood();
            computer.GetComponent<Collider>().enabled = true;

            currentStep = GameStep.step7_Study;
        }
    }

    //---step7---//
    public void Study()
    {
        if (currentStep == GameStep.step7_Study)
        {
            computer.GetComponent<Collider>().enabled = false;
            player.GetComponent<Collider>().enabled = false;
            showerHead.GetComponent<Collider>().enabled = true;
            lightSwitch.GetComponent<Collider>().enabled = true;

            animatorController.Ani_Study();

            currentStep = GameStep.step8_Shower;
        }
    }

    //---step8---//
    public void Shower()
    {
        if (currentStep == GameStep.step8_Shower)
        {
            showerHead.GetComponent<Collider>().enabled = false;
            if (!waterFall.isPlaying)
                waterFall.Play();
            Invoke("TurnOffLights", showerTime1);
            StartCoroutine(WaitForSwitchOn());
        }
    }
    private void TurnOffLights()
    {
        FindFirstObjectByType<InteractionManager>().ToggleSwitch();
    }
    private IEnumerator WaitForSwitchOn()
    {
        //플레이어가 꺼진 불을 킬 때까지 기다린 후 불 켜면 step9로 넘어감
        yield return new WaitForSeconds(showerTime1);
        yield return new WaitUntil(() => interactionManager.isBathSwitchOn);
        currentStep = GameStep.step9_Shower2;
    }

    //---step9---//
    public void StartShowerTriggerSequence()
    {
        if (currentStep == GameStep.step9_Shower2)
        {
            Trigger trigger = FindFirstObjectByType<Trigger>();
            StartCoroutine(trigger.ShowerTrigger(() => currentStep = GameStep.step10_GoToSleep));
        }
    }



}
