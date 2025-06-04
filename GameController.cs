using UnityEngine;

public class GameController : MonoBehaviour
{

    void Start( ){
        //플레이 화면에서 커서 보이지 않게
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update( ){

    }
}
