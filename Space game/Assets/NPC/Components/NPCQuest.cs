using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 


public class NPCQuest : MonoBehaviour
{
    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText; 
    private InteractionManager interactionManager; 
    

    private bool questAccepted = false;
    private bool questCompleted = false;  
    private bool dialogueVisible = false;
    private int dialogueOpenedFrame;    

    public void Interact(GameObject player)
    {  

        interactionManager = player.GetComponent<InteractionManager>();

        if (interactionManager != null)
        {
            interactionManager.SetDialogueOpen(true);
        }

        dialogueVisible = true;
        dialogueOpenedFrame = Time.frameCount; 

        if (questCompleted)
        {
            ShowDialogue("Thank you for helping me. Good luck escaping this planet!");
            return;
        }

        PlayerInventory inventory =
            player.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.Log("Player inventory not found!");
            return;
        }

        // First interaction: Give the player the quest.
        if (!questAccepted)
        {
            questAccepted = true;

            ShowDialogue(
                "Survivor: Please, you have to help me!\n\n" +
                "My energy cell is damaged. I need a replacement.\n\n" +
                "Find an energy cell nearby and bring it back to me."
            );
        }

        // Second interaction: Check whether the player has the item.
        else if (inventory.hasEnergyCell)
        {
            inventory.UseEnergyCell();

            questCompleted = true;

            // Reward the player with a spaceship component.
            SpaceShipParts.partsCollected++;

            ShowDialogue(
                "Survivor: You found one! Thank you!\n\n" +
                "I managed to recover a spaceship component earlier.\n\n" +
                "Take it. You might need it to escape this planet."
            );

            Debug.Log("NPC quest completed! Spaceship part rewarded.");
        }

        // Player hasn't found the item yet.
        else
        {
            ShowDialogue(
                "Survivor: Have you found the energy cell yet?\n\n" +
                "I think there might be one near the wreckage."
            );
        }
    } 

    private void Update()
    {
        if (dialogueVisible &&
            Input.GetKeyDown(KeyCode.E) &&
            Time.frameCount > dialogueOpenedFrame)
        {
            CloseDialogue();
        }
    }

    private void ShowDialogue(string message)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = message;
    } 

    private void CloseDialogue()
    {
        dialoguePanel.SetActive(false);

        dialogueVisible = false;

        if (interactionManager != null)
        {
            interactionManager.SetDialogueOpen(false);
        }
    }
}
