﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Profiling;

using Den.Tools;
using Den.Tools.Matrices;
using Den.Tools.GUI;
using MapMagic.Core;
using MapMagic.Products;
using MapMagic.Nodes.GUI;

namespace MapMagic.Nodes.GUI
{
	public static class GeneratorEditors
	{
		[Draw.Editor(typeof(MatrixGenerators.Curve200))]
		public static void DrawCurve (MatrixGenerators.Curve200 gen)
		{
			using (Cell.LinePx(GeneratorDraw.nodeWidth)) //square cell
				using (Timer.Start("DrawCurve"))
			{
				Draw.Rect(new Color(1,1,1,0.5f)); //background

				using (Cell.Padded(5))
					CurveDraw.DrawCurve(gen.curve, gen.histogram);
			}
		}


		[Draw.Editor(typeof(MatrixGenerators.Levels200))]
		public static void DrawLevels (MatrixGenerators.Levels200 gen)
		{
			using (Cell.LinePx( (GeneratorDraw.nodeWidth-10)/2 )) //square internal grid cell
				using (Timer.Start("DrawLevels"))
			{
				Draw.Rect(new Color(1,1,1,0.5f));

				using (Cell.Padded(1,1,7,0))
					LevelsDraw.DrawLevels(ref gen.inMin, ref gen.inMax, ref gen.gamma, ref gen.outMin, ref gen.outMax, gen.histogram);
			}

			using (Cell.LineStd)
			{
				if (!gen.guiParams)
					Draw.Rect(new Color(1,1,1,0.5f));

				using (new Draw.FoldoutGroup(ref gen.guiParams, "Parameters", isLeft:true))
					if (gen.guiParams)
					{
						using (Cell.LineStd) Draw.Field(ref gen.inMin, "In Low");
						using (Cell.LineStd) Draw.Field(ref gen.gamma, "Gamma");
						using (Cell.LineStd) Draw.Field(ref gen.inMax, "In High");

						Cell.EmptyLinePx(5);
												
						using (Cell.LineStd) Draw.Field(ref gen.outMin, "Out Low");
						using (Cell.LineStd) Draw.Field(ref gen.outMax, "Out High");
					}
			}
		}


		[Draw.Editor(typeof(MatrixGenerators.Selector200))]
		public static void DrawSelector (MatrixGenerators.Selector200 gen)
		{
			using (Cell.Padded(1,1,0,0)) 
			{
				using (Cell.LineStd) Draw.Field(ref gen.rangeDet, "Set Range");
				using (Cell.LineStd) Draw.Field(ref gen.units, "Units");

				if (gen.rangeDet == MatrixGenerators.Selector200.RangeDet.MinMax)
				{
					using (Cell.LineStd) Draw.Field(ref gen.from, "From");
					using (Cell.LineStd) Draw.Field(ref gen.to, "To");
				}
				else
				{
					float from = (gen.from.x + gen.from.y)/2;
					float to = (gen.to.x + gen.to.y)/2;
					float transition = (gen.from.y - gen.from.x);

					using (Cell.LineStd) Draw.Field(ref from, "From");
					using (Cell.LineStd) Draw.Field(ref to, "To");
					using (Cell.LineStd) Draw.Field(ref transition, "Transition");

					gen.from.x = from-transition/2;
					gen.from.y = from+transition/2;
					gen.to.x = to-transition/2;
					gen.to.y = to+transition/2;
				}
			}
		}


		[Draw.Editor(typeof(MatrixGenerators.HeightOutput200))]
		public static void HeightOutputEditor (MatrixGenerators.HeightOutput200 heightOut)
		{
			using (Cell.Padded(1,1,0,0))
			{
				using (Cell.LinePx(0))
				{
					Cell.current.fieldWidth = 0.4f;

					if (GraphWindow.current.mapMagic != null)
						using (Cell.LineStd) GeneratorDraw.DrawGlobalVar(ref GraphWindow.current.mapMagic.globals.height, "Height");

					if (GraphWindow.current.mapMagic != null)
						using (Cell.LineStd) GeneratorDraw.DrawGlobalVar(ref GraphWindow.current.mapMagic.globals.heightInterpolation, "Interpolate");

					using (Cell.LineStd) Draw.Field(ref heightOut.outputLevel, "Out Level");

					if (Cell.current.valChanged)
						GraphWindow.RefreshMapMagic(heightOut);
				}
			}
		}


