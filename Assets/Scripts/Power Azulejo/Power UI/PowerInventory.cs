using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerInventory : MonoBehaviour{
    private Inventory playerInventory;
    
    [Header("UI References")]
    public RectTransform bagRect;
    public Transform heldItemParent;
    public GameObject nextButton;
    
    [Header("Prefabs")]
    public GameObject itemElementPrefab;
    
    [Header("Tile Placement")]
    public float bagTileScale = 93f;
    public float bagOffsetX = 5f;
    public float bagOffsetY = 5f;
    public float maxRotation = 20f;
    private int orderCount = 0;
    
    [Header("Slots")]
    public PowerHandSlot[] powerHandSlots;
    public Transform slotPosition;
    public float slotScale = 20;
    private PowerHandSlot currentSlot;
    

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
            element.transform.localScale = new Vector3(bagTileScale, bagTileScale, bagTileScale);
            PlaceElement(element);
            element.GetComponent<PowerItemElement>().UpdateSpriteOrder(orderCount*4);
            orderCount++;
        }
    }

    private GameObject CreateItemElementFromTile(GameObject tile){
        GameObject element = Instantiate(itemElementPrefab);
        PowerItemElement itemElement = element.GetComponent<PowerItemElement>();
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

    public void DropItemElement(PowerItemElement element){
        // Change to random rotation for extra sauce
        float rot = Random.Range(-maxRotation, maxRotation);
        element.transform.rotation = Quaternion.Euler(0,0,rot);
        orderCount++;
        element.UpdateSpriteOrder(orderCount*4);
        
        // If hovering power drop spot, drop it
        if(currentSlot != null ){
            PlaceOnSlot(element);
            SetCurrentSlot(null);
            CheckNextButtonStatus();
            return;
        }

        // Reset parent
        element.transform.SetParent(bagRect.transform);

        // If not hovering bag, return to previous place
        if(!IsInsideBag(element)){
            ReturnElementToBag(element);
        }

        CheckNextButtonStatus();
    }

    private void PlaceOnSlot(PowerItemElement element){
        int i = System.Array.IndexOf(powerHandSlots, currentSlot);
        if(i == -1) return;

        // Check if tile is empty
        PowerItemElement oldElement = currentSlot.GetComponentInChildren<PowerItemElement>();

        // If not empty, return tile there to bag
        if(oldElement != null){
            ReturnElementToBag(oldElement);
        }

        // Place new tile
        PositionTile(currentSlot, element);
    }

    private void PositionTile(PowerHandSlot slot, PowerItemElement item){
        item.transform.SetParent(slot.transform);
        item.transform.position = slot.transform.position;
        item.transform.rotation = Quaternion.identity;
        item.transform.localScale = new  Vector3(slotScale, slotScale, slotScale);
    }

    private void ReturnElementToBag(PowerItemElement element){
        element.transform.SetParent(bagRect);
        element.transform.localScale = new Vector3(bagTileScale, bagTileScale, bagTileScale);
        PlaceElement(element.gameObject);
    }

    private bool IsInsideBag(PowerItemElement element){
        float x = element.GetComponent<RectTransform>().localPosition.x;
        float y = element.GetComponent<RectTransform>().localPosition.y;
        
        if(x < bagRect.rect.xMin + bagOffsetX || x > bagRect.rect.xMax - bagOffsetX) return false;
        if(y < bagRect.rect.yMin + bagOffsetY || y > bagRect.rect.yMax - bagOffsetY) return false;

        return true;
    }

    public Vector3 GetSlotPosition(){
        return slotPosition.position;
    }

    public float GetSlotScale(){
        return slotScale;
    }

    // === Handling slot hovering ===
    public void SetCurrentSlot(PowerHandSlot slot){
        currentSlot = slot;
    }

    // === Next Button ===
    private void CheckNextButtonStatus(){
        List<Tile> tiles = new List<Tile>();
        foreach(PowerHandSlot slot in powerHandSlots){
            Tile tile = slot.GetComponentInChildren<Tile>();
            if(tile != null) tiles.Add(tile);
        }

        if (tiles.Count == powerHandSlots.Length){
            nextButton.SetActive(true);
        } else nextButton.SetActive(false);
    }

    public void OnNextButtonClicked(){
        List<Tile> tiles = new List<Tile>();
        foreach(PowerHandSlot slot in powerHandSlots){
            Tile tile = slot.GetComponentInChildren<Tile>();
            if(tile != null) tiles.Add(tile);
        }

        PowerManager.Instance.TriggerSelectionEnd(tiles);
    }
}
