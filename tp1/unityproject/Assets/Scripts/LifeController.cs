using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeController : MonoBehaviour
{

	public float nextLifeDistance;
	public GameObject nextLife;
	public GameObject prevLife;
	public GameObject lifePrefab;
    // Start is called before the first frame update
    void Start()
    {
        float width = GetComponent<SpriteRenderer>().bounds.size.x;
        this.nextLifeDistance = width * 2f;
    }

    // Update is called once per frame
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
    }
}
