using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour{
    public static PlayerUIManager instance;
    
    [Header("Inventory")]
    public GameObject inventoryScreen;
    public Transform collectionParent;
    public Transform activeParent;
    public Transform heldItemParent;
    public GameObject itemElementPrefab;

    [Header("Notification")]
    public Notification newTileNotification;

    private Inventory playerInventory;
    private ItemSlot currentItemSlot = null;

    void Start(){
        instance = this;
        playerInventory = PlayerInventory.Instance;
        SetInventoryUI();
    }

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
        return heldItemParent;
    }

    public void DropItemElement(ItemElement element){
        // If not hovering a new slot, return to previous place
        if(currentItemSlot == null){
            element.ReturnToPreviousParent();
            return;
        }

        // Getting tile from element
        Tile tile = element.GetTile();

        // If hovering empty slot, move to that slot and update inventory once
        ItemElement currentElement = currentItemSlot.GetComponentInChildren<ItemElement>();
        if(currentElement == null){
            element.SetNewParent(currentItemSlot.transform);
            playerInventory.MoveTile(tile, currentItemSlot.isActive);
            return;
        }

        // If hovering occupied slot, swap to that slot and update inventory twice
        currentElement.SetNewParent(element.GetPreviousParent());
        Tile currentElementTile = currentElement.GetTile();
        playerInventory.MoveTile(currentElementTile, element.GetPreviousParent().GetComponent<ItemSlot>().isActive);

        element.SetNewParent(currentItemSlot.transform);
        playerInventory.MoveTile(tile, currentItemSlot.isActive); 
    }

    public Transform GetNextFreeSlotCollection(){
        foreach(Transform child in collectionParent){
            if(child.GetComponentInChildren<ItemElement>() == null){
                return child.transform;
            }
        }
        return null;
    }

    public Transform GetNextFreeSlotActive(){
        foreach(Transform child in activeParent){
            if(child.GetComponentInChildren<ItemElement>() == null){
                return child.transform;
            }
        }
        return GetNextFreeSlotCollection();
    }

    public void SetInventoryUI(){
        ClearCollection();
        ClearActive();

        StartCoroutine(UpdateInventoryUI());
    }

    private IEnumerator UpdateInventoryUI(){
        // Has to be done next frame so Clear can take effect
        yield return new WaitForNextFrameUnit();
        
        List<GameObject> collection = playerInventory.GetTileCollection();
        foreach(GameObject item in collection){
            GameObject element = CreateItemElementFromTile(item);
            Transform slot = GetNextFreeSlotCollection();
            element.GetComponent<ItemElement>().SetNewParent(slot);
        }

        List<GameObject> active = playerInventory.GetActiveTiles();
        foreach(GameObject item in active){
            GameObject element = CreateItemElementFromTile(item);
            Transform slot = GetNextFreeSlotActive();
            element.GetComponent<ItemElement>().SetNewParent(slot);
        }
    }

    private GameObject CreateItemElementFromTile(GameObject tile){
        GameObject element = Instantiate(itemElementPrefab);
        element.GetComponent<ItemElement>().SetTile(tile);
        return element;
    }

    private void ClearCollection(){
        foreach(Transform child in collectionParent){
            if(child.GetComponentInChildren<ItemElement>() != null){
                Destroy(child.GetChild(0).gameObject);
            }
        }
    }

    private void ClearActive(){
        foreach(Transform child in activeParent){
            if(child.GetComponentInChildren<ItemElement>() != null){
                Destroy(child.GetChild(0).gameObject);
            }
        }
    }

    public void ShowNewTileNotification(){
        newTileNotification.Show();
    }
}
