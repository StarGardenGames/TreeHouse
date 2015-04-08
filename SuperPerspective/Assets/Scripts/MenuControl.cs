using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour {
	
	//suppress warnings
	#pragma warning disable 414
	
	public Canvas menu;

	Button[] saveSlots;
	Toggle[] options;
	Button[] resets;
	Text menuStatus;
	
	int slotSelected = -1;
	
	int numMenuButtons = 3;
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
	
	string[] saveNames = {
		"Inductive Reasoner",
		"Dexterous Traveller",
		"Intuitive Thinker"
	};
	
	// Use this for initialization
	void Start () {
		int numSlots = SaveManager.instance.getNumSaveSlots();
		//init saveslots[]
		saveSlots = new Button[numSlots];
		for (int i = 0; i < saveSlots.Length; i++) 
			saveSlots[i] = menu.transform.GetChild(i+numMenuButtons).GetComponent<Button>();
		
		//init options[]
		options = new Toggle[numOptions];
		for(int i = 0; i < options.Length; i++)
			options[i] = menu.transform.GetChild(i+numMenuButtons+numSlots).GetComponent<Toggle>();
		
		//init resets
		resets = new Button[numSlots];
		for(int i = 0; i<numSlots; i++)
			resets[i] = menu.transform.GetChild(i+numMenuButtons+numSlots+numOptions).GetComponent<Button>();
		
		//init text
		menuStatus = menu.transform.GetChild(numMenuButtons+numSlots*2+numOptions).GetComponent<Text>();
		
		//init data
		slotHasData = new bool[SaveManager.instance.getNumSaveSlots()];

		getSaveNames();

		//disable submenus
		setSaveMenuActive(false);
		setOptionMenuActive(false);
		for(int i = 0; i < numSlots; i++){	
			resets[i].gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//ensure that saves remain censistant
		for(int i = 0; i < SaveManager.instance.getNumSaveSlots(); i++)
			if(resets[i].IsActive() && !slotHasData[i])
					resets[i].gameObject.SetActive(false);
			
		
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
					GameStateManager.instance.StartGame();
					SaveManager.instance.loadSave();
				}else{
					string name = saveNames[selectedSlot];
					SaveManager.instance.setSaveName(selectedSlot,name);
					slotHasData[selectedSlot] = true;
					Text t = saveSlots[selectedSlot].transform.GetChild(0).GetComponent<Text>();
					t.text = name;
					resets[selectedSlot].gameObject.SetActive(true);
					updateText(name + " is playing");
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
		//update visibility of save slots
		for(int i = 0; i < saveSlots.Length; i++)
			saveSlots[i].gameObject.SetActive(status);
		//no slots are selected if menu isn't active
		if(!status)
			selectedSlot = -1;
		//update visibility of reset buttons for slots that have data
		for(int i = 0; i < 3; i++)
			resets[i].gameObject.SetActive(status && slotHasData[i]);
	}
	
	void updateText(string str){
		/*textAlpha = 3f;
		menuStatus.text = str;*/
	}
}
