﻿using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Den.Tools
{

		public class PopupMenu : UnityEditor.PopupWindowContent
		{
			public class Item 
			{
				public const int separatorHeight = 6;
				public const int lineHeight = 20;

				public string name;
				public bool clicked;
				public bool disabled;
				public int priority;
				public Texture2D icon;  //currently doing nothing, just store data for custom draw
				public Color color;		//same
				public Action<Item,Rect> onDraw;
				public int offset;  //aka tab symbol

				public float width;
				public float height = lineHeight;
				public bool isSeparator;

				public List<Item> subItems = null;
				public bool sortSubItems = true;
			
				public Action onClick; //action called when subitem selected

				public int Count { get{ return subItems==null ? 0 : subItems.Count; } }
				public bool hasSubs { get{ return subItems!=null;} }

				public Item () { }
				public Item (string name, Action<Item,Rect> onDraw=null, Action onClick=null, bool disabled=false, int priority=0) 
				{ 
					this.name=name;  
					this.priority=priority; 
					this.onClick=onClick; 
					this.onDraw = onDraw;
					this.disabled=disabled;

					this.width = UnityEngine.GUI.skin.label.CalcSize( new GUIContent(name) ).x + 20;  //20 for chevron
				}

				public static Item Separator (int priority=0) { return new Item() { isSeparator=true, height=separatorHeight, disabled=true, priority=priority}; }

				public void SortSubItems () { subItems.Sort(Compare); }
				public static void SortItems (List<Item> items) { items.Sort(Compare); }
				public static int Compare (Item a, Item b)
				{
					if (a.priority != b.priority) return b.priority - a.priority;

					if (a.name==null || b.name==null) return 0;
					if (a.name.Length==0) return -1; if (b.name.Length==0) return 1;
					if ((int)(a.name[0]) < (int)(b.name[0])) return -1;
					else if ((int)(a.name[0]) == (int)(b.name[0])) return 0;
					else return 1;
				}

				public IEnumerable<Item> All(bool inSubItems=true)
				{
					int subItemsCount = subItems.Count;
					for (int i=0; i<subItems.Count; i++)
					{
						yield return subItems[i];

						if (subItems[i].subItems != null  && inSubItems)
							foreach(Item sub in subItems[i].All(true))
								yield return sub;
					}
				}

				public Item Find (string findName, bool inSubItems=true)
				{
					int subItemsCount = subItems.Count;
					for (int i=0; i<subItems.Count; i++)
					{
						if (subItems[i].name == findName)
							return subItems[i];
					}

					if (inSubItems)
					for (int i=0; i<subItems.Count; i++)
						if (subItems[i].subItems != null)
						{
							Item subFound = subItems[i].Find(findName, true);
							if (subFound != null) 
								return subFound;
						}
							
					return null;
				}
			}

			
			public int minWidth = 100;

			public const int verticalOffset = 5; //HACK: first 5 pixels of popup window could not be clicked in Unity
			public const int verticalOffsetTmp = 4;

			static GUIStyle blackLabel;

			static private Texture2D background;
			static private Texture2D highlight;
			static private Texture2D triangle;
			static private Texture2D separator;

			public List<Item> items;
			public bool sortItems = true;

			private Item lastItem;
			private System.DateTime lastTimestart;
			//private bool timeUsed = false;

			private Item expandedItem;

			private PopupMenu parent;

			private PopupMenu expandedWindow = null;
		
			//void CloseMenuIfNotFocused () { if (UnityEditor.EditorWindow.focusedWindow.GetType() != typeof(PopupMenu)) this.Close(); } 
			//void OnEnable () { UnityEditor.EditorApplication.update += CloseMenuIfNotFocused; }
			//void OnDisable () { UnityEditor.EditorApplication.update -= CloseMenuIfNotFocused; }




			/*static public PopupMenu DrawPopup (List<Item> items, Vector2 pos, bool closeAllOther=false, bool sort=true, int minWidth=0, PopupMenu parent=null)
			{
				if (sort) Item.SortItems(items);
				PopupMenu popupWindow = new PopupMenu();
				popupWindow.items = items;
				popupWindow.minWidth = minWidth;
				popupWindow.parent = parent;
				PopupWindow.Show(new Rect(pos.x,pos.y, minWidth, 0), popupWindow);
				return popupWindow;
			}*/

			public void Show (Vector2 pos)
			{
				if (sortItems) Item.SortItems(items);
				PopupWindow.Show(new Rect(pos.x,pos.y- verticalOffset, minWidth, 0), this);
			}

			public override Vector2 GetWindowSize() 
			{
				float height = 0;
				float width = 0;
				int count = items.Count;
				for (int i=0; i<count; i++)
				{
					height += items[i].height;
					if (items[i].width > width) width = items[i].width;
				}

				if (width != minWidth) width = minWidth;

				return new Vector2(width, height+ verticalOffsetTmp);
			}

			public Rect GetIconRect (Rect srcRect, Texture2D texture)
			{
				Vector2 center = srcRect.center;
				return new Rect(
					center.x - texture.width/2f,
					center.y - texture.height/2f,
					texture.width,
					texture.height);
			}

			public override void OnGUI(Rect rect)
			{
				//preparing textures
				if (background==null)
				{
					background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
					background.SetPixel(0, 0, new Color(0.98f, 0.98f, 0.98f));
					background.Apply();
				}
			
				if (highlight==null)
				{
					highlight = new Texture2D(1, 1, TextureFormat.RGBA32, false);
					highlight.SetPixel(0, 0, new Color(0.6f, 0.7f, 0.9f));
					highlight.Apply();
				}

				Vector2 size = GetWindowSize();
				Vector2 pos = new Vector2(0, verticalOffset);

				//background
				//if (Event.current.type == EventType.repaint) GUI.skin.box.Draw(fullRect, false, true, true, false);
				UnityEngine.GUI.DrawTexture(new Rect(pos, size), background, ScaleMode.StretchToFill);

				//list
				float currentHeight = verticalOffsetTmp;
				int itemsCount = items.Count;
				for (int i=0; i<itemsCount; i++)
				{
					Item currentItem = items[i];

					//rects
					Rect lineRect = new Rect(1, currentHeight+1, size.x-2, currentItem.height-2);
					currentHeight += currentItem.height;
					
					Rect offsetRect = new Rect(lineRect.x, lineRect.y, Item.lineHeight*currentItem.offset, lineRect.height);
					Rect labelRect = new Rect(lineRect.x+offsetRect.width+3, lineRect.y+1, lineRect.width-offsetRect.width-3, lineRect.height-1);

					//background
					bool highlighted = lineRect.Contains(Event.current.mousePosition);
					if (currentItem.disabled) highlighted = false;
					if (highlighted) UnityEngine.GUI.DrawTexture(lineRect, highlight);
					/*{
						//GUIStyle style = texturesCache.GetElementStyle(tex);
						//if (Event.current.type == EventType.Repaint) style.Draw(leftRect, false, false, false ,false);

						GUIStyle style = new GUIStyle();
						style.normal.background = highlight;
						style.border = new RectOffset(highlight.width/2, highlight.width/2, highlight.height/2, highlight.height/2);
						
						if (Event.current.type == EventType.Repaint) style.Draw(lineRect, false, false, false ,false);
					}*/

					//clicking
					bool clicked = Event.current.rawType == EventType.MouseUp && Event.current.button == 0;
					if (highlighted && clicked && currentItem.onClick != null)
					{
						currentItem.onClick();
						CloseRecursive();
						Event.current.Use();
					}

					//label
					UnityEditor.EditorGUI.BeginDisabledGroup(currentItem.disabled);
					//if (blackLabel == null) { blackLabel = new GUIStyle(UnityEditor.EditorStyles.label); blackLabel.normal.textColor = Color.black; }
					if (currentItem.onDraw != null)
						currentItem.onDraw(currentItem,lineRect);
					else
						EditorGUI.LabelField(labelRect, currentItem.name);
					UnityEditor.EditorGUI.EndDisabledGroup();

					//separator
					if (currentItem.isSeparator) 
					{
						if (currentItem.onDraw == null)
						{
							Rect separatorRect = new Rect(lineRect.x+3, lineRect.y, lineRect.width-6, 1);
							if (separator == null) separator = TextureExtensions.ColorTexture(2,2,new Color(0.3f, 0.3f, 0.3f, 1)); 
							UnityEngine.GUI.DrawTexture(separatorRect, separator, ScaleMode.ScaleAndCrop);
						}
						else
							currentItem.onDraw(currentItem, lineRect);
					}

					//chevron
					if (currentItem.hasSubs)
					{
						Rect rightRect = lineRect; rightRect.width = 10; rightRect.height = 10; 
							rightRect.x = lineRect.x + lineRect.width - rightRect.width; rightRect.y = lineRect.y + lineRect.height/2 - rightRect.height/2;
						//UnityEditor.EditorGUI.LabelField(rightRect, "\u25B6");
						if (triangle == null) triangle = Resources.Load("DPUI/Chevrons/SmallRight") as Texture2D; 
						UnityEngine.GUI.DrawTexture(GetIconRect(rightRect, triangle), triangle, ScaleMode.ScaleAndCrop);

						//opening subsmenus
						if (highlighted)
						{
							//starting timer on selected item change
							if (currentItem != lastItem)
							{
								lastTimestart = System.DateTime.Now;
								lastItem = currentItem;
							}

							//when holding for too long
							double highlightTime = (System.DateTime.Now-lastTimestart).TotalMilliseconds;
							if ((highlightTime > 300 && expandedItem != currentItem) || clicked) 
							{
								//re-opening expanded window
								if (expandedWindow != null && expandedWindow.editorWindow != null) expandedWindow.editorWindow.Close();
								
								expandedWindow = new PopupMenu() { 
									items = currentItem.subItems, 
									minWidth = minWidth,
									parent = this };
								expandedItem = currentItem;

								expandedWindow.Show(lineRect.max-new Vector2(0,currentItem.height));

								//if (currentItem.subItems != null) PopupWindow.Show(new Rect(lineRect.max-new Vector2(0,currentItem.height), Vector2.zero), expandedWindow);
							}
						}
					}
				}

				//#if (!UNITY_EDITOR_LINUX)
				this.editorWindow.Repaint();
				//#endif
			}

			public override void OnClose () { if (expandedWindow != null && expandedWindow.editorWindow != null) expandedWindow.editorWindow.Close(); }

			public void CloseRecursive ()
			{
				if (parent != null) parent.CloseRecursive();
				editorWindow.Close();
			}
		}

}