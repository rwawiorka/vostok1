/*
			public class GizmoIconUtility 
			///thx HereticS1xth (https://www.reddit.com/r/Unity3D/comments/3yq4we/custom_icon_for_scriptableobjects/)
			{
				[UnityEditor.Callbacks.DidReloadScripts]
				static GizmoIconUtility()
				{
					EditorApplication.projectWindowItemOnGUI = ItemOnGUI;
				}

				static void ItemOnGUI(string guid, Rect rect)
				{
					if (rect.width>rect.height) rect.width = rect.height;
					else rect.height = rect.width;

					float iconSize = 0.5f;
					if (rect.width < 24) iconSize = 0.7f;

					Rect subRect = new Rect(
						rect.x+rect.width*(1-iconSize),
						rect.y+rect.height*(1-iconSize),
						rect.width*iconSize,
						rect.height*iconSize);

					string path = AssetDatabase.GUIDToAssetPath(guid);
					System.Type type = AssetDatabase.GetMainAssetTypeAtPath(path);
					if (type == typeof(TextureArrayDecorator))
						GUI.DrawTexture(subRect, EditorGUIUtility.IconContent("Texture Icon").image);
				}
			}
*/