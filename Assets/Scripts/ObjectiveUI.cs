using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ObjectiveUI : MonoBehaviour
{
    public TextMeshProUGUI[] objectives;

    public void MarkObjectiveComplete(int index)
    {
        objectives[index].text = $"<s>{objectives[index].text}</s>";
    }
}