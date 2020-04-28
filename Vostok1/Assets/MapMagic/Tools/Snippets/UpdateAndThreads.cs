using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Den.Tools
{
	[SelectionBase]
	[ExecuteInEditMode]
	public class UpdateAndThreads : MonoBehaviour 
	{
		public bool isEditor 
		{get{
			#if UNITY_EDITOR
				return 
					!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode; //if not playing
					//(UnityEditor.EditorWindow.focusedWindow != null && UnityEditor.EditorWindow.focusedWindow.GetType() == System.Type.GetType("UnityEditor.GameView,UnityEditor")) //if game view is focused
					//UnityEditor.SceneView.lastActiveSceneView == UnityEditor.EditorWindow.focusedWindow; //if scene view is focused
			#else
				return false;
			#endif
		}}

		public bool isSelected 
		{get{
			#if UNITY_EDITOR
				return UnityEditor.Selection.activeTransform == this.transform;
			#else
				return false;
			#endif
		}}

		#region Registering editor delegates
		
			#if UNITY_EDITOR
			public void OnEnable ()
			{
				//adding delegates
				UnityEditor.EditorApplication.update -= Update;	

				#if UNITY_2019_1_OR_NEWER
				UnityEditor.SceneView.duringSceneGui -= GetEditorControls; 	
				#else
				UnityEditor.SceneView.onSceneGUIDelegate -= GetEditorControls; 
				#endif
			
				if (isEditor) 
				{
					#if UNITY_2019_1_OR_NEWER
					UnityEditor.SceneView.duringSceneGui += GetEditorControls;
					#else
					UnityEditor.SceneView.onSceneGUIDelegate += GetEditorControls; 
					#endif

					UnityEditor.EditorApplication.update += Update;
				}
			}

			public void OnDisable ()
			{
				//removing delegates
				UnityEditor.EditorApplication.update -= Update;	

				#if UNITY_2019_1_OR_NEWER
				UnityEditor.SceneView.duringSceneGui -= GetEditorControls;
				#else
				UnityEditor.SceneView.onSceneGUIDelegate += GetEditorControls; 
				#endif
			}
			#endif

		/**/
		#endregion
		

		#region Controls and camera position 
			
			#if UNITY_EDITOR
			public void GetEditorControls (UnityEditor.SceneView sceneview)
			{
				if (!isEditor) return;

				//mousePos = Event.current.mousePosition;
				//if (Event.current.type == EventType.MouseDown) mouseButton = Event.current.button;
				//if (Event.current.rawType == EventType.MouseUp || !isSelected) mouseButton = -1; //resetting controls if not selected

				//camPos = UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;
				//aimRay = UnityEditor.SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePos); aimRay.origin = camPos;

				//camPos = transform.InverseTransformPoint(camPos); ???
			}
			#endif

			public void GetPlaymodeControls ()
			{
				//mousePos = Input.mousePosition;
				//mouseButton = Input.GetMouseButton(0) ? 0 : -1;
				//camPos = Camera.main.transform.position;
				//aimRay = Camera.main.ScreenPointToRay(mousePos); aimRay.origin = camPos;

				//camPos = transform.InverseTransformPoint(camPos); ???
			}

		/**/ 
		#endregion


		#region Thread Tools

			static public Queue<Action> actionQueue = new Queue<Action>();
			
			static public void StartThread (Action produce, Action consume)
			{
				ThreadStart threadStart = new ThreadStart( 
					delegate() 
					{ 
						produce(); 
						actionQueue.Enqueue( delegate() {consume();} );
					} );

				Thread thread = new Thread(threadStart);
				thread.IsBackground = true;
				thread.Start();
			}

			static public void StartThread (Action<object> produce, Action<object> consume, object obj)
			{
				ParameterizedThreadStart threadStart = new ParameterizedThreadStart( 
					delegate(object o) 
					{ 
						produce(o); 
						actionQueue.Enqueue( delegate() {consume(o);} );
					} );

				Thread thread = new Thread(threadStart);
				thread.IsBackground = true;
				thread.Start(obj);
			}

			//testing
			public bool testThread = false;
			void ProduceTestThread (object obj) { Debug.Log("This is written from thread"); }
			void ConsumeTestThread (object obj) { Debug.Log("And this is from main"); }

		/**/
		#endregion

		private void Update ()
		{
			//consuming threads
			while (actionQueue.Count != 0) 
				actionQueue.Dequeue()();
			
			//testing threads
			if (testThread) StartThread(ProduceTestThread, ConsumeTestThread, null);
			testThread=false;
		
			//getting playmode controls (editor controls done via scene view delegate)
			if (!isEditor) GetPlaymodeControls ();

			//Do all other update here
		}
	}
}
