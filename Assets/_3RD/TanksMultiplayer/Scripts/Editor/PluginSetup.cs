using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace TanksMP
{
    public class PluginSetup : EditorWindow
    {
        private static string packagesPath;

        private Packages selectedPackage = Packages.UnityNetworking;
        private enum Packages
        {
            UnityNetworking,
            Photon
        }


        [MenuItem("Window/Tanks Multiplayer/Network Setup")]
        static void Init()
        {
            packagesPath = "/Packages/";
            EditorWindow window = EditorWindow.GetWindowWithRect(typeof(PluginSetup), new Rect(0, 0, 360, 250), false, "Network Setup");

            var script = MonoScript.FromScriptableObject(window);
            string thisPath = AssetDatabase.GetAssetPath(script);
            packagesPath = thisPath.Replace("/PluginSetup.cs", packagesPath);
        }


        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tanks Multiplayer - Network Setup", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Please choose the network provider you would like to use:");

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            selectedPackage = (Packages)EditorGUILayout.EnumPopup(selectedPackage);

            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                switch (selectedPackage)
                {
                    case Packages.UnityNetworking:
                        Application.OpenURL("https://unity3d.com/services/multiplayer");
                        break;
                    case Packages.Photon:
                        Application.OpenURL("https://www.photonengine.com/en/Realtime");
                        break;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Import"))
            {                
				if(selectedPackage == Packages.Photon)
				{
					Debug.LogError("Tanks Multiplayer: Photon is not supported yet!");
					return;
				}
			
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                AssetDatabase.ImportPackage(packagesPath + selectedPackage.ToString() + ".unitypackage", true);
				this.Close();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Note:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("For a detailed comparison about features and pricing, please");
            EditorGUILayout.LabelField("refer to the official pages for UNET or Photon. The features");
            EditorGUILayout.LabelField("of this asset are the same across both multiplayer services.");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Please read the PDF documentation for further details.");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Support links: Window > Tanks Multiplayer > About.");
        }
    }
}