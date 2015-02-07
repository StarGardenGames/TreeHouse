using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour {
	
	public Canvas menu;

	Button[] saveSlots;
	Toggle[] options;
	InputField[] inputs;
	Button[] resets;
	Text menuStatus;
	
	int slotSelected = -1;
	
	int numMenuButtons = 3;
	int numSaveSlots = 3;
	int numOptions = 2;
	
	bool[] slotHasData;
	
	int selectedSlot = 1;
	
	float textAlpha = 0f;
	
	float menuAlpha = 1f;
	float prog = 0f;
	Vector3 startPos;
	Quaternion startRot;
	
	string defaultSaveName = "New Save";
	
	bool canvasDestroyed = false;
	
	// Use this for initialization
	void Start () {
		//init saveslots[]
		saveSlots = new Button[numSaveSlots];
		for (int i = 0; i < saveSlots.Length; i++) 
			saveSlots[i] = menu.transform.GetChild(i+numMenuButtons).GetComponent<Button>();
		
		//init options[]
		options = new Toggle[numOptions];
		for(int i = 0; i < options.Length; i++)
			options[i] = menu.transform.GetChild(i+numMenuButtons+numSaveSlots).GetComponent<Toggle>();
		
		//init inputs
		inputs = new InputField[numSaveSlots];
		for(int i = 0; i<numSaveSlots; i++)
			inputs[i] = menu.transform.GetChild(i+numMenuButtons+numSaveSlots+numOptions).GetComponent<InputField>();
		
		//init resets
		resets = new Button[numSaveSlots];
		for(int i = 0; i<numSaveSlots; i++)
			resets[i] = menu.transform.GetChild(i+numMenuButtons+(numSaveSlots*2)+numOptions).GetComponent<Button>();
		
		//init text
		menuStatus = menu.transform.GetChild(numMenuButtons+numSaveSlots*3+numOptions).GetComponent<Text>();
		
		//init data
		slotHasData = new bool[numSaveSlots];

		getSaveNames();

		//disable submenus
		setSaveMenuActive(false);
		setOptionMenuActive(false);
		for(int i = 0; i < numSaveSlots; i++){	
			inputs[i].gameObject.SetActive(false);
			resets[i].gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//ensure that saves remain censistant
		for(int i = 0; i < numSaveSlots; i++)
		if(inputs[i].IsActive() && resets[i].IsActive()){
			if(slotHasData[i])
				inputs[i].gameObject.SetActive(false);
			else{
				resets[i].gameObject.SetActive(false);
			}
		}
		
		//update text transparency
		if(textAlpha > 0)
			textAlpha -= .02f;
		Color c = menuStatus.color;
		c.a = textAlpha;
		menuStatus.color = c;
		//panning
		if(menuAlpha<1){
			//adjust menu alpha
			menuAlpha -=.02f;
			prog +=.01f;
			if(menuAlpha < 0)
				menu.enabled = false;
			menu.GetComponent<CanvasGroup>().alpha = menuAlpha;
		}
	}
	
	public void UIClicked(string item){
		if (item.Substring (0, 4).Equals ("Slot")) {
			selectedSlot = item.ToCharArray()[4]-'0'-1;
			if(item.Length == 5){
				if(slotHasData[selectedSlot]){
					SaveManager.instance.setSave(selectedSlot);
					string name = saveSlots[selectedSlot].transform.GetChild(0).GetComponent<Text>().text;
					updateText(name + " is playing");
					menuAlpha = .95f;
					CameraControl.instance.MountToPlayer();
					SaveManager.instance.loadSave();
				}else{
					for(int i = 0; i < numSaveSlots; i++)
						inputs[i].gameObject.SetActive(i == selectedSlot);
				}
			}else{
				//reset save
				SaveManager.instance.resetSave(selectedSlot);
				//update text
				Text t = saveSlots[selectedSlot].transform.GetChild(0).GetComponent<Text>();
				updateText(t.text+" is no more");
				t.text = defaultSaveName;
				//slot doesn't have data
				slotHasData[selectedSlot] = false;
				//reset button no longer active
				resets[selectedSlot].gameObject.SetActive(false);
				//input becomes
				Text t2 = inputs[selectedSlot].transform.Find("Text").GetComponent<Text>();
				t2.text = "";
			}
		} else {
			switch (item) {
			case "Play":
				setOptionMenuActive(false);
				setSaveMenuActive(true);
				break;
			case "Options":
				setSaveMenuActive(false);
				setOptionMenuActive(true);
				break;
			case "Quit":
				break;
			default:
				break;
			}
		}
	}
	
	public void SlotNameEntered(string name){
		if(name == "" || name == defaultSaveName)
			return;
		SaveManager.instance.setSaveName(selectedSlot,name);
		slotHasData[selectedSlot] = true;
		Text t = saveSlots[selectedSlot].transform.GetChild(0).GetComponent<Text>();
		t.text = name;
		inputs[selectedSlot].gameObject.SetActive(false);
		resets[selectedSlot].gameObject.SetActive(true);
		updateText(name + " is playing");
	}

	void getSaveNames(){
		for(int i = 0; i<3; i++){
			string name = SaveManager.instance.getSaveName(i);
			if(name!=""){
				Text t = saveSlots[i].transform.GetChild(0).GetComponent<Text>();
				t.text = name;
				slotHasData[i]=true;
			}
		}
	}

	void setOptionMenuActive(bool status){
		for(int i = 0; i < options.Length; i++)
			options[i].gameObject.SetActive(status);
	}
	
	void setSaveMenuActive(bool status){
		for(int i = 0; i < saveSlots.Length; i++)
			saveSlots[i].gameObject.SetActive(status);
		if(!status)
			selectedSlot = -1;
		for(int i = 0; i < 3; i++){
			resets[i].gameObject.SetActive(status && slotHasData[i]);
			if(status == false)
				inputs[i].gameObject.SetActive(status);
		}
	}
	
	void updateText(string str){
		/*textAlpha = 3f;
		menuStatus.text = str;*/
	}
}
