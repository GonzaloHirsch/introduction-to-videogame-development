using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LifeController : FrameLord.MonoBehaviorSingleton<LifeController>
{
    public Image lifeImagePrefab;
    public GameObject lifePanel;

    private float lifeImagePrefabWidth;
    private int nextLifeDistance;
    private RectTransform lifePanelRect;

    private Vector2 lastLifePosition;
    private Texture lifeTexture;
    private int lifeCounter = 0;
    private List<GameObject> createdLives = new List<GameObject>();
    private int lifeWidth;

    new void Awake()
    {
        this.nextLifeDistance = (int)(lifeImagePrefab.rectTransform.rect.width * 1.5f);
        this.lifeWidth = (int)lifeImagePrefab.rectTransform.rect.width;
        this.nextLifeDistance = (int)(this.lifeWidth * 2f);
        if (this.lifePanel == null){
            Debug.LogError("The life panel must be ste up to a UI component with RectTransform");
        } else {
            this.lifePanelRect = this.lifePanel.GetComponent<RectTransform>();
        }
    }

    void Start() {
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnPlayerDeath.EventName, OnPlayerDeath);
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnExtraLife.EventName, OnExtraLife);
    }

    private void OnExtraLife(System.Object sender, FrameLord.GameEvent e){
        this.AddLife();
    }

    public void AddLife()
    {
        // Check if we can just activate the life or not
        if (this.createdLives.Count > this.lifeCounter)
        {
            this.createdLives[this.lifeCounter++].SetActive(true);
        }
        else
        {
            // Increment the number of lives
            this.lifeCounter++;
            // Create the gameobject
            GameObject newLife = new GameObject();
            newLife.name = "Player Life " + this.lifeCounter;
            // Add the rect transform
            RectTransform trans = newLife.AddComponent<RectTransform>();
            trans.transform.SetParent(lifePanel.transform); // setting parent
            trans.localScale = Vector3.one;
            // If we have a last life position, create one depending on that, otherwise create it
            Vector2 newPosition = this.lastLifePosition;
            // If it's the first life we create, we won't have a previous position
            if (lifeCounter == 1)
            {
                newPosition = new Vector2((int)(lifePanelRect.rect.xMin + (this.lifeWidth / 2)), 0f);
            }
            else
            {
                newPosition = new Vector2(this.lastLifePosition.x + this.nextLifeDistance, 0f);
            }
            trans.anchoredPosition = newPosition;   // Position
            trans.sizeDelta = new Vector2(lifeImagePrefab.rectTransform.rect.width, lifeImagePrefab.rectTransform.rect.height); // Size
            // Store the position for the next one
            this.lastLifePosition = newPosition;
            // Add the image component
            Image img = newLife.AddComponent<Image>();
            img.sprite = lifeImagePrefab.sprite;
            // Make it pseudo transparent
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
            img.transform.SetParent(lifePanel.transform);
            // Add the life to the list
            this.createdLives.Add(newLife);
        }
    }

    public void SetInitialLives(int count)
    {
        for (int i = 0; i < count; i++){
            this.AddLife();
        }
    }

    /* ------------------------- REMOVE LIFE ------------------------- */

    private void OnPlayerDeath(System.Object sender, FrameLord.GameEvent e){
        this.RemoveLife();
    }

    private void RemoveLife() {
        this.createdLives[--this.lifeCounter].SetActive(false);
    }
}
