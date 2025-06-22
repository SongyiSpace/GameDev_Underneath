// PlayerMove.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    public float speed = 1f; //플레이어 속도
    Rigidbody rb;
    public float mouseSensitive = 400f;
    private Vector3 rt;
    private MonologueManager monoManager;
    public bool canMove = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rt = transform.rotation.eulerAngles;
        monoManager = FindFirstObjectByType<MonologueManager>();
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "AfterWork")
        {
            //게임 시작 시 정면바라보기
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
            monoManager.ShowMonologue(Monologue.AfterWork_Start);
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;
        //대사 몇 줄 나오는 동안 canMove false;

        MoveUpdate();
        LookUpdate();
    }

    //이동 함수
    void MoveUpdate()
    {

        float xInput = Input.GetAxisRaw("Horizontal"); //좌우 방향키
        float yInput = Input.GetAxisRaw("Vertical"); //상하 방향키
        //Raw넣어줘야 즉각적반응. Raw없으면 서서히 진행되어서 딜레이가 조금 생김

        Vector3 move = (transform.forward * yInput + transform.right * xInput).normalized;

        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);

    }

    //시야 회전 함수
    void LookUpdate(){
        //처음에 회전이 되어있는 상태로 시작해서 초깃값 넣어줘야 함

        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitive * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitive * Time.deltaTime;

        rt.x -= mouseY;
        rt.y += mouseX;

        transform.rotation = Quaternion.Euler(0f, rt.y, 0f);

        GameObject.Find("Main Camera").GetComponent<Transform>().transform.localRotation = Quaternion.Euler(rt.x, 0f, 0f);
    }
}
