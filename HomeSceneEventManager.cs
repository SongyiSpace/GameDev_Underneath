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
    [SerializeField] private GameObject monitor;
    [SerializeField] private GameObject showerHead;
    [SerializeField] private GameObject bathDoor;
    [SerializeField] private GameObject[] bathLights;
    [SerializeField] private GameObject bathLightSwitch;
    [SerializeField] private GameObject bed;
    [SerializeField] private ParticleSystem waterFall;

    private float microwaveWaitTime = 5f;
    private float showerTime1 = 7f;

    enum GameStep
    {
        step1_Intro,
        step2_DropRabbit,
        step3_PickUpFood_f,
        step4_DropFood_m,
        step5_PickUpFood_m,
        step6_EatFood,
        step7_Work,
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

    void Update()
    {
        if (SoundManager.IsLoopSoundPlaying(LoopType.MICRO_BEEP) && microAnimator.GetBool("isOpen"))
        {
            SoundManager.StopLoopSound();
        }
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
            SoundManager.PlaySFXSound(SFXType.MICRO_WAITING);

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
        SoundManager.StopSFXSound();
        SoundManager.PlayLoopSound(LoopType.MICRO_BEEP, 2f);
    }

    //---step6---//
    public void EatFood()
    {
        if (currentStep == GameStep.step6_EatFood)
        {
            switcher.SwitchToOrigin(food3);
            animatorController.Ani_EatFood();
            monitor.GetComponent<Collider>().enabled = true;

            currentStep = GameStep.step7_Work;
        }
    }

    //---step7---//
    public void Work()
    {
        if (currentStep == GameStep.step7_Work)
        {
            monitor.GetComponent<Collider>().enabled = false;
            player.GetComponent<Collider>().enabled = false;

            animatorController.Ani_Work();

            showerHead.GetComponent<Collider>().enabled = true;
            bathLightSwitch.GetComponent<Collider>().enabled = true;

            currentStep = GameStep.step8_Shower;
        }
    }

    //---step8---//
    public void Shower()
    {
        if (currentStep == GameStep.step8_Shower)
        {
            bathDoor.GetComponent<Collider>().enabled = false;
            showerHead.GetComponent<Collider>().enabled = false;
            StartCoroutine(PlayShowerRoutine());
            Invoke("TurnOffLights", showerTime1);
            StartCoroutine(WaitForSwitchOn());
        }
    }
    private IEnumerator PlayShowerRoutine()
    {
        if (!waterFall.isPlaying)
        {
            waterFall.Play();
            SoundManager.PlaySFXSound(SFXType.SHOWERVALVE);
            yield return new WaitForSeconds(0.5f);
            SoundManager.Play3DSound(GameObject.Find("showerHead")?.GetComponent<AudioSource>(), SFXType.SHOWERING);
        }
    }
    private void TurnOffLights()
    {
        FindFirstObjectByType<InteractionManager>().ToggleSwitch(bathLightSwitch, bathLights, ref interactionManager.isBathSwitchOn);
        bathDoor.GetComponent<Collider>().enabled = true;
    }
    private IEnumerator WaitForSwitchOn()
    {
        yield return new WaitForSeconds(showerTime1 + 1f);
        currentStep = GameStep.step9_Shower2;
    }

    //---step9---//
    public void Shower2()
    {
        if (currentStep == GameStep.step9_Shower2)
        {
            Trigger trigger = FindFirstObjectByType<Trigger>();
            StartCoroutine(trigger.ShowerTrigger(() => currentStep = GameStep.step10_GoToSleep));
            bed.GetComponent<Collider>().enabled = true;
        }
    }
    //---step10---//
    public void GoToSleep()
    {
        if (currentStep == GameStep.step10_GoToSleep)
        {
            if (interactionManager.isBathSwitchOn || interactionManager.isHallSwitchOn)
            {
                monoManager.ShowMonologue(Monologue.Home_HaveToLightOff);
            }
            else
            {
                bed.GetComponent<Collider>().enabled = false;
                animatorController.Ani_Sleep();
            }
            // currentStep = GameStep.step11
        }
    }
}
