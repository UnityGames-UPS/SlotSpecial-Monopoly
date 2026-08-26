using UnityEngine;
using UnityEngine.UI;

public class SymbolClickHandler : MonoBehaviour
{
    private int columnIndex;
    private int rowIndex;
    private SlotManager slotManager;
    private UIManager uiManager;
    private Button button;
    private Image image;

    public void Init(int col, int row, SlotManager manager, UIManager ui)
    {
        columnIndex = col;
        rowIndex = row;
        slotManager = manager;
        uiManager = ui;
        image = GetComponent<Image>();

        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
        }

        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (slotManager == null || uiManager == null || image == null) return;

        int symbolId = slotManager.GetSymbolIndex(image.sprite);
        uiManager.ShowSymbolInfoCard(symbolId, columnIndex, rowIndex, transform as RectTransform);
    }
}
