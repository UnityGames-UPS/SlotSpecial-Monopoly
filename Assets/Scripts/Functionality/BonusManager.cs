using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class BonusManager : MonoBehaviour
{
    [Header("Scene Fade Refs")]
    [SerializeField] private GameObject slotGameObject;
    [SerializeField] private GameObject uiGameObject;
    [SerializeField] private GameObject bonusWheelPanel;
    [SerializeField] private RectTransform rocketAnimationObject;

    [Header("Scene Fade Refs Portrait")]
    [SerializeField] private GameObject uiGameObjectPortrait;
    [SerializeField] private GameObject bonusWheelPanelPortrait;
    [SerializeField] private RectTransform rocketAnimationObjectPortrait;

    [Header("Wheels (0 = inner/wheel1, 1 = middle/wheel2, 2 = outer/wheel3)")]
    [SerializeField] private WheelRig[] wheelRigs;
    [SerializeField] private WheelRig[] wheelRigsPortrait;

    [Header("Managers")]
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioController audioController;

    [Header("Spin Tuning")]
    [SerializeField] private int fullRotations = 4;
    [SerializeField] private float spinTime = 4f;
    [SerializeField] private float postFadeInDelay = 3f;
    [SerializeField] private float stepUpDelay = 1.5f;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Rocket Spawn")]
    [SerializeField] private RocketItem rocketPrefab;
    [SerializeField] private int maxConcurrentRockets = 2;
    [SerializeField] private Vector2 spawnIntervalRange = new Vector2(1.5f, 3f);
    [SerializeField] private float riseSpeed = 300f; // units/second — same for every rocket, regardless of rise distance
    [SerializeField] private Ease riseEase = Ease.Linear;

    private const int SegmentCount = 12;
    private const float SegmentAngle = 360f / SegmentCount;

    internal bool isBonusFinished = true;

    private bool wheelStartRequested;

    private Coroutine rocketSpawnLoopLandscape;
    private Coroutine rocketSpawnLoopPortrait;
    private readonly List<RocketItem> activeRocketsLandscape = new List<RocketItem>();
    private readonly List<RocketItem> activeRocketsPortrait = new List<RocketItem>();

    [System.Serializable]
    private class WheelRig
    {
        public GameObject container;
        public RectTransform wheelTransform;
        public GameObject WiningOverlay;
        public GameObject FullWheelOverlay;
        public List<TMP_Text> segmentTexts;
    }

    // Called by UIManager's shared Start button dispatcher — the button (and its onClick
    // listener) live on UIManager since it's the same physical button used by the
    // free-spin-trigger flow.
    internal void RequestWheelStart()
    {
        if (wheelStartRequested) return;

        // Keep it visible but non-interactable — it shares its screen position with the
        // universal win popup's Take button, so it's only hidden once that popup is about to show.
        uiManager.SetStartButtonInteractable(false);
        wheelStartRequested = true;
    }

    internal void IntializeBonusWheelValue()
    {
        SuperWheelConfig config = socketManager.initialData.superWheel;
        if (config == null) return;

        // Both layouts are kept in sync at all times since they run side by side.
        ApplySegmentsToArray(wheelRigs, config);
        ApplySegmentsToArray(wheelRigsPortrait, config);
    }

    private void ApplySegmentsToArray(WheelRig[] rigs, SuperWheelConfig config)
    {
        if (rigs == null || rigs.Length < 3) return;

        ApplySegments(rigs[0], config.inner);
        ApplySegments(rigs[1], config.middle);
        ApplySegments(rigs[2], config.outer);
    }

    private void ApplySegments(WheelRig rig, List<WheelSegment> segments)
    {
        if (rig == null || segments == null) return;

        for (int i = 0; i < rig.segmentTexts.Count && i < segments.Count; i++)
        {
            WheelSegment segment = segments[i];
            //rig.segmentTexts[i].text = segment.isPowerUp ? "STEP UP" : "x" + segment.multiplier;
            Image powerUpImage = rig.segmentTexts[i].GetComponentInChildren<Image>(true);

            if (segment.isPowerUp)
            {
                rig.segmentTexts[i].text = "";
                if (powerUpImage != null) powerUpImage.gameObject.SetActive(true); // Scale up for power-up segments
            }
            else
            {
                rig.segmentTexts[i].text = segment.multiplier.ToString(); // Default color for normal segments
                if (powerUpImage != null) powerUpImage.gameObject.SetActive(false);
            }
        }
    }

    internal void BonusWheel()
    {
        SuperWheelBonus bonusData = socketManager.resultData.payload.superWheelBonus;
        if (bonusData == null || !bonusData.isTriggered) return;

        StartCoroutine(RunBonusSequence(bonusData));
    }

    private IEnumerator RunBonusSequence(SuperWheelBonus bonusData)
    {
        isBonusFinished = false;
        uiManager.SetSpinButtonInteractable(false);

        ResetWheels(wheelRigs);
        ResetWheels(wheelRigsPortrait);

        // Fire-and-forget fades for the slot/UI canvases on both layouts.
        FadeCanvasGroup(slotGameObject, 0f, fadeDuration);
        FadeCanvasGroup(uiGameObject, 0f, fadeDuration);
        FadeCanvasGroup(uiGameObjectPortrait, 0f, fadeDuration);

        // Show it right away (disabled) so it's already on screen for the fade-in, rather
        // than popping in only once the wheel is ready.
        wheelStartRequested = false;
        uiManager.ShowStartButton(false);

        // Wait for both bonus wheel panels to finish fading in before continuing.
        yield return FadeBothCanvasGroups(bonusWheelPanel, bonusWheelPanelPortrait, 1f, fadeDuration);

        uiManager.SetStartButtonInteractable(true);

        StartRocketAnimation();

        // Wheel only starts spinning once the player presses the start button.
        yield return new WaitUntil(() => wheelStartRequested);

        Spin finalSpin = null;

        for (int i = 0; i < bonusData.spins.Count; i++)
        {
            Spin spin = bonusData.spins[i];
            int wheelIndex = RingToWheelIndex(spin.ring);

            WheelRig rig = wheelRigs[wheelIndex];
            WheelRig rigPortrait = (wheelRigsPortrait != null && wheelRigsPortrait.Length > wheelIndex)
                ? wheelRigsPortrait[wheelIndex]
                : null;

            rig.FullWheelOverlay.SetActive(false);
            if (rigPortrait != null) rigPortrait.FullWheelOverlay.SetActive(false);

            yield return new WaitForSeconds(1f);

            // Spins both wheels together and waits until both have finished rotating.
            yield return RotateWheels(rig, rigPortrait, spin.stopIndex);

            rig.WiningOverlay.SetActive(true);
            if (rigPortrait != null) rigPortrait.WiningOverlay.SetActive(true);

            if (spin.isPowerUp)
            {
                yield return new WaitForSeconds(stepUpDelay);
            }
            else
            {
                yield return new WaitForSeconds(1f);
                finalSpin = spin;
                break;
            }
        }

        if (finalSpin != null)
        {
            bool popupClosed = false;
            audioController.PlayBonusWin();
            // The Take button is about to appear in the same screen slot as the start button.
            uiManager.HideStartButton();
            uiManager.ShowUniversalWinPopup(UIManager.WinPopupType.BonusComplete, bonusData.totalAwardValue, 0,
                () =>
                {
                    uiManager.currentBalance += bonusData.totalAwardValue;
                    uiManager.UpdateBalance(uiManager.currentBalance, true);
                    popupClosed = true;
                }, finalSpin.multiplier);

            yield return new WaitUntil(() => popupClosed);
        }

        FadeCanvasGroup(slotGameObject, 1f, fadeDuration);
        FadeCanvasGroup(uiGameObject, 1f, fadeDuration);
        FadeCanvasGroup(uiGameObjectPortrait, 1f, fadeDuration);

        yield return FadeBothCanvasGroups(bonusWheelPanel, bonusWheelPanelPortrait, 0f, fadeDuration);

        uiManager.HideStartButton();

        StopRocketAnimation();

        isBonusFinished = true;
        // Don't blindly re-enable spinButton here — if a free-spin round is still chaining,
        // SlotManager's own post-bonus logic (ShowFreeSpinBetweenSpinsPlaceholder / SetSpinButtonReady)
        // is what decides the correct button state once it resumes from waiting on isBonusFinished.
        // Setting it true unconditionally here left a brief window where spin was clickable
        // mid-free-spin-chain.
    }

    private int RingToWheelIndex(string ring)
    {
        switch (ring)
        {
            case "inner": return 0;
            case "middle": return 1;
            case "outer": return 2;
            default: return 0;
        }
    }

    private IEnumerator RotateWheels(WheelRig rig, WheelRig rigPortrait, int stopIndex)
    {
        audioController.PlayWheelArrowTick(true);

        float targetAngle = fullRotations * 360f + stopIndex * SegmentAngle;

        bool landscapeDone = true;
        bool portraitDone = true;

        if (rig != null)
        {
            landscapeDone = false;
            rig.wheelTransform
                .DOLocalRotate(new Vector3(0f, 0f, targetAngle), spinTime, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => landscapeDone = true);
        }

        if (rigPortrait != null)
        {
            portraitDone = false;
            rigPortrait.wheelTransform
                .DOLocalRotate(new Vector3(0f, 0f, targetAngle), spinTime, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => portraitDone = true);
        }

        yield return new WaitUntil(() => landscapeDone && portraitDone);

        audioController.PlayWheelArrowStop(false);
    }

    // Starts the rocket spawn loops for whichever orientation(s) have both a prefab and a spawn area
    // wired up. Landscape and portrait spawn independently of each other, matching every other
    // bonus-wheel visual in this file.
    private void StartRocketAnimation()
    {
        activeRocketsLandscape.Clear();
        activeRocketsPortrait.Clear();

        audioController.PlayRocketBackground();

        if (rocketPrefab != null && rocketAnimationObject != null)
        {
            rocketSpawnLoopLandscape = StartCoroutine(RocketSpawnLoop(rocketAnimationObject, activeRocketsLandscape));
        }
        if (rocketPrefab != null && rocketAnimationObjectPortrait != null)
        {
            rocketSpawnLoopPortrait = StartCoroutine(RocketSpawnLoop(rocketAnimationObjectPortrait, activeRocketsPortrait));
        }
    }

    private IEnumerator RocketSpawnLoop(RectTransform spawnArea, List<RocketItem> active)
    {
        while (true)
        {
            if (active.Count < maxConcurrentRockets)
            {
                SpawnRocket(spawnArea, active);
            }

            yield return new WaitForSeconds(Random.Range(spawnIntervalRange.x, spawnIntervalRange.y));
        }
    }

    private void SpawnRocket(RectTransform spawnArea, List<RocketItem> active)
    {
        RocketItem rocket = Instantiate(rocketPrefab, spawnArea);
        active.Add(rocket);

        rocket.Launch(spawnArea, riseSpeed, riseEase, () => active.Remove(rocket));
    }

    // Stops both spawn loops and clears out any rockets still mid-flight so nothing keeps floating
    // once the slot/UI canvases fade back in.
    private void StopRocketAnimation()
    {
        audioController.StopRocketBackground();

        if (rocketSpawnLoopLandscape != null)
        {
            StopCoroutine(rocketSpawnLoopLandscape);
            rocketSpawnLoopLandscape = null;
        }
        if (rocketSpawnLoopPortrait != null)
        {
            StopCoroutine(rocketSpawnLoopPortrait);
            rocketSpawnLoopPortrait = null;
        }

        DestroyActiveRockets(activeRocketsLandscape);
        DestroyActiveRockets(activeRocketsPortrait);
    }

    private void DestroyActiveRockets(List<RocketItem> active)
    {
        for (int i = 0; i < active.Count; i++)
        {
            if (active[i] != null) Destroy(active[i].gameObject);
        }
        active.Clear();
    }

    private void ResetWheels(WheelRig[] rigs)
    {
        if (rigs == null) return;

        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].wheelTransform.localEulerAngles = Vector3.zero;
            rigs[i].container.SetActive(true);
            rigs[i].FullWheelOverlay.SetActive(true);
            rigs[i].WiningOverlay.SetActive(false);
        }
    }

    private IEnumerator FadeBothCanvasGroups(GameObject a, GameObject b, float target, float duration)
    {
        bool aDone = true;
        bool bDone = true;

        if (a != null)
        {
            aDone = false;
            FadeCanvasGroup(a, target, duration, () => aDone = true);
        }

        if (b != null)
        {
            bDone = false;
            FadeCanvasGroup(b, target, duration, () => bDone = true);
        }

        yield return new WaitUntil(() => aDone && bDone);
    }

    private Tween FadeCanvasGroup(GameObject obj, float target, float duration, System.Action onComplete = null)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        if (target > 0f) obj.SetActive(true);

        return cg.DOFade(target, duration).OnComplete(() =>
        {
            if (target <= 0f) obj.SetActive(false);
            onComplete?.Invoke();
        });
    }
}