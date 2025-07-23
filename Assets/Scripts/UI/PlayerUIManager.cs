using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviour{
    public static PlayerUIManager instance;
    private Inventory playerInventory;

    [Header("Inventory")]
    public GameObject inventoryScreen;
    public RectTransform bagRect;
    public Transform heldItemParent;
    public PlayableDirector invDirector;
    public TimelineAsset openAnimation;
    public TimelineAsset closeAnimation;

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
    private BaseConvo currentConvo = null;
    private ConvoSlot convoSlot = null;

    [Header("Azulejo Phenomenon")]
    public PhenomenonUI phenomenonUI;
    public PlayableDirector bagIconAnimation;
    private AzulejoPhenomenon currentPhenomenon;
    private PhenomenonSlot phenomenonSlot = null;
    private TileComponent phenomenonTarget = null;
    public GameObject InventoryIcon;

    [Header("Bullshit ass dumb ass sounds")]
    public AudioSource audioSource;
    public AudioClip bagOpenSound;
    public AudioClip bagCloseSound;
    public AudioClip newTileNotificationSound;
    //public AudioClip PhenomenonPossibleSound;

    void Start(){
        instance = this;
        playerInventory = PlayerInventory.Instance;
        SetInventoryUI();
    }

    void Update(){
        if(Input.GetKeyDown("i") || Input.GetKeyDown(KeyCode.Tab)){
            if(!IsPlayerBusy()){
                if(inventoryScreen.activeSelf){
                    HideInventory(true);
                } else ShowInventory(true);
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
    public void ShowInventory(bool isManual = false){
        if(isManual && invDirector.state == PlayState.Playing) return;
        
        SetInventoryUI();
        invDirector.playableAsset = openAnimation;
        invDirector.Play();

        PlayerInteractor.instance.GetAudioSource().PlayOneShot(bagOpenSound);
    }

    public void HideInventory(bool isManual = false){
        if(isManual && invDirector.state == PlayState.Playing) return;

        invDirector.playableAsset = closeAnimation;
        invDirector.Play();
        tileDetailsScreen.SetActive(false);
        phenomenonUI.HideUI();
        PlayerInteractor.instance.GetAudioSource().PlayOneShot(bagCloseSound);
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
        
        // If hovering convo drop spot, drop it
        if(convoSlot != null && currentConvo != null && currentConvo.IsActive()){
            currentConvo.OnTileSelected(element.GetTile(), convoSlot);
            SetCurrentSlot(null);
            return;
        }

        // If hovering phenomenon drop spot, drop it
        if(phenomenonSlot != null && currentPhenomenon != null && currentPhenomenon.IsAMatch(element.GetTile())){
            phenomenonUI.PlaceTile(currentPhenomenon, element.GetTile());
            phenomenonSlot = null;
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
        itemElement.SetIsVisited(WorldState.Instance.WasTileVisited(tile));
        if(phenomenonTarget && itemElement.HasTileWithFace(phenomenonTarget) && !currentConvo) itemElement.SetTwitching(true);

        return element;
    }

    private void ClearCollection(){
        foreach(Transform child in bagRect){
            if(child.GetComponent<ItemElement>() != null){
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

        if(tile.HasFace(phenomenonTarget)){
            tileDetailsScreen.SetActive(false);
            ShowPhenomenonInput();
            return;
        } else phenomenonUI.HideUI();


        if(currentConvo == null){
            ItemElement element = tile.GetComponentInParent<ItemElement>();
            WorldState.Instance.VisitTile(element.GetOGTile());
            element.SetIsVisited(true);
        }

        tileDetailsScreen.GetComponent<TileDetailsUI>().SetTile(tile);
        tileDetailsScreen.SetActive(true);
    }

    private bool CanShowTileDetails(){
        if(currentConvo != null) return false;
        
        return inventoryScreen.activeSelf;
    }


    // ====== AZULEJO CONVO ======
    public void SetCurrentConvo(BaseConvo convo){
        currentConvo = convo;
    }

    public void SetCurrentSlot(ConvoSlot slot){
        convoSlot = slot;
    }

    public Vector3 GetSlotPosition(){
        if(currentConvo != null) return currentConvo.GetSlotPosition();

        if(currentPhenomenon != null && phenomenonUI.IsOn()) return phenomenonUI.GetSlotPosition();

        return Vector3.zero;
    }

    public float GetSlotScale(){
        if(currentConvo != null) return currentConvo.GetSlotScale();

        if(currentPhenomenon != null && phenomenonUI.IsOn()) return phenomenonUI.GetSlotScale();

        return 0;
    }

    // ====== PHENOMENON =======
    public void SetPhenomenon(AzulejoPhenomenon phenomenon){
        currentPhenomenon = phenomenon;
        phenomenonTarget = phenomenon == null ? null : phenomenon.GetFace();

        if(playerInventory.HasTileWithFace(phenomenonTarget)){
            AnimateBagIcon(true);
        } else AnimateBagIcon(false);
    }

    private void ShowPhenomenonInput(){
        phenomenonUI.ShowUI();
    }

    private void AnimateBagIcon(bool _status){
        if(_status){
            bagIconAnimation.Play();
            //PlayerInteractor.instance.GetAudioSource().PlayOneShot(PhenomenonPossibleSound);
            audioSource.loop = true;
            audioSource.Play(); 

            if(inventoryScreen.activeSelf == true){
                foreach(Transform child in bagRect){
                    ItemElement itemElement = child.GetComponent<ItemElement>();
                    if(itemElement != null && itemElement.HasTileWithFace(phenomenonTarget)){
                        itemElement.SetTwitching(true);
                    }
                }
            }
        } else {
            bagIconAnimation.Stop();
            audioSource.Stop();
            InventoryIcon.transform.localRotation = Quaternion.identity; 

            phenomenonUI.HideUI();

            if(inventoryScreen.activeSelf == true){
                foreach(Transform child in bagRect){
                    ItemElement itemElement = child.GetComponent<ItemElement>();
                    if(itemElement != null) itemElement.SetTwitching(false);   
                }
            }
        }
    }
    
    public void SetCurrentPhenomenonSlot(PhenomenonSlot _phenomenonSlot){
        phenomenonSlot = _phenomenonSlot;
    }

    // ====== NOTIFICATIONS ======

    public void ShowNewTileNotification(){
        newTileNotification.Show();
        PlayerInteractor.instance.GetAudioSource().PlayOneShot(newTileNotificationSound);
    }
}
