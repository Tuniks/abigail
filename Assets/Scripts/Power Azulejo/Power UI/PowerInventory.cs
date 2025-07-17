using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerInventory : MonoBehaviour{
    private Inventory playerInventory;
    
    [Header("UI References")]
    public RectTransform bagRect;
    public Transform heldItemParent;
    
    [Header("Prefabs")]
    public GameObject itemElementPrefab;
    
    [Header("Tile Placement")]
    public float bagOffsetX = 5f;
    public float bagOffsetY = 5f;
    public float maxRotation = 20f;
    private int orderCount = 0;

    // Power Azulejo slot
    

    public void Initialize(){
        playerInventory = PlayerInventory.Instance;
        SetInventoryUI();
    }

    // === Generating grabbable tiles visuals inside the bag ===
    private void SetInventoryUI(){
        List<GameObject> collection = playerInventory.GetTileCollection();
        orderCount = 0;
        foreach(GameObject item in collection){
            GameObject element = CreateItemElementFromTile(item);
            element.transform.SetParent(bagRect);
            element.transform.localScale = new Vector3(93, 93, 93);
            PlaceElement(element);
            element.GetComponent<ItemElement>().UpdateSpriteOrder(orderCount*4);
            orderCount++;
        }
    }

    private GameObject CreateItemElementFromTile(GameObject tile){
        GameObject element = Instantiate(itemElementPrefab);
        ItemElement itemElement = element.GetComponent<ItemElement>();
        itemElement.SetTile(tile);
        return element;
    }

    private void PlaceElement(GameObject element){
        float x = Random.Range(
            bagRect.rect.xMin + bagOffsetX,
            bagRect.rect.xMax - bagOffsetX
        );

        float y = Random.Range(
            bagRect.rect.yMin + bagOffsetY,
            bagRect.rect.yMax - bagOffsetY
        );

        float rot = Random.Range(-maxRotation, maxRotation);

        element.transform.SetLocalPositionAndRotation(new Vector3(x, y, 0), Quaternion.Euler(0,0,rot));
    }

    // === Drag and Drop handlers ===
    public Transform GetHeldItemParent(){
        return heldItemParent;
    }

    public void DropItemElement(ItemElement element){
        // Change to random rotation for extra sauce
        float rot = Random.Range(-maxRotation, maxRotation);
        element.transform.rotation = Quaternion.Euler(0,0,rot);
        orderCount++;
        element.UpdateSpriteOrder(orderCount*4);
        
        // If hovering power drop spot, drop it
        if(convoSlot != null ){
            currentConvo.OnTileSelected(element.GetTile(), convoSlot);
            SetCurrentSlot(null);
            return;
        }

        // Reset parent
        element.transform.SetParent(bagRect.transform);

        // If not hovering bag, return to previous place
        if(!IsInsideBag(element)){
            element.ReturnToPreviousPosition();
            return;
        }
    }
}
