using UnityEngine;
using UnityEngine.UI;

public class VictoryCanvas : BaseCanvas
{
    [SerializeField] private Button menuBtn,exitBtn;

    private void Start()
    {
        menuBtn.onClick.AddListener(MenuHandle);
        exitBtn.onClick.AddListener(ExitHandle);
    }

    private void MenuHandle()
    {
        DataManager.Inst.DataReset();
        FadeInFadeOutManager.Inst.FadeOut("Title");
    }

    private void ExitHandle()
    {
        DataManager.Inst.DataReset();
        Debug.Log("GameCloseHandle");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }
}
