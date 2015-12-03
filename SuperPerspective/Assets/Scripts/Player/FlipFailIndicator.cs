using UnityEngine;
using System.Collections;

public class FlipFailIndicator : MonoBehaviour {
	public static FlipFailIndicator instance;

	Transform perspCam;

	Collider overlappingBlock;

	float[] blinkDurations = new float[3]{.1f,.2f,.3f};
  int blinkFrame = 0;
	float blinkThresh = 0;
	float blinkTime = -1f;

	public void Start(){
		instance = this;
		this.GetComponent<Renderer>().enabled = false;
		perspCam = transform.parent.Find("CameraMounts").Find("2DCameraMount");
	}

	public void FixedUpdate(){
		if(blinkTime >= 0){
			blinkTime += Time.deltaTime;
		  if(blinkTime >= blinkThresh){
				toggleVisible();
				if(blinkFrame == blinkDurations.Length){
				  blinkTime = -1f;
				}else{
					blinkThresh+=blinkDurations[blinkFrame];
					blinkFrame++;
				}
			}
		}
	}

	private void toggleVisible(){
		bool vis = this.GetComponent<Renderer>().enabled;
		vis = !vis;
		this.GetComponent<Renderer>().enabled = vis;
	}

	public void blink(){
	  updateZPosition();
		bindToOverlap();
		initBlinkVars();
	}

	private void updateZPosition(){
		Vector3 pos = transform.position;
		pos.z = perspCam.transform.position.z + 1;
		transform.position = pos;
	}

	private void bindToOverlap(){
		Bounds myBounds = PlayerController.instance.GetComponent<Collider>().bounds;
		Bounds ovBounds = overlappingBlock.bounds;

		print(myBounds.min + " " + myBounds.max);
		print(ovBounds.min + " " + ovBounds.max);

		float minX = Mathf.Max(myBounds.min.x,ovBounds.min.x);
		float maxX = Mathf.Min(myBounds.max.x,ovBounds.max.x);
		float minY = Mathf.Max(myBounds.min.y,ovBounds.min.y);
		float maxY = Mathf.Min(myBounds.max.y,ovBounds.max.y);

		Vector3 newPos = new Vector3((minX + maxX)/2f,(minY + maxY)/2f,transform.position.z);
		Vector3 newScale = new Vector3((maxX - minX) / 10f, .1f,(maxY - minY) / 10f);

		transform.position = newPos;

		Vector3 parScale = transform.parent.transform.lossyScale;
		newScale.x /= parScale.x;
		newScale.y /= parScale.z; //not a typo, y/z is necessary due to the rotation on indicator
		newScale.z /= parScale.y;
		transform.localScale = newScale;
	}

	private void initBlinkVars(){
		blinkTime = 0;
		blinkFrame = 0;
		blinkThresh = 0;
	}

	public void setOverlappingBlock(Collider obj){
			overlappingBlock = obj;
	}
}
