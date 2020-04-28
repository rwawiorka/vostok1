using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using Den.Tools;

namespace MapMagic.Nodes 
{


	[Serializable]
	public class Exposed
	{
		public struct Entry
		{
			public Guid genGuid;
			public FieldInfo field; //IDEA: use field names instead of fields (better comparison and serialization)?
			public string guiName;

			public override bool Equals(object obj)
			{
				if (obj==null || !(obj is Entry)) return false;
				if (field==null) return false; //function's override dictionary serializes empty entries (in capacity but not in count)

				Entry entry = (Entry)obj;
				return  genGuid == entry.genGuid  &&
						field.Name == entry.field.Name  &&
						field.DeclaringType == entry.field.DeclaringType; //field == entry.field; 
						//note guiName is not taken into account
			}

			public override int GetHashCode () 
			{ 
				return genGuid.GetHashCode();
			}
		}

		public Entry[] entries;


		public Exposed () { } //for serializer
		public Exposed (Exposed src)
		{
			entries = (Entry[])src.entries.Clone();
		}



		public int Find (Generator gen, FieldInfo field)
		{
			//Predicate<Entry> predicate = e => e.genGuid==gen.guid && e.field.Name==field.Name;
			//return ArrayTools.Find(entries, predicate);

			Entry tmp = new Entry() { genGuid=gen.guid, field=field };
			return ArrayTools.Find(entries, tmp);
		}


		public void Expose (Generator gen, FieldInfo field, string name=null)
		{
			if (entries == null) entries = new Entry[0];

			if (Find(gen, field) >= 0) return; //already in list

			Entry entry = new Entry() {
				genGuid = gen.guid,
				field = field,
				guiName = name!=null ? name : field.Name.Nicify() };

			ArrayTools.Add(ref entries, entry);
		}


		public void Unexpose (Generator gen, FieldInfo field)
		{
			int index = Find(gen, field);
			if (index < 0) return; //already in not list

			ArrayTools.RemoveAt(ref entries, index);
			if (entries.Length == 0) entries = null;
		}


		public void Unexpose (Generator gen)
		{
			for (int i=entries.Length-1; i>=0; i--)
				if (entries[i].genGuid == gen.guid)
					ArrayTools.RemoveAt(ref entries, i);
		}


		[Obsolete] public static object GetValidTarget (Generator gen, FieldInfo field)
		/// Returns target object (either generator itself or it's helper class. Null if target has no such field.
		{
			return gen;
		}


		public object[] GetOriginalValues (Dictionary<Guid,Generator> guidLut)
		{
			if (entries == null || entries.Length==0) return new object[0];

			object[] vals = new object[entries.Length];
			for (int i=0; i<entries.Length; i++)
			{
				FieldInfo field = entries[i].field;
				Generator gen = guidLut[entries[i].genGuid];

				vals[i] = field.GetValue(gen);
			}
			return vals;
		}

		
		public void SetOriginalValues (Dictionary<Guid,Generator> guidLut, object[] vals)
		{
			for (int i=0; i<entries.Length; i++)
			{
				FieldInfo field = entries[i].field;
				Generator gen = guidLut[entries[i].genGuid];

				field.SetValue(gen, vals[i]);
			}
		}


		public void ClearObsoleteEntries (Graph graph)
		/// Removes THIS entries containing generators or fields that are not in grpah anymore
		{
			if (entries == null || entries.Length==0) return;
			for (int i=entries.Length-1; i>=0; i--)
			{
				//generator removed
				Guid guid = entries[i].genGuid;
				if (!graph.ContainsGenerator(guid))
				{
					ArrayTools.RemoveAt(ref entries, i);
					continue;
				}

				//field removed (or target like adjust changed)
				Generator gen = graph.GetGenerator(guid);
				if (gen == null)
				{
					ArrayTools.RemoveAt(ref entries, i);
					continue;
				}
			}
		}


		public void ClearObsoleteOverride (Dictionary<Entry,object> ovd)
		/// Does not change THIS entries, but removes unused entries from the ovd dictionary (the ones that do not exist in THIS)
		{
			if (entries == null || entries.Length==0) { ovd.Clear(); return; }

			HashSet<Entry> entHash = new HashSet<Entry>();
			for (int i=0; i<entries.Length; i++)
				entHash.Add(entries[i]);

			List<Entry> removedEntries = new List<Entry>();

			foreach (var kvp in ovd)
			{
				Entry entry = kvp.Key;
				if (!entHash.Contains(entry))
					removedEntries.Add(entry);
			}

			foreach (Entry entry in removedEntries)
				ovd.Remove(entry);
		}


		public void ReadOverride (Dictionary<Guid,Generator> guidLut, Dictionary<Entry,object> ovd)
		/// Adds to dictionary those values that are not in. Does not change the values that are already in ovd
		{
			if (entries == null || entries.Length==0) return;
			for (int i=0; i<entries.Length; i++)
			{
				if (ovd.ContainsKey(entries[i])) continue; //already in list

				FieldInfo field = entries[i].field;  
				Generator gen = guidLut[entries[i].genGuid];
				//call ClearObsoleteEntries first

				object val = field.GetValue(gen);
				ovd.Add(entries[i], val);
			}
		}