		[Draw.Editor(typeof(MatrixGenerators.UnityCurve200))]
		public static void CurveGeneratorEditor (MatrixGenerators.UnityCurve200 gen)
		{
			using (Cell.LinePx(GeneratorDraw.nodeWidth+4)) //don't really know why 4
				using (Cell.Padded(5))
					Draw.AnimationCurve(gen.curve);
		}


		[Draw.Editor(typeof(MatrixGenerators.Blend200.Layer))]
		public static void DrawBlendLayer (MatrixGenerators.Blend200.Layer layer, object[] arguments)
		{
			int num = (int)arguments[0];
			MatrixGenerators.Blend200 gen = (MatrixGenerators.Blend200)arguments[1];

			Cell.EmptyLinePx(2);

			using (Cell.LineStd)
			{
				using (Cell.RowPx(0)) 
					GeneratorDraw.DrawInlet(layer.inlet, gen);
				Cell.EmptyRowPx(10);

				using (Cell.RowPx(20)) Draw.Icon(UI.current.textures.GetTexture("DPUI/Icons/Layer"));

				if (!layer.guiExpanded)
				{
					using (Cell.Row) layer.algorithm = Draw.Field(layer.algorithm);
					using (Cell.RowPx(20)) layer.guiExpanded = Draw.LayerChevron(layer.guiExpanded);
				}

				else
				{
					using (Cell.Row) 
					{
						using (Cell.LineStd) layer.algorithm = Draw.Field(layer.algorithm);
						using (Cell.LineStd) Draw.FieldDragIcon(ref layer.opacity, UI.current.textures.GetTexture("DPUI/Icons/Opacity"));
					}

					using (Cell.RowPx(20))
						using (Cell.LineStd) layer.guiExpanded = Draw.LayerChevron(layer.guiExpanded);
				}
				
				/*using (Cell.RowPx(35)) 
				{
					//Draw.Field(ref layer.opacity);
					Draw.FieldDragIcon(ref layer.opacity, UI.current.textures.GetTexture("DPUI/Icons/Opacity"));
				}*/

				Cell.EmptyRowPx(3);
			}


			Cell.EmptyLinePx(2);
		}


		[Draw.Editor(typeof(MatrixGenerators.Normalize200.NormalizeLayer))]
		public static void DrawNormalizeLayer (MatrixGenerators.Normalize200.NormalizeLayer layer, object[] arguments)
		{
			int num = (int)arguments[0];
			MatrixGenerators.Normalize200 gen = (MatrixGenerators.Normalize200)arguments[1];

			if (layer == null) return;

			using (Cell.LinePx(20))
			{
				if (num!=0) 
					using (Cell.RowPx(0)) GeneratorDraw.DrawInlet(layer, gen);
				else 
					//disconnecting last layer inlet
					if (GraphWindow.current.graph.IsLinked(layer))
						GraphWindow.current.graph.UnlinkInlet(layer);

				Cell.EmptyRowPx(10);

				using (Cell.RowPx(80))
				{
					if (num==0) Draw.Label("Background");
					else Draw.Label("Layer " + num);
				}

				using (Cell.RowPx(10)) Draw.Icon(UI.current.textures.GetTexture("DPUI/Icons/Opacity"));
				using (Cell.Row) layer.Opacity = Draw.Field(layer.Opacity);

				Cell.EmptyRowPx(10);
				using (Cell.RowPx(0)) GeneratorDraw.DrawOutlet(layer);
			}
		}


