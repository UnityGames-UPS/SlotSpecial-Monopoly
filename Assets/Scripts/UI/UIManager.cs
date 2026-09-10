using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SlotManager slotManager;
    [SerializeField] private BonusManager bonusManager;
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField] private AudioController audioController;
    [SerializeField] private JSFunctCalls jsFunctCalls;
    [SerializeField] private SymbolInfoCard symbolInfoCard;

    [Header("Loading & Intro")]
    [SerializeField] private GameObject gameScreen;
    [SerializeField] internal GameObject gameLogoObject;

    [Header("Bet Controls")]
    [SerializeField] private TMP_Text betAmountText;
    [SerializeField] private Button betPlusButton;
    [SerializeField] private Button betMinusButton;
    [Header("Bet Controls - Portrait")]
    [SerializeField] private TMP_Text betAmountTextPortrait;
    [SerializeField] private Button betPlusButtonPortrait;
    [SerializeField] private Button betMinusButtonPortrait;

    [Header("Balance & Win")]
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text winAmountText;
    [SerializeField] private GameObject winTextObject;
    [SerializeField] private GameObject goodLuckObject;
    [Header("Balance & Win - Portrait")]
    [SerializeField] private TMP_Text balanceTextPortrait;
    [SerializeField] private TMP_Text winAmountTextPortrait;
    [SerializeField] private GameObject winTextObjectPortrait;
    [SerializeField] private GameObject goodLuckObjectPortrait;

    [Header("Background Animation")]
    [SerializeField] private ImageAnimation bgImage;

    [Header("Main Popup Backdrop")]
    [SerializeField] private GameObject mainPopupObject;
    [Header("Main Popup Backdrop - Portrait")]
    [SerializeField] private GameObject mainPopupObjectPortrait;

    [Header("Universal Win Popup")]
    [SerializeField] private GameObject universalWinPopup;
    [SerializeField] private GameObject sideStars;
    [SerializeField] private GameObject winTitleImageGameObject;
    [SerializeField] private Image winTitleImage;
    [SerializeField] private TMP_Text uwpWinAmountText;
    [SerializeField] private Button uwpTakeButton;
    [Header("Universal Win Popup - Portrait")]
    [SerializeField] private GameObject universalWinPopupPortrait;
    [SerializeField] private GameObject sideStarsPortrait;
    [SerializeField] private GameObject winTitleImageGameObjectPortrait;
    [SerializeField] private Image winTitleImagePortrait;
    [SerializeField] private TMP_Text uwpWinAmountTextPortrait;
    [SerializeField] private Button uwpTakeButtonPortrait;

    [Header("Universal Win Popup - Win Title Art")]
    [SerializeField] private Sprite bigWinTitleSprite;
    [SerializeField] private Sprite hugeWinTitleSprite;
    [SerializeField] private Sprite megaWinTitleSprite;

    [Header("Universal Win Popup - Tuning")]
    [SerializeField] private double bigWinMultiplier = 7;
    [SerializeField] private double hugeWinMultiplier = 15;
    [SerializeField] private double megaWinMultiplier = 30;
    [SerializeField] private float uwpFadeDuration = 0.5f;
    [SerializeField] private float uwpPulseScale = 1.1f;
    [SerializeField] private float uwpPulseDuration = 0.6f;
    [SerializeField] private float uwpWinCountDuration = 1.5f;
    [SerializeField] private float uwpAutoSpinAutoCloseDelay = 3f;

    [Header("Shared Start Button (free spins)")]
    [SerializeField] private Button WheelStartButton;
    [SerializeField] private Button WheelStartButtonPortrait;

    [Header("Spin Button")]
    [SerializeField] internal Button spinButton;
    [SerializeField] internal Button stopButton;
    [Header("Spin Button - Portrait")]
    [SerializeField] internal Button spinButtonPortrait;
    [SerializeField] internal Button stopButtonPortrait;

    [Header("Auto Spin Toggle")]
    [SerializeField] internal Button autoSpinButton;
    [Header("Auto Spin Toggle - Portrait")]
    [SerializeField] internal Button autoSpinButtonPortrait;

    [Header("Auto Play Stop Control")]
    [SerializeField] private Button autoSpinStopButton;
    [SerializeField] private TMP_Text autoSpinRemainingText;
    [Header("Auto Play Stop Control - Portrait")]
    [SerializeField] private Button autoSpinStopButtonPortrait;
    [SerializeField] private TMP_Text autoSpinRemainingTextPortrait;

    [Header("Auto Play Panel")]
    [SerializeField] private GameObject autoPlayPanel;
    [SerializeField] private RectTransform autoPlayPanelRect;
    [SerializeField] private Button autoPlayCloseButton;
    [Header("Auto Play Selection Buttons")]
    [SerializeField] private Button autoPlay10Button;
    [SerializeField] private Button autoPlay50Button;
    [SerializeField] private Button autoPlay100Button;
    [SerializeField] private Button autoPlay200Button;
    [SerializeField] private Button autoPlay500Button;
    [SerializeField] private Button autoPlayInfiniteButton;
    [Header("Auto Play Panel - Portrait")]
    [SerializeField] private GameObject autoPlayPanelPortrait;
    [SerializeField] private RectTransform autoPlayPanelRectPortrait;
    [SerializeField] private Button autoPlayCloseButtonPortrait;
    [SerializeField] private Button autoPlay10ButtonPortrait;
    [SerializeField] private Button autoPlay50ButtonPortrait;
    [SerializeField] private Button autoPlay100ButtonPortrait;
    [SerializeField] private Button autoPlay200ButtonPortrait;
    [SerializeField] private Button autoPlay500ButtonPortrait;
    [SerializeField] private Button autoPlayInfiniteButtonPortrait;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private RectTransform settingsPanelRect;
    [SerializeField] private Button settingsOpenButton;
    [SerializeField] private Button settingsCloseButton;
    [SerializeField] private Button settingsBgCloseButton;
    [SerializeField] private Button gameQuitButton;
    [Header("Settings Panel - Portrait")]
    [SerializeField] private GameObject settingsPanelPortrait;
    [SerializeField] private RectTransform settingsPanelRectPortrait;
    [SerializeField] private Button settingsOpenButtonPortrait;
    [SerializeField] private Button settingsCloseButtonPortrait;
    [SerializeField] private Button settingsBgCloseButtonPortrait;
    [SerializeField] private Button gameQuitButtonPortrait;

    [Header("Speed Buttons (Three-Layer Toggle)")]
    [SerializeField] private Button normalSpeedButton;
    [SerializeField] private Button turboSpeedButton;
    [SerializeField] private Button quickSpeedButton;
    [Header("Speed Buttons - Portrait")]
    [SerializeField] private Button normalSpeedButtonPortrait;
    [SerializeField] private Button turboSpeedButtonPortrait;
    [SerializeField] private Button quickSpeedButtonPortrait;

    [Header("Sound Panel")]
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private RectTransform soundPanelRect;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button soundPanelCloseButton;
    [SerializeField] private Button soundPanelOpenButton;
    [SerializeField] private Button soundPanelOpenButtonPortrait;

    [Header("Game Rules Panel")]
    [SerializeField] private GameObject gameRulesPanel;
    [SerializeField] private RectTransform gameRulesPanelRect;
    [SerializeField] private Button gameRulesOpenButton;
    [SerializeField] private Button gameRulesBackButton;
    [Header("Game Rules Panel - Portrait")]
    [SerializeField] private Button gameRulesOpenButtonPortrait;

    [Header("Guide Panel")]
    [SerializeField] private GameObject guidePanel;
    [SerializeField] private RectTransform guidePanelRect;
    [SerializeField] private Button guideOpenButton;
    [SerializeField] private Button guideBackButton;
    [Header("Guide Panel - Portrait")]
    [SerializeField] private Button guideOpenButtonPortrait;

    [Header("Game Rules Dynamic Texts")]
    [SerializeField] private TMP_Text totalLineCountText;
    [SerializeField] private TMP_Text ruleSymbol0Text;
    [SerializeField] private TMP_Text ruleSymbol1Text;
    [SerializeField] private TMP_Text ruleSymbol2Text;
    [SerializeField] private TMP_Text ruleSymbol3Text;
    [SerializeField] private TMP_Text ruleSymbol4Text;
    [SerializeField] private TMP_Text ruleSymbol5Text;
    [SerializeField] private TMP_Text ruleSymbol6Text;
    [SerializeField] private TMP_Text ruleSymbol7Text;
    [SerializeField] private TMP_Text ruleSymbol8Text;
    [SerializeField] private TMP_Text ruleSymbol9Text;

    [Header("Free Spin Count Display - Game Screen")]
    [SerializeField] private GameObject freeSpinCountContainer;
    [SerializeField] private TMP_Text remainingFreeSpinsText;
    [SerializeField] private TMP_Text FreeSpinTotalWinText;
    [SerializeField] private GameObject freeSpinCountContainerPortrait;
    [SerializeField] private TMP_Text remainingFreeSpinsTextPortrait;
    [SerializeField] private TMP_Text FreeSpinTotalWinTextPortrait;

    [Header("Ping Display")]
    [SerializeField] private TMP_Text pingText;
    [SerializeField] private TMP_Text pingTextPortrait;

    [Header("Platform Jackpot")]
    [SerializeField] private TMP_Text grandJackpotText;
    [SerializeField] private TMP_Text majorJackpotText;
    [SerializeField] private TMP_Text minorJackpotText;
    [SerializeField] private TMP_Text miniJackpotText;
    [Header("Platform Jackpot - Portrait")]
    [SerializeField] private TMP_Text grandJackpotTextPortrait;
    [SerializeField] private TMP_Text majorJackpotTextPortrait;
    [SerializeField] private TMP_Text minorJackpotTextPortrait;
    [SerializeField] private TMP_Text miniJackpotTextPortrait;

    [Header("Platform Jackpot Animation - Portrait")]
    [SerializeField] private RectTransform grandJackpotPortraitParent;
    [SerializeField] private RectTransform majorJackpotPortraitParent;
    [SerializeField] private RectTransform minorJackpotPortraitParent;
    [SerializeField] private RectTransform miniJackpotPortraitParent;
    [SerializeField] private bool enableJackpotPortraitLevitation = true;
    [SerializeField] private float jackpotLevitateHeight = 10f;
    [SerializeField] private float jackpotLevitateDuration = 1.4f;
    [SerializeField] private float jackpotStaggerDelay = 0.15f;

    [Header("Expand-Shrink Controls")]
    [SerializeField] private Button expandButton;
    [SerializeField] private Button shrinkButton;
    [Header("Expand-Shrink Controls - Portrait")]
    [SerializeField] private Button expandButtonPortrait;
    [SerializeField] private Button shrinkButtonPortrait;

    [Header("Disconnection Popup")]
    [SerializeField] private Button closeDisconnectButton;
    [SerializeField] private GameObject disconnectPopup;
    [Header("Disconnection Popup - Portrait")]
    [SerializeField] private Button closeDisconnectButtonPortrait;
    [SerializeField] private GameObject disconnectPopupPortrait;

    [Header("Reconnection Popup")]
    [SerializeField] private GameObject reconnectPopup;
    [Header("Reconnection Popup - Portrait")]
    [SerializeField] private GameObject reconnectPopupPortrait;

    [Header("Another Device Popup")]
    [SerializeField] private Button closeADButton;
    [SerializeField] private GameObject adPopup;
    [Header("Another Device Popup - Portrait")]
    [SerializeField] private Button closeADButtonPortrait;
    [SerializeField] private GameObject adPopupPortrait;

    [Header("Low Balance Popup")]
    [SerializeField] private Button lowBalanceExitButton;
    [SerializeField] private GameObject lowBalancePopup;
    [Header("Low Balance Popup - Portrait")]
    [SerializeField] private Button lowBalanceExitButtonPortrait;
    [SerializeField] private GameObject lowBalancePopupPortrait;

    [Header("Quit Popup")]
    [SerializeField] private GameObject quitPopup;
    [SerializeField] private Button yesQuitButton;
    [SerializeField] private Button noQuitButton;
    [SerializeField] private Button crossQuitButton;
    [Header("Quit Popup - Portrait")]
    [SerializeField] private GameObject quitPopupPortrait;
    [SerializeField] private Button yesQuitButtonPortrait;
    [SerializeField] private Button noQuitButtonPortrait;
    [SerializeField] private Button crossQuitButtonPortrait;

    [Header("Rapid Stop Cooldown")]
    [Tooltip("Seconds the player must wait before pressing Stop again after an immediate stop.")]
    [SerializeField] private float rapidStopCooldown = 1f;
    private float lastRapidStopTime = -99f;

    // Cross-file API surface: SlotManager/BonusManager/SocketManager/SymbolClickHandler
    // call directly into these exact field/method names — kept stable even though the
    // reference project routes the equivalent state through GameManager/PopupManager.
    internal double currentBalance = 0;
    internal double currentTotalBet = 0;
    internal int betCounter = 0;

    private bool isExit = false;
    private bool isExpanded = false;

    private Tween balanceTween;
    private Tween winTween;
    private double _currentWinDisplayAmount = 0;
    private int totalFreeSpinsAwarded = 0;

    private Coroutine bgAnimationRoutine;

    private System.Action universalWinPopupCallback;
    private Coroutine uwpAutoCloseCoroutine;
    private readonly List<Tween> uwpTweens = new List<Tween>();
    private Tween freeSpinTotalWinTween;

    private OrientationChange orientationChange;
    private readonly Dictionary<Transform, Vector3> jackpotInitialLocalPositions = new Dictionary<Transform, Vector3>();
    private readonly List<Tween> jackpotPortraitTweens = new List<Tween>();

    private void Awake()
    {
        if (jsFunctCalls != null)
        {
            jsFunctCalls.RegisterVisibilityListener(gameObject.name);
            jsFunctCalls.RegisterFullscreenListener(gameObject.name);
        }
    }

    public void OnFocusChanged(string value)
    {
        bool focused = value == "1";
        Debug.Log("UNITY FOCUS CHANGED: " + value + " (focused: " + focused + ")");
        if (audioController != null) audioController.SetMuteAll(!focused);
        if (socketManager != null) socketManager.HandleFocusChange(focused);
    }

    internal void OnFullscreenChanged(string isFullscreen)
    {
        bool newExpandedState = isFullscreen == "1";
        if (isExpanded != newExpandedState)
        {
            isExpanded = newExpandedState;
            SetExpandShrinkButtons(isExpanded);
        }
    }

    private void Start()
    {
        SetupButtons();
        SetupAutoPlayPanel();
        SetupSettingsPanel();
        SetupGameRulesPanel();
        SetupGuidePanel();

        InitializeExpandShrink();
        UpdateSpeedButtonsVisibility(SlotManager.SpinSpeed.Normal);

        if (gameScreen) gameScreen.SetActive(true);
        InitializeUI();
        UpdateJackpotPortraitLevitationFromCurrentOrientation();

        //ShowUniversalWinPopup(WinPopupType.HugeWin, 99.99,false);
    }

    private void OnEnable()
    {
        var oc = GetOrientationChange();
        if (oc != null) oc.OnOrientationChangedInstance += HandleOrientationChangedForJackpotLevitation;
    }

    private void OnDisable()
    {
        if (orientationChange != null) orientationChange.OnOrientationChangedInstance -= HandleOrientationChangedForJackpotLevitation;
        StopJackpotPortraitLevitation();
    }

    private void HandleOrientationChangedForJackpotLevitation(OrientationChange.OrientationMode mode, int width, int height)
    {
        UpdateJackpotPortraitLevitation(mode);
    }

    private void InitializeUI()
    {
        SetGameObjectActive(autoPlayPanel, autoPlayPanelPortrait, false);
        if (autoPlayPanelRect) autoPlayPanelRect.anchoredPosition = new Vector2(autoPlayPanelRect.anchoredPosition.x, -600f);
        if (autoPlayPanelRectPortrait) autoPlayPanelRectPortrait.anchoredPosition = new Vector2(autoPlayPanelRectPortrait.anchoredPosition.x, -600f);
        SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, false);

        SetGameObjectActive(settingsPanel, settingsPanelPortrait, false);
        SetGameObjectActive(soundPanel, null, false);
        SetGameObjectActive(gameRulesPanel, null, false);
        SetGameObjectActive(guidePanel, null, false);

        KillUwpTweens();
        SetGameObjectActive(universalWinPopup, universalWinPopupPortrait, false);

        if (freeSpinCountContainer) freeSpinCountContainer.SetActive(false);
        if (freeSpinCountContainerPortrait) freeSpinCountContainerPortrait.SetActive(false);

        UpdatePingDisplay("-- ms");
    }

    #region UI Synchronization Helpers

    private void SetGameObjectActive(GameObject obj1, GameObject obj2, bool active)
    {
        if (obj1) obj1.SetActive(active);
        if (obj2) obj2.SetActive(active);
    }

    private void SetButtonActive(Button btn1, Button btn2, bool active)
    {
        if (btn1) btn1.gameObject.SetActive(active);
        if (btn2) btn2.gameObject.SetActive(active);
    }

    private void SetButtonInteractable(Button btn1, Button btn2, bool interactable)
    {
        if (btn1) btn1.interactable = interactable;
        if (btn2) btn2.interactable = interactable;
    }

    private void SetTMPText(TMP_Text text1, TMP_Text text2, string content)
    {
        if (text1) text1.text = content;
        if (text2) text2.text = content;
    }

    private bool IsActivePair(GameObject a, GameObject b)
    {
        return (a != null && a.activeSelf) || (b != null && b.activeSelf);
    }

    #endregion

    #region Button Setup

    private void SetupButtons()
    {
        WireSpinButton(spinButton);
        WireSpinButton(spinButtonPortrait);

        if (stopButton) stopButton.onClick.AddListener(OnStopButtonPressed);
        if (stopButtonPortrait) stopButtonPortrait.onClick.AddListener(OnStopButtonPressed);

        if (autoSpinButton) autoSpinButton.onClick.AddListener(OnAutoSpinButtonPressed);
        if (autoSpinButtonPortrait) autoSpinButtonPortrait.onClick.AddListener(OnAutoSpinButtonPressed);

        if (autoSpinStopButton) autoSpinStopButton.onClick.AddListener(OnAutoSpinButtonPressed);
        if (autoSpinStopButtonPortrait) autoSpinStopButtonPortrait.onClick.AddListener(OnAutoSpinButtonPressed);

        if (betPlusButton) betPlusButton.onClick.AddListener(() => ChangeBet(true));
        if (betPlusButtonPortrait) betPlusButtonPortrait.onClick.AddListener(() => ChangeBet(true));
        if (betMinusButton) betMinusButton.onClick.AddListener(() => ChangeBet(false));
        if (betMinusButtonPortrait) betMinusButtonPortrait.onClick.AddListener(() => ChangeBet(false));

        if (gameQuitButton) gameQuitButton.onClick.AddListener(() => { audioController.PlayUIButton(false); OpenPopup(quitPopup, quitPopupPortrait); });
        if (gameQuitButtonPortrait) gameQuitButtonPortrait.onClick.AddListener(() => { audioController.PlayUIButton(false); OpenPopup(quitPopup, quitPopupPortrait); });

        if (yesQuitButton) yesQuitButton.onClick.AddListener(CallOnExitFunction);
        if (yesQuitButtonPortrait) yesQuitButtonPortrait.onClick.AddListener(CallOnExitFunction);
        if (noQuitButton) noQuitButton.onClick.AddListener(() => { if (!isExit) ClosePopup(quitPopup, quitPopupPortrait); });
        if (noQuitButtonPortrait) noQuitButtonPortrait.onClick.AddListener(() => { if (!isExit) ClosePopup(quitPopup, quitPopupPortrait); });
        if (crossQuitButton) crossQuitButton.onClick.AddListener(() => { if (!isExit) ClosePopup(quitPopup, quitPopupPortrait); });
        if (crossQuitButtonPortrait) crossQuitButtonPortrait.onClick.AddListener(() => { if (!isExit) ClosePopup(quitPopup, quitPopupPortrait); });

        if (closeDisconnectButton) closeDisconnectButton.onClick.AddListener(CallOnExitFunction);
        if (closeDisconnectButtonPortrait) closeDisconnectButtonPortrait.onClick.AddListener(CallOnExitFunction);
        if (closeADButton) closeADButton.onClick.AddListener(CallOnExitFunction);
        if (closeADButtonPortrait) closeADButtonPortrait.onClick.AddListener(CallOnExitFunction);
        if (lowBalanceExitButton) lowBalanceExitButton.onClick.AddListener(() => ClosePopup(lowBalancePopup, lowBalancePopupPortrait));
        if (lowBalanceExitButtonPortrait) lowBalanceExitButtonPortrait.onClick.AddListener(() => ClosePopup(lowBalancePopup, lowBalancePopupPortrait));

        if (expandButton) expandButton.onClick.AddListener(() => { audioController.PlayUIButton(false); OnExpand(); });
        if (shrinkButton) shrinkButton.onClick.AddListener(() => { audioController.PlayUIButton(false); OnShrink(); });
        if (expandButtonPortrait) expandButtonPortrait.onClick.AddListener(() => { audioController.PlayUIButton(false); OnExpand(); });
        if (shrinkButtonPortrait) shrinkButtonPortrait.onClick.AddListener(() => { audioController.PlayUIButton(false); OnShrink(); });

        if (uwpTakeButton) uwpTakeButton.onClick.AddListener(OnUniversalWinTakeButtonClicked);
        if (uwpTakeButtonPortrait) uwpTakeButtonPortrait.onClick.AddListener(OnUniversalWinTakeButtonClicked);

        // Shared by the bonus wheel and the free-spin-trigger flow — same physical button in
        // the scene. A single listener here dispatches to whichever flow is currently active.
        if (WheelStartButton) WheelStartButton.onClick.AddListener(OnSharedStartButtonClicked);
        if (WheelStartButtonPortrait) WheelStartButtonPortrait.onClick.AddListener(OnSharedStartButtonClicked);
        SetButtonActive(WheelStartButton, WheelStartButtonPortrait, false);

        if (normalSpeedButton) normalSpeedButton.onClick.AddListener(() => { SetSpeedMode(SlotManager.SpinSpeed.Turbo); });
        if (turboSpeedButton) turboSpeedButton.onClick.AddListener(() => { SetSpeedMode(SlotManager.SpinSpeed.QuickSpin); });
        if (quickSpeedButton) quickSpeedButton.onClick.AddListener(() => { SetSpeedMode(SlotManager.SpinSpeed.Normal); });
        if (normalSpeedButtonPortrait) normalSpeedButtonPortrait.onClick.AddListener(() => { SetSpeedMode(SlotManager.SpinSpeed.Turbo); });
        if (turboSpeedButtonPortrait) turboSpeedButtonPortrait.onClick.AddListener(() => { SetSpeedMode(SlotManager.SpinSpeed.QuickSpin); });
        if (quickSpeedButtonPortrait) quickSpeedButtonPortrait.onClick.AddListener(() => { SetSpeedMode(SlotManager.SpinSpeed.Normal); });
    }

    private void WireSpinButton(Button button)
    {
        if (!button) return;

        var holdHandler = button.GetComponent<SpinButtonHoldHandler>();
        if (holdHandler != null)
        {
            holdHandler.OnClick.AddListener(OnSpinButtonPressed);
            holdHandler.OnHoldThreeSeconds.AddListener(OnSpinButtonHeld);
        }
        else
        {
            button.onClick.AddListener(OnSpinButtonPressed);
        }
    }

    private void SetupAutoPlayPanel()
    {
        if (autoPlay10Button) autoPlay10Button.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(10); });
        if (autoPlay50Button) autoPlay50Button.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(50); });
        if (autoPlay100Button) autoPlay100Button.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(100); });
        if (autoPlay200Button) autoPlay200Button.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(200); });
        if (autoPlay500Button) autoPlay500Button.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(500); });
        if (autoPlayInfiniteButton) autoPlayInfiniteButton.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(-1); });

        if (autoPlay10ButtonPortrait) autoPlay10ButtonPortrait.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(10); });
        if (autoPlay50ButtonPortrait) autoPlay50ButtonPortrait.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(50); });
        if (autoPlay100ButtonPortrait) autoPlay100ButtonPortrait.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(100); });
        if (autoPlay200ButtonPortrait) autoPlay200ButtonPortrait.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(200); });
        if (autoPlay500ButtonPortrait) autoPlay500ButtonPortrait.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(500); });
        if (autoPlayInfiniteButtonPortrait) autoPlayInfiniteButtonPortrait.onClick.AddListener(() => { audioController.PlayAutoplaySelect(); StartAutoplayWithRounds(-1); });

        if (autoPlayCloseButton) autoPlayCloseButton.onClick.AddListener(CloseAutoPlayPanel);
        if (autoPlayCloseButtonPortrait) autoPlayCloseButtonPortrait.onClick.AddListener(CloseAutoPlayPanel);
    }

    private void SetupSettingsPanel()
    {
        if (settingsOpenButton) settingsOpenButton.onClick.AddListener(() => { audioController.PlayUIButton(false); OpenSettingsPanel(); });
        if (settingsOpenButtonPortrait) settingsOpenButtonPortrait.onClick.AddListener(() => { audioController.PlayUIButton(false); OpenSettingsPanel(); });

        if (settingsCloseButton) settingsCloseButton.onClick.AddListener(() => { audioController.PlayUIButton(false); CloseSettingsPanel(); });
        if (settingsCloseButtonPortrait) settingsCloseButtonPortrait.onClick.AddListener(() => { audioController.PlayUIButton(false); CloseSettingsPanel(); });
        if (settingsBgCloseButton) settingsBgCloseButton.onClick.AddListener(() => { audioController.PlayUIButton(false); CloseSettingsPanel(); });
        if (settingsBgCloseButtonPortrait) settingsBgCloseButtonPortrait.onClick.AddListener(() => { audioController.PlayUIButton(false); CloseSettingsPanel(); });

        SetButtonActive(settingsOpenButton, settingsOpenButtonPortrait, true);
        SetButtonActive(settingsCloseButton, settingsCloseButtonPortrait, false);
        SetButtonActive(settingsBgCloseButton, settingsBgCloseButtonPortrait, false);

        if (soundPanelOpenButton) soundPanelOpenButton.onClick.AddListener(OpenSoundPanel);
        if (soundPanelOpenButtonPortrait) soundPanelOpenButtonPortrait.onClick.AddListener(OpenSoundPanel);
        if (soundPanelCloseButton) soundPanelCloseButton.onClick.AddListener(CloseSoundPanel);

        if (musicSlider)
        {
            musicSlider.value = audioController.MusicVolume;
            musicSlider.onValueChanged.AddListener(audioController.SetMusicVolume);
        }
        if (sfxSlider)
        {
            sfxSlider.value = audioController.SfxVolume;
            sfxSlider.onValueChanged.AddListener(audioController.SetSfxVolume);
        }
    }

    private void SetupGameRulesPanel()
    {
        if (gameRulesOpenButton) gameRulesOpenButton.onClick.AddListener(OpenGameRulesPanel);
        if (gameRulesOpenButtonPortrait) gameRulesOpenButtonPortrait.onClick.AddListener(OpenGameRulesPanel);
        if (gameRulesBackButton) gameRulesBackButton.onClick.AddListener(() => { audioController.PlayUIButton(false); CloseGameRulesPanel(); });
    }

    private void SetupGuidePanel()
    {
        if (guideOpenButton) guideOpenButton.onClick.AddListener(OpenGuidePanel);
        if (guideOpenButtonPortrait) guideOpenButtonPortrait.onClick.AddListener(OpenGuidePanel);
        if (guideBackButton) guideBackButton.onClick.AddListener(() => { audioController.PlayUIButton(false); CloseGuidePanel(); });
    }

    #endregion

    #region Spin Button

    public void OnSpinButtonPressed()
    {
        if (slotManager._isAutoSpin)
        {
            audioController.PlayUIButton(false);
            SetButtonInteractable(autoSpinButton, autoSpinButtonPortrait, false);
            slotManager.StopAutoSpin();
            AutoSpinButtonAnimation(false);
            return;
        }

        // if (!bonusManager.isBonusFinished)
        // {
        //     bonusManager.wheelButtonPressed = true;
        //     SetButtonInteractable(spinButton, spinButtonPortrait, false);
        //     return;
        // }

        audioController.PlayUIButton(false);
        SetButtonInteractable(spinButton, spinButtonPortrait, false);
        SetBetControlsEnabled(false);

        slotManager.StartSlots();
        ShowReelSpinningButton(true);
    }

    // Which control represents "a reel is currently spinning" depends on spin speed:
    // normal → stopButton (can instant-stop this reel). Turbo/quick spin → spinButton
    // instead, disabled — those spins are already near-instant, so there's nothing
    // meaningful left to stop, and the stop control shouldn't be offered at all.
    private void ShowReelSpinningButton(bool stopInteractable)
    {
        bool useStopButton = !(slotManager != null && slotManager.IsTurboOn);
        SetButtonActive(stopButton, stopButtonPortrait, useStopButton);
        SetButtonInteractable(stopButton, stopButtonPortrait, useStopButton && stopInteractable);
        SetButtonActive(spinButton, spinButtonPortrait, !useStopButton);
        SetButtonInteractable(spinButton, spinButtonPortrait, false);
    }

    public void OnSpinButtonHeld()
    {
        if (!slotManager._isAutoSpin)
        {
            audioController.PlayUIButton(false);
            OpenAutoPlayPanel();
        }
    }

    private void OnStopButtonPressed()
    {
        if (Time.unscaledTime - lastRapidStopTime < rapidStopCooldown) return;
        lastRapidStopTime = Time.unscaledTime;

        audioController.PlayUIButton(false);
        slotManager.RequestInstantStop();

        // During a free-spin round, stop only fast-forwards the current reel — autoplay
        // (if on) keeps running and resumes normally once free spins end.
        if (slotManager._isAutoSpin && !slotManager.isInFreeSpins)
        {
            slotManager.StopAutoSpin();
            AutoSpinButtonAnimation(false);
            SetButtonInteractable(autoSpinButton, autoSpinButtonPortrait, false);
        }

        ShowReelSpinningButton(false);
    }

    internal void OnStopSpinButtonTrigger()
    {
        OnStopButtonPressed();
    }

    // Called the instant all reels have physically stopped — there's nothing left to
    // instant-stop, so the button disappears even while post-stop animations (win lines,
    // heatup, wheel/free-spin trigger) are still playing. spinButton takes its place as a
    // disabled placeholder in the meantime. SetSpinButtonReady() (or the free-spin
    // between-spins placeholder) decides what actually shows once those finish.
    internal void HideStopButtonAfterReelsStopped()
    {
        SetButtonInteractable(stopButton, stopButtonPortrait, false);
        SetButtonActive(stopButton, stopButtonPortrait, false);

        SetButtonActive(spinButton, spinButtonPortrait, true);
        SetButtonInteractable(spinButton, spinButtonPortrait, false);
    }

    internal void SetSpinButtonReady()
    {
        SetButtonInteractable(stopButton, stopButtonPortrait, true);

        if (slotManager._isAutoSpin)
        {
            // This state carries through the next autospin round's actual spinning too,
            // so it needs to respect turbo/quick spin the same as a fresh manual spin does.
            ShowReelSpinningButton(true);
        }
        else
        {
            SetButtonInteractable(spinButton, spinButtonPortrait, true);
            SetButtonActive(spinButton, spinButtonPortrait, true);
            SetButtonActive(stopButton, stopButtonPortrait, false);
            SetBetControlsEnabled(true);
            SetButtonInteractable(autoSpinButton, autoSpinButtonPortrait, true);
        }

        SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, slotManager._isAutoSpin);
    }

    private void OnAutoSpinButtonPressed()
    {
        audioController.PlayUIButton(false);
        // if (!bonusManager.isBonusFinished)
        // {
        //     ToggleAutoSpin();
        // }
        // else
        {
            ToggleAutoSpin();
            if (slotManager._isAutoSpin)
            {
                ShowReelSpinningButton(true);
            }
        }

        SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, slotManager._isAutoSpin);
        UpdateAutoPlayCount();
    }

    private void ToggleAutoSpin()
    {
        if (!slotManager._isAutoSpin)
        {
            slotManager.AutoSpin();
            SetBetControlsEnabled(false);
            AutoSpinButtonAnimation(true);
        }
        else
        {
            SetButtonInteractable(autoSpinButton, autoSpinButtonPortrait, false);
            slotManager.StopAutoSpin();
            ShowReelSpinningButton(false);
            AutoSpinButtonAnimation(false);
        }
    }

    // Disabled (still visible) while a free-spin/wheel trigger's symbol animation is
    // playing, so autoplay can't be cancelled mid-animation.
    internal void SetAutoSpinStopButtonInteractable(bool interactable)
    {
        SetButtonInteractable(autoSpinStopButton, autoSpinStopButtonPortrait, interactable);
    }

    internal void UpdateAutoPlayCount()
    {
        string displayStr = slotManager.autoSpinRoundsRemaining < 0 ? "∞" : slotManager.autoSpinRoundsRemaining.ToString();
        SetTMPText(autoSpinRemainingText, autoSpinRemainingTextPortrait, displayStr);
    }

    private void AutoSpinButtonAnimation(bool animate)
    {
        AnimateAutoSpinArrow(autoSpinButton, animate);
        AnimateAutoSpinArrow(autoSpinButtonPortrait, animate);
    }

    private void AnimateAutoSpinArrow(Button button, bool animate)
    {
        if (!button || button.transform.childCount == 0) return;
        var arrowObject = button.transform.GetChild(0).gameObject;
        if (animate)
        {
            arrowObject.transform.DORotate(new Vector3(0, 0, -360), 3f, RotateMode.LocalAxisAdd)
                       .SetEase(Ease.Linear)
                       .SetLoops(-1);
        }
        else
        {
            arrowObject.transform.DOKill();
        }
    }

    #endregion

    #region Bet Controls

    internal void UpdateBetDisplay()
    {
        if (socketManager.initialData == null) return;

        double totalPay = socketManager.initialData.bets[betCounter] * socketManager.initialData.lines.Count;
        currentTotalBet = totalPay;

        SetTMPText(betAmountText, betAmountTextPortrait, FormatAmount(totalPay));
        UpdateGameRulesDynamicTexts();
    }

    private void ChangeBet(bool increase)
    {
        if (increase)
        {
            betCounter++;
            if (betCounter >= socketManager.initialData.bets.Count) betCounter = 0;
        }
        else
        {
            betCounter--;
            if (betCounter < 0) betCounter = socketManager.initialData.bets.Count - 1;
        }

        if (betCounter == socketManager.initialData.bets.Count - 1)
            audioController.PlayMaxBet();
        else
            audioController.PlayUIButton(false);

        UpdateBetDisplay();
    }

    private void SetBetControlsEnabled(bool enabled)
    {
        SetButtonInteractable(betPlusButton, betPlusButtonPortrait, enabled);
        SetButtonInteractable(betMinusButton, betMinusButtonPortrait, enabled);
    }

    #endregion

    #region Auto Play Panel

    private void OpenAutoPlayPanel()
    {
        audioController.PlayAutoplayOpen();
        if (IsActivePair(settingsPanel, settingsPanelPortrait)) ClosePopup(settingsPanel, settingsPanelPortrait);

        SetGameObjectActive(autoPlayPanel, autoPlayPanelPortrait, true);
        if (autoPlayPanelRect)
        {
            autoPlayPanelRect.anchoredPosition = new Vector2(autoPlayPanelRect.anchoredPosition.x, -600f);
            autoPlayPanelRect.DOAnchorPosY(0f, 0.35f).SetEase(Ease.OutCubic);
        }
        if (autoPlayPanelRectPortrait)
        {
            autoPlayPanelRectPortrait.anchoredPosition = new Vector2(autoPlayPanelRectPortrait.anchoredPosition.x, -600f);
            autoPlayPanelRectPortrait.DOAnchorPosY(0f, 0.35f).SetEase(Ease.OutCubic);
        }
    }

    private void CloseAutoPlayPanel()
    {
        audioController.PlayUIButton(false);

        if (autoPlayPanelRect)
        {
            autoPlayPanelRect.DOAnchorPosY(-600f, 0.35f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if (autoPlayPanel) autoPlayPanel.SetActive(false);
            });
        }
        else if (autoPlayPanel) autoPlayPanel.SetActive(false);

        if (autoPlayPanelRectPortrait)
        {
            autoPlayPanelRectPortrait.DOAnchorPosY(-600f, 0.35f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if (autoPlayPanelPortrait) autoPlayPanelPortrait.SetActive(false);
            });
        }
        else if (autoPlayPanelPortrait) autoPlayPanelPortrait.SetActive(false);
    }

    private void StartAutoplayWithRounds(int rounds)
    {
        CloseAutoPlayPanel();
        SetBetControlsEnabled(false);
        AutoSpinButtonAnimation(true);
        slotManager.AutoSpin(rounds);
        ShowReelSpinningButton(true);
        SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, true);
        UpdateAutoPlayCount();
    }

    #endregion

    #region Spin Speed

    internal void SetSpeedMode(SlotManager.SpinSpeed speed)
    {
        if (speed == SlotManager.SpinSpeed.Normal) audioController.PlayUIButton(false);
        else audioController.PlayTurboActivate();

        slotManager.SetSpinSpeed(speed);
        UpdateSpeedButtonsVisibility(speed);
    }

    private void UpdateSpeedButtonsVisibility(SlotManager.SpinSpeed speed)
    {
        SetButtonActive(normalSpeedButton, normalSpeedButtonPortrait, speed == SlotManager.SpinSpeed.Normal);
        SetButtonActive(turboSpeedButton, turboSpeedButtonPortrait, speed == SlotManager.SpinSpeed.Turbo);
        SetButtonActive(quickSpeedButton, quickSpeedButtonPortrait, speed == SlotManager.SpinSpeed.QuickSpin);
    }

    #endregion

    #region Sound Panel

    private void OpenSoundPanel()
    {
        audioController.PlayUIButton(false);
        if (soundPanel == null) return;
        soundPanel.SetActive(true);
        if (soundPanelRect != null) AnimatePopupOpen(soundPanelRect);

        if (musicSlider) musicSlider.value = audioController.MusicVolume;
        if (sfxSlider) sfxSlider.value = audioController.SfxVolume;
    }

    private void CloseSoundPanel()
    {
        if (soundPanel == null || !soundPanel.activeSelf) return;
        if (soundPanelRect != null)
        {
            AnimatePopupClose(soundPanelRect, () => soundPanel.SetActive(false));
        }
        else
        {
            soundPanel.SetActive(false);
        }
    }

    #endregion

    #region Settings Panel

    private void OpenSettingsPanel()
    {
        if (IsActivePair(autoPlayPanel, autoPlayPanelPortrait)) CloseAutoPlayPanelImmediate();

        SetButtonActive(settingsOpenButton, settingsOpenButtonPortrait, false);
        SetButtonActive(settingsCloseButton, settingsCloseButtonPortrait, true);
        SetButtonActive(settingsBgCloseButton, settingsBgCloseButtonPortrait, true);

        FadeInSettingsPanel(settingsPanel);
        FadeInSettingsPanel(settingsPanelPortrait);
    }

    private void FadeInSettingsPanel(GameObject panel)
    {
        if (!panel) return;
        panel.SetActive(true);
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();
        cg.DOKill();
        cg.alpha = 0f;
        cg.DOFade(1f, 0.35f);
    }

    private void CloseSettingsPanel()
    {
        SetButtonActive(settingsOpenButton, settingsOpenButtonPortrait, true);
        SetButtonActive(settingsCloseButton, settingsCloseButtonPortrait, false);
        SetButtonActive(settingsBgCloseButton, settingsBgCloseButtonPortrait, false);

        FadeOutSettingsPanel(settingsPanel);
        FadeOutSettingsPanel(settingsPanelPortrait);
    }

    private void FadeOutSettingsPanel(GameObject panel)
    {
        if (!panel) return;
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();
        cg.DOKill();
        cg.DOFade(0f, 0.35f).OnComplete(() => panel.SetActive(false));
    }

    private void CloseSettingsPanelImmediate()
    {
        SetButtonActive(settingsOpenButton, settingsOpenButtonPortrait, true);
        SetButtonActive(settingsCloseButton, settingsCloseButtonPortrait, false);
        SetButtonActive(settingsBgCloseButton, settingsBgCloseButtonPortrait, false);

        SnapSettingsPanelClosed(settingsPanel);
        SnapSettingsPanelClosed(settingsPanelPortrait);
    }

    private void SnapSettingsPanelClosed(GameObject panel)
    {
        if (!panel) return;
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.DOKill();
            cg.alpha = 0f;
        }
        panel.SetActive(false);
    }

    private void CloseAutoPlayPanelImmediate()
    {
        if (autoPlayPanelRect) autoPlayPanelRect.localScale = Vector3.one;
        if (autoPlayPanelRectPortrait) autoPlayPanelRectPortrait.localScale = Vector3.one;
        SetGameObjectActive(autoPlayPanel, autoPlayPanelPortrait, false);
    }

    #endregion

    #region Game Rules Panel

    private void OpenGameRulesPanel()
    {
        if (IsActivePair(settingsPanel, settingsPanelPortrait)) CloseSettingsPanelImmediate();
        ShowGameRulesPanel();
    }

    private void ShowGameRulesPanel()
    {
        if (gameRulesPanel == null) return;
        audioController.PlayUIButton(false);
        gameRulesPanel.SetActive(true);
    }

    private void CloseGameRulesPanel()
    {
        if (gameRulesPanel == null) return;
        gameRulesPanel.SetActive(false);
    }

    private void UpdateGameRulesDynamicTexts()
    {
        if (socketManager.initUIData == null || socketManager.initUIData.paylines == null) return;

        if (totalLineCountText != null)
        {
            totalLineCountText.text = socketManager.initialData.lines.Count.ToString();
        }

        TMP_Text[] symbolTexts =
        {
            ruleSymbol0Text, ruleSymbol1Text, ruleSymbol2Text, ruleSymbol3Text,
            ruleSymbol4Text, ruleSymbol5Text, ruleSymbol6Text, ruleSymbol7Text,
            ruleSymbol8Text, ruleSymbol9Text
        };

        var symbols = socketManager.initUIData.paylines.symbols;
        if (symbols == null) return;

        for (int i = 0; i < symbolTexts.Length; i++)
        {
            if (symbolTexts[i] == null) continue;
            var symbol = symbols.Find(s => s.id == i);
            if (symbol == null || symbol.multiplier == null || symbol.multiplier.Count == 0) continue;

            string fullText = "";
            int matchCount = 5; // backend multiplier array is ordered starting from a 5-symbol match
            for (int m = 0; m < symbol.multiplier.Count; m++)
            {
                double win = symbol.multiplier[m];
                if (win == 0) break;

                string line = $"{matchCount} * {win:0.##}";
                fullText = (fullText == "") ? line : fullText + $"\n{line}";
                matchCount--;
            }

            symbolTexts[i].text = fullText;
        }
    }

    #endregion

    #region Guide Panel

    private void OpenGuidePanel()
    {
        if (IsActivePair(settingsPanel, settingsPanelPortrait)) CloseSettingsPanelImmediate();
        ShowGuidePanel();
    }

    private void ShowGuidePanel()
    {
        if (guidePanel == null) return;
        audioController.PlayUIButton(false);
        guidePanel.SetActive(true);
    }

    private void CloseGuidePanel()
    {
        if (guidePanel == null) return;
        guidePanel.SetActive(false);
    }

    #endregion

    #region Shared Start Button (bonus wheel + free-spin trigger — same physical button)

    internal void ShowStartButton(bool interactable)
    {
        SetButtonActive(WheelStartButton, WheelStartButtonPortrait, true);
        SetButtonInteractable(WheelStartButton, WheelStartButtonPortrait, interactable);
    }

    internal void HideStartButton()
    {
        SetButtonActive(WheelStartButton, WheelStartButtonPortrait, false);
    }

    internal void SetStartButtonInteractable(bool interactable)
    {
        SetButtonInteractable(WheelStartButton, WheelStartButtonPortrait, interactable);
    }

    // Dispatches the click to whichever flow currently owns the button — the two flows
    // never overlap, so exactly one of these is meaningful at any given time.
    private void OnSharedStartButtonClicked()
    {
        audioController.PlayUIButton(false);

        if (bonusManager != null && bonusManager.IsBonusRoundActive)
        {
            bonusManager.RequestRoll();
        }
        else
        {
            OnFreeSpinStartButtonClicked();
        }
    }

    #endregion

    #region Free Spins Flow

    private int _freeSpinsAwardedPending;

    internal void OnFreeSpinsTriggered(int spinsAwarded)
    {
        audioController.PlayFreeSpinsWon();
        _freeSpinsAwardedPending = spinsAwarded;



        if (gameLogoObject) gameLogoObject.SetActive(false);
        UpdateFreeSpinCount(spinsAwarded);
        ShowStartButton(true);
    }

    private void OnFreeSpinStartButtonClicked()
    {
        HideStartButton();
        StartFreeSpinsSequence(_freeSpinsAwardedPending);
    }

    private void StartFreeSpinsSequence(int spinsAwarded)
    {
        slotManager.isInFreeSpins = true;
        int totalSpins = spinsAwarded;

        // Keep SlotManager's own counter in sync so the first free spin's TweenRoutine (which does
        // UpdateFreeSpinCount(freeSpinsRemaining--)) doesn't stomp this correct value with stale data.
        slotManager.freeSpinsRemaining = totalSpins;

        // Hidden for the whole free-spin round — cancelling autoplay mid-bonus isn't offered;
        // SetSpinButtonReady() (called once free spins fully end) restores it if autoplay is still on.
        SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, false);

        //if (gameLogoObject) gameLogoObject.SetActive(false);
        //UpdateFreeSpinCount(totalSpins);
        UpdateWinDisplay(0);

        // Kick off the first free spin — pressing Start is what actually begins the bonus
        // round, same as OnSpinButtonPressed does for a normal manual spin.
        slotManager.StartSlots();
        ShowReelSpinningButton(true);
    }

    // Shown, disabled, in the gap between every chained free spin (manual or autoplay) —
    // a visual placeholder only, the next free spin starts itself automatically.
    internal void ShowFreeSpinBetweenSpinsPlaceholder()
    {
        if (slotManager != null && slotManager.IsTurboOn)
        {
            // Turbo/quick spin: that gap is too short to be worth swapping the Start button
            // in and out for — it just reads as a flicker. Keep the same disabled spinButton
            // used during a turbo spin itself, so nothing visibly changes across the gap.
            ShowReelSpinningButton(false);
            return;
        }

        SetButtonActive(stopButton, stopButtonPortrait, false);
        SetButtonActive(spinButton, spinButtonPortrait, false);
        ShowStartButton(false);
    }

    internal void HideFreeSpinBetweenSpinsPlaceholder()
    {
        // Idempotent no-op unless the placeholder was actually showing.
        HideStartButton();
        // Each new free spin's stop button must be clickable — nothing else resets this
        // during free-spin chaining (SetSpinButtonReady() isn't called between free spins).
        ShowReelSpinningButton(true);
    }

    internal void OnFreeSpinsEnded(double serverTotalRoundWin)
    {
        slotManager.isInFreeSpins = false;
        audioController.PlayFreeSpinsWinAmount();
        StartCoroutine(EndFreeSpinsTransitionSequence());
    }

    private IEnumerator EndFreeSpinsTransitionSequence()
    {
        if (freeSpinCountContainer) freeSpinCountContainer.SetActive(false);
        if (freeSpinCountContainerPortrait) freeSpinCountContainerPortrait.SetActive(false);
        if (gameLogoObject) gameLogoObject.SetActive(true);

        UpdateWinDisplay(0);
        SetSpinButtonReady();
        SetBetControlsEnabled(true);
        SetButtonInteractable(settingsOpenButton, settingsOpenButtonPortrait, true);

        yield return null;
    }

    internal void UpdateFreeSpinCount(int remainingSpins)
    {
        //if (totalSpins > 0) totalFreeSpinsAwarded = totalSpins;

        if (freeSpinCountContainer) freeSpinCountContainer.SetActive(true);
        if (freeSpinCountContainerPortrait) freeSpinCountContainerPortrait.SetActive(true);
        // remainingSpins = totalFreeSpinsAwarded - playedSpins;
        // if (remainingFreeSpinsText) remainingFreeSpinsText.text = $"{remainingSpins}";
        // if(remainingFreeSpinsTextPortrait) remainingFreeSpinsTextPortrait.text = $"{remainingSpins}";
        if(remainingFreeSpinsText) remainingFreeSpinsText.text = ToSpriteString(remainingSpins);
        if(remainingFreeSpinsTextPortrait) remainingFreeSpinsTextPortrait.text = ToSpriteString(remainingSpins);
    }

    #endregion

    #region Expand / Shrink

    private void InitializeExpandShrink()
    {
        SetExpandShrinkButtons(isExpanded: false);
    }

    private void OnExpand()
    {
        isExpanded = true;
        jsFunctCalls?.RequestExpandGame();
        SetExpandShrinkButtons(isExpanded: true);
    }

    private void OnShrink()
    {
        isExpanded = false;
        jsFunctCalls?.RequestShrinkGame();
        SetExpandShrinkButtons(isExpanded: false);
    }

    private void SetExpandShrinkButtons(bool isExpanded)
    {
        SetButtonActive(expandButton, expandButtonPortrait, !isExpanded);
        SetButtonActive(shrinkButton, shrinkButtonPortrait, isExpanded);
    }

    #endregion

    #region Popup Animations (Generic)

    private void AnimatePopupOpen(RectTransform popupRect)
    {
        if (!popupRect) return;
        popupRect.localScale = Vector3.zero;
        popupRect.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    private void AnimatePopupClose(RectTransform popupRect, System.Action onComplete)
    {
        if (!popupRect)
        {
            onComplete?.Invoke();
            return;
        }

        audioController.PlayUIButton(false);

        Sequence closeSeq = DOTween.Sequence();
        closeSeq.Append(popupRect.DOScale(1.1f, 0.1f));
        closeSeq.Append(popupRect.DOScale(0f, 0.2f).SetEase(Ease.InBack));
        closeSeq.OnComplete(() =>
        {
            popupRect.localScale = Vector3.one;
            onComplete?.Invoke();
        });
    }

    #endregion

    #region Display Updates

    internal void UpdatePingDisplay(int pingMs)
    {
        SetTMPText(pingText, pingTextPortrait, $"{pingMs} ms");
    }

    internal void UpdatePingDisplay(string content)
    {
        SetTMPText(pingText, pingTextPortrait, content);
    }

    internal void UpdateJackpotDisplay(Values values)
    {
        if (values == null) return;

        SetTMPText(grandJackpotText, grandJackpotTextPortrait, FormatJackpotValue(values.grandJackpot));
        SetTMPText(majorJackpotText, majorJackpotTextPortrait, FormatJackpotValue(values.majorJackpot));
        SetTMPText(minorJackpotText, minorJackpotTextPortrait, FormatJackpotValue(values.minorJackpot));
        SetTMPText(miniJackpotText, miniJackpotTextPortrait, FormatJackpotValue(values.miniJackpot));
    }

    private string FormatJackpotValue(string val)
    {
        if (string.IsNullOrEmpty(val)) return "$0.00";
        return val.StartsWith("$") ? val : "$" + val;
    }

    #endregion

    #region Jackpot Portrait Levitation Animation

    private void UpdateJackpotPortraitLevitationFromCurrentOrientation()
    {
        var oc = GetOrientationChange();
        if (oc != null)
        {
            UpdateJackpotPortraitLevitation(oc.CurrentMode);
        }
    }

    private void UpdateJackpotPortraitLevitation(OrientationChange.OrientationMode mode)
    {
        bool isPortrait = (mode == OrientationChange.OrientationMode.MobilePortrait || mode == OrientationChange.OrientationMode.DesktopPortrait);
        if (isPortrait)
        {
            StartJackpotPortraitLevitation();
        }
        else
        {
            StopJackpotPortraitLevitation();
        }
    }

    private List<Transform> GetJackpotPortraitTransforms()
    {
        List<Transform> list = new List<Transform>();

        Transform grandTr = grandJackpotPortraitParent != null ? grandJackpotPortraitParent : (grandJackpotTextPortrait != null ? grandJackpotTextPortrait.transform.parent : null);
        Transform majorTr = majorJackpotPortraitParent != null ? majorJackpotPortraitParent : (majorJackpotTextPortrait != null ? majorJackpotTextPortrait.transform.parent : null);
        Transform minorTr = minorJackpotPortraitParent != null ? minorJackpotPortraitParent : (minorJackpotTextPortrait != null ? minorJackpotTextPortrait.transform.parent : null);
        Transform miniTr = miniJackpotPortraitParent != null ? miniJackpotPortraitParent : (miniJackpotTextPortrait != null ? miniJackpotTextPortrait.transform.parent : null);

        if (grandTr != null) list.Add(grandTr);
        if (majorTr != null) list.Add(majorTr);
        if (minorTr != null) list.Add(minorTr);
        if (miniTr != null) list.Add(miniTr);

        return list;
    }

    private void StartJackpotPortraitLevitation()
    {
        if (!enableJackpotPortraitLevitation) return;

        StopJackpotPortraitLevitation();

        List<Transform> portraitJackpots = GetJackpotPortraitTransforms();
        if (portraitJackpots.Count == 0) return;

        for (int i = 0; i < portraitJackpots.Count; i++)
        {
            Transform tr = portraitJackpots[i];
            if (tr == null) continue;

            if (!jackpotInitialLocalPositions.ContainsKey(tr))
            {
                jackpotInitialLocalPositions[tr] = tr.localPosition;
            }

            Vector3 startPos = jackpotInitialLocalPositions[tr];
            tr.localPosition = startPos;

            float targetY = startPos.y + jackpotLevitateHeight;
            float delay = i * jackpotStaggerDelay;

            Tween posTween = tr.DOLocalMoveY(targetY, jackpotLevitateDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(delay);

            jackpotPortraitTweens.Add(posTween);
        }
    }

    private void StopJackpotPortraitLevitation()
    {
        for (int i = 0; i < jackpotPortraitTweens.Count; i++)
        {
            if (jackpotPortraitTweens[i] != null && jackpotPortraitTweens[i].IsActive())
            {
                jackpotPortraitTweens[i].Kill();
            }
        }
        jackpotPortraitTweens.Clear();

        foreach (var kvp in jackpotInitialLocalPositions)
        {
            if (kvp.Key != null)
            {
                DOTween.Kill(kvp.Key);
                kvp.Key.localPosition = kvp.Value;
            }
        }
    }

    private OrientationChange GetOrientationChange()
    {
        if (orientationChange != null) return orientationChange;
        orientationChange = Object.FindFirstObjectByType<OrientationChange>();
        return orientationChange;
    }

    #endregion

    #region Display Updates

    internal void UpdateBalance(double balance, bool doAnimate = false)
    {
        if (balanceTween != null) balanceTween.Kill();

        if (doAnimate)
        {
            double displayAmount = currentBalance;
            balanceTween = DOTween.To(() => displayAmount, val =>
            {
                displayAmount = val;
                SetTMPText(balanceText, balanceTextPortrait, val.ToString("F2"));
            }, balance, 1.5f);
        }
        else
        {
            SetTMPText(balanceText, balanceTextPortrait, balance.ToString("F2"));
        }
    }

    internal void UpdateBalanceDisplay(double newBalance)
    {
        currentBalance = newBalance;
        UpdateBalance(newBalance);
        if (currentBalance < currentTotalBet) LowBalPopup();
    }

    internal void UpdateWin(double winAmount, bool doAnimate = false)
    {
        UpdateWinDisplay(winAmount, doAnimate);
    }

    private void UpdateWinDisplay(double winAmount, bool doAnimate = false)
    {
        if (winTween != null) winTween.Kill();

        if (doAnimate)
        {
            double displayAmount = _currentWinDisplayAmount;
            winTween = DOTween.To(() => displayAmount, val =>
            {
                displayAmount = val;
                _currentWinDisplayAmount = val;
                SetTMPText(winAmountText, winAmountTextPortrait, val.ToString("F2"));
            }, winAmount, 1.5f);
        }
        else
        {
            _currentWinDisplayAmount = winAmount;
            SetTMPText(winAmountText, winAmountTextPortrait, winAmount.ToString("F2"));
        }

        bool showWinText = winAmount > 0 || (slotManager != null && slotManager.isInFreeSpins);
        SetGameObjectActive(goodLuckObject, goodLuckObjectPortrait, !showWinText);
        SetGameObjectActive(winTextObject, winTextObjectPortrait, showWinText);
    }

    internal void UpdateFreeSpinTotalWin(double total)
    {
        if (freeSpinTotalWinTween != null) freeSpinTotalWinTween.Kill();

        // double displayAmount = 0f;
        // freeSpinTotalWinTween = DOTween.To(() => displayAmount, val =>
        // {
        //     displayAmount = val;
        //     SetTMPText(FreeSpinTotalWinText, FreeSpinTotalWinTextPortrait, FormatAmount(val));
        // }, total, 0.5f);

        SetTMPText(FreeSpinTotalWinText, FreeSpinTotalWinTextPortrait, FormatAmount(total));
    }

    #endregion

    #region Helper Methods

    private string FormatAmount(double amount)
    {
        return amount.ToString("0.###");
    }

    #endregion

    #region Popups

    private void CallOnExitFunction()
    {
        isExit = true;
        audioController.PlayUIButton(false);
        socketManager.CloseGame();
    }

    private void OpenPopup(GameObject popup, GameObject popupPortrait = null)
    {
        audioController.PlayUIButton(false);
        SetGameObjectActive(popup, popupPortrait, true);
        if (popup != universalWinPopup || popupPortrait != universalWinPopupPortrait)
        {
            SetGameObjectActive(mainPopupObject, mainPopupObjectPortrait, true);
        }
    }

    private void ClosePopup(GameObject popup, GameObject popupPortrait = null)
    {
        audioController.PlayUIButton(false);
        if (!IsActivePair(disconnectPopup, disconnectPopupPortrait))
        {
            if ((popup == quitPopup && IsActivePair(quitPopup, quitPopupPortrait))
                || (popup == lowBalancePopup && IsActivePair(lowBalancePopup, lowBalancePopupPortrait))
                || (popup == settingsPanel && IsActivePair(settingsPanel, settingsPanelPortrait))
                || (popup == autoPlayPanel && IsActivePair(autoPlayPanel, autoPlayPanelPortrait))
                || (popup == universalWinPopup && IsActivePair(universalWinPopup, universalWinPopupPortrait)))
            {
                SetGameObjectActive(mainPopupObject, mainPopupObjectPortrait, false);
            }
        }
        SetGameObjectActive(popup, popupPortrait, false);
    }

    internal void CheckAndClosePopups()
    {
        if (IsActivePair(reconnectPopup, reconnectPopupPortrait)) ClosePopup(reconnectPopup, reconnectPopupPortrait);
        if (IsActivePair(disconnectPopup, disconnectPopupPortrait)) ClosePopup(disconnectPopup, disconnectPopupPortrait);
    }

    internal void LowBalPopup()
    {
        OpenPopup(lowBalancePopup, lowBalancePopupPortrait);
    }

    internal void DisconnectionPopup()
    {
        if (!isExit)
        {
            isExit = true;
            OpenPopup(disconnectPopup, disconnectPopupPortrait);
        }
    }

    internal void ReconnectionPopup()
    {
        OpenPopup(reconnectPopup, reconnectPopupPortrait);
    }

    internal void ADfunction()
    {
        OpenPopup(adPopup, adPopupPortrait);
    }

    #endregion

    #region Universal Win Popup

    internal enum WinPopupType
    {
        BigWin,
        HugeWin,
        MegaWin
    }

    // Single spin's win vs. current total bet, strictly greater-than each tier's multiplier.
    // Returns null when no tier qualifies (or there's no valid bet to divide by).
    internal WinPopupType? GetWinPopupType(double winAmount)
    {
        if (currentTotalBet <= 0) return null;

        double multiplier = winAmount / currentTotalBet;
        if (multiplier > megaWinMultiplier) return WinPopupType.MegaWin;
        if (multiplier > hugeWinMultiplier) return WinPopupType.HugeWin;
        if (multiplier > bigWinMultiplier) return WinPopupType.BigWin;
        return null;
    }

    // Fades the popup in, pulses the side stars/win title out of phase, counts the win amount up
    // (via the sprite-asset TMP text), and only closes on Take — plus, during autoplay, 3s after
    // Take becomes interactable. onClosed fires exactly once, whenever the popup actually closes.
    internal void ShowUniversalWinPopup(WinPopupType type, double winAmount, bool autoCloseAfterTake, System.Action onClosed = null)
    {
        if (universalWinPopup == null && universalWinPopupPortrait == null)
        {
            onClosed?.Invoke();
            return;
        }

        universalWinPopupCallback = onClosed;
        KillUwpTweens();

        Sprite titleSprite = GetWinTitleSprite(type);
        if (winTitleImage) winTitleImage.sprite = titleSprite;
        if (winTitleImagePortrait) winTitleImagePortrait.sprite = titleSprite;

        SetButtonActive(uwpTakeButton, uwpTakeButtonPortrait, true);
        SetButtonInteractable(uwpTakeButton, uwpTakeButtonPortrait, false);

        OpenPopup(universalWinPopup, universalWinPopupPortrait);

        FadeInUwp(universalWinPopup);
        FadeInUwp(universalWinPopupPortrait);

        StartUwpPulse(sideStars, winTitleImageGameObject);
        StartUwpPulse(sideStarsPortrait, winTitleImageGameObjectPortrait);

        AnimateUwpWinCount(winAmount, autoCloseAfterTake);
    }

    private Sprite GetWinTitleSprite(WinPopupType type)
    {
        switch (type)
        {
            case WinPopupType.BigWin: return bigWinTitleSprite;
            case WinPopupType.HugeWin: return hugeWinTitleSprite;
            case WinPopupType.MegaWin: return megaWinTitleSprite;
            default: return null;
        }
    }

    private void FadeInUwp(GameObject popup)
    {
        CanvasGroup cg = GetOrAddCanvasGroup(popup);
        if (cg == null) return;

        cg.DOKill();
        cg.alpha = 0f;
        uwpTweens.Add(cg.DOFade(1f, uwpFadeDuration));
    }

    // sideStars loops 1 -> uwpPulseScale -> 1 while winTitle loops the opposite phase
    // (uwpPulseScale -> 1 -> uwpPulseScale) at the same duration/start time, so they're
    // always scaled oppositely to each other.
    private void StartUwpPulse(GameObject star, GameObject winTitle)
    {
        if (star)
        {
            star.transform.DOKill();
            star.transform.localScale = Vector3.one;
            uwpTweens.Add(star.transform.DOScale(uwpPulseScale, uwpPulseDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
        }

        if (winTitle)
        {
            winTitle.transform.DOKill();
            winTitle.transform.localScale = Vector3.one * uwpPulseScale;
            uwpTweens.Add(winTitle.transform.DOScale(1f, uwpPulseDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
        }
    }

    private void AnimateUwpWinCount(double winAmount, bool autoCloseAfterTake)
    {
        int decimals = GetDecimalPlaces(winAmount);
        string formatStr = decimals > 0 ? "0." + new string('0', decimals) : "F2";

        SetUwpWinAmountText(0.0, formatStr);

        Tween countTween = DOVirtual.Float(0f, (float)winAmount, uwpWinCountDuration, val => SetUwpWinAmountText(val, formatStr))
            .OnComplete(() =>
            {
                SetUwpWinAmountText(winAmount, formatStr);
                SetButtonInteractable(uwpTakeButton, uwpTakeButtonPortrait, true);

                if (autoCloseAfterTake)
                {
                    if (uwpAutoCloseCoroutine != null) StopCoroutine(uwpAutoCloseCoroutine);
                    uwpAutoCloseCoroutine = StartCoroutine(AutoCloseUniversalWinPopupAfterDelay());
                }
            });
        uwpTweens.Add(countTween);
    }

    private void SetUwpWinAmountText(double value, string format)
    {
        string spriteText = ToSpriteString(value, format);
        if (uwpWinAmountText) uwpWinAmountText.text = spriteText;
        if (uwpWinAmountTextPortrait) uwpWinAmountTextPortrait.text = spriteText;
    }

    private int GetDecimalPlaces(double amount)
    {
        double rounded = System.Math.Round(amount, 4);
        string str = rounded.ToString(System.Globalization.CultureInfo.InvariantCulture);
        int dotIndex = str.IndexOf('.');
        if (dotIndex < 0) return 0;
        return str.Length - dotIndex - 1;
    }

    private IEnumerator AutoCloseUniversalWinPopupAfterDelay()
    {
        yield return new WaitForSeconds(uwpAutoSpinAutoCloseDelay);
        uwpAutoCloseCoroutine = null;
        CloseUniversalWinPopup();
    }

    private void OnUniversalWinTakeButtonClicked()
    {
        audioController.PlayUIButton(false);
        CloseUniversalWinPopup();
    }

    private void CloseUniversalWinPopup()
    {
        KillUwpTweens();

        if (uwpAutoCloseCoroutine != null)
        {
            StopCoroutine(uwpAutoCloseCoroutine);
            uwpAutoCloseCoroutine = null;
        }

        SetButtonActive(uwpTakeButton, uwpTakeButtonPortrait, false);

        var callback = universalWinPopupCallback;
        universalWinPopupCallback = null;

        ClosePopup(universalWinPopup, universalWinPopupPortrait);
        callback?.Invoke();
    }

    private void KillUwpTweens()
    {
        for (int i = 0; i < uwpTweens.Count; i++)
        {
            if (uwpTweens[i] != null && uwpTweens[i].IsActive()) uwpTweens[i].Kill();
        }
        uwpTweens.Clear();

        if (sideStars) sideStars.transform.DOKill();
        if (winTitleImageGameObject) winTitleImageGameObject.transform.DOKill();
        if (sideStarsPortrait) sideStarsPortrait.transform.DOKill();
        if (winTitleImageGameObjectPortrait) winTitleImageGameObjectPortrait.transform.DOKill();
    }

    private CanvasGroup GetOrAddCanvasGroup(GameObject go)
    {
        if (!go) return null;
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();
        return cg;
    }

    #endregion

    #region Symbol Info Card

    internal void ShowSymbolInfoCard(int symbolId, int col, int row, RectTransform symbolRect)
    {
        if (symbolInfoCard == null) return;
        symbolInfoCard.ShowCard(symbolId, col, row, symbolRect, socketManager, betCounter);
    }

    internal void HideSymbolInfoCard()
    {
        if (symbolInfoCard == null) return;
        symbolInfoCard.HideCard();
    }

    #endregion

    #region Bonus Background Toggle

    // internal void ToggleBonusBackground(bool isBonus)
    // {
    //     if (bgImage == null) return;
    //     bgImage.sprite = isBonus ? redSprite : blueSprite;
    // }

    #endregion

    #region Background Shine Animation

    internal void StartBGAnimation()
    {
        // if (bgAnimationRoutine != null) StopCoroutine(bgAnimationRoutine);
        // bgAnimationRoutine = StartCoroutine(BGAnimationRoutine());
        bgImage.StartAnimation();
    }

    internal void StopBGAnimation()
    {
        bgImage.StopAnimation();
    }

    #endregion

    #region Game Init / Bonus / Buttons Helper Surface

    // Kept as thin wrapper methods with these exact names/signatures — BonusManager.cs and
    // SlotManager.cs (both out of scope this pass) call directly into this surface.

    internal void InitialiseUIData(Root root)
    {
        currentBalance = root.player.balance;
        currentTotalBet = root.gameData.bets[betCounter] * root.gameData.lines.Count;

        UpdateBalance(root.player.balance);
        UpdateWin(0.00);
        UpdateBetDisplay();

        //bonusManager.IntializeBonusWheelValue();
        //StartBGAnimation();
        //UpdateJackpotDisplay(root.jackpotData.values);
    }

    internal void SetBetButtonsInteractable(bool interactable)
    {
        SetBetControlsEnabled(interactable);
    }

    internal void SetSpinButtonInteractable(bool interactable)
    {
        SetButtonInteractable(spinButton, spinButtonPortrait, interactable);
    }

    internal void SetSpinButtonActive(bool active)
    {
        SetButtonActive(spinButton, spinButtonPortrait, active);
    }

    internal void SetStopSpinButtonActive(bool active)
    {
        SetButtonActive(stopButton, stopButtonPortrait, active);
    }

    internal void SetAutoSpinButtonInteractable(bool interactable)
    {
        SetButtonInteractable(autoSpinButton, autoSpinButtonPortrait, interactable);
    }

    #endregion

    #region Sprite Fonts Functions

    internal static string ToSpriteString(string value)
    {
        var sb = new StringBuilder();
        foreach (char c in value)
        {
            if (c >= '0' && c <= '9')
                sb.Append($"<sprite index={(c - '0')}>");
            else if (c == '.')
                sb.Append("<sprite index=10>");
            else if (c == ',')
                sb.Append("<sprite index=11>");
            else if (c == '+')
                sb.Append("<sprite index=10>");
        }
        return sb.ToString();
    }

    internal static string ToSpriteString(double value, string format = "F2")
        => ToSpriteString(value.ToString(format));

    internal static string ToSpriteString(int value)
        => ToSpriteString(value.ToString());

    internal static double FromSpriteString(string spriteText)
    {
        if (string.IsNullOrEmpty(spriteText)) return 0;
        var sb = new StringBuilder();
        int i = 0;
        while (i < spriteText.Length)
        {
            if (spriteText[i] == '<')
            {
                int end = spriteText.IndexOf('>', i);
                if (end < 0) break;
                string tag = spriteText.Substring(i, end - i + 1);
                const string prefix = "<sprite index=";
                if (tag.StartsWith(prefix))
                {
                    string numStr = tag.Substring(prefix.Length, tag.Length - prefix.Length - 1);
                    if (int.TryParse(numStr, out int idx))
                    {
                        if (idx >= 0 && idx <= 9) sb.Append((char)('0' + idx));
                        else if (idx == 10) sb.Append('.');
                        else if (idx == 11) sb.Append(',');
                        else if (idx == 12) sb.Append('+');
                    }
                }
                i = end + 1;
            }
            else
            {
                sb.Append(spriteText[i]);
                i++;
            }
        }
        string plain = sb.ToString().Replace(",", "");
        return double.TryParse(plain, System.Globalization.NumberStyles.Any,
                               System.Globalization.CultureInfo.InvariantCulture, out double result)
               ? result : 0;
    }

    #endregion

    #region Cleanup

    private void OnDestroy()
    {
        StopJackpotPortraitLevitation();
        if (balanceTween != null) balanceTween.Kill();
        if (winTween != null) winTween.Kill();
        DOTween.KillAll();
    }

    #endregion
}
