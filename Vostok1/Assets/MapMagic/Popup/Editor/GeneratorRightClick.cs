using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Profiling;
using UnityEditor;

using Den.Tools;
using Den.Tools.GUI;
using MapMagic.Core;
using MapMagic.Nodes;
using MapMagic.Nodes.GUI;

namespace MapMagic.Nodes.GUI
{
	public static class GeneratorRightClick
	{
		public static PopupMenu.Item GeneratorItems (Generator gen, Graph graph, int priority=3)
		{
			PopupMenu.Item genItems = new PopupMenu.Item("Generator");
			genItems.onDraw = RightClick.DrawItem;
			genItems.icon = RightClick.texturesCache.GetTexture("MapMagic/Popup/Generator");
			genItems.color = Color.gray;
			genItems.subItems = new List<PopupMenu.Item>();
			genItems.priority = priority;

			genItems.disabled = gen==null; 

			PopupMenu.Item enableItem = new PopupMenu.Item(gen==null||gen.enabled ? "Disable" : "Enable", onDraw:RightClick.DrawItem, priority:11) { icon = RightClick.texturesCache.GetTexture("MapMagic/Popup/Eye"), color = Color.gray };
			enableItem.onClick = ()=> 
			{
				gen.enabled = !gen.enabled;
				GraphWindow.RefreshMapMagic(gen);
			};
			genItems.subItems.Add(enableItem);
				
			//genItems.subItems.Add( new PopupMenu.Item("Export", onDraw:RightClick.DrawItem, priority:10) { icon = RightClick.texturesCache.GetTexture("MapMagic/Popup/Export"), color = Color.gray } );
			//genItems.subItems.Add( new PopupMenu.Item("Import", onDraw:RightClick.DrawItem, priority:9) { icon = RightClick.texturesCache.GetTexture("MapMagic/Popup/Import"), color = Color.gray } );
			genItems.subItems.Add( new PopupMenu.Item("Duplicate", onDraw:RightClick.DrawItem, priority:8) { icon = RightClick.texturesCache.GetTexture("MapMagic/Popup/Duplicate"), color = Color.gray } );
			genItems.subItems.Add( new PopupMenu.Item("Update", onDraw:RightClick.DrawItem, priority:7) { icon = RightClick.texturesCache.GetTexture("MapMagic/Popup/Update"), color = Color.gray } );
			genItems.subItems.Add( new PopupMenu.Item("Reset", onDraw:RightClick.DrawItem, priority:4) { icon = RightClick.texturesCache.GetTexture("MapMagic/Popup/Reset"), color = Color.gray } );
				
			/*PopupMenu.Item testItem = new PopupMenu.Item("Create Test", onDraw:RightClick.DrawItem, priority:5);
			testItem.icon = RightClick.texturesCache.GetTexture("MapMagic/Popup/Export");
			testItem.color = Color.gray;
			testItem.onClick = ()=> GeneratorsTester.CreateTestCase(gen, GraphWindow.current.mapMagic.PreviewData);
			genItems.subItems.Add(testItem);*/

			PopupMenu.Item removeItem = new PopupMenu.Item("Remove", onDraw:RightClick.DrawItem, priority:5);
			removeItem.icon = RightClick.texturesCache.GetTexture("MapMagic/Popup/Remove");
			removeItem.color = Color.gray;
			removeItem.onClick = ()=> RemoveGenerator(gen, graph, GraphWindow.current.selected); 
			genItems.subItems.Add(removeItem);

			PopupMenu.Item unlinkItem = new PopupMenu.Item("Unlink", onDraw:RightClick.DrawItem, priority:6);
			unlinkItem.icon = RightClick.texturesCache.GetTexture("MapMagic/Popup/Unlink");
			unlinkItem.color = Color.gray;
			unlinkItem.onClick = ()=> 
			{
				graph.UnlinkGenerator(gen);
				GraphWindow.RefreshMapMagic(gen);
				//undo
			};
			genItems.subItems.Add(unlinkItem);
				

			return genItems;
		}


		public static void RemoveGenerator (Generator gen, Graph graph, HashSet<Generator> selected)
		{
			if (gen is IOutputGenerator)
			{
				if (EditorUtility.DisplayDialog("Purge Output", "Would you like to clear generated results on the terrain as well?", "Clear", "Keep"))
					GraphWindow.current.mapMagic.Purge((IOutputGenerator)gen);
			}

			GraphWindow.RecordCompleteUndo();

			//removing all of the selected gens (if gen is one of them)
			if (selected!=null && selected.Contains(gen))
				foreach (Generator sgen in selected)
					if (sgen != gen) //to avoid removing twice
						graph.Remove(sgen);

			graph.ThroughLink(gen); //trying to maintain connections between generators
			graph.Remove(gen);
			GraphWindow.RefreshMapMagic();

			//graph.RecordUndo(graph.Remove, gen);
		}

	}
}