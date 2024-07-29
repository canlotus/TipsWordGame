using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject languageSelectionPanel; // Panel referans�
    public Button openPanelButton; // Paneli a�an buton referans�
    public Button closePanelButton; // Paneli kapatan buton referans�

    private void Start()
    {
        // Paneli ba�lang��ta kapal� yap
        languageSelectionPanel.SetActive(false);

        // Butonlara t�klama dinleyicilerini ekle
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