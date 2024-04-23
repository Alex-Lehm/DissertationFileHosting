using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameFlowCore : MonoBehaviour
{
    public RectTransform canvas;

    private XMLHandler xmlHandler;
    private Dictionary<string, List<string>> attackDefencePairs = new();
    private List<GameObject> defenceObjects = new();
    private List<GameObject> threatObjects = new();
    private Random random = new();

    private List<Image> serverBorders = new();
    private Color borderRed = new(208, 0, 0);
    private Color borderGreen = new(0, 158, 0);

    private int correctPairs = 0;
    public Image backgroundOverlay;
    public GameObject beginGameWindow;
    public GameObject finishGameWindow;


    public void PostXMLLoadStart() {
        // Loading XML data
        xmlHandler = GetComponent<XMLHandler>();
        attackDefencePairs = xmlHandler.LoadAttackDefenceData();

        // Collecting threat objects
        foreach (GameObject threat in GameObject.FindGameObjectsWithTag("ThreatText"))
        {
            threatObjects.Add(threat);
        }

        // Collecting defence objects
        foreach (GameObject defence in GameObject.FindGameObjectsWithTag("DefenceText"))
        {
            defenceObjects.Add(defence);
        }

        // Collecting server icon borders
        foreach (GameObject border in GameObject.FindGameObjectsWithTag("Border"))
        {
            serverBorders.Add(border.GetComponent<Image>());
        }

        SetupGame();
    }

    public void SetupGame() {
        SetServerBorderColor(borderRed);
        correctPairs = 0;

        // Shuffle defence placeholders, Fisher-Yates Algorithm implementation
        for (int i = defenceObjects.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            GameObject defence = defenceObjects[j];
            defenceObjects[j] = defenceObjects[i];
            defenceObjects[i] = defence;
        }

        DistributeThreats();
    }


    public bool CheckAnswer(string threatText, string defenceText)
    {
        if(attackDefencePairs.ContainsKey(threatText))
        {
            // Debugging to verify outcomes in the console.
            Debug.Log("CheckAnswer Decision: " + (attackDefencePairs[threatText].Contains(defenceText)));
            if (attackDefencePairs[threatText].Contains(defenceText)) {
                AddCorrectPairing();
                return true;
            }

            return false;
        } else {
            Debug.LogWarning("Threat not defined in Dictionary.");
        }

        return false;
    }

    void DistributeThreats() {
        List<string> threats = new();
        int randInt = random.Next(attackDefencePairs.Keys.Count);

        // Selecting 3 random, unique threats
        for (int i = 0; i < threatObjects.Count; i++) {
            string threatSelection = attackDefencePairs.Keys.ElementAt(randInt);
            while (threats.Contains(threatSelection)) // uniqueness validation
            {
                randInt = random.Next(attackDefencePairs.Keys.Count);
                threatSelection = attackDefencePairs.Keys.ElementAt(randInt);
            }

            threats.Add(threatSelection);
            SetText(threatObjects[i].GetComponent<TextMeshProUGUI>(), threatSelection);

            DistributeDefences(threats);
        }
    }

    public void DistributeDefences(List<string> threatsSelected) {
        int threatsPairedCounter = 0;
        int defencePairInt;
        List<string> possibleDefences = new();

        foreach (GameObject defence in defenceObjects) { 
            if(threatsPairedCounter < threatsSelected.Count) {
                //Debug.Log("Assigning a mandatory threat defence");
                possibleDefences = attackDefencePairs[threatsSelected[threatsPairedCounter]];
                defencePairInt = random.Next(possibleDefences.Count);
                SetText(defence.GetComponentInChildren<TextMeshProUGUI>(), possibleDefences[defencePairInt]);
                threatsPairedCounter++;
                continue;
            }

            // Assigning 'filler' defences at random. Select random threat & select random viable defence
            defencePairInt = random.Next(attackDefencePairs.Keys.Count);
            possibleDefences = attackDefencePairs[attackDefencePairs.Keys.ElementAt(defencePairInt)];

            defencePairInt = random.Next(possibleDefences.Count);
            SetText(defence.GetComponentInChildren<TextMeshProUGUI>(), possibleDefences[defencePairInt]);
            Debug.Log("Defences Distributed");
        }
    }

    void SetText(TextMeshProUGUI textObject, string text) {
        textObject.text = text;
    }

    void SetServerBorderColor(Color color) {
        foreach (Image border in serverBorders) {
            border.color = color;
        }
    }

    // Update and keep track of correct matches
    // Once number of correct matches == number of threats in scene, display win screeen
    void AddCorrectPairing() {
        correctPairs++;

        if(correctPairs == threatObjects.Count) {
            SetServerBorderColor(borderGreen);
            DisplayWinScreen();
        }
    }

    void DisplayWinScreen() {
        backgroundOverlay.transform.SetAsLastSibling();
        backgroundOverlay.gameObject.SetActive(true);
        finishGameWindow.transform.SetAsLastSibling();
        finishGameWindow.SetActive(true);

        // Prevent any accidental dragging boxes
        foreach(GameObject defence in defenceObjects) {
            defence.GetComponent<DragHandler>().activeDraggable = false;
        }
    }

    public void PlayButton()
    {
        beginGameWindow.SetActive(false);
        backgroundOverlay.gameObject.SetActive(false);
    }

    public void RestartGame() {

        // Resetting correct answers
        foreach (GameObject threat in threatObjects) {
            threat.transform.parent.GetComponentInChildren<DropHandler>().SetTickMark(false);
        }

        // Return defence objects to original positions
        GameObject[] defencePlaceholders = GameObject.FindGameObjectsWithTag("DefencePlaceholder");
        for(int i = 0; i < defencePlaceholders.Length; i++) {
            DragHandler draggable = defenceObjects[i].GetComponent<DragHandler>();
            draggable.SetNewPosition(defencePlaceholders[i].GetComponent<DropHandler>());
            draggable.activeDraggable = true;

        }

        // Begin threat & defence distribution
        SetupGame(); 

        // Hide window
        finishGameWindow.SetActive(false);
        backgroundOverlay.gameObject.SetActive(false);
    } 
}
