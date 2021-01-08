using UnityEngine;
using static GGUI;
using ExClient;

public class LandingGUI : GGUIBehaviour {

	ExDaemon exDaemon;
	float timer = 0;
	public override void OnEnable() {
		base.OnEnable();
	}
	public override void OnDisable() {
		base.OnDisable();
	}

	public void Awake() {
		exDaemon = GetComponent<ExDaemon>();
	}

	public override void Update() {
		base.Update();

		timer += Time.deltaTime;
	}

	public override void RenderGUI() {
		LoadSkin("Spacy");
		Text(new Rect(0, 0, 1, .5f), "Logged in !");
	}

}
