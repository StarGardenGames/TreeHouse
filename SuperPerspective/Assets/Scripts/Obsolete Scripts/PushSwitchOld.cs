using UnityEngine;
using System.Collections;

public class PushSwitchOld : MonoBehaviour {

	public Activatable[] triggers;//Activatable objects which this switch triggers
	
	bool pushed = false; //whether switch is currently pushed

	Color baseColor = Color.white;

    public Rect parentPlatform;
	Collider pusher = null;
	
	void Update(){
        RaycastHit hit;
		parentPlatform = PlayerController.instance.GetComponent<BoundObject>().GetBounds();
        float xx = transform.position.x, yy = transform.position.y, zz = transform.position.z;
        Bounds check = new Bounds(new Vector3(transform.position.x, transform.position.y, parentPlatform.center.y), new Vector3(2f, 1f, parentPlatform.height));
		if (GameStateManager.is2D() && check.Intersects(PlayerController.instance.gameObject.GetComponent<Collider>().bounds)) {
            EnterCollisionWithPlayer();
        } else if (Physics.Raycast(new Vector3(xx + 1f, yy - 0.5f, zz + 2f), -Vector3.forward + Vector3.up * 0.25f, out hit, 4f, LayerMask.NameToLayer("RaycastIgnore")) ||
            Physics.Raycast(new Vector3(xx, yy - 0.5f, zz + 2f), -Vector3.forward + Vector3.up * 0.25f, out hit, 4f, LayerMask.NameToLayer("RaycastIgnore")) ||
            Physics.Raycast(new Vector3(xx - 1f, yy - 0.5f, zz + 2f), -Vector3.forward + Vector3.up * 0.25f, out hit, 4f, LayerMask.NameToLayer("RaycastIgnore"))) {
		    if (!pushed)
			    EnterCollisionWithGeneral(hit.collider.gameObject);
		} else if (pushed) {
			ExitCollisionWithGeneral(null);
		}
        if (pushed) {
            if (baseColor == Color.white)
                baseColor = gameObject.GetComponent<Renderer>().material.color;
            gameObject.GetComponent<Renderer>().material.color = Color.white;
        } else {
            if (baseColor != Color.white)
                gameObject.GetComponent<Renderer>().material.color = baseColor;
        }
    }

    public void EnterCollisionWithPlayer()
    {
        EnterCollisionWithGeneral(null);
    }

    public void EnterCollisionWithGeneral(GameObject other){
		pushed = true;//becomes pushed when it collides with player
		//pusher = other.GetComponent<Collider> ();
		//pushed is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}
	
	public void ExitCollisionWithGeneral(GameObject other){
		pushed = false;//becomes pushed when it collides with player
		//pushed is also updated for all activatable objects
		foreach(Activatable o in triggers)
			o.setActivated(pushed);
	}
}
