using UnityEngine;
using UnityEngine.InputSystem;

public enum SubMenu
{
    CONTROLLER,
    SOUND,
    GRAPHICS
}

public class MainMenu : MonoBehaviour
{
    [Header("Main Configuration")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private string firstElementTag = "InitialSelectableUIElement";

    [Header("Options Menu")]
    [SerializeField] private GameObject mainOptionsMenu;
    [SerializeField] private GameObject controllerMenu;
    [SerializeField] private GameObject soundMenu;
    [SerializeField] private GameObject graphicsMenu;

    void Start()
    {
        OpenMainMenuPanel();
        OpenSubMenu(null);

    }
    public void OpenMainMenuPanel()
    {
        GameManager.Instance.PlayUICancelSound();
        GameManager.Instance.SetFirstSelectedElement(
            GameObject.FindGameObjectWithTag(firstElementTag)
        );
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    void OpenSubMenu(SubMenu? subMenu)
    {
        controllerMenu.SetActive(SubMenu.CONTROLLER == subMenu);
        soundMenu.SetActive(SubMenu.SOUND == subMenu);
        graphicsMenu.SetActive(SubMenu.GRAPHICS == subMenu);
    }

    public void OpenControllerMenu()
    {
        GameManager.Instance.PlayUIConfirmSound();
        controllerMenu.SetActive(true);
        soundMenu.SetActive(false);
        graphicsMenu.SetActive(false);
    }
    public void OpenSoundMenu()
    {
        GameManager.Instance.PlayUIConfirmSound();
        controllerMenu.SetActive(false);
        soundMenu.SetActive(true);
        graphicsMenu.SetActive(false);
    }
    public void OpenGraphicsMenu()
    {
        GameManager.Instance.PlayUIConfirmSound();
        controllerMenu.SetActive(false);
        soundMenu.SetActive(false);
        graphicsMenu.SetActive(true);
    }

    public void OpenOptionsPanel()
    {
        GameManager.Instance.PlayUIConfirmSound();
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);

        mainOptionsMenu.SetActive(true);

        OpenSubMenu(SubMenu.CONTROLLER);
    }

    public void OpenCreditsPanel()
    {
        GameManager.Instance.PlayUIConfirmSound();
        GameManager.Instance.LoadCredits();
    }

    public void QuitGame()
    {
        GameManager.Instance.PlayUIConfirmSound();
        GameManager.Instance.QuitGame();
    }

    public void PlayGame()
    {
        GameManager.Instance.PlayUIConfirmSound();
        GameManager.Instance.StartGame();
    }
}