		[Draw.Editor(typeof(MatrixGenerators.GrassOutput200))]
		public static void DrawGrassOutput (MatrixGenerators.GrassOutput200 grassOut)
		{
			using (Cell.Padded(1,1,0,0)) 
			{
				//Cell.current.margins = new Padding(4,4,4,4);

				using (Cell.LineStd) Draw.Field(ref grassOut.renderMode, "Mode");

				if (grassOut.renderMode == MatrixGenerators.GrassOutput200.GrassRenderMode.MeshUnlit || grassOut.renderMode == MatrixGenerators.GrassOutput200.GrassRenderMode.MeshVertexLit)
				{
					using (Cell.LineStd) grassOut.prototype.prototype = Draw.Field(grassOut.prototype.prototype, "Object");
					grassOut.prototype.prototypeTexture = null; //otherwise this texture will be included to build even if not displayed
					grassOut.prototype.usePrototypeMesh = true;
				}
				else
				{
					using (Cell.LineStd) grassOut.prototype.prototypeTexture = Draw.Field(grassOut.prototype.prototypeTexture, "Texture");
					grassOut.prototype.prototype = null; //otherwise this object will be included to build even if not displayed
					grassOut.prototype.usePrototypeMesh = false;
				}
				switch (grassOut.renderMode)
				{
					case MatrixGenerators.GrassOutput200.GrassRenderMode.Grass: grassOut.prototype.renderMode = DetailRenderMode.Grass; break;
					case MatrixGenerators.GrassOutput200.GrassRenderMode.Billboard: grassOut.prototype.renderMode = DetailRenderMode.GrassBillboard; break;
					case MatrixGenerators.GrassOutput200.GrassRenderMode.MeshVertexLit: grassOut.prototype.renderMode = DetailRenderMode.VertexLit; break;
					case MatrixGenerators.GrassOutput200.GrassRenderMode.MeshUnlit: grassOut.prototype.renderMode = DetailRenderMode.Grass; break;
				}

				using (Cell.LineStd) Draw.Field(ref grassOut.density, "Density");
				using (Cell.LineStd) grassOut.prototype.dryColor = Draw.Field(grassOut.prototype.dryColor, "Dry");
				using (Cell.LineStd) grassOut.prototype.healthyColor = Draw.Field(grassOut.prototype.healthyColor, "Healthy");

				Vector2 temp = new Vector2(grassOut.prototype.minWidth, grassOut.prototype.maxWidth);
				using (Cell.LineStd) Draw.Field(ref temp, "Width", xName:"Min", yName:"Max", xyWidth:25);
				grassOut.prototype.minWidth = temp.x; grassOut.prototype.maxWidth = temp.y;

				temp = new UnityEngine.Vector2(grassOut.prototype.minHeight, grassOut.prototype.maxHeight);
				using (Cell.LineStd) Draw.Field(ref temp, "Height", xName:"Min", yName:"Max", xyWidth:25);
				grassOut.prototype.minHeight = temp.x; grassOut.prototype.maxHeight = temp.y;

				using (Cell.LineStd) grassOut.prototype.noiseSpread = (float)Draw.Field(grassOut.prototype.noiseSpread, "Noise");
			}
		}


