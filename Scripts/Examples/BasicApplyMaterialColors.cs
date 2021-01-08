using UnityEngine;

namespace ExClient.Examples {
	/// <summary> Basic example behaviour that can be attached to a displayable Model prefab to accept colors from server.
	/// If needed, custom logic can be provided inside the <see cref="SetColors(Ex.Display)"/> method in another behaviour, 
	/// so long as it accepts a single <see cref="Ex.Display"/> it will get called automatically by <see cref="DisplayColorsHook"/></summary>
	public class BasicApplyDisplayColors : MonoBehaviour {
		/// <summary> Flag to either share a single material (true) or use a new material per renderer(false). 
		/// Using per-renderer materials is required if individual materials are separately changed per script. </summary>
		public bool allUseSameMaterial = true;

		/// <summary> Called through <see cref="Component.SendMessage(string, object, SendMessageOptions)"/> when display colors are updated. </summary>
		/// <param name="display"> Information about what to display </param>
		public void SetColors(Ex.Display display) {
			var targets = GetComponentsInChildren<Renderer>(true);
			Material mat = null;
			foreach (var target in targets) {
				if (allUseSameMaterial) {
					if (mat == null) {
						mat = new Material(target.material);
						ApplyColor(mat, display);
					}
					target.sharedMaterial = mat;
				} else {
					ApplyColor(target.material, display);
				}
			}
		}

		static void ApplyColor(Material mat, Ex.Display display) {
			mat.color = display.color;
			ApplyConditional(mat, "Color2", display.color2);
			ApplyConditional(mat, "Color3", display.color3);
			ApplyConditional(mat, "Color4", display.color4);
		}

		static void ApplyConditional(Material mat, string name, Color color) {
			if (mat.HasProperty(name)) { mat.SetColor(name, color); }
		}
	}

}
