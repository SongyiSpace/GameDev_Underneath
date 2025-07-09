using UnityEngine;

public class AnimSoundPlayer : MonoBehaviour
{
    public void PlaySound(string soundName)
    {
        SoundManager.PlayAnimSound(soundName);
    }
}