		[Draw.Editor(typeof(MatrixGenerators.TexturesOutput200.TextureLayer))]
		public static void DrawTextureLayer (MatrixGenerators.TexturesOutput200.TextureLayer layer, object[] arguments)
		/// TexturesOutput.TextureLayer  Special Editor
		{
			MatrixGenerators.TexturesOutput200 texOut = (MatrixGenerators.TexturesOutput200)arguments[1];
			int num = (int)arguments[0];
			if (layer == null) return;

			Cell.EmptyLinePx(3);
			using (Cell.LinePx(28))
			{
				if (num!=0) 
					using (Cell.RowPx(0)) GeneratorDraw.DrawInlet(layer, texOut);
				else 
					//disconnecting last layer inlet
					if (GraphWindow.current.graph.IsLinked(layer))
						GraphWindow.current.graph.UnlinkInlet(layer);
				
				Cell.EmptyRowPx(10);
					
				Texture2D tex = layer.prototype!=null ? layer.prototype.diffuseTexture : UI.current.textures.GetTexture("DPUI/Backgrounds/Empty");
				using (Cell.RowPx(28)) Draw.TextureIcon(tex); 

				using (Cell.Row)
				{
					Cell.current.trackChange = false;
					Draw.EditableLabel(ref layer.name);
				}

				using (Cell.RowPx(20)) 
				{
					Cell.current.trackChange = false;
					Draw.LayerChevron(num, ref texOut.guiExpanded);
				}

				Cell.EmptyRowPx(10);
				using (Cell.RowPx(0)) GeneratorDraw.DrawOutlet(layer);
			}
			Cell.EmptyLinePx(2);

			if (texOut.guiExpanded == num)
			using (Cell.Line)
			{
				Cell.EmptyRowPx(2);

				using (Cell.Row)
				{
					using (Cell.LinePx(0))
						using (Cell.Padded(1,0,0,0))
					{
						//using (Cell.LineStd) layer.Opacity = Draw.Field(layer.Opacity, "Opacity");
						//using (Cell.LineStd) Draw.ObjectField(ref layer.prototype, "Layer");
						
						Draw.Class(layer, "Layer", addFieldsToCellObjs:true);
						//this will add terrainlayer to exposed
					}
				
					if (layer.prototype != null)
					{
						Cell.EmptyLinePx(2);

						using (Cell.LineStd) 
							using (new Draw.FoldoutGroup(ref layer.guiProperties, "Properties"))
								if (layer.guiProperties)
						{
							//textures
							using (Cell.LineStd) 
							{
								Texture2D tex = layer.prototype.diffuseTexture;
								Draw.Field(ref tex, "Diffuse");
								if (Cell.current.valChanged)
								{
									if (layer.prototype.diffuseTexture.name == "WrColorPlaceholder2x2")
										GameObject.DestroyImmediate(layer.prototype.diffuseTexture); // removing temporary color texture if assigned
									layer.prototype.diffuseTexture = tex;
								}
							}

							using (Cell.LineStd) 
							{
								Texture2D tex = layer.prototype.normalMapTexture;
								Draw.Field(ref tex, "Normal");
								if (Cell.current.valChanged)
									layer.prototype.normalMapTexture = tex;
							}

							using (Cell.LineStd) 
							{
									Texture2D tex = layer.prototype.maskMapTexture;
									Draw.Field(ref tex, "Mask");
									if (Cell.current.valChanged)
										layer.prototype.maskMapTexture = tex;
							}

							//color (after texture)
							if (layer.prototype.diffuseTexture == null) 
							{
								layer.prototype.diffuseTexture = TextureExtensions.ColorTexture(2,2,layer.color);
								layer.prototype.diffuseTexture.name = "WrColorPlaceholder2x2";
							}

							if (layer.prototype.diffuseTexture.name == "WrColorPlaceholder2x2")
							{
								using (Cell.LineStd)
								{
									using (Cell.LineStd) Draw.Field(ref layer.color, "Color");
									if (Cell.current.valChanged) layer.prototype.diffuseTexture.Colorize(layer.color);
								}
							}


							using (Cell.LineStd) layer.prototype.specular = Draw.Field(layer.prototype.specular, "Specular");
							using (Cell.LineStd) layer.prototype.smoothness = Draw.Field(layer.prototype.smoothness, "Smooth");
							using (Cell.LineStd) layer.prototype.metallic = Draw.Field(layer.prototype.metallic, "Metallic");
							using (Cell.LineStd) layer.prototype.normalScale = Draw.Field(layer.prototype.normalScale, "N. Scale");
						}
				
						using (Cell.LineStd) 
							using (new Draw.FoldoutGroup(ref layer.guiTileSettings, "Tile Settings"))
								if (layer.guiTileSettings)
						{
							using (Cell.LineStd) layer.prototype.tileSize = Draw.Field(layer.prototype.tileSize, "Size");
							using (Cell.LineStd) layer.prototype.tileOffset = Draw.Field(layer.prototype.tileOffset, "Offset");
						}

						if (layer.guiTileSettings)
							Cell.EmptyLinePx(3);
					}

					
				}

				/*using (UI.FoldoutGroup(ref layer.guiRemapping, "Remapping", inspectorOffset:0, margins:0))
				if (layer.guiTileSettings)
				{
					using (Cell.LineStd)
					{
						Draw.Label("Red", cell:UI.Empty(Size.row));
						layer.prototype.diffuseRemapMin.x = Draw.Field(layer.prototype.diffuseRemapMin.x, cell:UI.Empty(Size.row));
					}
				}*/

				Cell.EmptyRowPx(2);
			}
		}


