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

    [Header("Slot Images")]
    [SerializeField] private List<SlotImage> _totalImages;
    [SerializeField] internal List<SlotImage> _resultImages;
    [SerializeField] private List<SlotImage> winAnimationImages;
    [SerializeField] internal List<SlotImage> SlotOverlays;
    [SerializeField] internal List<SlotImage> WinFrames;

    [Header("Slots Transforms")]
    [SerializeField] private Transform[] _slotTransforms;

    [Header("HeatUp Fire UI")]
    [SerializeField] private List<ImageAnimation> HeatUpFireObjects;
    [SerializeField] private List<ImageAnimation> HeatUpFireObjectsPortrait;

    [Header("WinLine UI")]
    [SerializeField] private List<Image> payLines;
    [SerializeField] private List<TMP_Text> lineWinTexts;
    [SerializeField] private List<Image> PayBlockHighlights;
    [SerializeField] private List<Image> PayBlockHighlightsPortrait;


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
    // True from the moment the FreeSpinComplete popup is shown until its Take button is pressed.
    // Autospin must not start the next round while this is true.
    internal bool isFreeSpinCompletePopupPending = false;

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
                int randomSprite = UnityEngine.Random.Range(1, _symbolSprites.Length);
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
            // Don't start the next round while the free-spins-complete popup is still waiting for Take.
            yield return new WaitUntil(() => !isFreeSpinCompletePopupPending);

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

        if (isInFreeSpins)
        {
            // Values are computed now (result is in) but pushed to the UI only after the reels stop —
            // see below — so the count/total-win don't visibly change while the reels are still spinning.
            _freeSpinsRoundWinTotal += socketManager.resultData.payload.winAmount;
            freeSpinsRemaining = socketManager.resultData.payload.freeSpinsRemaining;
        }
        // // Load result matrix into result images
        for (int j = 0; j < socketManager.resultData.matrix.Count; j++)
        {
            for (int i = 0; i < socketManager.resultData.matrix[j].Count; i++)
            {
                if (int.TryParse(socketManager.resultData.matrix[j][i], out int symbolId))
                {
                    _resultImages[i].slotImages[j].sprite = _symbolSprites[symbolId];
                    SetSymbolSize(_resultImages[i].slotImages[j], symbolId);
                }
            }
        }

        if (_isTurboOn)
        {
            _stopSpinToggle = true;
        }

        if (!_stopSpinToggle)
        {
            for (int i = 0; i < _numberOfSlots; i++)
            {
                yield return new WaitForSeconds(0.2f);
                if (_stopSpinToggle) break;
            }
        }
        int HeatUpCount = 0;
        for (int i = 0; i < _numberOfSlots; i++)
        {
            yield return StopTweening(_slotTransforms[i], i, _stopSpinToggle);
            for (int j = 0; j < _resultImages[i].slotImages.Count; j++)
            {
                if (_resultImages[i].slotImages[j].sprite == _symbolSprites[9])
                {
                    HeatUpCount++;
                }
            }
            audioController.PlayReelStop();
        }
        isTweening = false;
        _stopSpinToggle = false;

        //if (_audioController) _audioController.StopWLAaudio();
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

        Debug.Log($"HeatUpCount: {HeatUpCount}");

        if (HeatUpCount > 1)
        {
            audioController.PlayHeatEmUp();
            double heatUpWin = socketManager.resultData.payload.heatEmUpWin;
            // ShowWinLineAnimation (below) is the only other place that credits the balance
            // display, but it only runs when there are line wins — a heatup-only spin (no line
            // wins) would otherwise never get its balance UI refreshed with this win.
            uiManager.UpdateBalance(uiManager.currentBalance + heatUpWin, true);
            if (isInFreeSpins)
            {
                _freeSpinsRoundWinTotal += heatUpWin;
                uiManager.UpdateFreeSpinTotalWin(_freeSpinsRoundWinTotal);
                uiManager.UpdateWin(_freeSpinsRoundWinTotal, false);
            }
            else
            {
                uiManager.UpdateWin(heatUpWin, false);
            }
            if (HeatUpCount > HeatUpFireObjects.Count + 1)
            {
                HeatUpCount = HeatUpFireObjects.Count;
            }
            HeatUpFireObjects[HeatUpCount - 2].gameObject.SetActive(true);
            HeatUpFireObjects[HeatUpCount - 2].StartAnimation();

            if (HeatUpCount > HeatUpFireObjectsPortrait.Count + 1)
            {
                HeatUpCount = HeatUpFireObjectsPortrait.Count;
            }
            HeatUpFireObjectsPortrait[HeatUpCount - 2].gameObject.SetActive(true);
            HeatUpFireObjectsPortrait[HeatUpCount - 2].StartAnimation();

            for (int j = 0; j < SlotOverlays.Count; j++)
            {
                for (int k = 0; k < SlotOverlays[j].slotImages.Count; k++)
                {
                    SlotOverlays[j].slotImages[k].gameObject.SetActive(true);
                    WinFrames[j].slotImages[k].gameObject.SetActive(false);

                    Sprite symbolSprite = _resultImages[j].slotImages[k].sprite;
                    int symbolID = GetSymbolIndex(symbolSprite);
                    if (symbolID == 9)
                    {
                        winAnimationImages[j].slotImages[k].sprite = _symbolSprites[symbolID];
                        winAnimationImages[j].slotImages[k].gameObject.SetActive(true);
                        //_resultImages[j].slotImages[k].gameObject.SetActive(false);
                        SetAnimationSymbolSize(winAnimationImages[j].slotImages[k], symbolID);
                        ImageAnimation anim = winAnimationImages[j].slotImages[k].GetComponent<ImageAnimation>();
                        anim.textureArray = GetAnimationSprite(symbolID);
                        anim.AnimationSpeed = 55f;
                        anim.doLoopAnimation = true;
                        anim.StartAnimation();

                        SlotOverlays[j].slotImages[k].gameObject.SetActive(false);
                        WinFrames[j].slotImages[k].gameObject.SetActive(true);
                        WinFrames[j].slotImages[k].GetComponent<ImageAnimation>().StartAnimation();
                    }

                }
            }
            yield return new WaitForSeconds(2f);
        }

        if (socketManager.resultData.payload.isWheelTriggered)
        {
            // Don't let autoplay get cancelled mid-trigger-animation.
            if (_isAutoSpin) uiManager.SetAutoSpinStopButtonInteractable(false);

            foreach (var obj in HeatUpFireObjects)
            {
                obj.gameObject.SetActive(false);
            }
            foreach (var obj in HeatUpFireObjectsPortrait)
            {
                obj.gameObject.SetActive(false);
            }
            for (int m = 0; m < winAnimationImages.Count; m++)
            {
                for (int n = 0; n < winAnimationImages[m].slotImages.Count; n++)
                {
                    winAnimationImages[m].slotImages[n].gameObject.SetActive(false);
                }
            }
            for (int j = 0; j < SlotOverlays.Count; j++)
            {
                for (int k = 0; k < SlotOverlays[j].slotImages.Count; k++)
                {
                    SlotOverlays[j].slotImages[k].gameObject.SetActive(true);
                    WinFrames[j].slotImages[k].gameObject.SetActive(false);

                    Sprite symbolSprite = _resultImages[j].slotImages[k].sprite;
                    int symbolID = GetSymbolIndex(symbolSprite);
                    if (symbolID == 11)
                    {
                        winAnimationImages[j].slotImages[k].sprite = _symbolSprites[symbolID];
                        winAnimationImages[j].slotImages[k].gameObject.SetActive(true);
                        //_resultImages[j].slotImages[k].gameObject.SetActive(false);
                        SetAnimationSymbolSize(winAnimationImages[j].slotImages[k], symbolID);
                        ImageAnimation anim = winAnimationImages[j].slotImages[k].GetComponent<ImageAnimation>();
                        anim.textureArray = GetAnimationSprite(symbolID);
                        anim.AnimationSpeed = 55f;
                        anim.doLoopAnimation = true;
                        anim.StartAnimation();

                        SlotOverlays[j].slotImages[k].gameObject.SetActive(false);
                        WinFrames[j].slotImages[k].gameObject.SetActive(true);
                        WinFrames[j].slotImages[k].GetComponent<ImageAnimation>().StartAnimation();
                    }
                }
            }
            yield return new WaitForSeconds(3f);
            if (_isAutoSpin) uiManager.SetAutoSpinStopButtonInteractable(true);
            audioController.PlayBonusWheelTrigger();
            uiManager.ShowUniversalWinPopup(UIManager.WinPopupType.BonusTrigger, 0, 0, () => { bonusManager.BonusWheel(); });

            yield return new WaitUntil(() => !bonusManager.isBonusFinished);
            yield return new WaitUntil(() => bonusManager.isBonusFinished);
        }

        if (socketManager.resultData.payload.lineWins.Count > 0)
        {
            foreach (var obj in HeatUpFireObjects)
            {
                obj.gameObject.SetActive(false);
            }
            foreach (var obj in HeatUpFireObjectsPortrait)
            {
                obj.gameObject.SetActive(false);
            }
            yield return ShowWinLineAnimation(socketManager.resultData.payload.lineWins, socketManager.resultData.payload.winAmount, socketManager.resultData.payload.isFreeSpinsTriggered);
        }

        //if (!isInFreeSpins && socketManager.resultData.payload.isFreeSpinsTriggered)
        if (socketManager.resultData.payload.isFreeSpinsTriggered)
        {
            bool wasAlreadyInFreeSpins = isInFreeSpins;
            // Only reset the round total on the initial trigger — a retrigger while free spins
            // are already running should keep accumulating, not restart from 0. Reset happens here
            // (before the trigger popup shows) so the popup can display the correct starting value.
            if (!wasAlreadyInFreeSpins) _freeSpinsRoundWinTotal = 0;
            // Don't let autoplay get cancelled mid-trigger-animation.
            if (_isAutoSpin) uiManager.SetAutoSpinStopButtonInteractable(false);
            audioController.PlayScatterTrigger();
            foreach (var obj in HeatUpFireObjects)
            {
                obj.gameObject.SetActive(false);
            }
            foreach (var obj in HeatUpFireObjectsPortrait)
            {
                obj.gameObject.SetActive(false);
            }
            for (int m = 0; m < winAnimationImages.Count; m++)
            {
                for (int n = 0; n < winAnimationImages[m].slotImages.Count; n++)
                {
                    winAnimationImages[m].slotImages[n].gameObject.SetActive(false);
                }
            }
            isInFreeSpins = false;
            for (int j = 0; j < SlotOverlays.Count; j++)
            {
                for (int k = 0; k < SlotOverlays[j].slotImages.Count; k++)
                {
                    SlotOverlays[j].slotImages[k].gameObject.SetActive(true);
                    WinFrames[j].slotImages[k].gameObject.SetActive(false);

                    Sprite symbolSprite = _resultImages[j].slotImages[k].sprite;
                    int symbolID = GetSymbolIndex(symbolSprite);
                    if (symbolID == 10)
                    {
                        winAnimationImages[j].slotImages[k].sprite = _symbolSprites[symbolID];
                        winAnimationImages[j].slotImages[k].gameObject.SetActive(true);
                        //_resultImages[j].slotImages[k].gameObject.SetActive(false);
                        SetAnimationSymbolSize(winAnimationImages[j].slotImages[k], symbolID);
                        ImageAnimation anim = winAnimationImages[j].slotImages[k].GetComponent<ImageAnimation>();
                        anim.textureArray = GetAnimationSprite(symbolID);
                        anim.AnimationSpeed = 55f;
                        anim.doLoopAnimation = true;
                        anim.StartAnimation();

                        SlotOverlays[j].slotImages[k].gameObject.SetActive(false);
                        WinFrames[j].slotImages[k].gameObject.SetActive(true);
                        WinFrames[j].slotImages[k].GetComponent<ImageAnimation>().StartAnimation();
                    }

                }
            }
            yield return new WaitForSeconds(3f);
            if (_isAutoSpin) uiManager.SetAutoSpinStopButtonInteractable(true);
            uiManager.OnFreeSpinsTriggered(socketManager.resultData.payload.freeSpinsAdded);
            yield return new WaitUntil(() => isInFreeSpins);
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
                isFreeSpinCompletePopupPending = true;
                uiManager.OnFreeSpinsEnded(_freeSpinsRoundWinTotal);
            }
        }
        else
        {
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

        for (int j = 0; j < winLines.Count; j++)
        {
            foreach (var line in winLines[j].positions)
            {
                int col = line.position[0];
                int row = line.position[1];
                SlotOverlays[row].slotImages[col].gameObject.SetActive(false);
                WinFrames[row].slotImages[col].gameObject.SetActive(true);
                WinFrames[row].slotImages[col].GetComponent<ImageAnimation>().StartAnimation();

                int symbolID = int.Parse(socketManager.resultData.matrix[col][row]);

                winAnimationImages[row].slotImages[col].sprite = _symbolSprites[symbolID];
                winAnimationImages[row].slotImages[col].gameObject.SetActive(true);
                //_resultImages[row].slotImages[col].gameObject.SetActive(false);
                SetAnimationSymbolSize(winAnimationImages[row].slotImages[col], symbolID);
                ImageAnimation anim = winAnimationImages[row].slotImages[col].GetComponent<ImageAnimation>();

                Debug.Log($"Symbol ID: {symbolID} at Row: {row}, Col: {col}");
                anim.textureArray = GetAnimationSprite(symbolID);
                anim.AnimationSpeed = 31f;
                anim.doLoopAnimation = true;
                anim.StartAnimation();
            }
        }

        yield return new WaitForSeconds(2f);
    }

    // Cycles the winning lines one at a time, forever. StopWinLinesLoop() (called from
    // TweenRoutine the moment a new spin starts) is what actually ends this.
    private IEnumerator LoopWinLines(List<LineWin> winLines)
    {
        while (true)
        {
            for (int i = 0; i < winLines.Count; i++)
            {
                for (int j = 0; j < SlotOverlays.Count; j++)
                {
                    for (int k = 0; k < SlotOverlays[j].slotImages.Count; k++)
                    {
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
                    SlotOverlays[row].slotImages[col].gameObject.SetActive(false);
                    WinFrames[row].slotImages[col].gameObject.SetActive(true);
                    WinFrames[row].slotImages[col].GetComponent<ImageAnimation>().StartAnimation();

                    int symbolID = int.Parse(socketManager.resultData.matrix[col][row]);

                    winAnimationImages[row].slotImages[col].sprite = _symbolSprites[symbolID];
                    SetAnimationSymbolSize(winAnimationImages[row].slotImages[col], symbolID);
                    winAnimationImages[row].slotImages[col].gameObject.SetActive(true);
                    //_resultImages[row].slotImages[col].gameObject.SetActive(false);
                    ImageAnimation anim = winAnimationImages[row].slotImages[col].GetComponent<ImageAnimation>();

                    anim.StopAnimation();

                    anim.textureArray = GetAnimationSprite(symbolID);
                    anim.AnimationSpeed = 31f;
                    anim.doLoopAnimation = true;
                    anim.StartAnimation();
                }

                foreach (var payLine in payLines)
                {
                    payLine.gameObject.SetActive(false);
                }
                payLines[winLines[i].lineIndex].gameObject.SetActive(true);
                audioController.PlayPaylineHighlight();

                foreach (var payBlock in PayBlockHighlights)
                {
                    payBlock.gameObject.SetActive(false);
                }
                foreach (var payBlock in PayBlockHighlightsPortrait)
                {
                    payBlock.gameObject.SetActive(false);
                }
                PayBlockHighlights[winLines[i].lineIndex].gameObject.SetActive(true);
                PayBlockHighlightsPortrait[winLines[i].lineIndex].gameObject.SetActive(true);

                foreach (var linewinText in lineWinTexts)
                {
                    linewinText.gameObject.SetActive(false);
                }
                int tempIndex = GetPerLineWinIndex(winLines[i].lineIndex);
                lineWinTexts[tempIndex].text = winLines[i].payout.ToString("F2");
                lineWinTexts[tempIndex].gameObject.SetActive(true);

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

        foreach (var payLine in payLines)
        {
            payLine.gameObject.SetActive(false);
        }

        foreach (var lineWinText in lineWinTexts)
        {
            lineWinText.gameObject.SetActive(false);
        }

        foreach (var payBlock in PayBlockHighlights)
        {
            payBlock.gameObject.SetActive(false);
        }
        foreach (var payBlock in PayBlockHighlightsPortrait)
        {
            payBlock.gameObject.SetActive(false);
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

    private int GetPerLineWinIndex(int lineIndex)
    {
        lineIndex++;
        if (lineIndex == 1 || lineIndex == 8 || lineIndex == 9 || lineIndex == 10 || lineIndex == 11 || lineIndex == 12 || lineIndex == 13 || lineIndex == 22 || lineIndex == 23 || lineIndex == 24 || lineIndex == 25)
        {
            return 1;
        }
        else if (lineIndex == 2 || lineIndex == 5 || lineIndex == 6 || lineIndex == 14 || lineIndex == 16 || lineIndex == 19 || lineIndex == 21)
        {
            return 0;
        }
        else
        {
            return 2;
        }
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
                slotImage.transform.DOScale(1f, time);
                break;

            case 1:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(0.97f, time);
                break;

            case 2:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(0.95f, time);
                break;

            case 3:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(0.91f, time);
                break;

            case 4:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(0.95f, time);
                break;

            case 5:
                //slotImage.transform.localScale = new Vector2(0.87f, 0.87f);
                slotImage.transform.DOScale(0.91f, time);
                break;

            case 6:
                //slotImage.transform.localScale = new Vector2(0.87f, 0.87f);
                slotImage.transform.DOScale(1f, time);
                break;

            case 7:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(0.91f, time);
                break;

            case 8:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(0.88f, time);
                break;

            case 9:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(0.94f, time);
                break;

            case 10:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(0.94f, time);
                break;

            case 11:
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
                slotImage.transform.DOScale(1f, time);
                break;

            case 1:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(1.29f, time);
                break;

            case 2:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(0.95f, time);
                break;

            case 3:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(0.91f, time);
                break;

            case 4:
                //slotImage.transform.localScale = new Vector2(1.15f, 1.15f);
                slotImage.transform.DOScale(0.95f, time);
                break;

            case 5:
                //slotImage.transform.localScale = new Vector2(0.87f, 0.87f);
                slotImage.transform.DOScale(1.06f, time);
                break;

            case 6:
                //slotImage.transform.localScale = new Vector2(0.87f, 0.87f);
                slotImage.transform.DOScale(1f, time);
                break;

            case 7:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(1.21f, time);
                break;

            case 8:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(0.88f, time);
                break;

            case 9:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(0.94f, time);
                break;

            case 10:
                //slotImage.transform.localScale = new Vector2(1.7f, 1.7f);
                slotImage.transform.DOScale(0.94f, time);
                break;

            case 11:
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
        foreach (var slotimage in _resultImages)
        {
            foreach (var image in slotimage.slotImages)
            {
                image.transform.localScale = Vector2.one;
                image.GetComponent<ImageAnimation>().StopAnimation();
            }
        }
        foreach (var obj in HeatUpFireObjects)
        {
            obj.gameObject.SetActive(false);
        }
        foreach (var obj in HeatUpFireObjectsPortrait)
        {
            obj.gameObject.SetActive(false);
        }
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

        foreach (var payLine in payLines)
        {
            payLine.gameObject.SetActive(false);
        }

        foreach (var lineWinText in lineWinTexts)
        {
            lineWinText.gameObject.SetActive(false);
        }

        foreach (var payBlock in PayBlockHighlights)
        {
            payBlock.gameObject.SetActive(false);
        }
        foreach (var payBlock in PayBlockHighlightsPortrait)
        {
            payBlock.gameObject.SetActive(false);
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