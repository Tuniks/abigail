using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class AzulejoConvoCollage : BaseConvo{
    public AzulejoConvoUICollage convoUI;

    private PlayerStatus ps;

    public bool NoSigilActivated = false;
    public GameObject InWorldSigil;

    [Header("Conversation Nodes")]
    public List<FaceDialoguePair> faceDialoguePairs;
    public string defaultNode;

    [Header("Juice")]
    public float endConvoDelay = 1f;

    public float DelayforSigilAnimation = 1f;
    
    [Header("Sounds")]
    public AudioClip SigilActivated;
    public AudioClip FailureSound;
    public AudioClip TilePlacedInHand;
    public AudioSource audioSource;
    
    void Start(){
        ps = PlayerStatus.Instance;
        audioSource = GetComponent<AudioSource>();
    }

    protected override void StartConvo(){
        ps.OnStartAzulejoConversation();
        PlayerUIManager.instance.SetCurrentConvo(this);
        PlayerUIManager.instance.ShowInventory();
        NoSigilActivated = false;
        convoUI.Show();
    }

    private IEnumerator EndConvo(string node){
        yield return new WaitForSeconds(endConvoDelay);
        if (NoSigilActivated == true)
        {
            convoUI.HideNoSigil();
            audioSource.PlayOneShot(FailureSound, 0.7F);
            yield return new WaitForSeconds(endConvoDelay);
            ps.OnEndAzulejoConversation();
            PlayerUIManager.instance.SetCurrentConvo(null);
            PlayerUIManager.instance.HideInventory();
            
            convoUI.Hide();
            PlayerInteractor.instance.StartConversation(node);
        }
        else
        {
            convoUI.HideSigil();
            InWorldSigil.SetActive(false);
            audioSource.PlayOneShot(SigilActivated, 0.7F);
            yield return new WaitForSeconds(DelayforSigilAnimation);
            ps.OnEndAzulejoConversation();
            PlayerUIManager.instance.SetCurrentConvo(null);
            PlayerUIManager.instance.HideInventory();
            convoUI.Hide();

            PlayerInteractor.instance.StartConversation(node);
        }
        // If it is node == defaultNode
        // Quit, and run default dialogue
        
        // If it is not (aka there is a bespoke response)
        // Play the crumble animation
        // Wait a few second for the crumble animation to finish
        // yield return new WaitForSeconds(TIME OF ANIMATION)
        // Run the bespoke node
        // GOING TO YARN the bespoke node will kickstart the foot animation
    }

    public override void QuitConvo(){
        ps.OnEndAzulejoConversation();
        PlayerUIManager.instance.SetCurrentConvo(null);
        PlayerUIManager.instance.HideInventory();
        convoUI.Hide();
    }

    public override void OnTileSelected(Tile tile, ConvoSlot slot){
        audioSource.PlayOneShot(TilePlacedInHand, 0.7F);
        convoUI.SetTile(tile);
        string selectedFace = tile.GetName();

        PlayerInteractor.instance.UpdateLastTileUsed(tile);

        foreach(FaceDialoguePair pair in faceDialoguePairs){
            TileComponent face = pair.facePrefab.GetComponent<TileComponent>();
            if(face.title == selectedFace)
            {
                StartCoroutine(EndConvo(pair.dialogueNode));
                return;
            }
        }

        StartCoroutine(EndConvo(defaultNode));
        NoSigilActivated = true;
    }

    public override bool IsActive(){
        return true;
    }

    public override Vector3 GetSlotPosition(){
        return convoUI.GetSlotPosition();
    }

    public override float GetSlotScale(){
        return convoUI.GetSlotScale();
    }
}
