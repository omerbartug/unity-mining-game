using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image Background;

    public void SetProgress(float current, float max)
    {
        Background.enabled = true;
        fillImage.fillAmount = current / max;
    }
    public void ResetProgress(){
        fillImage.fillAmount = 0;
        Background.enabled = false;
    }
}