
using System.Collections;
using UnityEngine;

public enum BGMType
{
    BGM_AFTERWORK
}
public enum FootstepType
{
    ROADFOOTSTEP,
    INDOORFOOTSTEP
}
public enum SFXType
{
    FRIDGE_OPEN,
    FRIDGE_CLOSE,
    MICRO_OPEN,
    MICRO_CLOSE,
    WOODENDOOR_OPEN,
    WOODENDOOR_CLOSE,
    EATING_DINNER,
    MICRO_WAITING
}
public enum LoopType
{
    MICRO_BEEP
}

public class SoundManager : MonoBehaviour
{

    [SerializeField] private AudioClip[] BGMList;
    [SerializeField] private AudioClip[] FootstepList;
    [SerializeField] private AudioClip[] SFXList;
    [SerializeField] private AudioClip[] LoopList;

    private static SoundManager instance;
    [SerializeField] private AudioSource bgmSource; //배경음악
    [SerializeField] private AudioSource footstepSource; //발소리
    [SerializeField] private AudioSource SFXSource; //단발성 사운드
    [SerializeField] private AudioSource loopSource; //반복성, 상호작용 가능 사운드

    private static LoopType? currentLoopPlaying = null;
    private static Coroutine loopCoroutine;

    // private static readonly Dictionary<DoorType, (SoundType open, SoundType close)> doorSounds = new()
    // {
    //     { DoorType.Fridge, (SoundType.FRIDGE_OPEN, SoundType.FRIDGE_CLOSE) },
    //     { DoorType.Microwave, (SoundType.MICRO_OPEN, SoundType.MICRO_CLOSE) },
    //     { DoorType.Bathroom, (SoundType.BATH_OPEN, SoundType.BATH_CLOSE) }
    // };



    void Awake()
    {
        instance = this;
    }
    void Start()
    {

    }

//BGM 
    public static void PlayBGM(BGMType sound, float volume = 1)
    {
        instance.bgmSource.clip = instance.BGMList[(int)sound];
        instance.bgmSource.volume = volume;
        instance.bgmSource.loop = true;
        instance.bgmSource.Play();
    }

//Footstep
    public static void PlayFootStepSound(FootstepType sound, float volume = 1, float pitch = 1f)
    {
        AudioClip clip = instance.FootstepList[(int)sound];

        instance.footstepSource.volume = volume;
        instance.footstepSource.pitch = pitch;

        if (instance.footstepSource.isPlaying && instance.footstepSource.clip == clip) return; //사운드가 겹쳐서 재생되지 않게 하는 코드
        instance.footstepSource.clip = clip;

        instance.footstepSource.PlayOneShot(instance.FootstepList[(int)sound], volume);
    }

    public static void StopFootStepSound()
    {
        if (instance.footstepSource != null)
            instance.footstepSource.Stop();
    }

//SFX
    public static void PlaySFXSound(SFXType sound, float volume = 1f, float pitch = 1f)
    {
        AudioClip clip = instance.SFXList[(int)sound];
        instance.SFXSource.volume = volume;
        instance.SFXSource.pitch = pitch;

        instance.SFXSource.PlayOneShot(clip);
    }
    public static void StopSFXSound()
    {
        if (instance.SFXSource != null)
            instance.SFXSource.Stop();
    }

    //Loop & Reactive

    public static void PlayLoopSound(LoopType sound, float delaySecond = 0f, float volume = 1f, float pitch = 1f)
    {
        StopLoopSound();

        currentLoopPlaying = sound;
        loopCoroutine = instance.StartCoroutine(LoopSoundRoutine(sound, delaySecond, volume, pitch));
    }
    public static IEnumerator LoopSoundRoutine(LoopType sound, float delaySeconds, float volume, float pitch)
    {
        AudioClip clip = instance.LoopList[(int)sound];
        instance.loopSource.clip = clip;
        instance.loopSource.volume = volume;
        instance.loopSource.pitch = pitch;
        
        while (true)
        {
            instance.loopSource.Play();

            // 클립 재생 시간 + 추가 대기 시간
            yield return new WaitForSeconds(clip.length + delaySeconds);
        }
    }
    public static void StopLoopSound()
    {
        
        if (loopCoroutine != null)
        {
            instance.StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }


        if (instance.loopSource != null)
            instance.loopSource.Stop();
        currentLoopPlaying = null;
    }
    public static bool IsLoopSoundPlaying(LoopType sound)
    {
        return currentLoopPlaying == sound && instance.loopSource.isPlaying;
    }

// 애니메이션 기반 사운드 출력 (AnimSoundPlayer.cs)
    public static void PlayAnimSound(AudioSource source, SFXType sound, float volume = 1f, float pitch = 1f)
    {
        AudioClip clip = instance.SFXList[(int)sound];
        source.volume = volume;
        source.pitch = pitch;

        source.PlayOneShot(clip);
    }
    

    public static void PlayAnimSound(string soundName)
    {
        switch (soundName)
        {
            case "Fridge_Open":
                PlayAnimSound(GameObject.Find("fridgeDoor")?.GetComponent<AudioSource>(), SFXType.FRIDGE_OPEN);
                break;

            case "Fridge_Close":
                PlayAnimSound(GameObject.Find("fridgeDoor")?.GetComponent<AudioSource>(), SFXType.FRIDGE_CLOSE);
                break;
            case "Micro_Open":
                PlayAnimSound(GameObject.Find("microDoor")?.GetComponent<AudioSource>(), SFXType.MICRO_OPEN);
                break;

            case "Micro_Close":
                PlayAnimSound(GameObject.Find("microDoor")?.GetComponent<AudioSource>(), SFXType.MICRO_CLOSE);
                break;
            case "Bath_Open":
                PlayAnimSound(GameObject.Find("bathDoor")?.GetComponent<AudioSource>(), SFXType.WOODENDOOR_OPEN);
                break;

            case "Bath_Close":
                PlayAnimSound(GameObject.Find("bathDoor")?.GetComponent<AudioSource>(), SFXType.WOODENDOOR_CLOSE);
                break;

            default:
                Debug.LogWarning($"[SoundManager] Unknown AnimSoundEvent: {soundName}");
                break;
        }
    }

}