using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviour{
    public static PlayerUIManager instance;
    private Inventory playerInventory;

    [Header("Inventory")]
    public GameObject inventoryScreen;
    public RectTransform bagRect;
    public Transform heldItemParent;

    [Header("Tile Details")]
    public GameObject tileDetailsScreen;

    [Header("Prefabs")]
    public GameObject itemElementPrefab;

    [Header("Tile Placement")]
    public float bagOffsetX = 5f;
    public float bagOffsetY = 5f;
    public float maxRotation = 20f;
    private int orderCount = 0;

    [Header("Notification")]
    public Notification newTileNotification;

    // === Azulejo Conversation ===
    private AzulejoConvo currentConvo = null;
    private ConvoSlot convoSlot = null;

    void Start(){
        instance = this;
        playerInventory = PlayerInventory.Instance;
        SetInventoryUI();
    }

    void Update(){
        if(Input.GetKeyDown("i") || Input.GetKeyDown(KeyCode.Tab)){
            if(!IsPlayerBusy()){
                if(inventoryScreen.activeSelf){
                    HideInventory();
                } else ShowInventory();
            } else if(currentConvo != null){
                currentConvo.QuitConvo();
            }
        }
    }

    private bool IsPlayerBusy(){
        if(currentConvo != null) return true;
        if(PlayerInteractor.instance.IsPlayerBusy()) return true;

        return false;
    }

    // ====== INVENTORY =======
    public void ShowInventory(){
        SetInventoryUI();
        inventoryScreen.SetActive(true);
    }

    public void HideInventory(){
        inventoryScreen.SetActive(false);
        tileDetailsScreen.SetActive(false);
    }

    public Transform GetHeldItemParent(){
        return heldItemParent;
    }

    public void DropItemElement(ItemElement element){
        // Change to random rotation for extra sauce
        float rot = Random.Range(-maxRotation, maxRotation);
        element.transform.rotation = Quaternion.Euler(0,0,rot);
        orderCount++;
        element.UpdateSpriteOrder(orderCount*4);
        
        // If hovering drop spot, drop it
        if(convoSlot != null && currentConvo != null){
            currentConvo.OnTileSelected(element.GetTile());
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

    public void SetInventoryUI(){
        ClearCollection();
        StartCoroutine(UpdateInventoryUI());
    }

    private IEnumerator UpdateInventoryUI(){
        // Has to be done next frame so Clear can take effect
        yield return new WaitForNextFrameUnit();
        
        List<GameObject> collection = playerInventory.GetTileCollection();
        orderCount = 0;
        foreach(GameObject item in collection){
            GameObject element = CreateItemElementFromTile(item);
            element.transform.SetParent(bagRect);
            PlaceElement(element);
            element.GetComponent<ItemElement>().UpdateSpriteOrder(orderCount*4);
            orderCount++;
        }
    }

    private GameObject CreateItemElementFromTile(GameObject tile){
        GameObject element = Instantiate(itemElementPrefab);
        element.GetComponent<ItemElement>().SetTile(tile);
        return element;
    }

    private void ClearCollection(){
        foreach(Transform child in bagRect){
            if(child.GetComponentInChildren<ItemElement>() != null){
                Destroy(child.gameObject);
            }
        }
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
    
    private bool IsInsideBag(ItemElement element){
        float x = element.GetComponent<RectTransform>().localPosition.x;
        float y = element.GetComponent<RectTransform>().localPosition.y;
        
        if(x < bagRect.rect.xMin + bagOffsetX || x > bagRect.rect.xMax - bagOffsetX) return false;
        if(y < bagRect.rect.yMin + bagOffsetY || y > bagRect.rect.yMax - bagOffsetY) return false;

        return true;
    }

    // ======= TILE DETAILS SCREEN ======
    public void ShowTileDetails(Tile tile){
        if(!CanShowTileDetails()) return;

        tileDetailsScreen.GetComponent<TileDetailsUI>().SetTile(tile);

        tileDetailsScreen.SetActive(true);
    }

    private bool CanShowTileDetails(){
        if(currentConvo != null) return false;
        
        return inventoryScreen.activeSelf;
    }


    // ====== AZULEJO CONVO ======
    public void SetCurrentConvo(AzulejoConvo convo){
        currentConvo = convo;
    }

    public void SetCurrentSlot(ConvoSlot slot){
        convoSlot = slot;
    }

    // ====== NOTIFICATIONS ======

    public void ShowNewTileNotification(){
        newTileNotification.Show();
    }
}
