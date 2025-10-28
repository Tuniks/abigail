using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemElement : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler{
    // Manager References
    private PlayerUIManager ui;
    private CanvasGroup cg;

    // Parent Tile
    private GameObject ogTile;

    // Repositioning
    private Vector3 previousPos;
    private Dictionary<SpriteRenderer, int> originalSpriteOrder = null;

    // Tile UI
    public TileUI tileUI;
    public Vector3 tileUIScale = new Vector3(1, 1, 1);

    // Tile Scaling for Azulejo Convo
    [Header("Tile Scale")]
    public Vector3 tileScale = new Vector3(1, 1, 1);
    private float startingScale = 1f;

    // Animation for Azulejo Phenomenon
    [Header("Phenomenon")]
    public float twitchRange = 1f;
    public float twitchRotationRange = 5f;
    public Vector2 twitchTimeLimits = new Vector2(0, 1);
    private bool isTwitching = false;
    private float currentTwitchTimer = 0;

    // New tile decor
    [Header("Tile Decoration")]
    public GameObject newTileDecor;
    

    void Start(){
        ui = GetComponentInParent<PlayerUIManager>();
        cg = GetComponent<CanvasGroup>();
        startingScale = transform.localScale.x;
    }

    void Update(){
        if(isTwitching){
            currentTwitchTimer -= Time.deltaTime;
            if(currentTwitchTimer < 0){
                currentTwitchTimer = Random.Range(twitchTimeLimits.x, twitchTimeLimits.y);
                Transform tile = transform.GetChild(1);
                tile.localPosition = new Vector3(
                    Random.Range(-twitchRange, twitchRange),
                    Random.Range(-twitchRange, twitchRange),
                    0
                );
                tile.localRotation = Quaternion.Euler(0,0,Random.Range(-twitchRotationRange, twitchRotationRange));
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData){
        previousPos = transform.position;
        cg.blocksRaycasts = false;
        transform.SetParent(ui.GetHeldItemParent(), false);
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; 
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

        UpdateSpriteOrder(1000);
        UpdateMaskBehaviour(false);

        ui.ShowTileDetails(GetTile());
    }

    public void OnDrag(PointerEventData eventData){
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; 
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        
        Vector3 slotPos = PlayerUIManager.instance.GetSlotPosition();
        float targetScale = PlayerUIManager.instance.GetSlotScale();
        if(targetScale == 0) return;

        float mult = Mathf.Max(0, 1 - (Vector3.Distance(transform.position, slotPos)/Vector3.Distance(previousPos, slotPos)));
        float newScale = startingScale + (targetScale-startingScale)*mult;
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    public void OnPointerUp(PointerEventData eventData){
        transform.localScale = new Vector3(startingScale, startingScale, startingScale);
        cg.blocksRaycasts = true;
        UpdateMaskBehaviour(true);
        ui.DropItemElement(this);
    }

    public void ReturnToPreviousPosition(){
        transform.position = previousPos;
    }

    public void SetNewParent(Transform dad){
        transform.SetParent(dad,false);
    } 

    public void SetTile(GameObject tile){
        if(tile == null) return;

        ogTile = tile;
        
        // Creating copy of tile
        // TO DO might not be needed anymore
        GameObject tileCopy = Instantiate(tile, Vector3.zero, Quaternion.identity);
        tileCopy.transform.SetParent(transform, false);
        tileCopy.transform.localScale = tileScale;

        tileCopy.SetActive(false);

        // Creating tile UI
        tileUI.BuildTileUI(ogTile.GetComponent<Tile>());
        tileUI.transform.localScale = tileUIScale;
        tileUI.gameObject.SetActive(true);
    }

    public Tile GetTile(){
        return GetComponentInChildren<Tile>();
    }

    public void UpdateSpriteOrder(int order){
        if(originalSpriteOrder == null) InitializeOGOrder();

        SpriteRenderer[] sprs = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer spr in sprs){
            spr.sortingOrder = originalSpriteOrder[spr] + order;
        }
    }

    private void InitializeOGOrder(){
        originalSpriteOrder = new Dictionary<SpriteRenderer, int>();

        SpriteRenderer[] sprs = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer spr in sprs){
            originalSpriteOrder[spr] = spr.sortingOrder;
        }
    }

    private void UpdateMaskBehaviour(bool isMaskable){
        SpriteRenderer[] sprs = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer spr in sprs){
            if(!isMaskable){
                spr.maskInteraction = SpriteMaskInteraction.None;
            } else spr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
    }

    // Phenomenon
    public bool HasTileWithFace(TileComponent _face){
        if(_face == null) return false;
        
        Tile tile = GetComponentInChildren<Tile>();
        if(tile == null) return false;

        return tile.HasFace(_face);
    }

    public void SetTwitching(bool _status){
        if( _status == false){
            transform.GetChild(0).localPosition = Vector3.zero;
            transform.GetChild(0).localRotation = Quaternion.identity;
        }
        
        isTwitching = _status;
    }

    // Visited
    public GameObject GetOGTile(){
        return ogTile;
    }

    public void SetIsVisited(bool _status){
        newTileDecor.SetActive(!_status);
    }
}
