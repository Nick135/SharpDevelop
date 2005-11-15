﻿// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

using Debugger;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : AbstractPadContent
	{
		WindowsDebugger debugger;
		NDebugger debuggerCore;

		TreeListView localVarList;
		
		ColumnHeader name = new ColumnHeader();
		ColumnHeader val  = new ColumnHeader();
		ColumnHeader type = new ColumnHeader();
		
		public override Control Control {
			get {
				return localVarList;
			}
		}
		
		public LocalVarPad() //: base("${res:MainWindow.Windows.Debug.Local}", null)
		{
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			ImageList imageList = new ImageList();
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Class"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Field"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Property"));

			//iconsService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			localVarList = new TreeListView();
			localVarList.SmallImageList = imageList;
			localVarList.ShowPlusMinus = true;
			localVarList.FullRowSelect = true;
			localVarList.Dock = DockStyle.Fill;
			localVarList.Sorting = SortOrder.Ascending;
			//localVarList.GridLines  = false;
			//localVarList.Activation = ItemActivation.OneClick;
			localVarList.Columns.AddRange(new ColumnHeader[] {name, val, type} );
			name.Width = 250;
			val.Width = 300;
			type.Width = 250;
			localVarList.Visible = false;
			localVarList.SizeChanged += new EventHandler(localVarList_SizeChanged);
			localVarList.BeforeExpand += new TreeListViewCancelEventHandler(localVarList_BeforeExpand);
			
			
			RedrawContent();
			
			if (debugger.ServiceInitialized) {
				InitializeDebugger();
			} else {
				debugger.Initialize += delegate {
					InitializeDebugger();
				};
			}
		}
		
		// This is a walkarond for a visual issue
		void localVarList_SizeChanged(object sender, EventArgs e)
		{
			localVarList.Visible = true;
		}
		
		public void InitializeDebugger()
		{
			debuggerCore = debugger.DebuggerCore;
			
			debuggerCore.LocalVariables.VariableAdded += delegate(object sender, VariableEventArgs e) {
				AddVariable(e.Variable);
			};
			
			localVarList.BeginUpdate();
			foreach(Variable v in debuggerCore.LocalVariables) {
				AddVariable(v);
			}
			localVarList.EndUpdate();
		}
		
		void AddVariable(Variable variableToAdd)
		{
			if (variableToAdd.Name.StartsWith("CS$")) return;
			
			TreeListViewDebuggerItem newItem = new TreeListViewDebuggerItem(variableToAdd);
			
			debuggerCore.LocalVariables.VariableRemoved += delegate(object sender, VariableEventArgs removedArgs) {
				if (variableToAdd == removedArgs.Variable) newItem.Remove();
			};
			
			localVarList.Items.Add(newItem);
		}
		
		public override void RedrawContent()
		{
			name.Text = "Name";
			val.Text  = "Value";
			type.Text = "Type";
		}

		private void localVarList_BeforeExpand(object sender, TreeListViewCancelEventArgs e)
		{
			if (debuggerCore.IsPaused) {
				((TreeListViewDebuggerItem)e.Item).BeforeExpand();
			} else {
				MessageBox.Show("You can not explore variables while the debuggee is running.");
				e.Cancel = true;
			}
		}
	}
}
