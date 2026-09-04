using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System;
using System.Collections;
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
    [SerializeField] private List<SlotImage> winAnimationImages;
    [SerializeField] internal List<SlotImage> SlotOverlays;
    [SerializeField] internal List<SlotImage> WinFrames;

    [SerializeField] private List<SlotImage> DiceImages;

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


    internal bool SocketConnected = false;
    internal bool _isAutoSpin = false;
    internal bool isInFreeSpins = false;
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

    #region SlotSpin

    internal void RequestInstantStop()
    {
        if (isTweening)
        {
            _stopSpinToggle = true;
        }
    }

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
        // // Load result matrix into result images
        for (int j = 0; j < socketManager.resultData.matrix.Count; j++)
        {
            for (int i = 0; i < socketManager.resultData.matrix[j].Count; i++)
            {
                if (int.TryParse(socketManager.resultData.matrix[j][i], out int symbolId))
                {
                    _resultImages[i].slotImages[j].sprite = _symbolSprites[symbolId];
                    SetSymbolSize(_resultImages[i].slotImages[j], symbolId);

                    if (i == 3 && symbolId == 12)
                    {
                        _resultImages[i].slotImages[j].sprite = rightScatterSymbol;
                    }
                    else if (i == 4 && symbolId == 12)
                    {
                        _resultImages[i].slotImages[j].sprite = leftScatterSymbol;
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
        bool bonusZoomTriggered = false;
        for (int i = 0; i < _numberOfSlots; i++)
        {
            bool triggerBonusZoom = !bonusZoomTriggered && bonusSymbolCount >= 2;
            if (triggerBonusZoom)
            {
                bonusZoomTriggered = true;

                mainSlotTransform.localScale = new Vector3(1f, 1f, 1f);
                landscapeBackground.transform.localScale = new Vector3(1f, 1f, 1f);
                portraitBackground.transform.localScale = new Vector3(1f, 1f, 1f);

                mainSlotTransform.DOScale(1.2f, 1f).OnComplete(() => mainSlotTransform.DOScale(1f, 0.5f));
                landscapeBackground.transform.DOScale(1.15f, 1f).OnComplete(() => landscapeBackground.transform.DOScale(1f, 0.5f));
                portraitBackground.transform.DOScale(1.15f, 1f).OnComplete(() => portraitBackground.transform.DOScale(1f, 0.5f));
            }
            yield return StopTweening(_slotTransforms[i], i, _stopSpinToggle);
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
            if (!triggerBonusZoom)
            {
                mainSlotTransform.DOScale(1f, 0.5f);
                landscapeBackground.transform.DOScale(1f, 0.5f);
                portraitBackground.transform.DOScale(1f, 0.5f);
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

            yield return PlayScatterTicketSequence(mainSlotFreeSpinOffsetX);

            uiManager.OnFreeSpinsTriggered(freeSpinsRemaining);
        }


        // Big/Huge/Mega win popup only applies to a normal (non free-spin) spin's own win —
        // shown before the win-line loop, which then plays once it's closed (Take, or the
        // 3s autoplay auto-close after Take becomes interactable).
        if (!isInFreeSpins)
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
                var freeSpinPopupType = uiManager.GetWinPopupType(socketManager.resultData.payload.freeGames.totalWinCash)
                    ?? UIManager.WinPopupType.BigWin;
                bool freeSpinPopupClosed = false;
                uiManager.ShowUniversalWinPopup(freeSpinPopupType, socketManager.resultData.payload.freeGames.totalWinCash, _isAutoSpin, () => freeSpinPopupClosed = true);
                yield return new WaitUntil(() => freeSpinPopupClosed);

                yield return PlayScatterTicketSequence(0f);

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
                WinFrames[row].slotImages[col].gameObject.SetActive(true);
                WinFrames[row].slotImages[col].GetComponent<ImageAnimation>().StartAnimation();

                int symbolID = int.Parse(socketManager.resultData.matrix[col][row]);

                if (symbolID == 13)
                {
                    var md = socketManager.resultData.payload.magicDiceMultipliers?.Find(m => m.row == row && m.col == col);
                    var anim = winAnimationImages[row].slotImages[col].GetComponent<ImageAnimation>();
                    anim.textureArray = GetMultiplierAnimation(md?.multiplier ?? 1);
                    anim.doLoopAnimation = true;
                    anim.StartAnimation();
                }
                else
                {
                    winAnimationImages[row].slotImages[col].sprite = _symbolSprites[symbolID];
                }
                winAnimationImages[row].slotImages[col].gameObject.SetActive(true);
                _resultImages[row].slotImages[col].gameObject.SetActive(false);
                SetAnimationSymbolSize(winAnimationImages[row].slotImages[col], symbolID);

                // ImageAnimation anim = winAnimationImages[row].slotImages[col].GetComponent<ImageAnimation>();
                // anim.textureArray = GetAnimationSprite(symbolID);
                // anim.AnimationSpeed = 31f;
                // anim.doLoopAnimation = true;
                // anim.StartAnimation();
            }
        }

        yield return new WaitForSeconds(2f);
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
                        continue;

                    if (symbolID != 0)
                    {
                        WinFrames[row].slotImages[col].gameObject.SetActive(true);
                        WinFrames[row].slotImages[col].GetComponent<ImageAnimation>().StartAnimation();
                    }

                    if (symbolID == 13)
                    {
                        var md = socketManager.resultData.payload.magicDiceMultipliers?.Find(m => m.row == row && m.col == col);
                        var anim = winAnimationImages[row].slotImages[col].GetComponent<ImageAnimation>();
                        anim.textureArray = GetMultiplierAnimation(md?.multiplier ?? 1);
                        anim.doLoopAnimation = true;
                        anim.StartAnimation();
                    }
                    else
                    {
                        winAnimationImages[row].slotImages[col].sprite = _symbolSprites[symbolID];
                    }
                    SetAnimationSymbolSize(winAnimationImages[row].slotImages[col], symbolID);
                    winAnimationImages[row].slotImages[col].gameObject.SetActive(true);
                    _resultImages[row].slotImages[col].gameObject.SetActive(false);

                    // ImageAnimation anim = winAnimationImages[row].slotImages[col].GetComponent<ImageAnimation>();
                    // anim.StopAnimation();
                    // anim.textureArray = GetAnimationSprite(symbolID);
                    // anim.AnimationSpeed = 31f;
                    // anim.doLoopAnimation = true;
                    // anim.StartAnimation();
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

                yield return new WaitForSeconds(2f);
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

    // Shared by both the trigger (mainSlotTargetX = mainSlotFreeSpinOffsetX) and the
    // end-of-round (mainSlotTargetX = 0f) transitions — same visual sequence both times,
    // only the direction the main slot transform slides differs.
    private IEnumerator PlayScatterTicketSequence(float mainSlotTargetX)
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

        bool scaleDownDone = false;
        scatterTicketImage.DOScale(0f, scatterTicketScaleDownDuration).OnComplete(() => scaleDownDone = true);
        mainSlotTransform.DOAnchorPosX(mainSlotTargetX, scatterTicketScaleDownDuration);
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
            dice.DOFade(1f, diceFadeScaleDuration);
            dice.transform.DOScale(1f, diceFadeScaleDuration).OnComplete(() =>
            {
                _resultImages[col].slotImages[spinIndex].gameObject.SetActive(false);
            });

            var anim = dice.GetComponent<ImageAnimation>();
            anim.textureArray = dicerollingAnimation;
            anim.doLoopAnimation = true;
            anim.StartAnimation();
        }

        yield return new WaitForSeconds(diceRollDuration);

        // Fire all lasers in parallel at the marked columns.
        var laserRoutines = new List<Coroutine>();
        foreach (var md in multipliersThisRow)
            laserRoutines.Add(StartCoroutine(DestroyDiceAtColumn(diceRow[md.col], md.multiplier, spinIndex, md.col)));

        // Wait for the lasers to actually reach the marked dice before scaling the untouched
        // ones down, so the two don't visibly happen at the same time.
        yield return new WaitForSeconds(laserTravelDuration);

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

    private IEnumerator DestroyDiceAtColumn(Image dice, int multiplier, int row, int col)
    {
        if (LaserPrefab != null && LaserShootPosition != null)
        {
            GameObject laser = Instantiate(LaserPrefab, LaserShootPosition.position, Quaternion.identity, LaserParent != null ? LaserParent.transform : null);
            yield return laser.transform.DOMove(dice.transform.position, laserTravelDuration).WaitForCompletion();
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