		[Draw.Editor(typeof(MatrixGenerators.CustomShaderOutput200))]
		public static void DrawCustomShaderOutput (MatrixGenerators.CustomShaderOutput200 gen) 
		{ 
			MatrixGenerators.CustomShaderOutput200 cso = (MatrixGenerators.CustomShaderOutput200)gen;
			string[] controlTextureNames = MatrixGenerators.CustomShaderOutput200.controlTextureNames;
			string[] controlTextureChannelNames = MatrixGenerators.CustomShaderOutput200.controlTextureNames;

			int texturesCount = MatrixGenerators.CustomShaderOutput200.controlTextureNames.Length;
			using (Cell.LineStd) Draw.Field(ref texturesCount, "Textures Count");
			if (texturesCount != MatrixGenerators.CustomShaderOutput200.controlTextureNames.Length)
				ArrayTools.Resize(ref controlTextureNames, texturesCount, i=> "_ControlTexture"+i);

			using (Cell.LineStd) Draw.Label("Texture Names:");
			for (int i=0; i<controlTextureNames.Length; i++)
				using (Cell.LinePx(20)) 
				{
					Cell.current.fieldWidth = 0.9f;
					Draw.Field(ref controlTextureNames[i], i+":");
				}

			if (controlTextureNames==null || controlTextureChannelNames.Length!=controlTextureNames.Length*4)
				controlTextureChannelNames = new string[controlTextureNames.Length*4];
			for (int i=0; i<controlTextureNames.Length; i++)
			{
				controlTextureChannelNames[i*4] = controlTextureNames[i] + " R";
				controlTextureChannelNames[i*4+1] = controlTextureNames[i] + " G";
				controlTextureChannelNames[i*4+2] = controlTextureNames[i] + " B";
				controlTextureChannelNames[i*4+3] = controlTextureNames[i] + " A";
			}

			//using (Cell.Line)
			//	DrawCustomMaterialWarning();
		}


		[Draw.Editor(typeof(MatrixGenerators.CustomShaderOutput200.CustomShaderLayer))]
		public static void DrawCustomShaderLayer (MatrixGenerators.CustomShaderOutput200.CustomShaderLayer layer, object[] arguments)
		{
			MatrixGenerators.CustomShaderOutput200 texturesOut = (MatrixGenerators.CustomShaderOutput200)arguments[1];
			int num = (int)arguments[0];
			if (layer == null) return;

			using (Cell.LinePx(32))
			{
				if (num!=0) 
					using (Cell.RowPx(0)) GeneratorDraw.DrawInlet(layer, texturesOut);
				else 
					//disconnecting last layer inlet
					if (GraphWindow.current.graph.IsLinked(layer))
						GraphWindow.current.graph.UnlinkInlet(layer);
				
				Cell.EmptyRowPx(10);

				using (Cell.Row) Draw.PopupSelector(ref layer.channelNum, MatrixGenerators.CustomShaderOutput200.controlTextureNames, null);

				Cell.EmptyRowPx(10);
				using (Cell.RowPx(0)) GeneratorDraw.DrawOutlet(layer);
			}
		}


		/*public static void DrawMicroSplatLayer (LayersGenerator.Layer target, Graph graph, Generator gen, int num)
		{
			MicroSplatOutput.MicroSplatLayer layer = (MicroSplatOutput.MicroSplatLayer)target;
			MicroSplatOutput texturesOut = (MicroSplatOutput)gen;
			if (layer == null) return;

			using (Cell.LinePx(32)))
			{
				Cell.current.margins = new Padding(0,0,0,1); //1-pixel more padding from the bottom since layers are 1 pixel overlayed

				if (num!=0) GeneratorUI.DrawInlet(layer.inlet, cell:Cell.RowPx(0));
				else layer.inlet.link = null; //disconnecting last layer inlet just in case
				UI.Empty(Size.RowPixels(10));

				UI.PopupSelector(ref layer.channelNum, CustomShaderOutput.controlTextureChannelNames, null, cell:UI.Empty(Size.row));

				UI.Empty(Size.RowPixels(10));
				GeneratorUI.DrawOutlet(layer, Cell.EmptyRowPx(0););
			}
		}*/


		[Draw.Editor(typeof(MatrixGenerators.RTPOutput200))]
		public static void DrawRTP (MatrixGenerators.RTPOutput200 gen) 
		{ 
			#if RTP
			UpdateMaterial();
			rtp = MapMagic.instance.GetComponent<ReliefTerrain>();

			/*#if CTS_PRESENT
			ctsProfile = (CTS.CTSProfile)Draw.Field(ctsProfile, "CTS Profile", type:typeof(CTS.CTSProfile),
				onChange:np=>CTS_UpdateShader((CTS.CTSProfile)np, MapMagic.instance.terrainSettings.material) );

			if (Draw.Button(false, "Update Shader"))
				CTS_UpdateShader(ctsProfile, MapMagic.instance.terrainSettings.material);

			//populating texture names
			*/

			if (rtp != null)
			{
				if (textureNames==null || textureNames.Length!=rtp.globalSettingsHolder.numLayers) textureNames = new string[rtp.globalSettingsHolder.numLayers];
				textureNames.Process(i=>rtp.globalSettingsHolder.splats[i]!=null ? rtp.globalSettingsHolder.splats[i].name : null); //nb linq?
			}

			using (Cell.Line)
			{
				Cell.current.margins = new Padding(4);
				DrawCustomMaterialWarning(MapMagic.instance.terrainSettings);
				DrawRTPComponentWarning(MapMagic.instance.terrainSettings);
			}
				
			#else
			using (Cell.LinePx(36)) Draw.Label("RTP is not installed or RTP \ncompatibility is disabled");
			#endif

		}


