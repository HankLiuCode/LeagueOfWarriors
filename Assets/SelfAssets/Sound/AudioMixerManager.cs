using UnityEngine;
using UnityEngine.Audio;



public class AudioMixerManager : MonoBehaviour
{
    //秈︽北Mixer跑秖
    public AudioMixer audioMixer;

    /// <summary>
    /// 北秖ㄧ计
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterVolume(float volume)
    {
        //MasterVolumeи忌臩ㄓMaster把计
        audioMixer.SetFloat("MasterVolume", volume);
    }

    /// <summary>
    /// 北璉春贾秖ㄧ计
    /// </summary>
    /// <param name="volume"></param>
    public void SetBackgroundMusicVolume(float volume)
    {
        //MusicVolumeи忌臩ㄓMusic把计
        audioMixer.SetFloat("BackgroundMusicVolume", volume);
    }

    /// <summary>
    /// 北秖ㄧ计
    /// </summary>
    /// <param name="volume"></param>
    public void SetSoundEffectVolume(float volume)
    {
        //SoundEffectVolumeи忌臩ㄓSoundEffect把计
        audioMixer.SetFloat("SoundEffectVolume", volume);
    }

}
