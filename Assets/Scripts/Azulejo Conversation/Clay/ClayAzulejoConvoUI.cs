using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ClayAzulejoConvoUI : MonoBehaviour{
    [Header("UI References")]
    public GameObject convoScreen;
    public TextMeshProUGUI word;
    public GameObject[] playerSlots = new GameObject[3];
    public GameObject[] friendSlots = new GameObject[3];
    public Animator rockAnimator;


    [Header("Tile Placement")]
    public Vector3 tileOffset = Vector3.zero;
    public Vector3 tileRotation = Vector3.zero;
    public float tileScale = 1f;

    public void Show(){
        convoScreen.SetActive(true);
    }

    public void Hide(){
        // Clearing Player slots
        foreach(GameObject obj in playerSlots){
            ConvoSlot slot = obj.GetComponent<ConvoSlot>();
            if(slot)slot.enabled = true;
            
            foreach(Transform child in obj.transform){
                if(child.GetComponent<ItemElement>()){
                    Destroy(child.gameObject);
                }
            }
        }

        // Clearing Friend slots
        foreach(GameObject obj in friendSlots){
            foreach(Transform child in obj.transform){
                if(child.GetComponent<Tile>()){
                    Destroy(child.gameObject);
                }
            }
        }

        convoScreen.SetActive(false);
    }

    public void SetTile(Tile tile, ConvoSlot slot){
        ItemElement item = tile.GetComponentInParent<ItemElement>();

        GameObject tileSlot = slot.gameObject;

        item.transform.SetParent(tileSlot.transform);
        item.transform.localPosition = tileOffset;
        item.transform.localRotation = Quaternion.Euler(tileRotation);
        item.transform.localScale = new Vector3(tileScale, tileScale, tileScale);

        // Deactivates slot so no other tile can come on top
        slot.enabled = false;
    }

    public void SetFriendTile(Tile tile, int round){
        GameObject item = Instantiate(tile.gameObject);

        GameObject tileSlot = friendSlots[round-1];

        item.transform.SetParent(tileSlot.transform);
        item.transform.localPosition = tileOffset;
        item.transform.localRotation = Quaternion.Euler(tileRotation);
        item.transform.localScale = new Vector3(tileScale, tileScale, tileScale);
    }

    public void UpdateWord(string _word, int round){
        word.text = ScrewWord(_word, round);
    }

    private string ScrewWord(string _word, int round){
        if(round >= 3) return _word;

        float pct = .7f;
        if(round == 2) pct = .55f;

        StringBuilder newWord = new StringBuilder(_word);
        for(int i = 0; i < _word.Length; i++){
            if(Random.value <= pct) newWord[i] = ' ';
        }

        return newWord.ToString();
    }

    public void StartTransformationAnimation(){
        rockAnimator.SetTrigger("Transform");
    }

    public void StartSinkAnimation(){
        rockAnimator.SetTrigger("Sink");
    }

    public Vector3 GetSlotPosition(){
        return playerSlots[0].transform.position;
    }

    public float GetSlotScale(){
        return tileScale;
    }
}
