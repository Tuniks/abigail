using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbigailArrowOptions : MonoBehaviour
{
    private enum State { Choose, Leave }
    private State currentState = State.Choose;

    public GameObject option1;       // Option 1 (Claw)
    public GameObject option2;       // Option 2 (Garden)
    public GameObject option1Manager; // Manager for Option 1
    public GameObject option2Manager; // Manager for Option 2
    public GameObject BG;

    private int selection = 0; // 0 for Option 1, 1 for Option 2
    private Vector3 originalSize1;
    private Vector3 originalSize2;

    void Start()
    {
        currentState = State.Choose;
        originalSize1 = option1.transform.localScale;
        originalSize2 = option2.transform.localScale;
        HighlightSelection();
    }

    void Update()
    {
        if (currentState == State.Choose)
        {
            HandleSelectionChange();
            HandleMouseSelection(); // Detects mouse click selection

            if (Input.GetKeyDown(KeyCode.E))
            {
                ConfirmSelection();
            }
        }
    }

    void HandleSelectionChange()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selection != 0)
            {
                selection = 0;
                HighlightSelection();
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selection != 1)
            {
                selection = 1;
                HighlightSelection();
            }
        }
    }

    void HandleMouseSelection()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        float cameraDepth = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, cameraDepth));

        // Set dimensions for the box cast (adjust width and height)
        Vector2 boxCastSize = new Vector2(1f, 1f);  // Adjust the size to fit the objects

        // Use Physics2D.BoxCast for larger detection
        RaycastHit2D hit = Physics2D.BoxCast(mouseWorldPos, boxCastSize, 0f, Vector2.zero, Mathf.Infinity);

        if (hit.collider != null)
        {
            GameObject hoveredObject = hit.collider.gameObject;

            if (hoveredObject == option1)
            {
                selection = 0;
                HighlightSelection();

                if (Input.GetMouseButtonDown(0)) // Left click
                {
                    ConfirmSelection();
                }
            }
            else if (hoveredObject == option2)
            {
                selection = 1;
                HighlightSelection();

                if (Input.GetMouseButtonDown(0)) // Left click
                {
                    ConfirmSelection();
                }
            }
        }
    }

    void HighlightSelection()
    {
        option1.transform.localScale = (selection == 0) ? originalSize1 * 1.3f : originalSize1;
        option2.transform.localScale = (selection == 1) ? originalSize2 * 1.3f : originalSize2;
    }

    void ConfirmSelection()
    {
        currentState = State.Leave;
        BG.SetActive(false);
        option1.SetActive(false);
        option2.SetActive(false);

        if (selection == 0)
        {
            option1Manager.SetActive(true);
        }
        else
        {
            option2Manager.SetActive(true);
        }
    }
}
