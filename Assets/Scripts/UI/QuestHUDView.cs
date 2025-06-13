using TMPro;
using UnityEngine;

public class QuestHUDView : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private QuestHUDViewModel viewModel;
    [SerializeField] private TextMeshProUGUI taskLabel;
    void Start()
    {
        viewModel = new QuestHUDViewModel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
