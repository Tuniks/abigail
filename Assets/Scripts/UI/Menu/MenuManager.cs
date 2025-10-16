using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour{
    [Header("Menu UI References")]
    public GameObject menuScreen;
    public GameObject[] menuPages;

    private int currentTab = 0;

    void Start(){
        foreach(GameObject page in menuPages){
            MenuTab tab = page.GetComponent<MenuTab>();
            if(tab != null){
                tab.Initialize();
            }
        }
    }

    void Update(){
        if(Input.GetKeyDown("q") && menuScreen.activeSelf){
            ChangeMenuTab(-1);
        } else if (Input.GetKeyDown("e") && menuScreen.activeSelf){
            ChangeMenuTab(1);
        }
    }

    public void ToggleMenu(){
        if(menuScreen.activeSelf){
            HideMenu();
        } else ShowMenu();
    }

    private void ShowMenu(){
        // Pause Game
        Time.timeScale = 0;

        ShowTab(currentTab);
        menuScreen.SetActive(true);
    }

    private void HideMenu(){
        // Unpause Game
        Time.timeScale = 1;

        menuScreen.SetActive(false);
    }

    private void ChangeMenuTab(int dif){
        int prev = currentTab;
        currentTab +=  dif;
        
        // Looping
        if(currentTab < 0){
            currentTab = menuPages.Length - 1;
        } else if(currentTab >= menuPages.Length){
            currentTab = 0;
        }

        // Activating new tab
        ShowTab(currentTab);

        // Deactivating old one
        HideTab(prev);
    }

    private void ShowTab(int _tab){
        MenuTab tab = menuPages[_tab].GetComponent<MenuTab>();
        if(tab != null) tab.ShowTab();
        menuPages[_tab].SetActive(true);
    }

    private void HideTab(int _tab){
        menuPages[_tab].SetActive(false);
    }
}
