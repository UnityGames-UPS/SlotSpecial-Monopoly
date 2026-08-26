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
    [SerializeField] private GameObject gameLogoObject;

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
    [SerializeField] private RectTransform universalWinPopupRect;
    [SerializeField] private ImageAnimation uwpBg;
    [SerializeField] private GameObject sideDiamonds;
    [SerializeField] private GameObject cornerDiamonds;

    [SerializeField] private GameObject uwpCongratulationsTitle;
    [SerializeField] private GameObject uwpYouWonSubtitle;
    [SerializeField] private GameObject uwpBigWinTitle;
    [SerializeField] private TMP_Text uwpWinAmountText;

    [SerializeField] private ImageAnimation uwpSparkleAnimation;
    [SerializeField] private GameObject MainFreeSpinObject;
    [SerializeField] private GameObject uwpFreeSpinTitle;
    [SerializeField] private TMP_Text uwpFreeSpinCountText;
    [SerializeField] private GameObject uwpFreeSpinTextObject;

    [SerializeField] private GameObject uwpBonusTriggerObject;
    [SerializeField] private TMP_Text uwpBonusWinText;
    [SerializeField] private TMP_Text uwpBonusWheelMultiplierText;
    [SerializeField] private GameObject uwpBonusSubtitle;
    [SerializeField] private Button WheelStartButton;
    [SerializeField] private Button WheelStartButtonPortrait;

    [SerializeField] private Button uwpTakeButton;
    [SerializeField] private float uwpAutoCloseDelay = 5f;
    [SerializeField] private float uwpFreeSpinGroupScaleDuration = 0.4f;
    [SerializeField] private float uwpFreeSpinPopLegDuration = 0.15f;
    [SerializeField] private float uwpFreeSpinCompleteTitleScaleDuration = 1f;
    [SerializeField] private float uwpCongratsPulseDuration = 0.5f;
    [Header("Universal Win Popup - Portrait")]
    [SerializeField] private GameObject universalWinPopupPortrait;
    [SerializeField] private RectTransform universalWinPopupRectPortrait;
    [SerializeField] private ImageAnimation uwpBgPortrait;
    [SerializeField] private GameObject sideDiamondsPortrait;
    [SerializeField] private GameObject cornerDiamondsPortrait;
    [SerializeField] private GameObject uwpCongratulationsTitlePortrait;
    [SerializeField] private GameObject uwpYouWonSubtitlePortrait;
    [SerializeField] private GameObject uwpBigWinTitlePortrait;
    [SerializeField] private TMP_Text uwpWinAmountTextPortrait;
    [SerializeField] private ImageAnimation uwpSparkleAnimationPortrait;
    [SerializeField] private GameObject MainFreeSpinObjectPortrait;
    [SerializeField] private GameObject uwpFreeSpinTitlePortrait;
    [SerializeField] private TMP_Text uwpFreeSpinCountTextPortrait;
    [SerializeField] private GameObject uwpFreeSpinObjectPortrait;
    [SerializeField] private GameObject uwpBonusTriggerObjectPortrait;
    [SerializeField] private TMP_Text uwpBonusWinTextPortrait;
    [SerializeField] private TMP_Text uwpBonusWheelMultiplierTextPortrait;
    [SerializeField] private GameObject uwpBonusSubtitlePortrait;
    [SerializeField] private Button uwpTakeButtonPortrait;

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

    [Header("Heat Up Text Groups")]
    [SerializeField] private TMP_Text[] twoSymbolHeatUpTexts;
    [SerializeField] private TMP_Text[] threeSymbolHeatUpTexts;
    [SerializeField] private TMP_Text[] fourSymbolHeatUpTexts;
    [SerializeField] private TMP_Text[] fiveSymbolHeatUpTexts;
    [SerializeField] private TMP_Text[] sixSymbolHeatUpTexts;
    [SerializeField] private TMP_Text[] sevenSymbolHeatUpTexts;

    // Cached lookup array to eliminate runtime branching
    private TMP_Text[][] heatUpTextGroups;

    [Header("Free Spin Count Display - Game Screen")]
    [SerializeField] private GameObject freeSpinCountContainer;
    [SerializeField] private TMP_Text remainingFreeSpinsText;
    [SerializeField] private TMP_Text FreeSpinTotalWinText;
    [SerializeField] private GameObject freeSpinCountContainerPortrait;
    [SerializeField] private TMP_Text remainingFreeSpinsTextPortrait;
    [SerializeField] private TMP_Text FreeSpinTotalWinTextPortrait;
    [Header("Free Spin Transition")]
    [SerializeField] private CanvasGroup transitionBackFilm;

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
    private Tween uwpWinTween;
    private Sequence uwpFreeSpinIntroSeqLandscape;
    private Sequence uwpFreeSpinIntroSeqPortrait;
    private Tween freeSpinTotalWinTween;
    private Sequence uwpFreeSpinCompleteIntroSeqLandscape;
    private Sequence uwpFreeSpinCompleteIntroSeqPortrait;
    private Sequence uwpBonusTriggerIntroSeqLandscape;
    private Sequence uwpBonusTriggerIntroSeqPortrait;
    private Sequence uwpBonusCompleteIntroSeqLandscape;
    private Sequence uwpBonusCompleteIntroSeqPortrait;

    private void Awake()
    {
        if (jsFunctCalls != null)
        {
            jsFunctCalls.RegisterVisibilityListener(gameObject.name);
            jsFunctCalls.RegisterFullscreenListener(gameObject.name);
        }
        heatUpTextGroups = new TMP_Text[][]
        {
            twoSymbolHeatUpTexts,    // index 0 -> corresponds to heatUpValues[1]
            threeSymbolHeatUpTexts,  // index 1 -> corresponds to heatUpValues[2]
            fourSymbolHeatUpTexts,   // index 2 -> corresponds to heatUpValues[3]
            fiveSymbolHeatUpTexts,   // index 3 -> corresponds to heatUpValues[4]
            sixSymbolHeatUpTexts,    // index 4 -> corresponds to heatUpValues[5]
            sevenSymbolHeatUpTexts   // index 5 -> corresponds to heatUpValues[6]
        };
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

        //ShowUniversalWinPopup(WinPopupType.FreeSpinComplete, 1.879f, 0, null, 200);
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

        if (uwpWinTween != null) { uwpWinTween.Kill(); uwpWinTween = null; }
        SetGameObjectActive(universalWinPopup, universalWinPopupPortrait, false);

        if (freeSpinCountContainer) freeSpinCountContainer.SetActive(false);
        if (freeSpinCountContainerPortrait) freeSpinCountContainerPortrait.SetActive(false);
        if (transitionBackFilm) transitionBackFilm.gameObject.SetActive(false);

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
        if (!bonusManager.isBonusFinished)
        {
            ToggleAutoSpin();
        }
        else
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
            int matchCount = symbol.multiplier.Count;
            for (int m = symbol.multiplier.Count - 1; m >= 0; m--)
            {
                double win = symbol.multiplier[m];
                if (win == 0) break;

                string line = $"{matchCount} {win:0.##}";
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

    #region HeatUp Text Setup

    internal void UpdateHeatUpText(List<double> heatUpValues)
    {
        if (heatUpValues == null) return;

        // Iterate through groups directly, starting from value index 1
        int count = Mathf.Min(heatUpTextGroups.Length, heatUpValues.Count - 1);

        for (int i = 0; i < count; i++)
        {
            string formattedValue = heatUpValues[i + 1].ToString(); // Format as needed
            var targetTexts = heatUpTextGroups[i];

            if (targetTexts == null) continue;

            for (int j = 0; j < targetTexts.Length; j++)
            {
                if (targetTexts[j] != null)
                    targetTexts[j].text = formattedValue;
            }
        }
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

        if (bonusManager != null && !bonusManager.isBonusFinished)
        {
            bonusManager.RequestWheelStart();
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
        // Shown (disabled) right alongside the trigger popup rather than popping in after —
        // popup auto-closes itself once its intro animation finishes (no Take button), and this
        // callback then enables the already-visible start button.
        ShowStartButton(false);
        ShowUniversalWinPopup(WinPopupType.FreeSpinTrigger, 0, spinsAwarded, () => SetStartButtonInteractable(true));
    }

    private void OnFreeSpinStartButtonClicked()
    {
        HideStartButton();
        StartFreeSpinsSequence(_freeSpinsAwardedPending);
    }

    private void StartFreeSpinsSequence(int spinsAwarded)
    {
        slotManager.isInFreeSpins = true;
        int totalSpins = socketManager.resultData.payload.freeSpinsRemaining;

        // Keep SlotManager's own counter in sync so the first free spin's TweenRoutine (which does
        // UpdateFreeSpinCount(freeSpinsRemaining--)) doesn't stomp this correct value with stale data.
        slotManager.freeSpinsRemaining = totalSpins;

        // Hidden for the whole free-spin round — cancelling autoplay mid-bonus isn't offered;
        // SetSpinButtonReady() (called once free spins fully end) restores it if autoplay is still on.
        SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, false);

        if (gameLogoObject) gameLogoObject.SetActive(false);
        UpdateFreeSpinCount(totalSpins);
        UpdateWinDisplay(0);
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
        ShowUniversalWinPopup(WinPopupType.FreeSpinComplete, serverTotalRoundWin, 0, () => StartCoroutine(EndFreeSpinsTransitionSequence()));
    }

    private IEnumerator EndFreeSpinsTransitionSequence()
    {
        // Take was just pressed on the FreeSpinComplete popup — autospin is now clear to proceed.
        slotManager.isFreeSpinCompletePopupPending = false;

        if (transitionBackFilm != null)
        {
            transitionBackFilm.gameObject.SetActive(true);
            transitionBackFilm.alpha = 0f;
            yield return transitionBackFilm.DOFade(1f, 0.5f).WaitForCompletion();
            yield return new WaitForSeconds(0.2f);
        }

        if (freeSpinCountContainer) freeSpinCountContainer.SetActive(false);
        if (freeSpinCountContainerPortrait) freeSpinCountContainerPortrait.SetActive(false);
        if (gameLogoObject) gameLogoObject.SetActive(true);

        UpdateWinDisplay(0);
        SetSpinButtonReady();
        SetBetControlsEnabled(true);
        SetButtonInteractable(settingsOpenButton, settingsOpenButtonPortrait, true);

        if (transitionBackFilm != null)
        {
            yield return transitionBackFilm.DOFade(0f, 0.5f).WaitForCompletion();
            transitionBackFilm.gameObject.SetActive(false);
        }
    }

    internal void UpdateFreeSpinCount(int remainingSpins)
    {
        //if (totalSpins > 0) totalFreeSpinsAwarded = totalSpins;

        if (freeSpinCountContainer) freeSpinCountContainer.SetActive(true);
        if (freeSpinCountContainerPortrait) freeSpinCountContainerPortrait.SetActive(true);
        // remainingSpins = totalFreeSpinsAwarded - playedSpins;
        if (remainingFreeSpinsText) remainingFreeSpinsText.text = $"{remainingSpins}";
        if(remainingFreeSpinsTextPortrait) remainingFreeSpinsTextPortrait.text = $"{remainingSpins}";
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
        FreeSpinTrigger,
        FreeSpinComplete,
        BonusTrigger,
        BonusComplete
    }

    internal void ShowUniversalWinPopup(WinPopupType type, double winAmount = 0, int freeSpinCount = 0, System.Action onTakePressed = null, double bonusMultiplier = 0)
    {
        if (universalWinPopup == null && universalWinPopupPortrait == null) return;

        universalWinPopupCallback = onTakePressed;

        if (uwpWinTween != null)
        {
            uwpWinTween.Kill();
            uwpWinTween = null;
        }

        SetGameObjectActive(uwpCongratulationsTitle, uwpCongratulationsTitlePortrait, false);
        SetGameObjectActive(uwpYouWonSubtitle, uwpYouWonSubtitlePortrait, false);
        SetGameObjectActive(uwpBigWinTitle, uwpBigWinTitlePortrait, false);
        SetGameObjectActive(uwpFreeSpinTitle, uwpFreeSpinTitlePortrait, false);
        if (uwpSparkleAnimation) uwpSparkleAnimation.gameObject.SetActive(false);
        if (uwpSparkleAnimationPortrait) uwpSparkleAnimationPortrait.gameObject.SetActive(false);
        if (uwpWinAmountText) uwpWinAmountText.gameObject.SetActive(false);
        if (uwpWinAmountTextPortrait) uwpWinAmountTextPortrait.gameObject.SetActive(false);
        if (uwpFreeSpinCountText) uwpFreeSpinCountText.gameObject.SetActive(false);
        if (uwpFreeSpinCountTextPortrait) uwpFreeSpinCountTextPortrait.gameObject.SetActive(false);
        SetGameObjectActive(uwpFreeSpinTextObject, uwpFreeSpinObjectPortrait, false);
        SetGameObjectActive(uwpBonusTriggerObject, uwpBonusTriggerObjectPortrait, false);
        SetGameObjectActive(uwpBonusSubtitle, uwpBonusSubtitlePortrait, false);
        if (uwpBonusWheelMultiplierText) uwpBonusWheelMultiplierText.gameObject.SetActive(false);
        if (uwpBonusWheelMultiplierTextPortrait) uwpBonusWheelMultiplierTextPortrait.gameObject.SetActive(false);
        if (uwpBonusWinText) uwpBonusWinText.gameObject.SetActive(false);
        if (uwpBonusWinTextPortrait) uwpBonusWinTextPortrait.gameObject.SetActive(false);

        switch (type)
        {
            case WinPopupType.FreeSpinTrigger:
                //SetGameObjectActive(uwpCongratulationsTitle, uwpCongratulationsTitlePortrait, true);
                //SetGameObjectActive(uwpYouWonSubtitle, uwpYouWonSubtitlePortrait, true);
                SetTMPText(uwpFreeSpinCountText, uwpFreeSpinCountTextPortrait, freeSpinCount.ToString());
                if (uwpFreeSpinCountText) uwpFreeSpinCountText.gameObject.SetActive(true);
                if (uwpFreeSpinCountTextPortrait) uwpFreeSpinCountTextPortrait.gameObject.SetActive(true);
                SetGameObjectActive(uwpFreeSpinTitle, uwpFreeSpinTitlePortrait, true);
                SetGameObjectActive(uwpFreeSpinTextObject, uwpFreeSpinObjectPortrait, true);
                break;

            case WinPopupType.FreeSpinComplete:
                SetGameObjectActive(uwpCongratulationsTitle, uwpCongratulationsTitlePortrait, true);
                SetGameObjectActive(uwpYouWonSubtitle, uwpYouWonSubtitlePortrait, true);
                if (uwpWinAmountText) { uwpWinAmountText.gameObject.SetActive(true); uwpWinAmountText.text = FormatAmount(winAmount); }
                if (uwpWinAmountTextPortrait) { uwpWinAmountTextPortrait.gameObject.SetActive(true); uwpWinAmountTextPortrait.text = FormatAmount(winAmount); }
                break;

            case WinPopupType.BigWin:
                SetGameObjectActive(uwpBigWinTitle, uwpBigWinTitlePortrait, true);
                if (uwpWinAmountText) { uwpWinAmountText.gameObject.SetActive(true); uwpWinAmountText.text = FormatAmount(winAmount); }
                if (uwpWinAmountTextPortrait) { uwpWinAmountTextPortrait.gameObject.SetActive(true); uwpWinAmountTextPortrait.text = FormatAmount(winAmount); }
                break;

            case WinPopupType.BonusComplete:
                SetGameObjectActive(uwpCongratulationsTitle, uwpCongratulationsTitlePortrait, true);
                SetGameObjectActive(uwpBonusSubtitle, uwpBonusSubtitlePortrait, true);
                if (uwpBonusWheelMultiplierText) { uwpBonusWheelMultiplierText.gameObject.SetActive(true); uwpBonusWheelMultiplierText.text = "x" + bonusMultiplier.ToString("0.##"); }
                if (uwpBonusWheelMultiplierTextPortrait) { uwpBonusWheelMultiplierTextPortrait.gameObject.SetActive(true); uwpBonusWheelMultiplierTextPortrait.text = "x" + bonusMultiplier.ToString("0.##"); }
                if (uwpBonusWinText) uwpBonusWinText.gameObject.SetActive(true);
                if (uwpBonusWinTextPortrait) uwpBonusWinTextPortrait.gameObject.SetActive(true);
                break;
        }

        bool showTakeButton = type != WinPopupType.BigWin && type != WinPopupType.FreeSpinTrigger;
        SetButtonActive(uwpTakeButton, uwpTakeButtonPortrait, showTakeButton);

        OpenPopup(universalWinPopup, universalWinPopupPortrait);

        if (uwpAutoCloseCoroutine != null)
        {
            StopCoroutine(uwpAutoCloseCoroutine);
            uwpAutoCloseCoroutine = null;
        }

        if (type == WinPopupType.FreeSpinTrigger)
        {
            // No Take button for this popup — it auto-closes itself once the intro animation finishes.
            if (universalWinPopupRect) universalWinPopupRect.localScale = Vector3.one;
            if (universalWinPopupRectPortrait) universalWinPopupRectPortrait.localScale = Vector3.one * 0.6f;

            // SlotManager already resets its round total to 0 for a genuine fresh trigger, and
            // preserves it across a retrigger — mirror whichever it currently holds, don't force 0.
            string currentFreeSpinTotal = FormatAmount(slotManager != null ? slotManager.FreeSpinsRoundWinTotal : 0);
            if (FreeSpinTotalWinText) FreeSpinTotalWinText.text = currentFreeSpinTotal;
            if (FreeSpinTotalWinTextPortrait) FreeSpinTotalWinTextPortrait.text = currentFreeSpinTotal;

            uwpFreeSpinIntroSeqLandscape = AnimateFreeSpinTriggerIntro(universalWinPopup, uwpSparkleAnimation, uwpFreeSpinCountText, uwpFreeSpinTextObject, uwpFreeSpinTitle, MainFreeSpinObject, uwpTakeButton);
            uwpFreeSpinIntroSeqPortrait = AnimateFreeSpinTriggerIntro(universalWinPopupPortrait, uwpSparkleAnimationPortrait, uwpFreeSpinCountTextPortrait, uwpFreeSpinObjectPortrait, uwpFreeSpinTitlePortrait, MainFreeSpinObjectPortrait, uwpTakeButtonPortrait);
        }
        else if (type == WinPopupType.FreeSpinComplete)
        {
            SetButtonInteractable(uwpTakeButton, uwpTakeButtonPortrait, false);
            if (uwpTakeButton) uwpTakeButton.interactable = false;
            if (uwpTakeButtonPortrait) uwpTakeButtonPortrait.interactable = false;

            if (universalWinPopupRect) universalWinPopupRect.localScale = Vector3.one;
            if (universalWinPopupRectPortrait) universalWinPopupRectPortrait.localScale = Vector3.one* 0.6f;

            uwpFreeSpinCompleteIntroSeqLandscape = AnimateFreeSpinCompleteIntro(universalWinPopup, uwpCongratulationsTitle, uwpYouWonSubtitle, uwpWinAmountText, winAmount, false, uwpTakeButton, uwpBg, sideDiamonds, cornerDiamonds);
            uwpFreeSpinCompleteIntroSeqPortrait = AnimateFreeSpinCompleteIntro(universalWinPopupPortrait, uwpCongratulationsTitlePortrait, uwpYouWonSubtitlePortrait, uwpWinAmountTextPortrait, winAmount, false, uwpTakeButtonPortrait, uwpBgPortrait, sideDiamondsPortrait, cornerDiamondsPortrait);
        }
        else if (type == WinPopupType.BonusTrigger)
        {
            SetButtonInteractable(uwpTakeButton, uwpTakeButtonPortrait, false);

            if (universalWinPopupRect) universalWinPopupRect.localScale = Vector3.one;
            if (universalWinPopupRectPortrait) universalWinPopupRectPortrait.localScale = Vector3.one* 0.6f;

            SetGameObjectActive(uwpBonusTriggerObject, uwpBonusTriggerObjectPortrait, true);

            uwpBonusTriggerIntroSeqLandscape = AnimateBonusTriggerIntro(universalWinPopup, uwpTakeButton);
            uwpBonusTriggerIntroSeqPortrait = AnimateBonusTriggerIntro(universalWinPopupPortrait, uwpTakeButtonPortrait);
        }
        else if (type == WinPopupType.BonusComplete)
        {
            SetButtonInteractable(uwpTakeButton, uwpTakeButtonPortrait, false);
            if (uwpTakeButton) uwpTakeButton.interactable = false;
            if (uwpTakeButtonPortrait) uwpTakeButtonPortrait.interactable = false;

            if (universalWinPopupRect) universalWinPopupRect.localScale = Vector3.one;
            if (universalWinPopupRectPortrait) universalWinPopupRectPortrait.localScale = Vector3.one* 0.6f;

            uwpBonusCompleteIntroSeqLandscape = AnimateBonusCompleteIntro(universalWinPopup, uwpCongratulationsTitle, uwpBonusSubtitle, uwpBonusWheelMultiplierText ? uwpBonusWheelMultiplierText.gameObject : null, uwpBonusWinText, winAmount, uwpTakeButton, uwpBg, sideDiamonds, cornerDiamonds);
            uwpBonusCompleteIntroSeqPortrait = AnimateBonusCompleteIntro(universalWinPopupPortrait, uwpCongratulationsTitlePortrait, uwpBonusSubtitlePortrait, uwpBonusWheelMultiplierTextPortrait ? uwpBonusWheelMultiplierTextPortrait.gameObject : null, uwpBonusWinTextPortrait, winAmount, uwpTakeButtonPortrait, uwpBgPortrait, sideDiamondsPortrait, cornerDiamondsPortrait);
        }
        else
        {
            SetButtonInteractable(uwpTakeButton, uwpTakeButtonPortrait, showTakeButton);

            AnimateUwpOpen(universalWinPopupRect);
            AnimateUwpOpen(universalWinPopupRectPortrait);

            if (winAmount > 0)
            {
                AnimateUwpWinCount(uwpWinAmountText, winAmount, type == WinPopupType.BigWin);
                AnimateUwpWinCount(uwpWinAmountTextPortrait, winAmount, type == WinPopupType.BigWin);
            }

            uwpAutoCloseCoroutine = StartCoroutine(AutoCloseUniversalWinPopup());
        }
    }

    private void AnimateUwpOpen(RectTransform rect)
    {
        if (!rect) return;
        rect.localScale = Vector3.zero;
        Sequence openSeq = DOTween.Sequence();
        openSeq.Append(rect.DOScale(1.2f, 0.5f).SetEase(Ease.OutCubic));
        openSeq.Append(rect.DOScale(1f, 0.3f).SetEase(Ease.InOutSine));
    }

    private Sequence AnimateFreeSpinTriggerIntro(GameObject popup, ImageAnimation sparkle, TMP_Text countText, GameObject freeSpinTextObj, GameObject FreeSpinTitle, GameObject mainFreeSpinObj, Button takeButton)
    {
        if (!popup) return null;

        CanvasGroup cg = popup.GetComponent<CanvasGroup>();
        if (cg == null) cg = popup.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        RectTransform countTextRect = countText ? countText.rectTransform : null;
        if (countTextRect)
        {
            countTextRect.localScale = Vector3.one * 0;
            countTextRect.anchoredPosition = new Vector2(0f, countTextRect.anchoredPosition.y);
        }
        if (freeSpinTextObj) freeSpinTextObj.transform.localScale = Vector3.zero;
        if (mainFreeSpinObj) mainFreeSpinObj.transform.localScale = Vector3.one;
        if (FreeSpinTitle) FreeSpinTitle.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(cg.DOFade(1f, 0.5f));
        seq.AppendCallback(() =>
        {
            if (sparkle) sparkle.gameObject.SetActive(true);
            if (sparkle) sparkle.StartAnimation();
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            if (countTextRect)
            {
                countTextRect.DOScale(1f, 1f).SetEase(Ease.OutBack);
                if (FreeSpinTitle) FreeSpinTitle.transform.DOScale(1f, 1f).SetEase(Ease.OutBack);
            }
        });
        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            if (countTextRect) countTextRect.DOAnchorPosX(-270f, uwpFreeSpinGroupScaleDuration).SetEase(Ease.OutCubic);
            if (freeSpinTextObj) freeSpinTextObj.transform.DOScale(1f, uwpFreeSpinGroupScaleDuration).SetEase(Ease.OutBack);
        });
        //seq.Append(uwpFreeSpinTitle.transform.DOScale(1f, 1f).SetEase(Ease.OutBack));
        seq.AppendInterval(uwpFreeSpinGroupScaleDuration);
        if (mainFreeSpinObj)
        {
            seq.Append(mainFreeSpinObj.transform.DOScale(1.15f, uwpFreeSpinPopLegDuration).SetEase(Ease.OutQuad));
            seq.Append(mainFreeSpinObj.transform.DOScale(1f, uwpFreeSpinPopLegDuration).SetEase(Ease.InQuad));
        }
        seq.OnComplete(() =>
        {
            // No Take button on this popup — close it as soon as the intro finishes.
            // CloseUniversalWinPopup() is safe to call twice (once from each orientation's
            // sequence): its stored callback is nulled out after the first invocation.
            CloseUniversalWinPopup();
        });

        return seq;
    }

    private Sequence AnimateFreeSpinCompleteIntro(GameObject popup, GameObject congratsTitle, GameObject subtitle, TMP_Text winAmountText, double winAmount, bool isBigWin, Button takeButton, ImageAnimation bg, GameObject sideDiamondsObj, GameObject cornerDiamondsObj)
    {
        if (!popup) return null;

        CanvasGroup cg = popup.GetComponent<CanvasGroup>();
        if (cg == null) cg = popup.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        if (congratsTitle) congratsTitle.transform.localScale = Vector3.zero;

        CanvasGroup subtitleCg = null;
        if (subtitle)
        {
            subtitleCg = subtitle.GetComponent<CanvasGroup>();
            if (subtitleCg == null) subtitleCg = subtitle.AddComponent<CanvasGroup>();
            subtitleCg.alpha = 0f;
        }

        CanvasGroup winAmountCg = null;
        if (winAmountText)
        {
            winAmountCg = winAmountText.GetComponent<CanvasGroup>();
            if (winAmountCg == null) winAmountCg = winAmountText.gameObject.AddComponent<CanvasGroup>();
            winAmountCg.alpha = 0f;
        }

        float halfDuration = uwpFreeSpinCompleteTitleScaleDuration * 0.5f;

        Sequence seq = DOTween.Sequence();
        seq.Append(cg.DOFade(1f, 0.5f));
        seq.AppendCallback(() =>
        {
            StartPopupBgPulse(bg, sideDiamondsObj, cornerDiamondsObj);
            if (congratsTitle) StartCongratsTitlePulse(congratsTitle);
        });
        seq.AppendInterval(halfDuration);
        seq.AppendCallback(() =>
        {
            if (subtitleCg) subtitleCg.DOFade(1f, halfDuration);
            if (winAmountCg) winAmountCg.DOFade(1f, halfDuration);
            if (winAmountText) AnimateUwpWinCount(winAmountText, winAmount, isBigWin);
        });
        seq.AppendInterval(halfDuration);
        seq.OnComplete(() =>
        {
            if (takeButton) takeButton.interactable = true;
        });

        return seq;
    }

    // Scales the congratulations title in, then pulses it (1 -> 1.2 -> 1, forever) until CloseUniversalWinPopup kills it.
    private void StartCongratsTitlePulse(GameObject congratsTitle)
    {
        congratsTitle.transform.DOScale(1f, uwpFreeSpinCompleteTitleScaleDuration).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                congratsTitle.transform.DOScale(1.2f, uwpCongratsPulseDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            });
    }

    private Sequence AnimateBonusTriggerIntro(GameObject popup, Button takeButton)
    {
        if (!popup) return null;

        CanvasGroup cg = popup.GetComponent<CanvasGroup>();
        if (cg == null) cg = popup.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        seq.Append(cg.DOFade(1f, 0.5f));
        seq.OnComplete(() =>
        {
            if (takeButton) takeButton.interactable = true;
            if (uwpAutoCloseCoroutine == null) uwpAutoCloseCoroutine = StartCoroutine(AutoCloseUniversalWinPopup());
        });

        return seq;
    }

    private Sequence AnimateBonusCompleteIntro(GameObject popup, GameObject congratsTitle, GameObject subtitle, GameObject multiplierText, TMP_Text winText, double bonusWinAmount, Button takeButton, ImageAnimation bg, GameObject sideDiamondsObj, GameObject cornerDiamondsObj)
    {
        if (!popup) return null;

        CanvasGroup cg = popup.GetComponent<CanvasGroup>();
        if (cg == null) cg = popup.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        if (congratsTitle) congratsTitle.transform.localScale = Vector3.zero;

        CanvasGroup subtitleCg = null;
        if (subtitle)
        {
            subtitleCg = subtitle.GetComponent<CanvasGroup>();
            if (subtitleCg == null) subtitleCg = subtitle.AddComponent<CanvasGroup>();
            subtitleCg.alpha = 0f;
        }

        CanvasGroup multiplierCg = null;
        if (multiplierText)
        {
            multiplierCg = multiplierText.GetComponent<CanvasGroup>();
            if (multiplierCg == null) multiplierCg = multiplierText.AddComponent<CanvasGroup>();
            multiplierCg.alpha = 0f;
        }

        CanvasGroup winCg = null;
        if (winText)
        {
            winCg = winText.GetComponent<CanvasGroup>();
            if (winCg == null) winCg = winText.gameObject.AddComponent<CanvasGroup>();
            winCg.alpha = 0f;
        }

        float halfDuration = uwpFreeSpinCompleteTitleScaleDuration * 0.5f;

        Sequence seq = DOTween.Sequence();
        seq.Append(cg.DOFade(1f, 0.5f));
        seq.AppendCallback(() =>
        {
            StartPopupBgPulse(bg, sideDiamondsObj, cornerDiamondsObj);
            if (congratsTitle) StartCongratsTitlePulse(congratsTitle);
        });
        seq.AppendInterval(halfDuration);
        seq.AppendCallback(() =>
        {
            if (subtitleCg) subtitleCg.DOFade(1f, halfDuration);
            if (multiplierCg) multiplierCg.DOFade(1f, halfDuration);
            if (winCg) winCg.DOFade(1f, halfDuration);
            if (winText) AnimateUwpWinCount(winText, bonusWinAmount, false);
        });
        seq.AppendInterval(halfDuration);
        seq.OnComplete(() =>
        {
            if (takeButton) takeButton.interactable = true;
        });

        return seq;
    }

    private void StartPopupBgPulse(ImageAnimation bg, GameObject sideDiamondsObj, GameObject cornerDiamondsObj)
    {
        if (bg) bg.StartAnimation();

        if (sideDiamondsObj)
        {
            sideDiamondsObj.transform.DOKill();
            sideDiamondsObj.transform.localScale = Vector3.one;
            sideDiamondsObj.transform.DOScale(0.97f, 0.7f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
        if (cornerDiamondsObj)
        {
            cornerDiamondsObj.transform.DOKill();
            cornerDiamondsObj.transform.localScale = Vector3.one * 0.97f;
            cornerDiamondsObj.transform.DOScale(1f, 0.7f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void StopPopupBgPulse()
    {
        if (uwpBg) uwpBg.StopAnimation();
        if (uwpBgPortrait) uwpBgPortrait.StopAnimation();

        if (sideDiamonds) sideDiamonds.transform.DOKill();
        if (cornerDiamonds) cornerDiamonds.transform.DOKill();
        if (sideDiamondsPortrait) sideDiamondsPortrait.transform.DOKill();
        if (cornerDiamondsPortrait) cornerDiamondsPortrait.transform.DOKill();
    }

    private void AnimateUwpWinCount(TMP_Text text, double winAmount, bool isBigWin)
    {
        if (text == null || !text.gameObject.activeSelf) return;

        int decimals = GetDecimalPlaces(winAmount);
        string formatStr = decimals > 0 ? "0." + new string('0', decimals) : "0";
        text.text = (0.0).ToString(formatStr);

        float countUpDuration = isBigWin ? 1.5f : 1.0f;
        uwpWinTween = DOVirtual.Float(0f, (float)winAmount, countUpDuration, val => text.text = val.ToString(formatStr))
            .OnComplete(() => text.text = FormatAmount(winAmount));
    }

    private int GetDecimalPlaces(double amount)
    {
        double rounded = System.Math.Round(amount, 4);
        string str = rounded.ToString(System.Globalization.CultureInfo.InvariantCulture);
        int dotIndex = str.IndexOf('.');
        if (dotIndex < 0) return 0;
        return str.Length - dotIndex - 1;
    }

    private IEnumerator AutoCloseUniversalWinPopup()
    {
        yield return new WaitForSeconds(uwpAutoCloseDelay);
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
        if (uwpWinTween != null)
        {
            uwpWinTween.Kill();
            uwpWinTween = null;
        }

        if (uwpFreeSpinIntroSeqLandscape != null)
        {
            uwpFreeSpinIntroSeqLandscape.Kill();
            uwpFreeSpinIntroSeqLandscape = null;
        }
        if (uwpFreeSpinIntroSeqPortrait != null)
        {
            uwpFreeSpinIntroSeqPortrait.Kill();
            uwpFreeSpinIntroSeqPortrait = null;
        }
        if (uwpSparkleAnimation) uwpSparkleAnimation.StopAnimation();
        if (uwpSparkleAnimationPortrait) uwpSparkleAnimationPortrait.StopAnimation();

        // The congrats-title pulse is a standalone tween started from inside the intro sequences'
        // OnComplete, so killing those sequences (below) doesn't stop it — kill it explicitly here.
        if (uwpCongratulationsTitle) uwpCongratulationsTitle.transform.DOKill();
        if (uwpCongratulationsTitlePortrait) uwpCongratulationsTitlePortrait.transform.DOKill();

        if (uwpFreeSpinCompleteIntroSeqLandscape != null)
        {
            uwpFreeSpinCompleteIntroSeqLandscape.Kill();
            uwpFreeSpinCompleteIntroSeqLandscape = null;
        }
        if (uwpFreeSpinCompleteIntroSeqPortrait != null)
        {
            uwpFreeSpinCompleteIntroSeqPortrait.Kill();
            uwpFreeSpinCompleteIntroSeqPortrait = null;
        }
        StopPopupBgPulse();

        if (uwpBonusTriggerIntroSeqLandscape != null)
        {
            uwpBonusTriggerIntroSeqLandscape.Kill();
            uwpBonusTriggerIntroSeqLandscape = null;
        }
        if (uwpBonusTriggerIntroSeqPortrait != null)
        {
            uwpBonusTriggerIntroSeqPortrait.Kill();
            uwpBonusTriggerIntroSeqPortrait = null;
        }

        if (uwpBonusCompleteIntroSeqLandscape != null)
        {
            uwpBonusCompleteIntroSeqLandscape.Kill();
            uwpBonusCompleteIntroSeqLandscape = null;
        }
        if (uwpBonusCompleteIntroSeqPortrait != null)
        {
            uwpBonusCompleteIntroSeqPortrait.Kill();
            uwpBonusCompleteIntroSeqPortrait = null;
        }

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

        bonusManager.IntializeBonusWheelValue();
        //StartBGAnimation();
        UpdateHeatUpText(root.uiData.paylines.symbols[9].multiplier);
        UpdateJackpotDisplay(root.jackpotData.values);
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
                sb.Append("<sprite index=12>");
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
        if (balanceTween != null) balanceTween.Kill();
        if (winTween != null) winTween.Kill();
        DOTween.KillAll();
    }

    #endregion
}
