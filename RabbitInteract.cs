using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RabbitInteract : MonoBehaviour
{
    private MonologueManager monoManager;
    private AnimatorController animatorController;
    private HomeSceneEventManager homeEventManager;

    void Start()
    {
        monoManager = FindFirstObjectByType<MonologueManager>();
        animatorController = FindFirstObjectByType<AnimatorController>();
        homeEventManager = FindFirstObjectByType<HomeSceneEventManager>();
    }

    //토끼캐릭터가 클릭됐을때 실행할 상호작용
    public void Interact()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "AfterWork")
        {
            monoManager.ShowMonologue(Monologue.AfterWork_MeetRabbit, () =>
            {
                animatorController.PickUpRabbit();
            });
        }
        if (currentScene == "Home")
        {
            homeEventManager.DropRabbit();
        }

    }

}