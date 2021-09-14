using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeController : MonoBehaviour
{
    public Image lifeImagePrefab;
    public GameObject lifePanel;

    private float lifeImagePrefabWidth;
    private int nextLifeDistance;
    private GameController gameController;
    private RectTransform lifePanelRect;

    private Vector2 lastLifePosition;
    private Texture lifeTexture;
    private int lifeCounter = 0;
    private List<GameObject> createdLives = new List<GameObject>();
    private int lifeWidth;

    void Awake()
    {
        this.nextLifeDistance = (int)(lifeImagePrefab.rectTransform.rect.width * 1.5f);
        this.gameController = GetComponent<GameController>();
        this.lifePanelRect = this.lifePanel.GetComponent<RectTransform>();
    }

    void Start()
    {
        this.lifeWidth = (int)lifeImagePrefab.rectTransform.rect.width;
        this.nextLifeDistance = (int)(this.lifeWidth * 2f);
    }

    public void addLife()
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
            // Debug.Log(lastLifePosition);
            // Debug.Log(lifePanelRect.rect.xMin + "-" + lifePanelRect.rect.xMax + "-" + lifePanelRect.rect);
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

    public void addLives(int count)
    {
        for (int i = 0; i < count; i++){
            this.addLife();
        }
    }

    public void removeLife() {
        this.createdLives[--this.lifeCounter].SetActive(false);
    }
}
