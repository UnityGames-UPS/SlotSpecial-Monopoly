using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class BonusManager : MonoBehaviour
{
    [Header("CloudAnimation")]
    [SerializeField] private GameObject CloudAnimation;
    [SerializeField] private List<Sprite> cloudFullScreenAnimation;
    [SerializeField] private List<Sprite> cloudClearScreenAnimation;
    [SerializeField] private ImageAnimation cloudAnimationPlayer;

    [Header("BonusPanel")]
    [SerializeField] private GameObject BonusPanel;
    [SerializeField] private GameObject LandscapeBackground;
    [SerializeField] private GameObject PortraitBackground;
    [SerializeField] private TMP_Text TotalWinText;
    [SerializeField] private TMP_Text RemainingRollsText;

    [Header("BoardGame References")]
    [SerializeField] private GameObject boardGame;
    [SerializeField] private GameObject Character;
    [SerializeField] private TMP_Text Dice1Text;
    [SerializeField] private TMP_Text Dice2Text;
    [SerializeField] private Button RollButton;
    [SerializeField] private List<GameObject> boardPositions;
    [SerializeField] private GameObject vaultPrefab;
    [SerializeField] private GameObject addRollsPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject resetPrefab;
    [SerializeField] private GameObject endPrefab;

    [Header("Hop Tuning")]
    [SerializeField] private float hopDuration = 0.15f;
    [SerializeField] private float hopJumpPower = 40f;
    [SerializeField] private float diceRevealDelay = 1f;
    [SerializeField] private float postBonusPause = 2f;

    [Header("External References")]
    [SerializeField] private UIManager uiManager;

    // --- runtime state ---
    private int _currentPosition;
    private int _rollsRemaining;
    private double _bonusTotalWin;
    private bool _isRolling;
    private bool _rollRequested;
    private readonly List<GameObject> _spawnedDecorations = new List<GameObject>();

    internal bool IsBonusRoundActive { get; private set; }

    private void Start()
    {
        if (RollButton) RollButton.onClick.AddListener(OnRollButtonClicked);
    }

    // Entry point — SlotManager.TweenRoutine() yields directly on this once
    // payload.checkersBonus.triggered is true, before the base spin's own win popup/lines.
    internal IEnumerator PlayBonusRound(CheckersBonus data, int startingRollsCount)
    {
        IsBonusRoundActive = true;
        _bonusTotalWin = 0;
        _currentPosition = 0; // math cursor for the first roll always starts at 0

        yield return PlayCloudTransition(cloudFullScreenAnimation, reverse: false);

        BonusPanel.SetActive(true);
        int boardLength = data.board.Count;
        PopulateBoard(data.board);
        PlaceCharacterAtIndex(boardPositions.Count - 1); // cosmetic resting spot, "just before" index 0

        _rollsRemaining = startingRollsCount;
        if (RemainingRollsText) RemainingRollsText.text = _rollsRemaining.ToString();
        if (TotalWinText) TotalWinText.text = FormatAmount(0);

        yield return PlayCloudTransition(cloudClearScreenAnimation, reverse: true);

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
            yield return AnimateRoll(roll, boardLength);

            _rollsRemaining--;
            if (roll.rollsAdded.HasValue) _rollsRemaining += roll.rollsAdded.Value;
            if (RemainingRollsText) RemainingRollsText.text = Mathf.Max(0, _rollsRemaining).ToString();

            ended = roll.isEnd || _rollsRemaining <= 0;
            rollIndex++;

            if (!ended) SetRollButtonsInteractable(true);
        }

        uiManager.HideStartButton();
        yield return new WaitForSeconds(postBonusPause);

        yield return PlayCloudTransition(cloudFullScreenAnimation, reverse: false);
        BonusPanel.SetActive(false);
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
                Transform cellTransform = boardPositions[i].transform;
                instance = Instantiate(prefab, cellTransform.position, Quaternion.identity, cellTransform);
            }
            // Add even when null so the list stays index-aligned with `board` / `boardPositions` —
            // ApplyRollReward looks up _spawnedDecorations[roll.position] directly.
            _spawnedDecorations.Add(instance);
        }
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
        if (Character == null || boardPositions.Count == 0) return;
        index = Mathf.Clamp(index, 0, boardPositions.Count - 1);

        RectTransform characterRT = Character.GetComponent<RectTransform>();
        RectTransform targetRT = boardPositions[index].GetComponent<RectTransform>();
        if (characterRT && targetRT)
        {
            characterRT.anchoredPosition = targetRT.anchoredPosition;
        }
        else
        {
            Character.transform.position = boardPositions[index].transform.position;
        }
    }

    private IEnumerator AnimateRoll(Roll roll, int boardLength)
    {
        _isRolling = true;

        if (Dice1Text) Dice1Text.text = roll.dice1.ToString(); // TODO: dice roll animation
        if (Dice2Text) Dice2Text.text = roll.dice2.ToString(); // TODO: dice roll animation
        yield return new WaitForSeconds(diceRevealDelay);

        RectTransform characterRT = Character ? Character.GetComponent<RectTransform>() : null;
        for (int step = 0; step < roll.sum; step++)
        {
            _currentPosition = (_currentPosition + 1) % boardLength;
            if (characterRT == null) continue;

            RectTransform targetCell = boardPositions[_currentPosition].GetComponent<RectTransform>();
            if (targetCell == null) continue;

            yield return characterRT
                .DOJumpAnchorPos(targetCell.anchoredPosition, hopJumpPower, 1, hopDuration)
                .WaitForCompletion();
        }

        // Safety net — the server's reported position is authoritative even if this ever diverges.
        _currentPosition = roll.position;

        ApplyRollReward(roll);

        if (roll.isReset == true)
        {
            yield return new WaitForSeconds(0.3f);
            PlaceCharacterAtIndex(boardPositions.Count - 1);
            _currentPosition = 0; // math cursor resets exactly like round start
        }

        _isRolling = false;
    }

    private void ApplyRollReward(Roll roll)
    {
        if (roll.winInCash > 0)
        {
            _bonusTotalWin += roll.winInCash;
            if (TotalWinText) TotalWinText.text = FormatAmount(_bonusTotalWin);
        }

        if (roll.position >= 0 && roll.position < _spawnedDecorations.Count)
        {
            GameObject landed = _spawnedDecorations[roll.position];
            if (landed) landed.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f);
        }
        // addRolls / isEnd are applied in PlayBonusRound's loop (rolls-remaining math / loop exit).
    }

    private IEnumerator PlayCloudTransition(List<Sprite> frames, bool reverse)
    {
        if (cloudAnimationPlayer == null || frames == null || frames.Count == 0) yield break;

        //CloudAnimation.SetActive(true);
        cloudAnimationPlayer.textureArray = frames;
        bool done = false;
        cloudAnimationPlayer.onAnimationComplete = () => done = true;

        // if (reverse) cloudAnimationPlayer.StartReverseAnimation();
        // else 
        cloudAnimationPlayer.StartAnimation();

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
}
