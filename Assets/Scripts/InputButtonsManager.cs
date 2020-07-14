using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputButtonsManager : Singleton<InputButtonsManager>
{
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;
    private PointerEventData pointerEventData;

    public InputType GetInput()
    {
        InputType inputType = InputType.NONE;

        if (Input.GetMouseButtonDown(0))
        {
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            raycaster.Raycast(pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                InputButton inputButton = result.gameObject.GetComponent<InputButton>();
                if (inputButton != null)
                {
                    inputType = inputButton.InputType;
                }
            }
        }

        return inputType;
    }
}
