using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HelpWindow : MonoBehaviour
{

    Dictionary<string, string> mapping = new();

    public GameObject helpWindow;
    public GameObject backgroundOverlay;
    public TMP_Dropdown dropdownList;
    public TextMeshProUGUI textField;

    // Start is called before the first frame update
    public void PostXMLHelpSetup() {
        mapping = GetComponent<XMLHandler>().LoadDefenceDescriptions();

        // Assign defences as dropdown options
        dropdownList.AddOptions(mapping.Keys.ToList<string>());
        UpdateText();
    }

    public void UpdateText() { 
        textField.SetText(mapping[dropdownList.captionText.text]); // Assign corresponding defence description
        Debug.Log("");
    }

    // Enable background overlay & display help window
    public void OpenWindow() {
        backgroundOverlay.transform.SetAsLastSibling();
        backgroundOverlay.SetActive(true);
        helpWindow.transform.SetAsLastSibling();
        helpWindow.SetActive(true);
    }

    public void CloseWindow() {
        helpWindow.SetActive(false);
        backgroundOverlay.SetActive(false);
    }
}
