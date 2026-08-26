using System.Collections.Generic;
using UnityEngine;
internal class AudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgMusicSource;
    [SerializeField] private AudioSource gameSoundSource;

    // Dedicated source for the rocket background loop — added at runtime (see Awake) on this same
    // GameObject, so it can run for the whole rocket animation without being cut off by one-shot
    // SFX on gameSoundSource (wheel tick/stop, bonus win, etc.).
    private AudioSource rocketBgSource;

    [Header("Background")]
    [SerializeField] private AudioClip bgMusic; // bg .mp3

    [Header("Bonus Wheel")]
    [SerializeField] private AudioClip WheelArrowTick;      // wheelSpinning.wav
    [SerializeField] private AudioClip WheelArrowStop;      // wheel stop.mp3
    [SerializeField] private AudioClip bonusWheelTrigger;   // BonusWheelYTrigger.mp3
    [SerializeField] private AudioClip bonusWin;            // BonusWin.mp3
    [SerializeField] private AudioClip rocketBackground;    // RocketBG.mp3 — loops for the whole rocket animation, not per-rocket

    [Header("Reel Sounds")]
    [SerializeField] private AudioClip reelSpinning;   // spinning.mp3
    [SerializeField] private AudioClip reelStop;        // spin stop.mp3

    [Header("UI Sounds")]
    [SerializeField] private AudioClip uiButton;        // universal all button.mp3
    [SerializeField] private AudioClip maxBet;          // max bet.mp3
    [SerializeField] private AudioClip turboActivate;   // turbo rocket.mp3
    [SerializeField] private AudioClip autoplayOpen;    // hold for auto.mp3
    [SerializeField] private AudioClip autoplaySelect;  // hold for auto numbers select.mp3

    [Header("Win Line Sounds")]
    [SerializeField] private AudioClip winLineIntro;      // icons in machine.mp3 — one-shot at the start of a win line display
    [SerializeField] private AudioClip paylineHighlight;  // paylines in slot.mp3 — plays as each line is cycled through

    [Header("Feature Sounds")]
    [SerializeField] private AudioClip heatEmUp;            // hit em up.mp3
    [SerializeField] private AudioClip scatterTrigger;      // 3 free spin in slot.mp3
    [SerializeField] private AudioClip freeSpinsWon;        // you won free spins.mp3
    [SerializeField] private AudioClip freeSpinsWinAmount;  // win amount in free spins .mp3

    private bool isGameMuted = false;
    private bool isMusicMuted = false;

    private readonly Dictionary<AudioSource, bool> preFocusMuteState = new Dictionary<AudioSource, bool>();
    private bool isForceMuted = false;

    private const string PrefKeyMusicVol = "audio_music_volume";
    private const string PrefKeySfxVol = "audio_sfx_volume";

    private float _musicVolume = 0.5f;
    private float _sfxVolume = 1.0f;

    internal float MusicVolume => _musicVolume;
    internal float SfxVolume => _sfxVolume;

    private void Awake()
    {
        _musicVolume = PlayerPrefs.GetFloat(PrefKeyMusicVol, 0.5f);
        _sfxVolume = PlayerPrefs.GetFloat(PrefKeySfxVol, 1.0f);

        rocketBgSource = gameObject.AddComponent<AudioSource>();
        rocketBgSource.loop = true;
        rocketBgSource.playOnAwake = false;
    }

    internal void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(PrefKeyMusicVol, _musicVolume);
        PlayerPrefs.Save();
        if (bgMusicSource) bgMusicSource.volume = _musicVolume;
        MuteBackground(_musicVolume <= 0f);
    }

    internal void SetSfxVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(PrefKeySfxVol, _sfxVolume);
        PlayerPrefs.Save();
        if (gameSoundSource) gameSoundSource.volume = _sfxVolume;
        if (rocketBgSource) rocketBgSource.volume = _sfxVolume;
        MuteGame(_sfxVolume <= 0f);
    }

    private void Start()
    {
        if (bgMusicSource) bgMusicSource.volume = _musicVolume;
        if (gameSoundSource) gameSoundSource.volume = _sfxVolume;
        if (rocketBgSource) rocketBgSource.volume = _sfxVolume;

        PlayBackground();
    }

    internal void PlayBackground()
    {
        if (!bgMusic) return;

        bgMusicSource.clip = bgMusic;
        bgMusicSource.loop = true;
        if (!bgMusicSource.isPlaying)
            bgMusicSource.Play();
    }

    internal void StopBackground()
    {
        bgMusicSource.Stop();
    }

    internal void PlayWheelArrowTick(bool loop)
    {
        PlayGame(WheelArrowTick, loop);
    }

    internal void PlayWheelArrowStop(bool loop)
    {
        PlayGame(WheelArrowStop, loop);
    }

    internal void PlayBonusWheelTrigger()
    {
        PlayGame(bonusWheelTrigger, false);
    }

    internal void PlayBonusWin()
    {
        PlayGame(bonusWin, false);
    }

    // Loops for the whole rocket animation (start-to-stop of the bonus wheel's rocket spawner),
    // not per individual rocket — uses its own AudioSource so it isn't cut off by gameSoundSource
    // one-shots (wheel tick/stop, bonus win, etc.) playing at the same time.
    internal void PlayRocketBackground()
    {
        if (!rocketBackground || rocketBgSource == null) return;

        rocketBgSource.clip = rocketBackground;
        rocketBgSource.loop = true;
        if (!rocketBgSource.isPlaying)
            rocketBgSource.Play();
    }

    internal void StopRocketBackground()
    {
        if (rocketBgSource == null) return;
        rocketBgSource.Stop();
    }

    internal void PlayReelSpinning(bool loop)
    {
        PlayGame(reelSpinning, loop);
    }

    internal void PlayReelStop(bool loop = false)
    {
        PlayGame(reelStop, loop);
    }

    internal void PlayUIButton(bool loop)
    {
        PlayGame(uiButton, loop);
    }

    internal void PlayMaxBet()
    {
        PlayGame(maxBet, false);
    }

    internal void PlayTurboActivate()
    {
        PlayGame(turboActivate, false);
    }

    internal void PlayAutoplayOpen()
    {
        PlayGame(autoplayOpen, false);
    }

    internal void PlayAutoplaySelect()
    {
        PlayGame(autoplaySelect, false);
    }

    internal void PlayWinLineIntro()
    {
        PlayGame(winLineIntro, false);
    }

    internal void PlayPaylineHighlight()
    {
        PlayGame(paylineHighlight, false);
    }

    internal void PlayHeatEmUp()
    {
        PlayGame(heatEmUp, false);
    }

    internal void PlayScatterTrigger()
    {
        PlayGame(scatterTrigger, false);
    }

    internal void PlayFreeSpinsWon()
    {
        PlayGame(freeSpinsWon, false);
    }

    internal void PlayFreeSpinsWinAmount()
    {
        PlayGame(freeSpinsWinAmount, false);
    }

    private void PlayGame(AudioClip clip, bool loop)
    {
        if (!clip) return;

        gameSoundSource.Stop();
        gameSoundSource.clip = clip;
        gameSoundSource.loop = loop;
        gameSoundSource.Play();
    }

    internal void StopGameAudio()
    {
        gameSoundSource.Stop();
        gameSoundSource.loop = false;
    }

    // Focus-driven — called from BOTH UIManager.OnFocusChanged (JS path) and OnApplicationFocus below.
    internal void SetMuteAll(bool forceMute)
    {
        if (forceMute == isForceMuted) return;
        isForceMuted = forceMute;

        AudioSource[] sources = { bgMusicSource, gameSoundSource, rocketBgSource };
        foreach (var source in sources)
        {
            if (source == null) continue;
            if (forceMute)
            {
                preFocusMuteState[source] = source.mute;
                source.mute = true;
            }
            else
            {
                source.mute = preFocusMuteState.TryGetValue(source, out bool prevMuted) ? prevMuted : source.mute;
            }
        }
    }

    internal void MuteBackground(bool mute)
    {
        bgMusicSource.mute = mute;
        if (isForceMuted) preFocusMuteState[bgMusicSource] = mute;
    }

    internal void MuteGame(bool mute)
    {
        gameSoundSource.mute = mute;
        if (rocketBgSource != null) rocketBgSource.mute = mute;
        if (isForceMuted)
        {
            preFocusMuteState[gameSoundSource] = mute;
            if (rocketBgSource != null) preFocusMuteState[rocketBgSource] = mute;
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetMuteAll(!hasFocus);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        AudioListener.volume = pauseStatus ? 0.0f : 1.0f;
    }
}