		[Draw.Editor(typeof(MatrixGenerators.RTPOutput200.RTPLayer))]
		public static void DrawRTPLayer (MatrixGenerators.RTPOutput200.RTPLayer layer, object[] arguments)
		{
			int num = (int)arguments[0];
			MatrixGenerators.RTPOutput200 gen = (MatrixGenerators.RTPOutput200)arguments[1];
			if (layer == null) return;

			using (Cell.LinePx(32))
			{
				//Cell.current.margins = new Padding(0,0,0,1); //1-pixel more padding from the bottom since layers are 1 pixel overlayed

				if (num!=0) 
					using (Cell.RowPx(0)) GeneratorDraw.DrawInlet(layer, gen);
				else 
					//disconnecting last layer inlet
					if (GraphWindow.current.graph.IsLinked(layer))
						GraphWindow.current.graph.UnlinkInlet(layer);

				Cell.EmptyRowPx(10);

				//icon
				#if RTP
				Texture2D icon = null;
				if (rtp != null)
				{
					if (layer.channelNum < rtp.globalSettingsHolder.splats.Length)
						icon = rtp.globalSettingsHolder.splats[layer.channelNum];
				}
				Draw.TextureIcon(icon, cell:UI.Empty(Size.RowPixels(32))); 

				//channel selector
				UI.Empty(Size.RowPixels(3));
				UI.PopupSelector(ref layer.channelNum, textureNames, null);
				#else
				using (Cell.LineStd) Draw.Field(ref layer.channelNum, "Channel");
				#endif

				Cell.EmptyRowPx(10);
				using (Cell.RowPx(0)) GeneratorDraw.DrawOutlet(layer);
			}
		}


		[Draw.Editor(typeof(Placeholders.InletOutletPlaceholder))]
		[Draw.Editor(typeof(Placeholders.InletPlaceholder))]
		[Draw.Editor(typeof(Placeholders.OutletPlaceholder))]
		public static void DrawPlaceholder (Placeholders.GenericPlaceholder placeholder)
		{
			using (Cell.LinePx(80))
				Draw.Helpbox ("Generator type not found. It might be a custom generator, or a generator from the package that has not been installed.");
		}


		/*[Draw.Editor(typeof(Placeholders.InletOutletPlaceholder), cat="Header")]
		public static void DrawPlaceholderHeader (Placeholders.GenericPlaceholder placeholder)
		{
			using (Cell.LinePx(0))
			{
				using (Cell.Row)
				{
					foreach (IInlet<object> inlet in placeholder.inlets)
					{
						if (inlet == null) continue;
						using (Cell.LineStd)
						{
							using (Cell.RowPx(0)) GeneratorDraw.DrawInlet(inlet, placeholder);
							Cell.EmptyRowPx(8);
							//using (Cell.Row) Draw.Label(fn.inlets[i].Name);
						}
					}
				}
			}
		}*/


		#region Warnings

			/*public static void DrawCustomMaterialWarning ()
			{
				Terrains.TerrainSettings settings = GraphWindow.current.mapMagic.terrainSettings;
				if (settings.materialType != Terrain.MaterialType.Custom)
				{
					using (Cell.LinePx(56))
					{
						//Cell.current.margins = new Padding(4);

						GUIStyle backStyle = UI.current.textures.GetElementStyle("DPUI/Backgrounds/Foldout");
						Draw.Element(backStyle);
						Draw.Element(backStyle);

						using (Cell.Row) Draw.Label("Material Type \nis not switched \nto Custom.");

						using (Cell.RowPx(30))
							if (Draw.Button("Fix"))
							{
								settings.materialType = Terrain.MaterialType.Custom;
								GraphWindow.current.mapMagic.ApplyTerrainSettings();

								GraphWindow.current.mapMagic.ClearAllNodes();
								GraphWindow.current.mapMagic.StartGenerate();
							}
					}
					Cell.EmptyLinePx(5);
				}
			}*/


