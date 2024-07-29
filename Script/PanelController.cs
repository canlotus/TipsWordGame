using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject languageSelectionPanel; // Panel referansý
    public Button openPanelButton; // Paneli açan buton referansý
    public Button closePanelButton; // Paneli kapatan buton referansý

    private void Start()
    {
        // Paneli baþlangýçta kapalý yap
        languageSelectionPanel.SetActive(false);

        // Butonlara týklama dinleyicilerini ekle
        openPanelButton.onClick.AddListener(OpenPanel);
        closePanelButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        languageSelectionPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        languageSelectionPanel.SetActive(false);
    }
}