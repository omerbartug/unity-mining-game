using UnityEngine;
using UnityEngine.UI;

// Dünyadaki nesnelerin (işçi, bina, maden) ilerleme ve doluluk durumunu görselleştiren çubuktur.
public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image background;

    // İlerleme çubuğunun dünya tıklamalarını yutmasını önlemek için UI raycast algılayıcılarını devre dışı bırakır.
    private void Awake()
    {
        if (fillImage != null) fillImage.raycastTarget = false;
        if (background != null) background.raycastTarget = false;

        GraphicRaycaster[] raycasters = GetComponentsInChildren<GraphicRaycaster>(true);
        foreach (var raycaster in raycasters)
        {
            raycaster.enabled = false;
        }
    }

    // İlerleme oranını (0 ile 1 arası) ayarlar ve arka planı görünür kılar.
    public void SetProgress(float progress)
    {
        if (background != null) background.enabled = true;
        if (fillImage != null) fillImage.fillAmount = Mathf.Clamp01(progress);
    }

    // İlerlemeyi sıfırlar ve arka planı gizler.
    public void ResetProgress()
    {
        if (fillImage != null) fillImage.fillAmount = 0;
        if (background != null) background.enabled = false;
    }
}