			public static void DrawMegaSplatShaderNameWarning ()
			{
				Terrains.TerrainSettings settings = GraphWindow.current.mapMagic.terrainSettings;
				{
					using (Cell.LinePx(70))
					{
						//Cell.current.margins = new Padding(4);

						GUIStyle backStyle = UI.current.textures.GetElementStyle("DPUI/Backgrounds/Foldout");
						Draw.Element(backStyle);
						Draw.Element(backStyle);

						using (Cell.Row) Draw.Label("No MegaSplat material \nis assigned as \nCustom Material in \nTerrain Settings");

						using (Cell.RowPx(30))
							if (Draw.Button("Fix"))
							{
								Shader shader = ReflectionExtensions.CallStaticMethodFrom("Assembly-CSharp-Editor", "SplatArrayShaderGUI", "NewShader", null) as Shader;
								settings.material = new Material(shader);
								settings.material.EnableKeyword("_TERRAIN");

								GraphWindow.current.mapMagic.ApplyTerrainSettings();

								GraphWindow.RefreshMapMagic();
							}
					}
					Cell.EmptyLinePx(5);
				}
			}


			public static void DrawMegaSplatAssignedTextureArraysWarning ()
			{
				Terrains.TerrainSettings settings = GraphWindow.current.mapMagic.terrainSettings;
				{
					using (Cell.LinePx(70))
					{
						//Cell.current.margins = new Padding(4);

						GUIStyle backStyle = UI.current.textures.GetElementStyle("DPUI/Backgrounds/Foldout");
						Draw.Element(backStyle);
						Draw.Element(backStyle);

						using (Cell.Row) Draw.Label("Material has \nno Albedo/Height \nTexture Array \nassigned");
						
						using (Cell.RowPx(30))
							if (Draw.Button("Fix"))
							{
								Shader shader = ReflectionExtensions.CallStaticMethodFrom("Assembly-CSharp-Editor", "SplatArrayShaderGUI", "NewShader", null) as Shader;
								settings.material = new Material(shader);
								settings.material.EnableKeyword("_TERRAIN");

								GraphWindow.current.mapMagic.ApplyTerrainSettings();

								GraphWindow.RefreshMapMagic();
							}
					}
					Cell.EmptyLinePx(5);
				}
			}





			public static void DrawRTPComponentWarning ()
			{
				#if RTP
				if (MapMagic.instance.gameObject.GetComponent<ReliefTerrain>()==null || MapMagic.instance.gameObject.GetComponent<Renderer>()==null)
				{
					using (Cell.LinePx(70)))
					{
						Cell.current.margins = new Padding(4);

						GUIStyle backStyle = UI.current.textures.GetElementStyle("DPUI/Backgrounds/Foldout");
						Draw.Element(backStyle, Cell.current);
						Draw.Element(backStyle, Cell.current);

						Draw.Label("RTP or Renderer \ncomponents are \nnot assigned to \nMapMagic object", cell:UI.Empty(Size.row));

						if (Draw.Button("Fix", cell:UI.Empty(Size.RowPixels(30))))
						{
							if (MapMagic.instance.gameObject.GetComponent<Renderer>() == null)
							{
								MeshRenderer renderer = MapMagic.instance.gameObject.AddComponent<MeshRenderer>();
								renderer.enabled = false;
							}
							if (MapMagic.instance.gameObject.GetComponent<ReliefTerrain>() == null)
							{
								ReliefTerrain rtp = MapMagic.instance.gameObject.AddComponent<ReliefTerrain>();

								//filling empty splats
								Texture2D emptyTex = TextureExtensions.ColorTexture(4,4,new Color(0.5f, 0.5f, 0.5f, 1f));
								emptyTex.name = "Empty";
								rtp.globalSettingsHolder.splats = new Texture2D[] { emptyTex,emptyTex,emptyTex,emptyTex };
							}
							MapMagic.instance.OnSettingsChanged();
						}
					}
					UI.Empty(Size.LinePixels(5));
				}
				#endif
			}


			public static void UpdateMaterial ()
			{
				Renderer renderer = GraphWindow.current.mapMagic.gameObject.GetComponent<Renderer>();
				if (renderer == null) return;

				if (GraphWindow.current.mapMagic.terrainSettings.material != renderer.sharedMaterial)
				{
					GraphWindow.current.mapMagic.terrainSettings.material = renderer.sharedMaterial;
					GraphWindow.current.mapMagic.ApplyTerrainSettings();
				}
			}
		#endregion
	}
}