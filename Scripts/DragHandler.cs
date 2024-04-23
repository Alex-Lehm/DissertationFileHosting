using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private RectTransform thisObject;
    public RectTransform parent;
    public Vector3 imageOldPos;
    public DropHandler currentBase;
    public GameObject defencesParent;

    public bool activeDraggable = true;

    void Start()
    {
        // initialise objects
        thisObject = GetComponent<RectTransform>();
        defencesParent = GameObject.FindGameObjectWithTag("DefenceParent");

        // set vals
        imageOldPos = thisObject.position;
        currentBase.isOccupied = true;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if(!activeDraggable) {
            eventData.pointerDrag = null;
            return;
        }

        // Clarifying whether currently placed on a Threat or Defence spot:
        Transform currentPlaceholder = gameObject.transform.parent; // Store the current parent transform
        if (currentPlaceholder.CompareTag("DefencePlaceholder")) {
            Debug.Log("Picked up a defence from the defences section.");
            // if a defence, also move the defences overall parent to below all threats
            defencesParent.transform.SetAsLastSibling(); 
        }

        // Finally move the overall placeholder parent object
        currentPlaceholder.gameObject.transform.parent.SetAsLastSibling();

        thisObject.GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        ResetPosition();
        thisObject.GetComponent<Image>().raycastTarget = true; // enable object interactions again
    }

    public void ResetPosition() {
        thisObject.localPosition = new Vector3(0, 0, 0); // centre the object in new parent
    }

    public void SetNewPosition(DropHandler dropTarget) {
        
        // if dropped on a new target, reset original placeholder to be a viable new drop target
        if (currentBase != dropTarget)
        {
            currentBase.isOccupied = false;
            currentBase.SetWarningMessage(false);
            currentBase = dropTarget;
        }

        thisObject.SetParent(dropTarget.transform);
        ResetPosition();
    }

}
