using UnityEngine;
using TMPro;

public class VersionDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text versionText;

    void Awake()
    {
        if (versionText != null)
        {
            // Versión del proyecto (Player Settings > Version)
            versionText.text = $"v {Application.version}";
        }
    }

}
