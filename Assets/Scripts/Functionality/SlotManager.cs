using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System;
using System.Collections;
using Spine.Unity;
using Unity.VisualScripting;

public class SlotManager : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite[] _symbolSprites;
    [SerializeField] private List<AnimationSprites> symbolAnimations;

    [SerializeField] private Sprite rightScatterSymbol;
    [SerializeField] private Sprite leftScatterSymbol;

    [Header("Slot Images")]
    [SerializeField] private List<SlotImage> _totalImages;
    [SerializeField] internal List<SlotImage> _resultImages;
    [SerializeField] private List<ImageAnimation> reelBgs;
    [SerializeField] private List<SlotImage> winAnimationImages;
    [SerializeField] internal List<SlotImage> SlotOverlays;
    [SerializeField] internal List<SlotImage> WinFrames;

    [SerializeField] private List<SlotImage> DiceImages;

    [Header("Character Animations")]
    [SerializeField] internal GameObject characterAnimationParent;
    [SerializeField] private SpineAnimController spineAnimController;

    [SerializeField] private GameObject NormalCharacterAnimationParentLandscape;
    [SerializeField] private GameObject NormalCharacterAnimationParentPortrait;
    [SerializeField] private GameObject BonusCharacterAnimationParentBoth;

    [SerializeField] private Vector2 characterNormalAnimationLandscapePosition;
    [SerializeField] private Vector2 characterNormalAnimationPortraitPosition;
    [SerializeField] private Vector3 characterNormalAnimationLandscapeScale;
    [SerializeField] private Vector3 characterNormalAnimationPortraitScale;

    [SerializeField] private Vector2 characterBonusAnimationLandscapePosition;
    [SerializeField] private Vector2 characterBonusAnimationPortraitPosition;
    [SerializeField] private Vector3 characterBonusAnimationLandscapeScale;
    [SerializeField] private Vector3 characterBonusAnimationPortraitScale;

    internal enum CharacterAnimState { Normal, Bonus }
    private CharacterAnimState? _activeCharacterState;

    [Header("Free Spin Sprites and Animations")]
    [SerializeField] private List<Sprite> dicerollingAnimation;
    [SerializeField] private List<Sprite> dicedestroyingAnimation;

    [SerializeField] private GameObject LaserParent;
    [SerializeField] private GameObject LaserPrefab;
    [SerializeField] private Transform LaserShootPosition;

    [SerializeField] private Sprite OneXMultiplier;
    [SerializeField] private Sprite TwoXMultiplier;
    [SerializeField] private Sprite FourXMultiplier;
    [SerializeField] private Sprite SixMultiplier;
    [SerializeField] private Sprite EightXMultiplier;

    [SerializeField] private List<Sprite> OneXMultiplierAnimation;
    [SerializeField] private List<Sprite> TwoXMultiplierAnimation;
    [SerializeField] private List<Sprite> FourXMultiplierAnimation;
    [SerializeField] private List<Sprite> SixXMultiplierAnimation;
    [SerializeField] private List<Sprite> EightXMultiplierAnimation;

    [Header("Free Spin Scatter Ticket")]
    [SerializeField] private RectTransform scatterTicketImage;
    [SerializeField] private float scatterTicketFlyDuration = 1f;
    [SerializeField] private float scatterTicketWaitDuration = 2f;
    [SerializeField] private float scatterTicketScaleUpDuration = 0.7f;
    [SerializeField] private float scatterTicketScaleUpTarget = 2f;
    [SerializeField] private float scatterTicketScaleDownDuration = 0.7f;
    [SerializeField] private float mainSlotFreeSpinOffsetX = 190f;

    [Header("Free Spin Scatter Combine")]
    [SerializeField] private RectTransform scatterCombineAnimationParent; // parent to spawn the combined-ticket object under (controls sort order)
    [SerializeField] private RectTransform scatterCombinedTicketPrefab;   // prefab with Image + ImageAnimation, frames pre-assigned in the Inspector
    [SerializeField] private float scatterCombineMoveDuration = 0.6f;     // time for both half-tickets to travel to the meeting point

    [Header("Free Spin Dice Sequence")]
    [SerializeField] private float diceFadeScaleDuration = 0.4f;
    [SerializeField] private float diceRollDuration = 2f;
    [SerializeField] private float laserTravelDuration = 0.5f;

    private Vector3 _scatterTicketInitialLocalPos;
    private Quaternion _scatterTicketInitialLocalRot;

    [Header("Slots Transforms")]
    [SerializeField] private Transform[] _slotTransforms;
    [SerializeField] private RectTransform mainSlotTransform;

    [Header("Bgs")]
    [SerializeField] private GameObject landscapeBackground;
    [SerializeField] private GameObject portraitBackground;

    [Header("WinLine UI")]
    [SerializeField] private TMP_Text winAmountBigText;
    [SerializeField] private List<TMP_Text> lineWinTexts;

    [Header("Managers")]
    [SerializeField] private AudioController audioController;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private BonusManager bonusManager;
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private OrientationChange orientationChange;

    [Header("Bonus Zoom")]
    [SerializeField] private float bonusZoomInDuration = 0.3f;
    [SerializeField] private float bonusZoomOutDuration = 0.1f;
    [SerializeField] private float bonusZoomScaleLandscape = 1.2f;
    [SerializeField] private float bonusZoomScalePortrait = 0.9f;


    internal bool SocketConnected = false;
    internal bool _isAutoSpin = false;
    internal bool isInFreeSpins = false;
    internal bool isBonus = false;
    internal int freeSpinsRemaining = 0;
    private bool _wasAutoSpinOn;
    private List<Tween> _alltweens = new List<Tween>();
    private Coroutine _autoSpinRoutine = null;
    private Coroutine _tweenRoutine;
    private Coroutine _winLinesLoopRoutine;
    private bool _isSpinning = false;
    internal int _numberOfSlots = 5;
    private bool _stopSpinToggle;
    private float _spinDelay = 0.3f;
    private bool _isTurboOn;
    internal bool IsTurboOn => _isTurboOn;
    private bool isTweening = false;
    private double _freeSpinsRoundWinTotal = 0;
    internal double FreeSpinsRoundWinTotal => _freeSpinsRoundWinTotal;

    internal enum SpinSpeed { Normal, Turbo, QuickSpin }
    internal SpinSpeed currentSpinSpeed = SpinSpeed.Normal;

    internal void SetSpinSpeed(SpinSpeed speed)
    {
        currentSpinSpeed = speed;
        _isTurboOn = (speed != SpinSpeed.Normal);
        _spinDelay = speed switch
        {
            SpinSpeed.Normal => 0.3f,
            SpinSpeed.Turbo => 0.2f,
            SpinSpeed.QuickSpin => 0.05f,
            _ => 0.3f
        };
    }

    #region Initial Functions

    private void Start()
    {
        shuffleSlotImages();

        SetupSymbolClickHandlers();

        if (scatterTicketImage != null)
        {
            _scatterTicketInitialLocalPos = scatterTicketImage.localPosition;
            _scatterTicketInitialLocalRot = scatterTicketImage.localRotation;
        }
    }

    private void SetupSymbolClickHandlers()
    {
        if (uiManager == null || _resultImages == null) return;

        for (int col = 0; col < _resultImages.Count; col++)
        {
            for (int row = 0; row < _resultImages[col].slotImages.Count; row++)
            {
                Image symbolImage = _resultImages[col].slotImages[row];
                if (symbolImage == null) continue;

                SymbolClickHandler handler = symbolImage.GetComponent<SymbolClickHandler>();
                if (handler == null) handler = symbolImage.gameObject.AddComponent<SymbolClickHandler>();
                handler.Init(col, row, this, uiManager);
            }
        }
    }

    internal void shuffleSlotImages(bool midTween = false)
    {
        foreach (var slotImg in _totalImages)
        {
            for (int j = 0; j < slotImg.slotImages.Count; j++)
            {
                int randomSprite = UnityEngine.Random.Range(0, 11);
                // Sprite image = (j % 2 == 0)
                //     ? _symbolSprites[randomSprite] // random symbol
                //     : _symbolSprites[0]; // blank

                Sprite image = _symbolSprites[randomSprite];
                slotImg.slotImages[j].sprite = image;
                slotImg.slotImages[j].preserveAspect = true;
                SetSymbolSize(slotImg.slotImages[j], randomSprite);
            }
        }
    }

    #endregion

    #region Autospin
    internal int autoSpinRoundsRemaining = -1;

    internal void AutoSpin()
    {
        if (!_isAutoSpin)
        {
            _isAutoSpin = true;

            if (_autoSpinRoutine != null)
            {
                StopCoroutine(_autoSpinRoutine);
                _autoSpinRoutine = null;
            }
            _autoSpinRoutine = StartCoroutine(AutoSpinCoroutine());
        }
    }

    internal void AutoSpin(int rounds)
    {
        autoSpinRoundsRemaining = rounds;
        AutoSpin();
    }

    internal void StopAutoSpin()
    {
        //_audioController.PlayButtonAudio();
        if (_isAutoSpin)
        {
            _isAutoSpin = false;
            _wasAutoSpinOn = false;
            autoSpinRoundsRemaining = -1;
            StartCoroutine(StopAutoSpinCoroutine());
        }
    }

    private IEnumerator AutoSpinCoroutine()
    {
        while (_isAutoSpin)
        {
            StartSlots(_isAutoSpin);
            yield return new WaitUntil(() => !_isSpinning);
            // Free spins (once triggered) chain themselves automatically inside TweenRoutine, and
            // briefly flip _isSpinning false between each chained spin — wait out the whole free-spin
            // bonus round (isInFreeSpins) so it doesn't count as multiple autospin rounds.
            yield return new WaitUntil(() => !isInFreeSpins);

            if (autoSpinRoundsRemaining > 0)
            {
                autoSpinRoundsRemaining--;
                uiManager.UpdateAutoPlayCount();
                if (autoSpinRoundsRemaining == 0)
                {
                    StopAutoSpin();
                    break;
                }
            }

            yield return new WaitForSeconds(_spinDelay);
        }
        if (_wasAutoSpinOn)
            _wasAutoSpinOn = false;
    }

    private IEnumerator StopAutoSpinCoroutine()
    {
        yield return new WaitUntil(() => !_isSpinning);
        _isAutoSpin = false;
        uiManager.SetAutoSpinButtonInteractable(true);
        if (_autoSpinRoutine != null)
        {
            StopCoroutine(_autoSpinRoutine);
            _autoSpinRoutine = null;
        }
        // The current spin just finished and autospin is now fully cancelled.
        // Tell UIManager to restore the spin button now that we are truly idle.
        uiManager.SetSpinButtonReady();
    }
    #endregion

    internal void RequestInstantStop()
    {
        if (isTweening)
        {
            _stopSpinToggle = true;
        }
    }

    private OrientationChange GetOrientationChange()
    {
        if (orientationChange != null) return orientationChange;
        orientationChange = FindFirstObjectByType<OrientationChange>();
        return orientationChange;
    }

    #region Character Animation

    private void OnEnable()
    {
        OrientationChange.OnOrientationChanged += HandleCharacterOrientationChanged;
    }

    private void OnDisable()
    {
        OrientationChange.OnOrientationChanged -= HandleCharacterOrientationChanged;
    }

    private void HandleCharacterOrientationChanged(OrientationChange.OrientationMode mode, int width, int height)
    {
        // Only reposition the character while a sequence has actually placed it on screen —
        // an orientation flip with no active sequence has nothing to re-orient.
        if (_activeCharacterState.HasValue)
        {
            ApplyCharacterOrientationVisuals(_activeCharacterState.Value);
        }
    }

    // Marks which position/scale/rotation set (Normal vs Bonus) the character currently belongs
    // to and applies it immediately. Kept live via HandleCharacterOrientationChanged until
    // DeactivateCharacterAnimation() clears it.
    internal void SetCharacterOrientationState(CharacterAnimState state)
    {
        _activeCharacterState = state;
        ApplyCharacterOrientationVisuals(state);
    }

    private void ApplyCharacterOrientationVisuals(CharacterAnimState state)
    {
        if (characterAnimationParent == null) return;

        var currentOrientation = GetOrientationChange();
        bool isPortrait = currentOrientation != null && currentOrientation.CurrentMode == OrientationChange.OrientationMode.MobilePortrait;

        // Bonus shares one parent across both orientations; Normal has a separate parent per orientation.
        GameObject targetParent = state == CharacterAnimState.Bonus
            ? BonusCharacterAnimationParentBoth
            : (isPortrait ? NormalCharacterAnimationParentPortrait : NormalCharacterAnimationParentLandscape);

        if (targetParent != null && characterAnimationParent.transform.parent != targetParent.transform)
        {
            // worldPositionStays: false — anchoredPosition/localScale below are set explicitly
            // right after, so there's nothing worth preserving across the reparent.
            characterAnimationParent.transform.SetParent(targetParent.transform, false);
        }

        RectTransform rt = characterAnimationParent.GetComponent<RectTransform>();
        if (rt != null)
        {
            if (state == CharacterAnimState.Bonus)
            {
                rt.anchoredPosition = isPortrait ? characterBonusAnimationPortraitPosition : characterBonusAnimationLandscapePosition;
                rt.localScale = isPortrait ? characterBonusAnimationPortraitScale : characterBonusAnimationLandscapeScale;
            }
            else
            {
                rt.anchoredPosition = isPortrait ? characterNormalAnimationPortraitPosition : characterNormalAnimationLandscapePosition;
                rt.localScale = isPortrait ? characterNormalAnimationPortraitScale : characterNormalAnimationLandscapeScale;
            }
        }

        // Bonus animations always face rotated 180 on Y; normal animations only flip in portrait.
        float rotationY = state == CharacterAnimState.Bonus ? 180f : (isPortrait ? 180f : 0f);
        if (spineAnimController != null)
        {
            spineAnimController.transform.localRotation = Quaternion.Euler(0f, rotationY, 0f);
        }
    }

    // Sets animName and plays it, overriding whatever was playing/looping before (matches the
    // manual isPlaying-reset pattern SpineAnimController already expects between clips). Waits
    // for the clip's duration when not looping so the caller can chain the next clip immediately.
    internal IEnumerator PlayCharacterAnim(string animName, bool loop)
    {
        if (spineAnimController == null) yield break;

        spineAnimController.isPlaying = false;
        spineAnimController.animName = animName;
        spineAnimController.Play(loop);

        if (!loop)
        {
            float duration = spineAnimController.GetAnimationDuration();
            yield return new WaitForSeconds(duration);
            spineAnimController.isPlaying = false;
        }
    }

    // Fire-and-forget variant for looping clips (idle_left) — no wait, caller continues immediately.
    internal void PlayCharacterAnimLoop(string animName)
    {
        if (spineAnimController == null) return;

        spineAnimController.isPlaying = false;
        spineAnimController.animName = animName;
        spineAnimController.Play(true);
    }

    internal void DeactivateCharacterAnimation()
    {
        if (characterAnimationParent != null) characterAnimationParent.SetActive(false);
        _activeCharacterState = null;
    }

    #endregion

    #region SlotSpin
    internal void StartSlots(bool autoSpin = false)
    {
        if (_isSpinning) return;

        if (!autoSpin)
        {
            if (_autoSpinRoutine != null)
            {
                StopCoroutine(_autoSpinRoutine);
                StopCoroutine(_tweenRoutine);
                _tweenRoutine = null;
                _autoSpinRoutine = null;
            }
        }
        _tweenRoutine = StartCoroutine(TweenRoutine());
    }

    private IEnumerator TweenRoutine()
    {
        _isSpinning = true;
        isBonus = false;

        // No-op unless the between-chained-free-spins placeholder was showing.
        uiManager.HideFreeSpinBetweenSpinsPlaceholder();

        StopWinLinesLoop();

        if (!isInFreeSpins)
        {
            if (uiManager.currentBalance < uiManager.currentTotalBet)
            {
                StopAutoSpin();
                _isSpinning = false;
                yield return new WaitForSeconds(1);
                uiManager.LowBalPopup();
                uiManager.SetBetButtonsInteractable(true);
                yield break;
            }

            uiManager.UpdateBalance(uiManager.currentBalance - uiManager.currentTotalBet);
            uiManager.currentBalance = uiManager.currentBalance - uiManager.currentTotalBet;
            //Debug.Log(uiManager.currentTotalBet);
        }
        else
        {
            uiManager.UpdateFreeSpinCount(--freeSpinsRemaining);
        }
        // During free spins, keep showing the accumulated round total instead of resetting to 0 each spin.
        uiManager.UpdateWin(isInFreeSpins ? _freeSpinsRoundWinTotal : 0.00);
        yield return null;

        ResetOverlays();

        //if (_audioController) _audioController.PlayWLAudio("spin");

        _isSpinning = true;
        isTweening = true;

        for (int i = 0; i < _numberOfSlots; i++)
        {
            InitializeTweening(_slotTransforms[i]);
        }
        audioController.PlayReelSpinning(true);

        ResetAllSymbol();

        socketManager.AccumulateResult(uiManager.betCounter);
        yield return new WaitUntil(() => socketManager.isResultdone);
        // Backend sends the running cumulative free-spin win in freeGames.totalWinCash on every
        // free spin — mirror it locally so the win-amount UI reflects it instead of staying at 0.
        if (isInFreeSpins && socketManager.resultData.payload.freeGames != null)
        {
            _freeSpinsRoundWinTotal = socketManager.resultData.payload.freeGames.totalWinCash;
        }
        // // Load result matrix into result images
        int? reel3ScatterRow = null;
        int? reel4ScatterRow = null;
        for (int j = 0; j < socketManager.resultData.matrix.Count; j++)
        {
            for (int i = 0; i < socketManager.resultData.matrix[j].Count; i++)
            {
                if (int.TryParse(socketManager.resultData.matrix[j][i], out int symbolId))
                {
                    _resultImages[i].slotImages[j].sprite = _symbolSprites[symbolId];
                    _resultImages[i].slotImages[j].preserveAspect = true;
                    SetSymbolSize(_resultImages[i].slotImages[j], symbolId);

                    if (i == 3 && symbolId == 12)
                    {
                        _resultImages[i].slotImages[j].sprite = rightScatterSymbol;
                        _resultImages[i].slotImages[j].preserveAspect = false;
                        reel3ScatterRow = j;
                    }
                    else if (i == 4 && symbolId == 12)
                    {
                        _resultImages[i].slotImages[j].sprite = leftScatterSymbol;
                        _resultImages[i].slotImages[j].preserveAspect = false;
                        reel4ScatterRow = j;
                    }

                    if (symbolId == 13)
                    {
                        // if (isInFreeSpins)
                        // {
                        //     var md = socketManager.resultData.payload.magicDiceMultipliers
                        //         ?.Find(m => m.row == j && m.col == i);
                        //     if (md != null)
                        //         _resultImages[i].slotImages[j].sprite = GetMultiplierSprite(md.multiplier);
                        // }
                        // else
                        {
                            int randomSprite = UnityEngine.Random.Range(1, 9);
                            _resultImages[i].slotImages[j].sprite = _symbolSprites[randomSprite];
                        }
                    }
                }
            }
        }

        if (_isTurboOn)
        {
            _stopSpinToggle = true;
        }

        int bonusSymbolCount = 0;
        if (!_stopSpinToggle)
        {
            for (int i = 0; i < _numberOfSlots; i++)
            {
                yield return new WaitForSeconds(0.2f);
                if (_stopSpinToggle) break;
            }
        }
        bool bonusThresholdReached = false;
        Vector3 mainSlotRestScale = mainSlotTransform.localScale;
        Vector3 landscapeBgRestScale = landscapeBackground.transform.localScale;
        Vector3 portraitBgRestScale = portraitBackground.transform.localScale;
        var currentOrientation = GetOrientationChange();
        bool isPortraitOrientation = currentOrientation != null && currentOrientation.CurrentMode == OrientationChange.OrientationMode.MobilePortrait;
        for (int i = 0; i < _numberOfSlots; i++)
        {
            if (!bonusThresholdReached && bonusSymbolCount >= 2) bonusThresholdReached = true;

            // Stop button / Turbo / Quick Spin (which already force _stopSpinToggle true before
            // this loop even starts) should never show the zoom at all — and a Stop press mid-hold
            // should cut it immediately rather than waiting the animation out.
            bool playZoomThisReel = bonusThresholdReached && !_stopSpinToggle;

            if (playZoomThisReel)
            {
                mainSlotTransform.DOKill();
                landscapeBackground.transform.DOKill();
                portraitBackground.transform.DOKill();

                float targetScale = isPortraitOrientation ? bonusZoomScalePortrait : bonusZoomScaleLandscape;
                mainSlotTransform.DOScale(targetScale, bonusZoomInDuration);
                landscapeBackground.transform.DOScale(1.15f, bonusZoomInDuration);
                portraitBackground.transform.DOScale(1.15f, bonusZoomInDuration);

                if (reelBgs != null && i < reelBgs.Count && reelBgs[i] != null)
                {
                    reelBgs[i].gameObject.SetActive(true);
                    reelBgs[i].StartAnimation();
                }

                // Polled instead of a flat WaitForSeconds so a Stop press during the hold breaks out immediately.
                float zoomHoldElapsed = 0f;
                while (zoomHoldElapsed < 1.3f && !_stopSpinToggle)
                {
                    yield return null;
                    zoomHoldElapsed += Time.deltaTime;
                }
            }

            yield return StopTweening(_slotTransforms[i], i, _stopSpinToggle);

            if (playZoomThisReel)
            {
                mainSlotTransform.DOKill();
                landscapeBackground.transform.DOKill();
                portraitBackground.transform.DOKill();

                if (_stopSpinToggle)
                {
                    // Stop landed mid-hold — snap back instantly, no animated wind-down.
                    mainSlotTransform.localScale = mainSlotRestScale;
                    landscapeBackground.transform.localScale = landscapeBgRestScale;
                    portraitBackground.transform.localScale = portraitBgRestScale;
                }
                else
                {
                    mainSlotTransform.DOScale(mainSlotRestScale, bonusZoomOutDuration);
                    landscapeBackground.transform.DOScale(landscapeBgRestScale, bonusZoomOutDuration);
                    portraitBackground.transform.DOScale(portraitBgRestScale, bonusZoomOutDuration);
                }

                if (reelBgs != null && i < reelBgs.Count && reelBgs[i] != null)
                {
                    reelBgs[i].gameObject.SetActive(false);
                    reelBgs[i].StopAnimation();
                }

                if (!_stopSpinToggle)
                {
                    // Let the reset fully play out before the next reel's zoom-in fires,
                    // instead of the next iteration's DOKill() cutting it short.
                    yield return new WaitForSeconds(bonusZoomOutDuration);
                }
            }

            for (int j = 0; j < _resultImages[i].slotImages.Count; j++)
            {
                if (_resultImages[i].slotImages[j].sprite == _symbolSprites[11])
                {
                    Transform symbolTransform = _resultImages[i].slotImages[j].transform;
                    symbolTransform.DOScale(1.5f, 0.3f).OnComplete(() =>
                    {
                        symbolTransform.DOScale(1.28f, 0.3f);
                    });
                    bonusSymbolCount++;
                }
            }
            audioController.PlayReelStop();
        }
        isTweening = false;
        _stopSpinToggle = false;

        yield return _alltweens[^1].WaitForCompletion();
        KillAllTweens();

        // Reels have fully stopped — nothing left to instant-stop, so the stop button goes away
        // right here rather than staying visible through any heatup/free-spin/wheel-trigger animations.
        uiManager.HideStopButtonAfterReelsStopped();

        // Checkers Bonus plays out fully (cloud-in, board reveal, all rolls, cloud-out, win
        // popup) before any base-spin win popup / line-win display below — control only returns
        // here once the whole round has finished.
        if (socketManager.resultData.payload.checkersBonus != null &&
            socketManager.resultData.payload.checkersBonus.triggered)
        {
            isBonus = true;

            //SpineAnimation
            characterAnimationParent.SetActive(true);
            SetCharacterOrientationState(CharacterAnimState.Normal);
            yield return PlayCharacterAnim("pop_left", false);
            yield return PlayCharacterAnim("action_left", false);

            // Fall back to 3 whenever the config value is missing OR zero — initData's
            // checkersBonus config isn't always populated with a positive rollsCount, and a
            // starting count of 0 would end the round right after its very first roll.
            int startingRolls = socketManager.features?.checkersBonus?.rollsCount ?? 3;
            if (startingRolls <= 0) startingRolls = 3;
            yield return bonusManager.PlayBonusRound(socketManager.resultData.payload.checkersBonus, startingRolls);
        }

        // Reels have fully stopped now — safe to reveal the updated free-spin count/total-win.
        if (isInFreeSpins)
        {
            uiManager.UpdateFreeSpinCount(freeSpinsRemaining);
            uiManager.UpdateFreeSpinTotalWin(_freeSpinsRoundWinTotal);
        }

        if (isInFreeSpins)
        {
            yield return PlayMagicDiceSequence();
        }

        if (socketManager.resultData.payload.isFreeSpinTriggered)
        {
            freeSpinsRemaining = socketManager.resultData.payload.freeGames.totalSpins;
            _freeSpinsRoundWinTotal = 0;

            yield return PlayScatterSymbolsCombineIntro(reel3ScatterRow, reel4ScatterRow);
            yield return PlayFreeSpinCharacterIntro();
            StopScatterSymbolsCombineIntro();
            yield return PlayScatterTicketSequence(mainSlotFreeSpinOffsetX);

            uiManager.OnFreeSpinsTriggered(freeSpinsRemaining);
        }

        // Big/Huge/Mega win popup only applies to a normal (non free-spin) spin's own win —
        // shown before the win-line loop, which then plays once it's closed (Take, or the
        // 3s autoplay auto-close after Take becomes interactable).
        if (!isInFreeSpins && !isBonus)
        {
            var popupType = uiManager.GetWinPopupType(socketManager.resultData.payload.winAmount);
            if (popupType.HasValue)
            {
                bool popupClosed = false;
                uiManager.ShowUniversalWinPopup(popupType.Value, socketManager.resultData.payload.winAmount, _isAutoSpin, () => popupClosed = true);
                yield return new WaitUntil(() => popupClosed);
            }
        }

        if (socketManager.resultData.payload.lineWins.Count > 0 && !socketManager.resultData.payload.isFreeSpinTriggered)
        {
            yield return ShowWinLineAnimation(socketManager.resultData.payload.lineWins, socketManager.resultData.payload.winAmount);
        }

        _isSpinning = false;
        uiManager.currentBalance = socketManager.resultData.player.balance;

        if (isInFreeSpins)
        {
            if (socketManager.resultData.payload.isFreeSpinActive)
            {
                uiManager.ShowFreeSpinBetweenSpinsPlaceholder();
                yield return new WaitForSeconds(_spinDelay);
                StartSlots(true);
            }
            else
            {
                ResetOverlays();
                if (socketManager.resultData.payload.freeGames.totalWinCash > 0)
                {
                    var freeSpinPopupType = uiManager.GetWinPopupType(socketManager.resultData.payload.freeGames.totalWinCash)
                        ?? UIManager.WinPopupType.BigWin;
                    bool freeSpinPopupClosed = false;
                    uiManager.ShowUniversalWinPopup(freeSpinPopupType, socketManager.resultData.payload.freeGames.totalWinCash, _isAutoSpin, () => freeSpinPopupClosed = true);
                    yield return new WaitUntil(() => freeSpinPopupClosed);
                }

                yield return PlayScatterTicketSequence(0f, hideCharacterAfterScaleUp: true);

                uiManager.OnFreeSpinsEnded(_freeSpinsRoundWinTotal);
            }
        }
        else if (!socketManager.resultData.payload.isFreeSpinTriggered)
        {
            // Free spins were just triggered above (Start button now showing) — leave the
            // spin/stop buttons alone until StartFreeSpinsSequence kicks the bonus round off.
            uiManager.SetSpinButtonReady();
        }
    }
    #endregion

    #region Win Line Animation

    private IEnumerator ShowWinLineAnimation(List<LineWin> winLines, double winAmount, bool oneShot = false)
    {
        audioController.PlayWinLineIntro();
        // During free spins the "current win" text mirrors the running free-spin total, same as UpdateFreeSpinTotalWin.
        uiManager.UpdateWin(isInFreeSpins ? _freeSpinsRoundWinTotal : winAmount, true);
        uiManager.UpdateBalance(uiManager.currentBalance + winAmount, true);

        if (_isAutoSpin || oneShot || isInFreeSpins)
        {
            // Autospin/free-spins: flash every winning line together once, then normally let
            // the next spin proceed — unless autoplay gets stopped while that's showing, in
            // which case switch to the manual-style per-line loop instead of silently ending.
            yield return ShowCombinedWinLinesHighlight(winLines);

            if (!oneShot && !isInFreeSpins && !_isAutoSpin)
            {
                _winLinesLoopRoutine = StartCoroutine(LoopWinLines(winLines));
            }
        }
        else
        {
            // Manual spin: keep cycling the winning lines until the player starts the next spin.
            yield return ShowCombinedWinLinesHighlight(winLines);
            _winLinesLoopRoutine = StartCoroutine(LoopWinLines(winLines));
        }
    }

    // Flashes every winning line together once — used both as the whole display for
    // autospin/free-spins (normally) and as the lead-in before LoopWinLines for manual spins.
    private IEnumerator ShowCombinedWinLinesHighlight(List<LineWin> winLines)
    {
        for (int j = 0; j < SlotOverlays.Count; j++)
        {
            for (int k = 0; k < SlotOverlays[j].slotImages.Count; k++)
            {
                SlotOverlays[j].slotImages[k].gameObject.SetActive(true);
                WinFrames[j].slotImages[k].gameObject.SetActive(false);
                winAnimationImages[j].slotImages[k].GetComponent<ImageAnimation>().StopAnimation();
                winAnimationImages[j].slotImages[k].gameObject.SetActive(false);
                _resultImages[j].slotImages[k].gameObject.SetActive(true);
            }
        }

        winAmountBigText.gameObject.SetActive(true);
        winAmountBigText.text = socketManager.resultData.payload.winAmount.ToString("F2");

        for (int j = 0; j < winLines.Count; j++)
        {
            foreach (var line in winLines[j].positions)
            {
                int col = line.position[0];
                int row = line.position[1];
                //SlotOverlays[row].slotImages[col].gameObject.SetActive(false);
                int symbolID = int.Parse(socketManager.resultData.matrix[col][row]);

                if (symbolID != 0)
                {
                    WinFrames[row].slotImages[col].gameObject.SetActive(true);
                    WinFrames[row].slotImages[col].GetComponent<ImageAnimation>().StartAnimation();
                }


                if (symbolID == 13)
                {
                    var md = socketManager.resultData.payload.magicDiceMultipliers?.Find(m => m.row == row && m.col == col);
                    var anim1 = winAnimationImages[row].slotImages[col].GetComponent<ImageAnimation>();
                    anim1.textureArray = GetMultiplierAnimation(md?.multiplier ?? 1);
                    anim1.doLoopAnimation = true;
                    anim1.AnimationSpeed = GetAnimationSpeed(symbolID);
                    anim1.StartAnimation();
                }
                else
                {
                    winAnimationImages[row].slotImages[col].sprite = _symbolSprites[symbolID];
                }
                winAnimationImages[row].slotImages[col].gameObject.SetActive(true);
                _resultImages[row].slotImages[col].gameObject.SetActive(false);
                SetAnimationSymbolSize(winAnimationImages[row].slotImages[col], symbolID);

                if (symbolID != 13)
                {
                    ImageAnimation anim = winAnimationImages[row].slotImages[col].GetComponent<ImageAnimation>();
                    anim.textureArray = GetAnimationSprite(symbolID);
                    anim.AnimationSpeed = GetAnimationSpeed(symbolID);
                    anim.doLoopAnimation = true;
                    anim.StartAnimation();
                }
            }
        }

        yield return new WaitForSeconds(3f);
    }

    // Cycles the winning lines one at a time, forever. StopWinLinesLoop() (called from
    // TweenRoutine the moment a new spin starts) is what actually ends this.
    private IEnumerator LoopWinLines(List<LineWin> winLines)
    {
        winAmountBigText.gameObject.SetActive(false);
        // Cells (row, col) that were left highlighted/animating by the previously shown line.
        // Cells common to the previous line and the current one are left untouched below so
        // their WinFrames/winAnimationImages animation keeps playing instead of restarting.
        var previousPositions = new HashSet<(int row, int col)>();
        while (true)
        {
            for (int i = 0; i < winLines.Count; i++)
            {
                var currentPositions = new HashSet<(int row, int col)>();
                foreach (var line in winLines[i].positions)
                {
                    currentPositions.Add((line.position[1], line.position[0]));
                }

                for (int j = 0; j < SlotOverlays.Count; j++)
                {
                    for (int k = 0; k < SlotOverlays[j].slotImages.Count; k++)
                    {
                        // Still part of the highlighted set from the previous line and this one
                        // -> leave it fully alone so its animation keeps playing uninterrupted.
                        if (previousPositions.Contains((j, k)) && currentPositions.Contains((j, k)))
                            continue;

                        SlotOverlays[j].slotImages[k].gameObject.SetActive(true);
                        WinFrames[j].slotImages[k].gameObject.SetActive(false);
                        winAnimationImages[j].slotImages[k].GetComponent<ImageAnimation>().StopAnimation();
                        _resultImages[j].slotImages[k].gameObject.SetActive(true);
                        winAnimationImages[j].slotImages[k].gameObject.SetActive(false);
                    }
                }

                foreach (var line in winLines[i].positions)
                {
                    int col = line.position[0];
                    int row = line.position[1];
                    int symbolID = int.Parse(socketManager.resultData.matrix[col][row]);
                    //SlotOverlays[row].slotImages[col].gameObject.SetActive(false);

                    // Already animating for the previous line and still part of this one ->
                    // skip re-activating/restarting it, let it keep animating as-is.
                    if (previousPositions.Contains((row, col)))
                    {
                        WinFrames[row].slotImages[col].GetComponent<ImageAnimation>().StartAnimation();
                        continue;
                    }

                    if (symbolID != 0)
                    {
                        WinFrames[row].slotImages[col].gameObject.SetActive(true);
                        WinFrames[row].slotImages[col].GetComponent<ImageAnimation>().StartAnimation();
                    }

                    if (symbolID == 13)
                    {
                        var md = socketManager.resultData.payload.magicDiceMultipliers?.Find(m => m.row == row && m.col == col);
                        var anim1 = winAnimationImages[row].slotImages[col].GetComponent<ImageAnimation>();
                        anim1.textureArray = GetMultiplierAnimation(md?.multiplier ?? 1);
                        anim1.AnimationSpeed = GetAnimationSpeed(symbolID);
                        anim1.doLoopAnimation = true;
                        anim1.StartAnimation();
                    }
                    else
                    {
                        winAnimationImages[row].slotImages[col].sprite = _symbolSprites[symbolID];
                    }
                    SetAnimationSymbolSize(winAnimationImages[row].slotImages[col], symbolID);
                    winAnimationImages[row].slotImages[col].gameObject.SetActive(true);
                    _resultImages[row].slotImages[col].gameObject.SetActive(false);

                    if (symbolID != 13)
                    {
                        ImageAnimation anim = winAnimationImages[row].slotImages[col].GetComponent<ImageAnimation>();
                        anim.StopAnimation();
                        anim.textureArray = GetAnimationSprite(symbolID);
                        anim.AnimationSpeed = GetAnimationSpeed(symbolID);
                        anim.doLoopAnimation = true;
                        anim.StartAnimation();
                    }
                }
                audioController.PlayPaylineHighlight();

                foreach (var linewinText in lineWinTexts)
                {
                    linewinText.gameObject.SetActive(false);
                }
                int tempIndex = GetPerLineWinIndex(winLines[i].lineIndex);
                lineWinTexts[tempIndex].text = winLines[i].payout.ToString("F2");
                lineWinTexts[tempIndex].gameObject.SetActive(true);

                previousPositions = currentPositions;

                yield return new WaitForSeconds(2.4f);
            }
        }
    }

    /// <summary>
    /// Stops the repeating win-lines display (if running) and snaps its visuals
    /// back to idle so nothing is left mid-fade or mid-scale when the next spin starts.
    /// </summary>
    private void StopWinLinesLoop()
    {
        if (_winLinesLoopRoutine == null) return;

        StopCoroutine(_winLinesLoopRoutine);
        _winLinesLoopRoutine = null;

        for (int j = 0; j < SlotOverlays.Count; j++)
        {
            for (int k = 0; k < SlotOverlays[j].slotImages.Count; k++)
            {
                SlotOverlays[j].slotImages[k].gameObject.SetActive(false);
                WinFrames[j].slotImages[k].gameObject.SetActive(false);
                _resultImages[j].slotImages[k].gameObject.SetActive(true);
                winAnimationImages[j].slotImages[k].GetComponent<ImageAnimation>().StopAnimation();
                winAnimationImages[j].slotImages[k].gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region TweeningCode

    private void InitializeTweening(Transform slotTransform)
    {
        // Snap to top off-screen
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 800f);

        Tween tween = slotTransform
            .DOLocalMoveY(-800f, 0.27f)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Flash);          // accelerate in, decelerate out → feels weighted
                                           // .OnStepComplete(() =>
                                           // {
                                           //     shuffleSlotImages(midTween: true);
                                           // });

        _alltweens.Add(tween);
    }

    private IEnumerator StopTweening(Transform slotTransform, int index, bool isStop)
    {
        if (!isStop)
        {
            // Wait for the current loop cycle to naturally complete
            // (the reel is at the top — off screen — at this moment)
            bool isComplete = false;
            _alltweens[index].OnStepComplete(() => isComplete = true);
            yield return new WaitUntil(() => isComplete);
        }

        _alltweens[index].Kill();

        // Start from just above the visible area, not from 600
        // This makes the landing feel like a natural continuation of the scroll
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 350f);

        // Smooth slide down with a gentle overshoot — reduce elastic strength
        _alltweens[index] = slotTransform
            .DOLocalMoveY(0f, 0.25f)        // SlotManager landing y (use 400f for BonusManager)
            .SetEase(Ease.OutBack)            // subtle overshoot, not a full elastic bounce
            .SetSpeedBased(false);

        if (!isStop)
            yield return new WaitForSeconds(0.2f);
        else
            yield return null;
    }

    private void KillAllTweens()
    {
        if (_alltweens.Count > 0)
        {
            for (int i = 0; i < _alltweens.Count; i++)
                _alltweens[i].Kill();
            _alltweens.Clear();
        }
    }

    #endregion
    #region Free Spins

    private Image _scatterCombineReel3Image;
    private Image _scatterCombineReel4Image;
    private Vector3 _scatterCombineReel3OriginalPos;
    private Vector3 _scatterCombineReel4OriginalPos;
    private GameObject _spawnedScatterCombineObject;

    // Plays before the character intro: activates every SlotOverlay (dims the board), reveals the
    // two half-ticket win-animation images at their landed positions, slides them toward each other
    // until they meet, then spawns the combined-ticket prefab there and loops its ImageAnimation.
    // Runs (and keeps looping) through PlayFreeSpinCharacterIntro; StopScatterSymbolsCombineIntro()
    // tears it down right before PlayScatterTicketSequence starts.
    private IEnumerator PlayScatterSymbolsCombineIntro(int? reel3Row, int? reel4Row)
    {
        if (!reel3Row.HasValue || !reel4Row.HasValue) yield break;

        for (int j = 0; j < SlotOverlays.Count; j++)
            for (int k = 0; k < SlotOverlays[j].slotImages.Count; k++)
                SlotOverlays[j].slotImages[k].gameObject.SetActive(true);

        // _scatterCombineReel3Image = winAnimationImages[reel3Row.Value].slotImages[3];
        // _scatterCombineReel4Image = winAnimationImages[reel4Row.Value].slotImages[4];
        _scatterCombineReel3Image = winAnimationImages[3].slotImages[reel3Row.Value];
        _scatterCombineReel4Image = winAnimationImages[4].slotImages[reel4Row.Value];

        ImageAnimation reel3Anim = _scatterCombineReel3Image.GetComponent<ImageAnimation>();
        if (reel3Anim != null) reel3Anim.StopAnimation();
        ImageAnimation reel4Anim = _scatterCombineReel4Image.GetComponent<ImageAnimation>();
        if (reel4Anim != null) reel4Anim.StopAnimation();
        _scatterCombineReel3Image.sprite = rightScatterSymbol;
        _scatterCombineReel3Image.preserveAspect = false;
        _scatterCombineReel4Image.sprite = leftScatterSymbol;
        _scatterCombineReel4Image.preserveAspect = false;
        SetAnimationSymbolSize(_scatterCombineReel3Image, 12);
        SetAnimationSymbolSize(_scatterCombineReel4Image, 12);
        _scatterCombineReel3Image.gameObject.SetActive(true);
        _scatterCombineReel4Image.gameObject.SetActive(true);

        _resultImages[3].slotImages[reel3Row.Value].gameObject.SetActive(false);
        _resultImages[4].slotImages[reel4Row.Value].gameObject.SetActive(false);

        _scatterCombineReel3OriginalPos = _scatterCombineReel3Image.transform.position;
        _scatterCombineReel4OriginalPos = _scatterCombineReel4Image.transform.position;

        // Vertical-only meet: each half keeps its own reel's X position and only slides up/down
        // to the shared row between them. The spawn point still centers horizontally between
        // the two reels so the combined ticket lands squarely between them.
        float midY = Mathf.Lerp(_scatterCombineReel3OriginalPos.y, _scatterCombineReel4OriginalPos.y, 0.5f);
        Vector3 midPoint = new Vector3(
            Mathf.Lerp(_scatterCombineReel3OriginalPos.x, _scatterCombineReel4OriginalPos.x, 0.5f),
            midY,
            _scatterCombineReel3OriginalPos.z);

        DG.Tweening.Sequence moveSeq = DOTween.Sequence();
        moveSeq.Join(_scatterCombineReel3Image.transform.DOMoveY(midY, scatterCombineMoveDuration).SetEase(Ease.OutQuad));
        moveSeq.Join(_scatterCombineReel4Image.transform.DOMoveY(midY, scatterCombineMoveDuration).SetEase(Ease.OutQuad));
        yield return moveSeq.WaitForCompletion();

        yield return new WaitForSeconds(0.5f);

        if (scatterCombinedTicketPrefab != null)
        {
            Transform parent = scatterCombineAnimationParent != null ? scatterCombineAnimationParent : mainSlotTransform;
            RectTransform spawned = Instantiate(scatterCombinedTicketPrefab, parent);
            spawned.position = midPoint;

            RectTransform reel3Rect = _scatterCombineReel3Image.rectTransform;
            RectTransform reel4Rect = _scatterCombineReel4Image.rectTransform;
            spawned.sizeDelta = new Vector2(reel3Rect.rect.width + reel4Rect.rect.width, reel3Rect.rect.height);

            _spawnedScatterCombineObject = spawned.gameObject;

            ImageAnimation combineAnim = spawned.GetComponent<ImageAnimation>();
            if (combineAnim != null)
            {
                combineAnim.doLoopAnimation = true;
                combineAnim.StartAnimation();
            }
        }
        yield return new WaitForSeconds(2f);
    }

    // Cleanup counterpart — stops/destroys the looping combined-ticket object, restores the two
    // half-ticket win images to where they started, and reuses ResetOverlays() to put every
    // overlay/win-animation/result-image cell back to its normal idle state.
    private void StopScatterSymbolsCombineIntro()
    {
        if (_spawnedScatterCombineObject != null)
        {
            Destroy(_spawnedScatterCombineObject);
            _spawnedScatterCombineObject = null;
        }

        if (_scatterCombineReel3Image != null) _scatterCombineReel3Image.transform.position = _scatterCombineReel3OriginalPos;
        if (_scatterCombineReel4Image != null) _scatterCombineReel4Image.transform.position = _scatterCombineReel4OriginalPos;

        ResetOverlays();

        for (int j = 0; j < SlotOverlays.Count; j++)
        {
            for (int k = 0; k < SlotOverlays[j].slotImages.Count; k++)
            {
                SlotOverlays[j].slotImages[k].gameObject.SetActive(true);
            }
        }

        _scatterCombineReel3Image = null;
        _scatterCombineReel4Image = null;
    }

    // Character intro that plays before the free-spin-trigger scatter ticket flies in: pop_left
    // -> toss_left -> idle_left (looping), then a short beat before the ticket sequence starts.
    private IEnumerator PlayFreeSpinCharacterIntro()
    {
        characterAnimationParent.SetActive(true);
        SetCharacterOrientationState(CharacterAnimState.Normal);
        yield return PlayCharacterAnim("pop_left", false);
        yield return PlayCharacterAnim("toss_left", false);
        PlayCharacterAnimLoop("idle_left");
        yield return new WaitForSeconds(0.7f);
    }

    // Shared by both the trigger (mainSlotTargetX = mainSlotFreeSpinOffsetX) and the
    // end-of-round (mainSlotTargetX = 0f) transitions — same visual sequence both times,
    // only the direction the main slot transform slides differs. hideCharacterAfterScaleUp is
    // only set on the end-of-round call, once the ticket has scaled up to full size.
    private IEnumerator PlayScatterTicketSequence(float mainSlotTargetX, bool hideCharacterAfterScaleUp = false)
    {
        if (scatterTicketImage == null) yield break;

        scatterTicketImage.gameObject.SetActive(true);
        scatterTicketImage.localPosition = _scatterTicketInitialLocalPos;
        scatterTicketImage.localRotation = _scatterTicketInitialLocalRot;
        scatterTicketImage.localScale = Vector3.one;

        DG.Tweening.Sequence flyIn = DOTween.Sequence();
        flyIn.Join(scatterTicketImage.DOLocalMove(Vector3.zero, scatterTicketFlyDuration).SetEase(Ease.OutQuad));
        flyIn.Join(scatterTicketImage.DOLocalRotate(new Vector3(0f, 0f, 360f), scatterTicketFlyDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        yield return flyIn.WaitForCompletion();

        yield return new WaitForSeconds(scatterTicketWaitDuration);

        yield return scatterTicketImage.DOScale(scatterTicketScaleUpTarget, scatterTicketScaleUpDuration).WaitForCompletion();

        if (hideCharacterAfterScaleUp)
        {
            DeactivateCharacterAnimation();
        }

        bool scaleDownDone = false;
        scatterTicketImage.DOScale(0f, scatterTicketScaleDownDuration).OnComplete(() => scaleDownDone = true);

        // The main slot only needs to make room for the ticket sideways in landscape — in
        // portrait it stays put.
        var currentOrientation = GetOrientationChange();
        bool isPortraitOrientation = currentOrientation != null && currentOrientation.CurrentMode == OrientationChange.OrientationMode.MobilePortrait;
        if (!isPortraitOrientation)
        {
            mainSlotTransform.DOAnchorPosX(mainSlotTargetX, scatterTicketScaleDownDuration);
        }
        yield return new WaitUntil(() => scaleDownDone);

        scatterTicketImage.gameObject.SetActive(false);
    }

    // Plays the magic-dice row for the free spin that just resolved: fades/scales the row's
    // dice in, rolls for diceRollDuration, then fires a laser (in parallel) at every dice
    // marked by the backend's magicDiceMultipliers for this row, swapping it to the destroy
    // animation and finally the multiplier sprite — while the untouched dice in the same row
    // just scale back down once their result image is revealed again.
    private IEnumerator PlayMagicDiceSequence()
    {
        var payload = socketManager.resultData.payload;
        int spinIndex = payload.freeGames.totalSpins - payload.freeGames.spinsRemaining - 1;
        if (spinIndex < 0 || spinIndex >= DiceImages.Count) yield break;

        var diceRow = DiceImages[spinIndex].slotImages; // 5 dice, one per reel column

        var multipliersThisRow = payload.magicDiceMultipliers ?? new List<MagicDiceMultiplier>();
        var destroyCols = new HashSet<int>();
        foreach (var m in multipliersThisRow) destroyCols.Add(m.col);

        // Hide this row's result images, fade+scale the dice row in, start rolling anim.
        for (int col = 0; col < diceRow.Count; col++)
        {

            var dice = diceRow[col];
            dice.gameObject.SetActive(true);
            Color diceColor = dice.color;
            diceColor.a = 0f;
            dice.color = diceColor;
            dice.transform.localScale = Vector3.zero;
            var anim = dice.GetComponent<ImageAnimation>();
            anim.textureArray = dicerollingAnimation;
            anim.doLoopAnimation = true;
            anim.StartAnimation();

            dice.DOFade(1f, diceFadeScaleDuration);
            dice.transform.DOScale(1f, diceFadeScaleDuration).OnComplete(() =>
            {
                _resultImages[col].slotImages[spinIndex].gameObject.SetActive(false);
            });

        }

        yield return new WaitForSeconds(diceRollDuration);

        // One snap_left per row, only when this row actually has something to shoot — idle_left
        // just keeps looping uninterrupted on a row with no hits.
        if (multipliersThisRow.Count > 0)
        {
            yield return PlayCharacterAnim("snap_left", false);
            PlayCharacterAnimLoop("idle_left");
            //yield return new WaitForSeconds(0.5f);
        }

        // Reference angle/length for every laser this row: the real path from LaserShootPosition to
        // the row0/col0 dice, recomputed fresh off live transforms (not cached) so it self-corrects
        // across orientation changes. Vector3.zero is used as a "no reference" sentinel meaning
        // DestroyDiceAtColumn should fall back to firing straight from LaserShootPosition.
        Vector3 referenceDirection = Vector3.zero;
        float referenceDistance = 0f;
        if (LaserShootPosition != null && DiceImages.Count > 0 && DiceImages[0].slotImages.Count > 0)
        {
            Vector3 referenceOffset = DiceImages[0].slotImages[0].transform.position - LaserShootPosition.position;
            if (referenceOffset.sqrMagnitude > 0.0001f)
            {
                referenceDistance = referenceOffset.magnitude;
                referenceDirection = referenceOffset / referenceDistance;
            }
        }

        // Fire all lasers in parallel at the marked columns.
        var laserRoutines = new List<Coroutine>();
        foreach (var md in multipliersThisRow)
            laserRoutines.Add(StartCoroutine(DestroyDiceAtColumn(diceRow[md.col], md.multiplier, spinIndex, md.col, referenceDirection, referenceDistance)));

        // Wait for the lasers to actually reach the marked dice before scaling the untouched
        // ones down, so the two don't visibly happen at the same time.
        yield return new WaitForSeconds(laserTravelDuration + 0.35f);

        // Scale down the non-destroyed dice now that the lasers have hit.
        for (int col = 0; col < diceRow.Count; col++)
        {
            if (destroyCols.Contains(col)) continue;

            var dice = diceRow[col];
            dice.GetComponent<ImageAnimation>().StopAnimation();
            _resultImages[col].slotImages[spinIndex].gameObject.SetActive(true);
            dice.transform.DOScale(0f, laserTravelDuration).OnComplete(() => dice.gameObject.SetActive(false));
        }

        foreach (var routine in laserRoutines)
            yield return routine;
    }

    private IEnumerator DestroyDiceAtColumn(Image dice, int multiplier, int row, int col, Vector3 referenceDirection, float referenceDistance)
    {
        if (LaserPrefab != null && LaserShootPosition != null)
        {
            Vector3 targetPos = dice.transform.position;
            Vector3 startPos = referenceDirection == Vector3.zero
                ? LaserShootPosition.position
                : targetPos - referenceDirection * referenceDistance;

            GameObject laser = Instantiate(LaserPrefab, startPos, Quaternion.identity, LaserParent != null ? LaserParent.transform : null);
            yield return laser.transform.DOMove(targetPos, laserTravelDuration).WaitForCompletion();
            yield return new WaitForSeconds(0.35f);
            Destroy(laser);
        }

        var anim = dice.GetComponent<ImageAnimation>();
        anim.textureArray = dicedestroyingAnimation;
        anim.doLoopAnimation = false;
        anim.StartAnimation();
        yield return new WaitUntil(() => anim.currentAnimationState == ImageAnimation.ImageState.FINISHED);

        dice.gameObject.SetActive(false);
        _resultImages[col].slotImages[row].sprite = GetMultiplierSprite(multiplier);
        _resultImages[col].slotImages[row].gameObject.SetActive(true);
    }

    private Sprite GetMultiplierSprite(int multiplier)
    {
        switch (multiplier)
        {
            case 1: return OneXMultiplier;
            case 2: return TwoXMultiplier;
            case 4: return FourXMultiplier;
            case 6: return SixMultiplier;
            case 8: return EightXMultiplier;
            default: return OneXMultiplier;
        }
    }

    private List<Sprite> GetMultiplierAnimation(int multiplier)
    {
        switch (multiplier)
        {
            case 1: return OneXMultiplierAnimation;
            case 2: return TwoXMultiplierAnimation;
            case 4: return FourXMultiplierAnimation;
            case 6: return SixXMultiplierAnimation;
            case 8: return EightXMultiplierAnimation;
            default: return OneXMultiplierAnimation;
        }
    }

    #endregion
    #region Helper Function

    internal int GetSymbolIndex(Sprite sprite)
    {
        for (int i = 0; i < _symbolSprites.Length; i++)
        {
            if (sprite == _symbolSprites[i])
            {
                return i;
            }
        }
        return 0;
    }

    // Row (0=top ... 3=bottom) that each payline's middle reel passes through,
    // derived from Assets/Graphics/Asstes/Paytable/Win_lines.png (line 1 = index 1).
    private static readonly int[] _middleColumnRowByLine = new int[]
    {
        -1, // unused, index 0
        0, 1, 2, 3, 0, 3, 1, 2, 2, 1,
        0, 3, 2, 1, 0, 3, 2, 1, 2, 1,
        1, 2, 1, 2, 1, 2, 2, 1, 1, 2,
        1, 2, 1, 2, 0, 3, 2, 1, 1, 2,
    };

    private int GetPerLineWinIndex(int lineIndex)
    {
        lineIndex++;
        return _middleColumnRowByLine[lineIndex];
    }

    private List<Sprite> GetAnimationSprite(int symbolID)
    {
        return symbolAnimations[symbolID].sprites;
    }

    private void SetSymbolSize(Image slotImage, int symbolID, float time = 0f)
    {
        switch (symbolID)
        {
            case 0:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(1.28f, time);
                break;

            case 1:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(1.15f, time);
                break;

            case 2:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(1.3f, time);
                break;

            case 3:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(1.06f, time);
                break;

            case 4:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(1.1f, time);
                break;

            case 5:
                //slotImage.transform.localScale = new Vector2(0.87f, 0.87f);
                slotImage.transform.DOScale(1f, time);
                break;

            case 6:
                //slotImage.transform.localScale = new Vector2(0.87f, 0.87f);
                slotImage.transform.DOScale(1.21f, time);
                break;

            case 7:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(0.91f, time);
                break;

            case 8:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(1f, time);
                break;

            case 9:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(1.08f, time);
                break;

            case 10:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(1.08f, time);
                break;

            case 11:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(1.28f, time);
                break;

            case 12:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(1f, time);
                break;

            default:
                slotImage.transform.DOScale(1f, time);
                break;
        }
    }

    private void SetAnimationSymbolSize(Image slotImage, int symbolID, float time = 0f)
    {
        switch (symbolID)
        {
            case 0:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(1.925f, time);
                break;

            case 1:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(1.15f, time);
                break;

            case 2:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(1.3f, time);
                break;

            case 3:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(1.06f, time);
                break;

            case 4:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(1.1f, time);
                break;

            case 5:
                //slotImage.transform.localScale = new Vector2(0.87f, 0.87f);
                slotImage.transform.DOScale(1f, time);
                break;

            case 6:
                //slotImage.transform.localScale = new Vector2(0.87f, 0.87f);
                slotImage.transform.DOScale(1.21f, time);
                break;

            case 7:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(0.91f, time);
                break;

            case 8:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(1f, time);
                break;

            case 9:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(1.08f, time);
                break;

            case 10:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(1.08f, time);
                break;

            case 11:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(1.28f, time);
                break;

            case 12:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(1f, time);
                break;

            default:
                slotImage.transform.DOScale(1f, time);
                break;
        }
    }

    // Per-symbol AnimationSpeed for the winning-line loop animation (ImageAnimation.AnimationSpeed).
    // All symbols currently share the same speed the win-loop code used to hardcode inline;
    // tune individual cases here once per-symbol timing is needed.
    private float GetAnimationSpeed(int symbolID)
    {
        switch (symbolID)
        {
            case 0:
                return 111;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
                return 31f;

            default:
                return 31f;
        }
    }

    private void ResetAllSymbol()
    {
        // foreach (var slotimage in _resultImages)
        // {
        //     foreach (var image in slotimage.slotImages)
        //     {
        //         image.transform.localScale = Vector2.one;
        //         image.GetComponent<ImageAnimation>().StopAnimation();
        //     }
        // }
        foreach (var item in SlotOverlays)
        {
            foreach (var image in item.slotImages)
            {
                image.gameObject.SetActive(false);
            }
        }
        foreach (var item in WinFrames)
        {
            foreach (var image in item.slotImages)
            {
                image.gameObject.SetActive(false);
                image.GetComponent<ImageAnimation>().StopAnimation();
            }
        }
    }

    private void ResetOverlays()
    {
        for (int j = 0; j < SlotOverlays.Count; j++)
        {
            for (int k = 0; k < SlotOverlays[j].slotImages.Count; k++)
            {
                SlotOverlays[j].slotImages[k].gameObject.SetActive(false);
                WinFrames[j].slotImages[k].gameObject.SetActive(false);
                _resultImages[j].slotImages[k].gameObject.SetActive(true);
                winAnimationImages[j].slotImages[k].GetComponent<ImageAnimation>().StopAnimation();
                winAnimationImages[j].slotImages[k].gameObject.SetActive(false);
            }
        }
        winAmountBigText.gameObject.SetActive(false);
        foreach (var lineWinText in lineWinTexts)
        {
            lineWinText.gameObject.SetActive(false);
        }
    }

    #endregion
}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

[Serializable]
public class AnimationSprites
{
    public List<Sprite> sprites = new List<Sprite>();
}