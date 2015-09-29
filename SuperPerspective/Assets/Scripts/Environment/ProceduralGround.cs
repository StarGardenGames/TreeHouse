using UnityEngine;
using System.Collections;

public class ProceduralGround : MonoBehaviour {

	public GameObject[] generatableObjects;
	public int numToGenerate;
	public bool calculateAmountForMe, rotateX, rotateY, rotateZ, changeSize, evenDistribution, cleanUpIn2D;

	private GameObject[] renderedObjects;
	// Use this for initialization
	void Start () {
		//me = this
		Vector3 mePos = this.transform.position;
		MeshRenderer m = this.GetComponent<MeshRenderer>();
		Vector3 meSize = new Vector3(m.bounds.size.x, m.bounds.size.y, m.bounds.size.z);


		if(calculateAmountForMe){
			float avgSize = 5.0f;
			numToGenerate = (int)Mathf.Ceil((meSize.x * meSize.z)/avgSize);
		}
		renderedObjects = new GameObject[numToGenerate];

		for(int i=0;i<numToGenerate;i++){
			GameObject obj = getRandomObject();

			//random positions
			Vector3 newPos = new Vector3(mePos.x, mePos.y, mePos.z);
			newPos.y -= 0.25f;
			newPos.x += Random.Range(-meSize.x/2, meSize.x/2);
			newPos.z += Random.Range(-meSize.z/2, meSize.z/2);

			//random rotations
			Quaternion newRotation = Quaternion.identity;
			if(rotateX){
				newRotation.x = Random.rotation.x;
			}
			if(rotateY){
				newRotation.y = Random.rotation.y;
			}
			if(rotateZ){
				newRotation.z = Random.rotation.z;
			}

			//create
			GameObject newObj = Object.Instantiate(obj, newPos, newRotation) as GameObject;
			renderedObjects[i] = newObj;
			newObj.transform.parent = this.gameObject.transform;
		}
		//hide
		m.enabled = false;
	}

	GameObject getRandomObject(){
		int randomIndex = Random.Range(0, generatableObjects.Length);
		return generatableObjects[randomIndex];
	}
	
	// Update is called once per frame
	void Update () {
		if(cleanUpIn2D){
			if(!GameStateManager.is3D()){
				for(int i=0;i<Mathf.Ceil(renderedObjects.Length/2);i++){
					MeshRenderer r = renderedObjects[i].GetComponentInChildren<MeshRenderer>();
					r.enabled = false;
				}
			}else{
				for(int i=0;i<Mathf.Ceil(renderedObjects.Length);i++){
					MeshRenderer r = renderedObjects[i].GetComponentInChildren<MeshRenderer>();
					r.enabled = true;
				}
			}
		}
	}
}
