using UnityEngine;
using UnityEngine.SceneManagement;

public class RabbitInteract : MonoBehaviour
{
    private MonologueManager monoManager;
    private AnimatorController animatorController;
    private HomeSceneEventManager homeEventManager;
    private GameObject rabbit;

    void Start()
    {
        monoManager = FindFirstObjectByType<MonologueManager>();
        animatorController = FindFirstObjectByType<AnimatorController>();
        homeEventManager = FindFirstObjectByType<HomeSceneEventManager>();
        rabbit = GameObject.Find("rabbit");
    }

    //토끼캐릭터가 클릭됐을때 실행할 상호작용
    public void Interact()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        rabbit.GetComponent<Collider>().enabled = false;

        if (currentScene == "AfterWork")
        {
            monoManager.ShowMonologue(Monologue.AfterWork_MeetRabbit, () =>
            {
                animatorController.Ani_PickUpRabbit();
            });
        }
        if (currentScene == "Home")
        {
            homeEventManager.DropRabbit();
        }

    }

}