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
	
	int numMenuButtons = 4;
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
		
		//disable submenus
		setSaveMenuActive(false);
		setOptionMenuActive(false);
		for(int i = 0; i < numSaveSlots; i++)	
			setSlotMenuActive(i,false);
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
			if(item.Length == 5){
				selectedSlot = item.ToCharArray()[4]-'0'-1;
				for(int i = 0; i < numSaveSlots; i++)
					if(i == selectedSlot)
						setSlotMenuActive(i, true);
				else
					setSlotMenuActive(i, false);
				if(slotHasData[selectedSlot]){
					string name = saveSlots[selectedSlot].transform.GetChild(0).GetComponent<Text>().text;
					updateText(name + " is playing");
				}
			}else{
				Text t = saveSlots[selectedSlot].transform.GetChild(0).GetComponent<Text>();
				updateText(t.text+" is no more");
				t.text = name;
				t.text = defaultSaveName;
				slotHasData[selectedSlot] = false;
				setSlotMenuActive(selectedSlot,true);
				Text t2 = inputs[selectedSlot].transform.Find("Text").GetComponent<Text>();
				t2.text = "";
			}
		} else {
			switch (item) {
			case "Play":
				menuAlpha=.95f;
				break;
			case "LoadSave":
				setOptionMenuActive(false);
				setSaveMenuActive(true);
				break;
			case "Options":
				setSaveMenuActive(false);
				for(int i = 0; i < numSaveSlots; i++)
					setSlotMenuActive(i,false);
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
		slotHasData[selectedSlot] = true;
		Text t = saveSlots[selectedSlot].transform.GetChild(0).GetComponent<Text>();
		t.text = name;
		setSlotMenuActive(selectedSlot, true);
		updateText(name + " is playing");
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
	}
	
	void setSlotMenuActive(int index, bool status){
		if(slotHasData[index] || !status)
			resets[index].gameObject.SetActive(status);
		if(!slotHasData[index] || !status)
			inputs[index].gameObject.SetActive(status);
	}
	
	void updateText(string str){
		textAlpha = 3f;
		menuStatus.text = str;
	}
}
