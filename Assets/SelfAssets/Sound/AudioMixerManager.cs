using UnityEngine;
using UnityEngine.Audio;



public class AudioMixerManager : MonoBehaviour
{
    //�i�汱�Mixer�ܶq
    public AudioMixer audioMixer;

    /// <summary>
    /// ����D���q�����
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterVolume(float volume)
    {
        //MasterVolume���ڭ̼��S�X�Ӫ�Master���Ѽ�
        audioMixer.SetFloat("MasterVolume", volume);
    }

    /// <summary>
    /// ����I�����֭��q�����
    /// </summary>
    /// <param name="volume"></param>
    public void SetBackgroundMusicVolume(float volume)
    {
        //MusicVolume���ڭ̼��S�X�Ӫ�Music���Ѽ�
        audioMixer.SetFloat("BackgroundMusicVolume", volume);
    }

    /// <summary>
    /// ����ĭ��q�����
    /// </summary>
    /// <param name="volume"></param>
    public void SetSoundEffectVolume(float volume)
    {
        //SoundEffectVolume���ڭ̼��S�X�Ӫ�SoundEffect���Ѽ�
        audioMixer.SetFloat("SoundEffectVolume", volume);
    }

}
