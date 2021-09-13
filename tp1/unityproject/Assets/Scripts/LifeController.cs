using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeController : MonoBehaviour
{
    public Image lifeImagePrefab;
    public GameObject lifePanel;

    private float lifeImagePrefabWidth;
    private float nextLifeDistance;
    private GameController gameController;
    private RectTransform lifePanelRect;

    private Vector2 lastLifePosition;
    private Texture lifeTexture;

    void Awake()
    {
        this.nextLifeDistance = lifeImagePrefab.rectTransform.rect.width * 1.5f;
        this.gameController = GetComponent<GameController>();
        this.lifePanelRect = this.lifePanel.GetComponent<RectTransform>();
        this.lastLifePosition = new Vector2(-(lifePanelRect.rect.width / 2), 0f);
    }

    void Start()
    {
        float width = lifeImagePrefab.rectTransform.rect.width;
        this.nextLifeDistance = width * 2f;
        this.updateLives(0);
    }

    /*  void OnGUI()
     {
         Vector2 newPosition = lastLifePosition + new Vector2(this.nextLifeDistance, 0f);
         Debug.Log(newPosition);
         Debug.Log(this.nextLifeDistance);
         Rect rect = new Rect(newPosition.x, newPosition.y, lifeSprite.texture.width, lifeSprite.texture.height);
         GUI.DrawTexture(rect, this.getSpriteTexture());
     } */
    /* 
        Texture2D getSpriteTexture()
        {
            // assume "sprite" is your Sprite object
            var croppedTexture = new Texture2D((int)this.lifeSprite.rect.width, (int)this.lifeSprite.rect.height);
            var pixels = this.lifeSprite.texture.GetPixels((int)this.lifeSprite.textureRect.x,
                                                    (int)this.lifeSprite.textureRect.y,
                                                    (int)this.lifeSprite.textureRect.width,
                                                    (int)this.lifeSprite.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            return croppedTexture;
        }
     */
    public void updateLives(int count)
    {
        GameObject go = new GameObject();
        RectTransform trans = go.AddComponent<RectTransform>();
        trans.transform.SetParent(lifePanel.transform); // setting parent
        trans.localScale = Vector3.one;
        trans.anchoredPosition = new Vector2(lifePanelRect.rect.left, lifePanelRect.rect.top); // setting position, will be on center
        trans.sizeDelta = new Vector2(lifeImagePrefab.rectTransform.rect.width, lifeImagePrefab.rectTransform.rect.height); // custom size
        Image img = go.AddComponent<Image>();
        img.sprite = lifeImagePrefab.sprite;
        img.transform.SetParent(lifePanel.transform);
        // Image newLife = Instantiate(lifeImagePrefab, newPosition, Quaternion.identity);
        // newLife.GetComponent<RectTransform>().SetParent(lifePanel.transform);
        // GUI.DrawTexture(this.lifePanel.gameObject.)
    }

    /* // Update is called once per frame
    void Update()
    {
        
    }

    public void setLives(int n)
    {
    	if (n > 1) {
    		if( nextLife == null) {
    			this.nextLife = createLife();
    		}
            LifeController nextLifeController = this.nextLife.GetComponent<LifeController>();
            nextLifeController.nextLifeDistance = this.nextLifeDistance;
            nextLifeController.setLives(n-1);
    	} else if (n == 1) {
            if (nextLife != null) {
                LifeController nextLifeController = this.nextLife.GetComponent<LifeController>();
                nextLifeController.setLives(n-1);
            }
        } else {
            if (nextLife != null) {
                LifeController nextLifeController = this.nextLife.GetComponent<LifeController>();
                nextLifeController.setLives(n-1);
            }
            Destroy(this.gameObject);
        }
    }

    private GameObject createLife()
    {
    	GameObject life = Instantiate(this.lifePrefab, transform.position + new Vector3(this.nextLifeDistance,0,0), transform.rotation);
        life.transform.localScale = transform.localScale;        
        return life;
    } */
}
