using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TeamMember
{
    [Tooltip("Example: Jedidiah Lopez - Game Designer")]
    public string memberNameAndRole;
    public Sprite portrait;
    [TextArea(5, 10)]
    public string contributions;
}

public class TeamCreditsManager : MonoBehaviour
{
    [Header("Team Data")]
    public TeamMember[] teamMembers;

    [Header("UI References (MemberPreview Panel)")]
    public Image displayPortrait;
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI displayContributions;

    void Start()
    {
        // Start the scene completely blank
        ClearPreview();
    }

    // Empties the text and hides the image
    private void ClearPreview()
    {
        displayName.text = "";
        displayContributions.text = "";

        if (displayPortrait != null)
        {
            displayPortrait.sprite = null;
            // Set the Alpha to 0 so we don't just see a blank white square
            displayPortrait.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    // The buttons in your TeamGrid will call this function!
    public void ShowMember(int index)
    {
        if (index < 0 || index >= teamMembers.Length) return;

        TeamMember selectedMember = teamMembers[index];

        // 1. Update the image and make sure the Alpha is back to 1 (fully visible)
        displayPortrait.sprite = selectedMember.portrait;
        displayPortrait.color = new Color(1f, 1f, 1f, 1f);

        // 2. Update the text
        displayName.text = selectedMember.memberNameAndRole;
        displayContributions.text = selectedMember.contributions;
    }
}