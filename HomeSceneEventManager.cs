using UnityEngine;
// 집 씬 전체 흐름 제어

public class HomeSceneEventManager : MonoBehaviour
{
    private MonologueManager monoManager;
    private GameObject rabbit;
    private TransparentSwitcher switcher;
    private Animator microAnimator;
    [SerializeField] private GameObject food1;
    [SerializeField] private GameObject food2;
    [SerializeField] private GameObject microDoor;

    enum GameStep
    {
        step1_Intro,
        step2_DropRabbit,
        step3_PickUpFood_f,
        step4_DropFood_m,
        step5_PickUpFood_m
    }
    GameStep currentStep;

    void Start()
    {
        monoManager = FindFirstObjectByType<MonologueManager>();
        switcher = FindFirstObjectByType<TransparentSwitcher>();
        rabbit = GameObject.Find("rabbit");
        microAnimator = microDoor.GetComponent<Animator>();

        switcher.SwitchToTransparent(rabbit);
        switcher.SwitchToTransparent(food2);

        StartMonologue();
    }


    //step1
    public void StartMonologue()
    {
        if (currentStep == GameStep.step1_Intro)
        {
            monoManager.HideMonologue();
            monoManager.ShowMonologue(Monologue.Home_Start);

            currentStep = GameStep.step2_DropRabbit;
        }
    }

    //step2
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

    //step3
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
        if (currentStep == GameStep.step4_DropFood_m) //step4
        {
            switcher.SwitchToOrigin(food2);
            microAnimator.SetBool("isOpen", false);
            monoManager.ShowMonologueForSeconds(Monologue.Home_WaitFood, 5f);
            Invoke(nameof(OpenMicrowave), 5f);
            currentStep = GameStep.step5_PickUpFood_m;
        }
        else if (currentStep == GameStep.step5_PickUpFood_m) //step5
        {
            food2.SetActive(false);

            // currentStep = GameStep.step5_PickUpFood_m;
        }
    }
    private void OpenMicrowave()
    {
        microAnimator.SetBool("isOpen", true);
    }
    


}
