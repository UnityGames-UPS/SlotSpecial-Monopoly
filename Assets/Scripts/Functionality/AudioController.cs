using System.Collections.Generic;
using UnityEngine;
internal class AudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgMusicSource;
    [SerializeField] private AudioSource gameSoundSource; // reel spin loop + its stop hit only — the one case where stopping a loop and replacing it with a one-shot is the desired behavior
    [SerializeField] private AudioSource sfxSource;        // every other one-shot SFX, via PlayOneShot so they layer instead of cutting each other off

    [Header("Background")]
    [SerializeField] private AudioClip bgMusic; // bg.mp3

    [Header("Reel Sounds")]
    [SerializeField] private AudioClip reelSpinning;   // reel spining.mp3
    [SerializeField] private AudioClip reelStop;        // reel stop.mp3

    [Header("UI Sounds")]
    [SerializeField] private AudioClip uiButton;        // universal all button.mp3
    [SerializeField] private AudioClip maxBet;          // max bet.mp3
    [SerializeField] private AudioClip turboActivate;   // turbo rocket.mp3
    [SerializeField] private AudioClip autoplayOpen;    // hold for auto.mp3
    [SerializeField] private AudioClip autoplaySelect;  // hold for auto numbers select.mp3

    [Header("Win Line Sounds")]
    [SerializeField] private AudioClip winLineIntro;      // icons in machine.mp3 — one-shot at the start of a win line display
    [SerializeField] private AudioClip paylineHighlight;  // paylines in slot.mp3 — plays as each line is cycled through

    [Header("Free Spins")]
    [SerializeField] private AudioClip scatterTrigger;      // roulette scatter.mp3

    [Header("Base Win")]
    [SerializeField] private AudioClip normalSmallWin;   // normal small win.mp3 — base-spin win too small for the Big/Huge/Mega popup

    [Header("Ludo Bonus Board")]
    [SerializeField] private AudioClip diceJump;          // dice jump.mp3
    [SerializeField] private AudioClip ludoAddRolls;      // +2+3+4 in ludo.mp3
    [SerializeField] private AudioClip ludoRoundEnd;      // end in ludo.mp3
    [SerializeField] private AudioClip ludoCharacterMove; // man move ludo.mp3
    [SerializeField] private AudioClip ludoGoldenMagic;   // man golden magic animation in ludo.mp3 — coin/vault landing shine

    [Header("Character (shared: ludo board + free spins)")]
    [SerializeField] private AudioClip characterAppear;  // man appear.mp3 — pop_left
    [SerializeField] private AudioClip characterSmoke;   // man smoke.mp3 — action_left

    [Header("Golden Multiplier / Red Monopoly")]
    [SerializeField] private AudioClip monopolyChange;     // man x2.mp3 — symbol-0 to Red Monopoly conversion animation
    [SerializeField] private AudioClip monopolyWinReveal;  // purple box in slot win.mp3 — Red Monopoly win-line highlight

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
        if (sfxSource) sfxSource.volume = _sfxVolume;
        MuteGame(_sfxVolume <= 0f);
    }

    private void Start()
    {
        if (bgMusicSource) bgMusicSource.volume = _musicVolume;
        if (gameSoundSource) gameSoundSource.volume = _sfxVolume;
        if (sfxSource) sfxSource.volume = _sfxVolume;

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
        PlaySfx(uiButton);
    }

    internal void PlayMaxBet()
    {
        PlaySfx(maxBet);
    }

    internal void PlayTurboActivate()
    {
        PlaySfx(turboActivate);
    }

    internal void PlayAutoplayOpen()
    {
        PlaySfx(autoplayOpen);
    }

    internal void PlayAutoplaySelect()
    {
        PlaySfx(autoplaySelect);
    }

    internal void PlayWinLineIntro()
    {
        PlaySfx(winLineIntro);
    }

    internal void PlayPaylineHighlight()
    {
        PlaySfx(paylineHighlight);
    }

    internal void PlayScatterTrigger()
    {
        PlaySfx(scatterTrigger);
    }

    internal void PlayFreeSpinsWon()
    {
        //PlaySfx(freeSpinsWon);
    }

    internal void PlayFreeSpinsWinAmount()
    {
        //PlaySfx(freeSpinsWinAmount);
    }

    internal void PlayNormalSmallWin()
    {
        PlaySfx(normalSmallWin);
    }

    internal void PlayDiceJump()
    {
        PlaySfx(diceJump);
    }

    internal void PlayLudoAddRolls()
    {
        PlaySfx(ludoAddRolls);
    }

    internal void PlayLudoRoundEnd()
    {
        PlaySfx(ludoRoundEnd);
    }

    internal void PlayLudoCharacterMove()
    {
        PlaySfx(ludoCharacterMove);
    }

    internal void PlayLudoGoldenMagic()
    {
        PlaySfx(ludoGoldenMagic);
    }

    internal void PlayCharacterAppear()
    {
        PlaySfx(characterAppear);
    }

    internal void PlayCharacterSmoke()
    {
        PlaySfx(characterSmoke);
    }

    internal void PlayMonopolyChange()
    {
        PlaySfx(monopolyChange);
    }

    internal void PlayMonopolyWinReveal()
    {
        PlaySfx(monopolyWinReveal);
    }

    // Stop-and-replace — only for the reel spin loop and its stop hit, where interrupting the
    // currently playing clip is the desired behavior (starting a new spin should cut the old one).
    private void PlayGame(AudioClip clip, bool loop)
    {
        if (!clip) return;

        gameSoundSource.Stop();
        gameSoundSource.clip = clip;
        gameSoundSource.loop = loop;
        gameSoundSource.Play();
    }

    // Fire-and-forget one-shot — layers on top of whatever else sfxSource is already playing
    // instead of cutting it off, so the frequent ludo-board/dice hits don't silence UI clicks
    // or each other.
    private void PlaySfx(AudioClip clip)
    {
        if (!clip || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
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

        AudioSource[] sources = { bgMusicSource, gameSoundSource, sfxSource };
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
        if (sfxSource != null) sfxSource.mute = mute;
        if (isForceMuted)
        {
            preFocusMuteState[gameSoundSource] = mute;
            if (sfxSource != null) preFocusMuteState[sfxSource] = mute;
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