		public void ReadOverride (Graph graph, Dictionary<Entry,object> ovd)
		{
			Dictionary<Guid,Generator> guidLut = graph.GuidLut();
			ClearObsoleteEntries(graph);
			ClearObsoleteOverride(ovd);
			
			ReadOverride(guidLut, ovd);
		}


		public void ApplyOverride (Dictionary<Guid,Generator> guidLut, Dictionary<Entry,object> ovd)
		/// Sets the generators values to the ones given in dict (only if they are exposed)
		{
			if (entries==null || entries.Length==0 || ovd==null || ovd.Count==0) return;
			for (int i=0; i<entries.Length; i++)
			{
				if (!ovd.ContainsKey(entries[i])) continue; //exposed but not not overrided

				FieldInfo field = entries[i].field;
				Generator gen = guidLut[entries[i].genGuid];
				//call ClearObsoleteEntries first

				object val = ovd[entries[i]];
				field.SetValue(gen, val);
			}
		}


		public void ApplyOverride (Graph graph, Dictionary<Entry,object> ovd)
		{
			Dictionary<Guid,Generator> guidLut = graph.GuidLut();
			ClearObsoleteEntries(graph);
			
			ApplyOverride(guidLut, ovd);
		}
	}


	[Serializable] 
	[Obsolete]
	public class GenExposed
	{
		public FieldInfo[] fields;
		public string[] guiFieldNames;

		public void ExposeField (FieldInfo field, string name=null) 
		{
			if (fields == null) { fields = new FieldInfo[0]; guiFieldNames = new string[0]; }

			if (ArrayTools.Contains(fields, field)) return;

			if (name == null) name = field.Name.Nicify();

			ArrayTools.Add(ref fields, field);
			ArrayTools.Add(ref guiFieldNames, name);
		}

		public void ExposeFieldByName (Generator gen, string fieldName)
		{
			FieldInfo field = gen.GetType().GetField(fieldName);
			ExposeField(field);
		}

		public void UnexposeField (FieldInfo field)
		{
			int num = ArrayTools.Find(fields, field);
			if (num < 0) return;

			ArrayTools.RemoveAt(ref fields, num);
			ArrayTools.RemoveAt(ref guiFieldNames, num);

			if (fields.Length == 0) fields = null;
		}

		public void UnexposeFieldByName (Generator gen, string fieldName)
		{
			FieldInfo field = gen.GetType().GetField(fieldName);
			UnexposeField(field);
		}

		public bool IsFieldExposed (FieldInfo field)
		{
			return ArrayTools.Contains(fields, field);
		}

		public object GetOriginalValue (Generator gen, FieldInfo field)
		{
			return field.GetValue(gen);
		}

		public void ReadOverride (Generator gen, Dictionary<FieldInfo,object> ovd)
		/// Reads gen values and overwrites the values in ovd
		{
			if (fields == null) return;
			for (int i=0; i<fields.Length; i++)
			{
				object val = fields[i].GetValue(gen);
				
				if (ovd.ContainsKey(fields[i])) ovd[fields[i]] = val;
				else ovd.Add(fields[i], val);
			}
		}

		public void ReadNewOverride (Generator gen, Dictionary<FieldInfo,object> ovd)
		/// Adds to dictionary those values that are not in ovd
		{
			if (fields == null) return;
			for (int i=0; i<fields.Length; i++)
			{
				if (!ovd.ContainsKey(fields[i]))
				{
					object val = fields[i].GetValue(gen);
					ovd.Add(fields[i], val);
				}
			}
		}

		public void ApplyOverride (Generator gen, Dictionary<FieldInfo,object> ovd)
		{
			if (fields==null || fields.Length==0 || ovd==null || ovd.Count==0) return;
			for (int i=0; i<fields.Length; i++)
			{
				if (!ovd.ContainsKey(fields[i])) continue;
				object val = ovd[fields[i]];

				fields[i].SetValue(gen, val);
			}
		}

		/*private static void SyncOverride (Dictionary<Guid, Dictionary<FieldInfo,object>> exposedOverride, Graph graph, bool read=true, bool apply=true)
		/// Sets graph values to the ones found in exposedOverride, adds to exposedOverride new values if not exposed
		{
			HashSet<Guid> outdatedGuids = new HashSet<Guid>(exposedOverride.Keys);

			for (int g=0; g<graph.nodes.Length; g++)
			{
				Generator gen = graph.nodes[g];
				if (gen.exposed == null || gen.exposed.fields == null || gen.exposed.fields.Length == 0) continue;
				
				//adding new gen
				if (!exposedOverride.ContainsKey(gen.guid)) 
					exposedOverride.Add(gen.guid, new Dictionary<FieldInfo,object>());

				//syncing gen override
				if (read) gen.exposed.ReadNewOverride(gen, exposedOverride[gen.guid]);
				if (apply) gen.exposed.ApplyOverride(gen, exposedOverride[gen.guid]);

				//marking gen as used
				outdatedGuids.Remove(gen.guid);
			}

			//removing unused
			foreach (Guid guid in outdatedGuids)
				exposedOverride.Remove(guid);
		}*/
	}

}
