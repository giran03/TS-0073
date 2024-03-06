using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonInteract : MonoBehaviour, IInteractable
{
    [Header("Button")]
    [SerializeField] GameObject[] buttonActivatedObjects;   // 'EnabledObjects' and 'DisabledObjects' active state will be flipped
    [SerializeField] float offset;
    [SerializeField] float duration;
    [SerializeField] float buttonCooldownDuration;
    [SerializeField] ButtonDirection buttonDirection;
    bool isButtonOnCooldown;
    bool buttonPressed;
    Vector3 originalPosition;
    Vector3 offsetPosition;

    enum ButtonDirection
    {
        Top,
        Down,
        Right,
        Left
    }

    public void Interact()
    {
        if (isButtonOnCooldown) return;
        ButtonPress(true);
        StartCoroutine(MoveButton());
        StartCoroutine(ButtonCooldown());
    }

    private void Update()
    {
        ButtonPlacement();
    }

    void ButtonPlacement()
    {
        if (buttonDirection == ButtonDirection.Right)
            offsetPosition = originalPosition + new Vector3(0, 0, offset);
        else if (buttonDirection == ButtonDirection.Left)
            offsetPosition = originalPosition + new Vector3(0, 0, -offset);
        else if (buttonDirection == ButtonDirection.Top)
            offsetPosition = originalPosition + new Vector3(0, offset, 0);
        else if (buttonDirection == ButtonDirection.Down)
            offsetPosition = originalPosition + new Vector3(0, -offset, 0);
    }

    // flips the active state of the game objects in this array
    void ButtonPress(bool enable)
    {
        if (buttonPressed != enable)
        {
            foreach (GameObject obj in buttonActivatedObjects)
                obj.SetActive(!obj.activeSelf);
            buttonPressed = enable;
        }
        buttonPressed = !buttonPressed;
    }

    IEnumerator MoveButton()
    {
        originalPosition = transform.position;


        // Move to offset position
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(originalPosition, offsetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move back to original position
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(offsetPosition, originalPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }

    IEnumerator ButtonCooldown()
    {
        isButtonOnCooldown = true;
        yield return new WaitForSeconds(buttonCooldownDuration);
        isButtonOnCooldown = false;
    }
}
