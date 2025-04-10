using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviour{
    public static PlayerUIManager instance;
    
    [Header("Inventory")]
    public GameObject inventoryScreen;
    public RectTransform bagRect;
    public Transform heldItemParent;

    [Header("Prefabs")]
    public GameObject itemElementPrefab;

    [Header("Tile Placement")]
    public float bagOffsetX = 5f;
    public float bagOffsetY = 5f;
    public float maxRotation = 20f;

    [Header("Notification")]
    public Notification newTileNotification;

    private Inventory playerInventory;

    private ConvoSlot convoSlot;

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
        if(inventoryScreen.activeSelf){
            inventoryScreen.SetActive(false);
        } else {
            SetInventoryUI();
            inventoryScreen.SetActive(true);
        }
    }


    public Transform GetHeldItemParent(){
        return heldItemParent;
    }

    public void DropItemElement(ItemElement element){
        // Change to random rotation for extra sauce
        float rot = Random.Range(-maxRotation, maxRotation);
        element.transform.rotation = Quaternion.Euler(0,0,rot);
        
        // If hovering drop spot, drop it


        // Reset parent
        element.transform.SetParent(bagRect.transform);

        // If not hovering bag, return to previous place
        Debug.Log(IsInsideBag(element));
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
        foreach(GameObject item in collection){
            GameObject element = CreateItemElementFromTile(item);
            element.transform.SetParent(bagRect);
            PlaceElement(element);
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

    // ====== AZULEJO CONVO ======
    public void SetCurrentSlot(ConvoSlot slot){
        convoSlot = slot;
    }


    // ====== NOTIFICATIONS ======

    public void ShowNewTileNotification(){
        newTileNotification.Show();
    }
}
