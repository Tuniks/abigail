using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour{
    public static PlayerUIManager instance;
    
    [Header("Inventory")]
    public GameObject inventoryScreen;
    public Transform collectionParent;
    public Transform heldItemParent;
    public GameObject itemElementPrefab;

    [Header("Notification")]
    public Notification newTileNotification;

    private Inventory playerInventory;

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


    }

    public void SetInventoryUI(){
        ClearCollection();

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

    public void ShowNewTileNotification(){
        newTileNotification.Show();
    }
}
