using static GGUI;
using ExClient;
using UnityEngine;

namespace ExClient.Examples {

	public class ConnectingGUI : GGUIBehaviour {

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

		public void OnExConnected() {
			Debug.Log("Got OnExConnected event");
			rebuildOnUpdate = true;
		}

		public override void RenderGUI() {
			LoadSkin("Spacy");
			if (exDaemon.client == null) {
				Text(new Rect(0, 0, 1, .5f), "Connecting...");
			} else {

				SwitchTo<LoginGUI>();
			}




		}
	}
}
