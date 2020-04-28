/*
			[MenuItem("Assets/Create/Texture Array", priority = 202)]
			static void MenuCreatePostProcessingProfile(MenuCommand menuCommand)
			{
				var icon = EditorGUIUtility.FindTexture("ScriptableObject Icon"); //https://gist.github.com/MattRix/c1f7840ae2419d8eb2ec0695448d4321
				ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
					0, 
					ScriptableObject.CreateInstance<TmpCallbackReciever>(), 
					"TextureArray.asset", 
					icon, 
					null);

				
			}

			class TmpCallbackReciever : UnityEditor.ProjectWindowCallback.EndNameEditAction
			{
				public override void Action(int instanceId, string pathName, string resourceFile)
				{
					TextureArrayDecorator tad = Default();
					tad.name = System.IO.Path.GetFileName(pathName);
					AssetDatabase.CreateAsset(tad, pathName);

					ProjectWindowUtil.ShowCreatedAsset(tad);
				}
			}

			static TextureArrayDecorator Default ()
			{
				TextureArrayDecorator tad = ScriptableObject.CreateInstance<TextureArrayDecorator>();
				tad.texArr = new Texture2DArray(128, 128, 1, TextureFormat.ARGB32, true);
				tad.layers = new TextureArrayDecorator.Layer[] { new TextureArrayDecorator.Layer() };
				
				return tad;
			}

*/