using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    void Start()
    {
        //플레이 화면에서 커서 보이지 않게
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "AfterWork")
        {
            SoundManager.PlayBGM(BGMType.BGM_AFTERWORK, 0.8f);
        }
    }

    void Update(){

    }
}
