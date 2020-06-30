using UnityEngine;


namespace PhyGames.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        public enum AudioChannel
        {
            Master,
            Sfx,
            Music,
        }

        public float MasterVolumePercent { get; private set; }
        public float SfxVolumePercent { get; private set; }
        public float MusicVolumePercent { get; private set; }

        private AudioSource m_SfxSource;

        private Transform m_AudioListener;

        private SoundLibrary m_Library;


        private void Awake()
        {
            m_Library = GetComponent<SoundLibrary>();

            GameObject sfxSource = new GameObject("SFX Source");
            sfxSource.transform.parent = transform;
            m_SfxSource = sfxSource.AddComponent<AudioSource>();

            m_AudioListener = FindObjectOfType<AudioListener>().transform;

            MasterVolumePercent = PlayerPrefs.GetFloat("master_volume", 1f);
            SfxVolumePercent = PlayerPrefs.GetFloat("sfx_volume", 1f);
            MusicVolumePercent = PlayerPrefs.GetFloat("music_volume", 1f);
        }


        public void SetVolume(float volumePercent, AudioChannel channel)
        {
            switch(channel)
            {
                case AudioChannel.Master:
                    MasterVolumePercent = volumePercent;
                    break;
                case AudioChannel.Sfx:
                    SfxVolumePercent = volumePercent;
                    break;
                case AudioChannel.Music:
                    MusicVolumePercent = volumePercent;
                    break;
            }

            PlayerPrefs.SetFloat("master_volume", volumePercent);
            PlayerPrefs.SetFloat("sfx_volume", volumePercent);
            PlayerPrefs.SetFloat("music_volume", volumePercent);
            PlayerPrefs.Save();            
        }

        public void PlaySound(string soundName)
        {
            m_SfxSource.PlayOneShot(m_Library.GetClipFromName(soundName), SfxVolumePercent * MasterVolumePercent);
        }
    }
}
