using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour{
    [Header("Inventory")]
    public GameObject inventoryScreen;
    public GameObject heldItemParent;
    private ItemSlot currentItemSlot = null;

    void Update(){
        if(Input.GetKeyDown("i") || Input.GetKeyDown(KeyCode.Tab)){
            ToggleInventory();
        }
    }

    // ====== INVENTORY =======

    private void ToggleInventory(){
        inventoryScreen.SetActive(!inventoryScreen.activeSelf);
    }

    public void SetCurrentSlot(ItemSlot itemSlot){
        currentItemSlot = itemSlot;
    }

    public Transform GetHeldItemParent(){
        return heldItemParent.transform;
    }

    public void DropItemElement(ItemElement element){
        if(currentItemSlot == null){
            element.ReturnToPreviousParent();
            return;
        }

        ItemElement currentElement = currentItemSlot.GetComponentInChildren<ItemElement>();
        if(currentElement == null){
            element.SetNewParent(currentItemSlot.transform);
            return;
        }

        currentElement.SetNewParent(element.GetPreviousParent());
        element.SetNewParent(currentItemSlot.transform);
    }
}
