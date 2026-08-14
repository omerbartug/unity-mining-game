using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image background;

    public void SetProgress(float progress)
    {
        background.enabled = true;
        fillImage.fillAmount = progress;
    }
    public void ResetProgress(){
        fillImage.fillAmount = 0;
        background.enabled = false;
    }
}