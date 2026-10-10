using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource soundEffectsSource;
    public AudioSource musicSource;

    [Header("Attack Sounds")]
    public AudioClip strikeSound;
    public AudioClip heavySound;
    public AudioClip starburstSound;

    [Header("Skill Sounds")]
    public AudioClip blockSound;
    public AudioClip focusSound;
    public AudioClip healSound;
    public AudioClip freezeSound;

    [Header("Scene Music")]
    public AudioClip homeMusic;
    public AudioClip level1Music;
    public AudioClip bossMusic;
    public AudioClip deathMusic;
    public AudioClip victoryMusic;

    private void OnDestroy(){
        if (Instance == this){
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void PlaySceneMusic(string sceneName){
        if (musicSource == null){
            return;
        }

        AudioClip music = null;

        switch (sceneName){
            case "Homescreen":
                music = homeMusic;
                break;
            case "Level1":
                music = level1Music;
                break;
            case "Boss Level":
                music = bossMusic;
                break;
            case "Death":
                music = deathMusic;
                break;
            case "Victory":
                music = victoryMusic;
                break;
            default:
                break;
        }

        if (musicSource.clip == music && musicSource.isPlaying){
            return;
        }

        musicSource.Stop();
        musicSource.clip = music;
        musicSource.loop = true;

        if (music != null){
            musicSource.Play();
        }
    }

    public void PlayCard(string cardName){
        if (soundEffectsSource == null){
            return;
        }

        AudioClip sound = null;

        switch (cardName){
            case "Strike":
                sound = strikeSound;
                break;
            case "Heavy":
                sound = heavySound;
                break;
            case "Starburst Stream":
                sound = starburstSound;
                break;
            case "Block":
                sound = blockSound;
                break;
            case "Focus":
                sound = focusSound;
                break;
            case "Freeze":
                sound = freezeSound;
                break;
            case "Heal":
                sound = healSound;
                break;
        }

        if (sound != null){
            soundEffectsSource.PlayOneShot(sound);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        PlaySceneMusic(scene.name);
    }
    

    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start(){
        PlaySceneMusic(SceneManager.GetActiveScene().name);
    }

    
}
