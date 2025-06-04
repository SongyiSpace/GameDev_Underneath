using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimatorController : MonoBehaviour
{
    private PlayerMove playerMove;
    private GameObject rabbit;
    private GameObject player;
    private Transform cam;
    private ScreenFader screenFader;
    private SpriteRenderer spriteRenderer;

    

    void Start()
    {
        playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
        rabbit = GameObject.Find("rabbit");
        player = GameObject.Find("Player");
        cam = Camera.main.transform;
        screenFader = FindFirstObjectByType<ScreenFader>();
        // spriteRenderer = blackScreen.GetComponent<SpriteRenderer>();
    }


    //---------------토끼인형 줍는 애니메이션---------------//
    public void PickUpRabbit()
    {
        StartCoroutine(PickUpRabbitAnimation(cam.transform, player.transform));
    }

    private IEnumerator PickUpRabbitAnimation(Transform cam, Transform player)
    {
        playerMove.canMove = false;


        // 1. 기존 카메라 roation값 저장
        Quaternion originalCamRot = cam.rotation;
        Quaternion originalPlayerRot = player.rotation;

        // 2. 고개숙이기 (x축:20)
        float t;
        Quaternion camDown = Quaternion.Euler(cam.eulerAngles.x + 20f, cam.eulerAngles.y, cam.eulerAngles.z);

        t = 0;
        while (t < 1.5f)
        {
            cam.rotation = Quaternion.Slerp(originalCamRot, camDown, t / 1.5f);
            t += Time.deltaTime;
            yield return null;
        }
        rabbit.SetActive(false);

        // 3. 고개 들기
        t = 0;
        while (t < 1.5f)
        {
            cam.rotation = Quaternion.Slerp(camDown, originalCamRot, t / 1.5f);
            t += Time.deltaTime;
            yield return null;
        }
        cam.rotation = originalCamRot;


        // 4. 정면 쳐다보기
        Quaternion playerLookFront = Quaternion.Euler(0f, 0f, 0f);
        Quaternion camLookFront = Quaternion.Euler(7f, 0f, 0f);

        t = 0;
        while (t < 1.5f)
        {
            player.rotation = Quaternion.Lerp(originalPlayerRot, playerLookFront, t / 1.5f);
            cam.rotation = Quaternion.Lerp(originalCamRot, camLookFront, t / 1.5f);
            t += Time.deltaTime;
            yield return null;
        }

        //5. 직진하기
        Vector3 originalPlayerPos = player.position;
        Vector3 targetPos = originalPlayerPos + player.forward * 10f;

        screenFader.StartFadeOut(3f);

        t = 0;
        while (t < 5f)
        {
            player.position = Vector3.Lerp(originalPlayerPos, targetPos, t / 5f);
            t += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("Home");
        screenFader.StartFadeIn(3f);
    }
}
