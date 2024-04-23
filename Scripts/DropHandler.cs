using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropHandler : MonoBehaviour, IDropHandler
{
    private Image thisTargetImage;
    public bool isHomeBase;
    private DropHandler thisTargetDrop;
    public bool isOccupied = false;

    // Only assigned if this is a Threat target
    public TextMeshProUGUI  threatText;
    public Image tickImage;
    public GameObject warningText;

    // Variable to access necessary game flow functions
    [SerializeField] private GameFlowCore gameplay;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped on " + thisTargetDrop.name);

        if (!isOccupied) {
            isOccupied = true;

            // Assign the new position based on the target object
            eventData.pointerDrag.GetComponent<DragHandler>()
                .SetNewPosition(thisTargetDrop);

            // If not a defences placeholder (i.e., a threat placeholder), begin match validation
            if (!isHomeBase) {
                Debug.Log("Attempted to fight threat " + threatText.text + " with defence " + 
                    eventData.pointerDrag.GetComponentInChildren<TextMeshProUGUI>().text + ".");
                if(gameplay.CheckAnswer(threatText.text, eventData.pointerDrag.GetComponentInChildren<TextMeshProUGUI>().text))
                {
                    SetTickMark(true);
                    SetWarningMessage(false);
                    eventData.pointerDrag.GetComponent<DragHandler>().activeDraggable = false;
                } else
                {
                    SetTickMark(false);
                    SetWarningMessage(true);
                }
            }
        }
    }

    public void SetTickMark(bool activeBool)
    {
        if(tickImage != null)
            tickImage.gameObject.SetActive(activeBool);
        return;
    }

    public void SetWarningMessage(bool activeBool)
    {
        if(warningText != null)
            warningText.SetActive(activeBool);
        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialise objects
        thisTargetImage = GetComponent<Image>();
        thisTargetDrop = GetComponent<DropHandler>();
        gameplay = GameObject.Find("Canvas").GetComponent<GameFlowCore>();
    }
}
