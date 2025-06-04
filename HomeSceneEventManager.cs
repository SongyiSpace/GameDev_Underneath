using UnityEngine;
// 집 씬 전체 흐름 제어

public class HomeSceneEventManager : MonoBehaviour
{
    private int eventIndex = 1;
    private MonologueManager monoManager;
    private GameObject rabbit;
    private TransparentSwitcher switcher;
    [SerializeField] private GameObject food1;
    [SerializeField] private GameObject food2;

    void Start()
    {
        monoManager = FindFirstObjectByType<MonologueManager>();
        switcher = FindFirstObjectByType<TransparentSwitcher>();
        rabbit = GameObject.Find("rabbit");

        switcher.SwitchToTransparent(rabbit);
        switcher.SwitchToTransparent(food2);

        NextEvent();
    }


    public void NextEvent()
    {
        eventIndex++;
        switch (eventIndex)
        {
            case 1:
                StartMonologue();
                break;
            case 2:
                DropRabbit();
                break;
            case 3:
                pickUpFood();
                break;
            case 4:
                DropFood();
                break;
        }
    }

    void StartMonologue()
    {
        monoManager.HideMonologue();
        monoManager.ShowMonologue(Monologue.Home_Start);
    }
    void DropRabbit()
    {
        switcher.SwitchToOrigin(rabbit);
    }
    void pickUpFood()
    {
        food1.SetActive(false);
    }
    void DropFood()
    {
        switcher.SwitchToOrigin(food2);
    }


}
