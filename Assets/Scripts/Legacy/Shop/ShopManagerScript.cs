using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopManagerScript : MonoBehaviour
{
    public int[,] shopItems = new int[5, 5]; //2D array for storing shop data
    public float coins; //or whatever currency we ultimately decide
    public TextMeshProUGUI Coinstxt; //text to display current coin amount
    public List<Button> shopItemButtons; // List of item buttons - so that they can be navigated w WASD
    public int columns = 2; // number of columns in the shop grid - so they can be navigated w WASD
    public Image highlightImage;//image to indicate which tile is currently selected
    public GameObject soldOutPrefab;//prefab for a tint to appear on the button when its already been purchased
    
    private List<bool> purchasedItems = new List<bool>(); // tracks if an item is bought
    private List<GameObject> soldOutImages = new List<GameObject>(); // reference to each buttons sold out tint
    private int currentIndex = 0; // variable to track the current selected tile
  
    void Start()
    {
        Coinstxt.text = "Coins:" + coins.ToString();

        //Ids - starting at 1 because if you start at 0 it does weird stuff in player prefs
        shopItems[1, 1] = 1;
        shopItems[1, 2] = 2;
        shopItems[1, 3] = 3;
        shopItems[1, 4] = 4;

        //Price
        shopItems[2, 1] = 10;
        shopItems[2, 2] = 20;
        shopItems[2, 3] = 30;
        shopItems[2, 4] = 40;

        //The tutorial had a whole quantity thing that i feel like we don't need because I feel like each tile should be unique

        // Set tile buttons that are in the shop - make sure they are there
        if (shopItemButtons == null || shopItemButtons.Count == 0)
        {
            Debug.LogError("ShopItemButtons not assigned in the Inspector.");
            return;
        }
        // Initialize purchasedItems and SoldOut images for each tile
        for (int i = 0; i < shopItemButtons.Count; i++)
        {
            purchasedItems.Add(false); // At start none of them are bought yet
            
            // Instantiate the SoldOut prefab as a child of each button so that they overlay properly
            GameObject soldOutImage = Instantiate(soldOutPrefab, shopItemButtons[i].transform);
            soldOutImage.SetActive(false); // hide at start
            soldOutImages.Add(soldOutImage); // add to list
        }
    }

    void Update()
    {
        HandleNavigation(); //when player presses enter that tile is bought

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Buy();
        }
    }

    private void HandleNavigation() //when WASD is pressed changes selected tile
    {
        if (Input.GetKeyDown(KeyCode.W)) // Up
        {
            MoveSelection(-1 * columns); // move up by one row
        }
        else if (Input.GetKeyDown(KeyCode.S)) // Down
        {
            MoveSelection(columns); // move down by one row
        }
        else if (Input.GetKeyDown(KeyCode.A)) // Left
        {
            MoveSelection(-1); // move left
        }
        else if (Input.GetKeyDown(KeyCode.D)) // right
        {
            MoveSelection(1); // move right
        }
    }

    private void MoveSelection(int offset)
    {
        //keeps us within valid range 
        int newIndex = Mathf.Clamp(currentIndex + offset, 0, shopItemButtons.Count - 1);
        
        if (newIndex != currentIndex)
        {
            currentIndex = newIndex;
            UpdateHighlight();
            //updates UI so that currently selected object is able to be purchased
            EventSystem.current.SetSelectedGameObject(shopItemButtons[currentIndex].gameObject);
        }
    }

    private void UpdateHighlight()
    {
        // Move the highlight/ selection image to the position of the currently selected button
        highlightImage.transform.position = shopItemButtons[currentIndex].transform.position;
    }
    
    public void Buy()
    {
        Button selectedButton = shopItemButtons[currentIndex];
        int itemID = selectedButton.GetComponent<ButtonInfo>().ItemID - 1; // Adjust for zero-based index 

        // Check for valid index
        if (itemID < 0 || itemID >= purchasedItems.Count)
        {
            Debug.LogError("Item ID is out of range: " + itemID);
            return;
        }

        // Check if the item is already purchased
        if (purchasedItems[itemID])
        {
            Debug.Log("Item already purchased.");
            return;
        }

        // Check if player has enough coins
        if (coins >= shopItems[2, itemID + 1]) // +1 to adjust for original 1-based indexing in shopItems array
        {
            // take coins and update text
            coins -= shopItems[2, itemID + 1];
            Coinstxt.text = "Coins: " + coins.ToString();
            Debug.Log("Purchased item with ID: " + (itemID + 1));

            // tile is marked as purchased
            purchasedItems[itemID] = true;

            // Enable SoldOut tint on the button
            soldOutImages[itemID].SetActive(true);

            // Set the SoldOut tint as a child of the button to make sure it's positioned on top
            soldOutImages[itemID].transform.SetParent(selectedButton.transform);

            // Reset the local position of the SoldOut image to be centered on the button - none of that off to the side shit
            RectTransform soldOutRectTransform = soldOutImages[itemID].GetComponent<RectTransform>();
            soldOutRectTransform.localPosition = Vector3.zero;  // Set to (0, 0) relative to the button
        }
        else
        {
            Debug.Log("Not enough coins to purchase item with ID: " + (itemID + 1));
        }
    }



}
