using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class BonusManager : MonoBehaviour
{
    [Header("CloudAnimation")]
    [SerializeField] private List<Sprite> cloudFullScreenAnimation;
    [SerializeField] private List<Sprite> cloudClearScreenAnimation;
    [SerializeField] private ImageAnimation cloudAnimationPlayerLandscape;
    [SerializeField] private ImageAnimation cloudAnimationPlayerPortrait;

    [Header("BonusPanel")]
    [SerializeField] private GameObject BonusPanel;
    [SerializeField] private GameObject LandscapeBackground;
    [SerializeField] private GameObject LandScapeEnvironment;
    [SerializeField] private GameObject PortraitBackground;
    [SerializeField] private TMP_Text TotalWinTextLandscape;
    [SerializeField] private ImageAnimation winTextPanelLandscape;
    [SerializeField] private TMP_Text RemainingRollsTextLandscape;
    [SerializeField] private TMP_Text TotalWinTextPortrait;
    [SerializeField] private ImageAnimation winTextPanelPortrait;
    [SerializeField] private TMP_Text RemainingRollsTextPortrait;

    [Header("BonusBoard Screen Settings")]
    [SerializeField] private Vector2 LandscapeBoardPosition;
    [SerializeField] private Vector2 PortraitBoardPosition;
    [SerializeField] private Vector3 LandscapeBoardScale;
    [SerializeField] private Vector3 PortraitBoardScale;

    [Header("BoardGame References")]
    [SerializeField] private GameObject boardGame;
    [SerializeField] private GameObject CharacterParent;
    [SerializeField] private GameObject Character;
    [SerializeField] private GameObject CharacterBottomShadow;
    [SerializeField] private GameObject WinLaser;
    [SerializeField] private RectTransform laserEndPointLandscape;
    [SerializeField] private RectTransform laserEndPointPortrait;
    [SerializeField] private ImageAnimation LandingShineAnimation;
    [SerializeField] private TMP_Text Dice1Text;
    [SerializeField] private TMP_Text Dice2Text;
    [SerializeField] private Button RollButton;
    [SerializeField] private List<GameObject> boardPositions;
    [SerializeField] private GameObject vaultPrefab;
    [SerializeField] private GameObject addRollsPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject resetPrefab;
    [SerializeField] private GameObject endPrefab;

    [SerializeField] private GameObject EndBoardLogo;

    [Header("Hop Tuning")]
    [SerializeField] private float hopDuration = 0.15f;
    [SerializeField] private float hopJumpPower = 40f;
    [SerializeField] private float diceRevealDelay = 1f;
    [SerializeField] private float postBonusPause = 2f;

    [Header("Hop Squish Tuning")]
    [SerializeField] private float squishAscendScaleY = 1.15f;    // stretch tall going up
    [SerializeField] private float squishAscendScaleX = 0.9f;     // narrow going up
    [SerializeField] private float squishDescendScaleY = 0.9f;    // start compressing on the way down
    [SerializeField] private float squishDescendScaleX = 1.1f;
    [SerializeField] private float squishLandScaleY = 0.75f;      // deepest squash at impact
    [SerializeField] private float squishLandScaleX = 1.25f;
    [SerializeField] private float squishAscendFraction = 0.4f;   // fraction of hopDuration spent ascending
    [SerializeField] private float squishDescendFraction = 0.35f; // fraction spent descending (remainder = land hold)
    [SerializeField] private float squishReboundDuration = 0.12f; // single snap-back to normal after landing
    [SerializeField] private Ease squishReboundEase = Ease.OutBack;

    [Header("Shadow Pulse Tuning")]
    [SerializeField] private float shadowMinScale = 0.7f;         // shrink at mid-air peak, grows back to 1 on landing

    [Header("Win Laser Settings")]
    [SerializeField] private float laserTravelDuration = 0.4f;
    [SerializeField] private float laserCurveAmount = 0.15f; // fraction of travel distance offset for the curve's midpoint — keep small for a subtle arc
    [SerializeField] private float laserArrivalWaitDuration = 0.7f;

    [Header("External References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SlotManager slotManager;
    [SerializeField] private OrientationChange orientationChange;

    // --- runtime state ---
    private int _currentPosition;
    private int _rollsRemaining;
    private double _bonusTotalWin;
    private bool _isRolling;
    private bool _rollRequested;
    private RectTransform _boardGameRT;
    private readonly List<GameObject> _spawnedDecorations = new List<GameObject>();

    internal bool IsBonusRoundActive { get; private set; }

    private void Start()
    {
        if (RollButton) RollButton.onClick.AddListener(OnRollButtonClicked);

        var oc = GetOrientationChange();
        HandleOrientationChange(oc != null ? oc.CurrentMode : OrientationChange.OrientationMode.Landscape, Screen.width, Screen.height);
    }

    // Subscribing here (rather than relying solely on the manual call in Start()) guarantees the
    // "starting" orientation broadcast is received correctly regardless of component execution
    // order — Unity runs every object's OnEnable() before any object's Start(), so this is already
    // listening by the time OrientationChange.Start() fires its first ApplyMatch(). It also keeps
    // BonusManager in sync with any orientation change that happens at runtime (matches OCController).
    private void OnEnable()
    {
        OrientationChange.OnOrientationChanged += HandleOrientationChange;
        var oc = GetOrientationChange();
        if (oc != null)
        {
            oc.OnOrientationChangedInstance += HandleOrientationChange;
        }
    }

    private void OnDisable()
    {
        OrientationChange.OnOrientationChanged -= HandleOrientationChange;
        if (orientationChange != null)
        {
            orientationChange.OnOrientationChangedInstance -= HandleOrientationChange;
        }
    }

    private OrientationChange GetOrientationChange()
    {
        if (orientationChange != null) return orientationChange;
        orientationChange = Object.FindFirstObjectByType<OrientationChange>();
        return orientationChange;
    }

    private void HandleOrientationChange(OrientationChange.OrientationMode mode, int width, int height)
    {
        bool isPortrait = mode == OrientationChange.OrientationMode.MobilePortrait;

        if (LandscapeBackground) LandscapeBackground.SetActive(!isPortrait);
        if (LandScapeEnvironment) LandScapeEnvironment.SetActive(!isPortrait);
        if (PortraitBackground) PortraitBackground.SetActive(isPortrait);

        if (boardGame)
        {
            if (_boardGameRT == null) _boardGameRT = boardGame.GetComponent<RectTransform>();
            if (_boardGameRT)
            {
                _boardGameRT.anchoredPosition = isPortrait ? PortraitBoardPosition : LandscapeBoardPosition;
                Vector3 targetScale = isPortrait ? PortraitBoardScale : LandscapeBoardScale;
                _boardGameRT.localScale = targetScale == Vector3.zero ? Vector3.one : targetScale;
            }
        }
    }

    // Entry point — SlotManager.TweenRoutine() yields directly on this once
    // payload.checkersBonus.triggered is true, before the base spin's own win popup/lines.
    internal IEnumerator PlayBonusRound(CheckersBonus data, int startingRollsCount)
    {
        IsBonusRoundActive = true;
        _bonusTotalWin = 0;
        _currentPosition = 0; // math cursor for the first roll always starts at 0

        yield return PlayCloudTransition(cloudFullScreenAnimation, reverse: false);

        uiManager.gameLogoObject.SetActive(false);
        slotManager.DeactivateCharacterAnimation();
        CharacterParent.SetActive(true);
        BonusPanel.SetActive(true);
        int boardLength = data.board.Count;
        PopulateBoard(data.board);
        PlaceCharacterAtIndex(boardPositions.Count - 1); // cosmetic resting spot, "just before" index 0

        _rollsRemaining = startingRollsCount;
        SetRollsText(RemainingRollsTextLandscape, RemainingRollsTextPortrait, _rollsRemaining.ToString());
        SetBonusText(TotalWinTextLandscape, TotalWinTextPortrait, FormatAmount(0));

        yield return PlayCloudTransition(cloudClearScreenAnimation, reverse: false);

        uiManager.ShowStartButton(false); // visible, disabled until the first roll is armed
        SetRollButtonsInteractable(true);

        int rollIndex = 0;
        bool ended = false;
        while (rollIndex < data.rolls.Count && !ended)
        {
            _rollRequested = false;
            yield return new WaitUntil(() => _rollRequested);

            SetRollButtonsInteractable(false);

            Roll roll = data.rolls[rollIndex];
            _rollsRemaining--;
            SetRollsText(RemainingRollsTextLandscape, RemainingRollsTextPortrait, Mathf.Max(0, _rollsRemaining).ToString());

            yield return AnimateRoll(roll, boardLength);

            if (roll.rollsAdded.HasValue) _rollsRemaining += roll.rollsAdded.Value;
            SetRollsText(RemainingRollsTextLandscape, RemainingRollsTextPortrait, Mathf.Max(0, _rollsRemaining).ToString());

            ended = roll.isEnd || _rollsRemaining <= 0;
            rollIndex++;

            if (!ended) SetRollButtonsInteractable(true);
        }

        uiManager.HideStartButton();

        yield return new WaitForSeconds(postBonusPause);

        EndBoardLogo.transform.localScale = Vector3.zero;
        EndBoardLogo.SetActive(true);
        EndBoardLogo.transform.DOScale(Vector3.one, 1f).SetEase(Ease.InSine);

        yield return new WaitForSeconds(postBonusPause);

        CharacterParent.SetActive(false);

        // Character outro: pop_left -> action_left at the bonus position/rotation, then the
        // closing clouds hide the board and the character together, same timing as the intro.
        slotManager.SetCharacterOrientationState(SlotManager.CharacterAnimState.Bonus);
        slotManager.characterAnimationParent.SetActive(true);
        yield return slotManager.PlayCharacterAnim("pop_left", false);
        yield return slotManager.PlayCharacterAnim("action_left", false);

        yield return PlayCloudTransition(cloudFullScreenAnimation, reverse: true);
        EndBoardLogo.SetActive(false);
        BonusPanel.SetActive(false);
        slotManager.DeactivateCharacterAnimation();
        uiManager.gameLogoObject.SetActive(true);
        yield return PlayCloudTransition(cloudClearScreenAnimation, reverse: true);

        IsBonusRoundActive = false;
        ClearBoardDecorations();

        var popupType = uiManager.GetWinPopupType(data.winInCash) ?? UIManager.WinPopupType.BigWin;
        bool popupClosed = false;
        uiManager.ShowUniversalWinPopup(popupType, data.winInCash, false, () => popupClosed = true);
        yield return new WaitUntil(() => popupClosed);
    }

    private void PopulateBoard(List<Board> board)
    {
        ClearBoardDecorations();
        int count = Mathf.Min(board.Count, boardPositions.Count); // defensive against a count mismatch
        for (int i = 0; i < count; i++)
        {
            GameObject prefab = ResolveCellPrefab(board[i]);
            GameObject instance = null;
            if (prefab != null)
            {
                Transform cellTransform = boardPositions[i + 1].transform;
                instance = Instantiate(prefab, cellTransform.position, Quaternion.identity, cellTransform);
                if (prefab == addRollsPrefab && board[i].addRolls.HasValue)
                {
                    SetAddRollsText(instance, board[i].addRolls.Value);
                }
            }
            // Add even when null so the list stays index-aligned with `board` / `boardPositions` —
            // ApplyRollReward looks up _spawnedDecorations[roll.position] directly.
            _spawnedDecorations.Add(instance);
        }
    }

    private void SetAddRollsText(GameObject instance, int addRollsValue)
    {
        TMP_Text addRollsText = instance.GetComponentInChildren<TMP_Text>();
        //if (addRollsText) addRollsText.text = $"+{addRollsValue}";
        addRollsText.text = UIManager.ToSpriteString($"+{addRollsValue}");
    }


    private GameObject ResolveCellPrefab(Board cell)
    {
        if (cell.isEnd == true) return endPrefab;
        if (cell.resetPosition == true) return resetPrefab;
        if (cell.vault != null) return vaultPrefab;
        if (cell.addRolls.HasValue && cell.addRolls.Value > 0) return addRollsPrefab;
        if (cell.multiplier > 0) return coinPrefab;
        // Completely empty cell — nothing landed here, leave it undecorated.
        return null;
    }

    private void ClearBoardDecorations()
    {
        foreach (var go in _spawnedDecorations)
        {
            if (go) Destroy(go);
        }
        _spawnedDecorations.Clear();
    }

    private void PlaceCharacterAtIndex(int index)
    {
        if (CharacterParent == null || boardPositions.Count == 0) return;
        index = Mathf.Clamp(index, 0, boardPositions.Count - 1);

        RectTransform characterParentRT = CharacterParent.GetComponent<RectTransform>();
        RectTransform targetRT = boardPositions[index].GetComponent<RectTransform>();
        if (characterParentRT && targetRT)
        {
            characterParentRT.anchoredPosition = targetRT.anchoredPosition;
        }
        else
        {
            CharacterParent.transform.position = boardPositions[index].transform.position;
        }

        // Defensive reset — an instant snap should never leave Character/the shadow mid-hop
        // (mid-squish or offset from the local jump-arc), even if one happens to interrupt a hop.
        if (Character != null)
        {
            RectTransform characterRT = Character.GetComponent<RectTransform>();
            if (characterRT != null)
            {
                characterRT.DOKill();
                characterRT.anchoredPosition = Vector2.zero;
                characterRT.localScale = Vector3.one;
            }
        }
        if (CharacterBottomShadow != null)
        {
            RectTransform shadowRT = CharacterBottomShadow.GetComponent<RectTransform>();
            if (shadowRT != null)
            {
                shadowRT.DOKill();
                shadowRT.localScale = Vector3.one;
            }
        }
    }

    // The visual up/down arc lives on Character's local anchored position (not CharacterParent's),
    // so siblings under CharacterParent — the shadow, WinLaser, LandingShineAnimation — ride
    // CharacterParent's plain ground-path move without inheriting the bounce.
    private Sequence PlayHopSquish(RectTransform characterRT, float duration)
    {
        float ascendT = duration * squishAscendFraction;
        float descendT = duration * squishDescendFraction;
        float landT = Mathf.Max(0.01f, duration - ascendT - descendT);

        Sequence seq = DOTween.Sequence();
        seq.Append(characterRT.DOScale(new Vector3(squishAscendScaleX, squishAscendScaleY, 1f), ascendT).SetEase(Ease.OutSine));
        seq.Append(characterRT.DOScale(new Vector3(squishDescendScaleX, squishDescendScaleY, 1f), descendT).SetEase(Ease.InSine));
        seq.Append(characterRT.DOScale(new Vector3(squishLandScaleX, squishLandScaleY, 1f), landT).SetEase(Ease.OutQuad));
        seq.Append(characterRT.DOScale(Vector3.one, squishReboundDuration).SetEase(squishReboundEase));
        return seq;
    }

    // Uniform pulse only — the shadow never gets the asymmetric X/Y squish, and stays on the
    // ground since it's just a fixed-offset child of CharacterParent's (now non-arcing) move.
    private Sequence PlayShadowPulse(RectTransform shadowRT, float duration)
    {
        float half = duration * 0.5f;
        Sequence seq = DOTween.Sequence();
        seq.Append(shadowRT.DOScale(shadowMinScale, half).SetEase(Ease.OutSine));
        seq.Append(shadowRT.DOScale(1f, duration - half).SetEase(Ease.InSine));
        return seq;
    }

    private IEnumerator AnimateRoll(Roll roll, int boardLength)
    {
        _isRolling = true;

        if (Dice1Text) Dice1Text.text = roll.dice1.ToString(); // TODO: dice roll animation
        if (Dice2Text) Dice2Text.text = roll.dice2.ToString(); // TODO: dice roll animation
        yield return new WaitForSeconds(diceRevealDelay);

        RectTransform parentRT = CharacterParent ? CharacterParent.GetComponent<RectTransform>() : null;
        RectTransform characterRT = Character ? Character.GetComponent<RectTransform>() : null;
        RectTransform shadowRT = CharacterBottomShadow ? CharacterBottomShadow.GetComponent<RectTransform>() : null;
        for (int step = 0; step < roll.sum; step++)
        {
            _currentPosition = (_currentPosition + 1) % boardLength;
            if (parentRT == null) continue;

            RectTransform targetCell = boardPositions[_currentPosition].GetComponent<RectTransform>();
            if (targetCell == null) continue;

            Sequence hop = DOTween.Sequence();
            hop.Join(parentRT.DOAnchorPos(targetCell.anchoredPosition, hopDuration).SetEase(Ease.Linear));

            if (characterRT != null)
            {
                characterRT.anchoredPosition = Vector2.zero; // resting local pos — the arc jumps out from and back to here
                hop.Join(characterRT.DOJumpAnchorPos(Vector2.zero, hopJumpPower, 1, hopDuration));
                hop.Join(PlayHopSquish(characterRT, hopDuration));
            }

            if (shadowRT != null)
            {
                hop.Join(PlayShadowPulse(shadowRT, hopDuration));
            }

            yield return hop.WaitForCompletion();
        }

        // Safety net — the server's reported position is authoritative even if this ever diverges.
        _currentPosition = roll.position;

        ApplyRollReward(roll);

        if (roll.isReset == true)
        {
            yield return new WaitForSeconds(0.3f);
            //PlaceCharacterAtIndex(boardPositions.Count - 1);
            CharacterParent.transform.DOMove(boardPositions[boardPositions.Count - 1].transform.position, 0.7f).SetEase(Ease.InOutSine);
            _currentPosition = 0; // math cursor resets exactly like round start
        }

        _isRolling = false;
    }

    private void ApplyRollReward(Roll roll)
    {
        if (roll.winInCash > 0)
        {
            _bonusTotalWin += roll.winInCash;
            SetBonusText(TotalWinTextLandscape, TotalWinTextPortrait, FormatAmount(_bonusTotalWin));

            // Fire-and-forget, like the punch-scale below — the roll loop doesn't wait on these.
            StartCoroutine(PlayLandingShine());
            StartCoroutine(PlayWinLaser());
        }

        if (roll.rollsAdded.HasValue && roll.rollsAdded.Value > 0)
        {
            if (RemainingRollsTextLandscape) RemainingRollsTextLandscape.transform.DOPunchScale(Vector3.one * 0.35f, 1f,0);
            if (RemainingRollsTextPortrait) RemainingRollsTextPortrait.transform.DOPunchScale(Vector3.one * 0.35f, 1f,0);
        }
        // addRolls count / isEnd are applied in PlayBonusRound's loop (rolls-remaining math / loop exit).
    }

    // One-shot flash on the landed tile — same SetActive(true) -> play -> SetActive(false)
    // pattern as PlayCloudTransition, just without waiting on it from the caller.
    private IEnumerator PlayLandingShine()
    {
        if (LandingShineAnimation == null) yield break;

        LandingShineAnimation.gameObject.SetActive(true);
        bool done = false;
        LandingShineAnimation.onAnimationComplete = () => done = true;
        LandingShineAnimation.StartAnimation();

        yield return new WaitUntil(() => done);
        LandingShineAnimation.gameObject.SetActive(false);
    }

    // Travels from WinLaser's rest position to the orientation-correct end point along a slight
    // curve (world space, since WinLaser and the laser end points live under different parents),
    // then triggers the win-text panel reveal on arrival before resetting for next time.
    private IEnumerator PlayWinLaser()
    {
        if (WinLaser == null) yield break;

        bool isPortrait = GetOrientationChange() != null && GetOrientationChange().CurrentMode == OrientationChange.OrientationMode.MobilePortrait;
        RectTransform endPoint = isPortrait ? laserEndPointPortrait : laserEndPointLandscape;
        if (endPoint == null) yield break;

        //WinLaser.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        WinLaser.SetActive(true);

        Vector3 startPos = WinLaser.transform.position;
        Vector3 upPos = startPos + new Vector3(1.7f,1.7f,0); // slight vertical offset to avoid a perfectly straight line
        Vector3 endPos = endPoint.position;
        Vector3 mid = Vector3.Lerp(startPos, endPos, 0.5f);
        Vector3 perpendicular = Vector3.Cross((endPos - startPos).normalized, Vector3.forward);
        mid += perpendicular * (Vector3.Distance(startPos, endPos) * laserCurveAmount);

        yield return WinLaser.transform
            .DOPath(new Vector3[] { startPos, upPos, mid, endPos }, laserTravelDuration, PathType.CatmullRom)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();


        PlayWinTextPanel(isPortrait);
        yield return new WaitForSeconds(laserArrivalWaitDuration);
        WinLaser.SetActive(false);
        WinLaser.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void PlayWinTextPanel(bool isPortrait)
    {
        ImageAnimation panel = isPortrait ? winTextPanelPortrait : winTextPanelLandscape;
        if (panel == null) return;
        panel.StartAnimation();
    }

    private IEnumerator PlayCloudTransition(List<Sprite> frames, bool reverse)
    {
        if (cloudAnimationPlayerLandscape == null ||cloudAnimationPlayerPortrait == null || frames == null || frames.Count == 0) yield break;

        //CloudAnimation.SetActive(true);
        cloudAnimationPlayerLandscape.textureArray = frames;
        cloudAnimationPlayerPortrait.textureArray = frames; 

        bool done = false;

        cloudAnimationPlayerLandscape.onAnimationComplete = () => done = true;
        cloudAnimationPlayerPortrait.onAnimationComplete = () => done = true;

        if (reverse) {
            cloudAnimationPlayerLandscape.transform.localRotation = Quaternion.Euler(0, 180, 0);
            cloudAnimationPlayerPortrait.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else {
            cloudAnimationPlayerLandscape.transform.localRotation = Quaternion.Euler(0, 0, 0);
            cloudAnimationPlayerPortrait.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        // else 
        cloudAnimationPlayerLandscape.StartAnimation();
        cloudAnimationPlayerPortrait.StartAnimation();

        yield return new WaitUntil(() => done);
        //CloudAnimation.SetActive(false);
    }

    private void OnRollButtonClicked() => TryRequestRoll();

    // Called by UIManager's shared start button while a bonus round is active.
    internal void RequestRoll() => TryRequestRoll();

    private void TryRequestRoll()
    {
        if (!IsBonusRoundActive || _isRolling) return;
        _rollRequested = true;
    }

    private void SetRollButtonsInteractable(bool interactable)
    {
        if (RollButton) RollButton.interactable = interactable;
        if (uiManager) uiManager.SetStartButtonInteractable(interactable);
    }

    private string FormatAmount(double amount) => amount.ToString("0.###");

    private void SetBonusText(TMP_Text landscape, TMP_Text portrait, string content)
    {
        if (landscape) landscape.text = content;
        if (portrait) portrait.text = content;
    }

    private void SetRollsText(TMP_Text landscape, TMP_Text portrait, string content)
    {
        if (landscape) landscape.text = UIManager.ToSpriteString($"+{content}");
        if (portrait) portrait.text = UIManager.ToSpriteString($"+{content}");
    }
}
